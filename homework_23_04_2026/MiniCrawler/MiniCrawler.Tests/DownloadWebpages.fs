// <copyright file="DownloadWebpagesTests.fs" company="Bengya Kirill">
// Copyright (c) Bengya Kirill under MIT License.
// </copyright>

module MiniCrawler.Tests.DownloadWebpagesTests

open System
open System.Net
open System.Net.Http
open System.Text
open System.Threading
open System.Threading.Tasks
open System.Collections.Concurrent
open NUnit.Framework
open FsUnit
open MiniCrawler.DownloadWebpages

type TestServer(port: int) =
    let listener = new HttpListener()
    let prefix = $"http://localhost:%d{port}/"
    let mutable cts: CancellationTokenSource option = None
    let mutable serverTask: Task option = None
    let routes = ConcurrentDictionary<string, HttpListenerResponse -> Async<unit>>()

    do listener.Prefixes.Add(prefix)

    member _.BaseUrl = prefix

    member this.AddRoute(path: string, handler: HttpListenerResponse -> Async<unit>) =
        let fullPath = if path.StartsWith("/") then path else "/" + path
        routes[fullPath] <- handler

    member this.AddHtmlRoute(path: string, html: string) =
        this.AddRoute(
            path,
            fun response ->
                async {
                    let buffer = Encoding.UTF8.GetBytes(html)
                    response.ContentType <- "text/html; charset=utf-8"
                    response.ContentLength64 <- int64 buffer.Length
                    response.OutputStream.Write(buffer, 0, buffer.Length)
                    response.OutputStream.Close()
                }
        )

    member this.AddErrorRoute(path: string, statusCode: int) =
        this.AddRoute(
            path,
            fun response ->
                async {
                    response.StatusCode <- statusCode
                    response.Close()
                }
        )

    member this.Start() =
        if serverTask.IsSome then
            failwith "Server already started"

        let tokenSource = new CancellationTokenSource()
        cts <- Some tokenSource
        let token = tokenSource.Token

        let serverLoop =
            async {
                listener.Start()

                while not token.IsCancellationRequested do
                    let! context = listener.GetContextAsync() |> Async.AwaitTask
                    let path = context.Request.Url.AbsolutePath

                    match routes.TryGetValue(path) with
                    | true, handler -> do! handler context.Response
                    | false, _ ->
                        context.Response.StatusCode <- 404
                        context.Response.Close()
            }

        serverTask <- Some(Async.StartAsTask(serverLoop, cancellationToken = token))
        Thread.Sleep(100)

    member this.Stop() =
        match cts, serverTask with
        | Some tokenSource, Some task ->
            tokenSource.Cancel()
            listener.Stop()
            task |> Async.AwaitTask |> ignore
        | _ -> ()

    interface IDisposable with
        member this.Dispose() = this.Stop()

let getRandomPort () =
    let listener = new System.Net.Sockets.TcpListener(IPAddress.Loopback, 0)
    listener.Start()
    let port = (listener.LocalEndpoint :?> IPEndPoint).Port
    listener.Stop()
    port

[<TestFixture>]
type DownloadWebpagesTests() =

    let createHttpClient () = new HttpClient()

    [<Test>]
    member _.``Should download single page and return its length``() =
        let port = getRandomPort ()
        use server = new TestServer(port)
        let html = "<html><body>Hello</body></html>"
        server.AddHtmlRoute("/", html)
        server.Start()

        let httpClient = createHttpClient ()
        let url = server.BaseUrl

        let result = downloadWebpages httpClient 1 url |> Async.RunSynchronously

        result |> Seq.toList |> should equal [ html.Length ]

    [<Test>]
    member _.``Should follow links and download multiple pages``() =
        let port = getRandomPort ()
        use server = new TestServer(port)
        let baseUrl = server.BaseUrl

        let page1Html = $"<a href='%s{baseUrl}page2'>Page2</a>"
        let page2Html = $"<a href='%s{baseUrl}page3'>Page3</a>"
        let page3Html = "<html>No links</html>"

        server.AddHtmlRoute("/", page1Html)
        server.AddHtmlRoute("/page2", page2Html)
        server.AddHtmlRoute("/page3", page3Html)
        server.Start()

        let httpClient = createHttpClient ()
        let result = downloadWebpages httpClient 2 baseUrl |> Async.RunSynchronously

        let lengths = result |> Seq.toList |> List.sort

        lengths
        |> should equal (List.sort [ page1Html.Length; page2Html.Length; page3Html.Length ])

    [<Test>]
    member _.``Should respect maximum degree of parallelism``() =
        let port = getRandomPort ()
        use server = new TestServer(port)
        let baseUrl = server.BaseUrl

        let links =
            [ 1..3 ]
            |> List.map (fun i -> $"<a href='%s{baseUrl}page%d{i}'>Page%d{i}</a>")
            |> String.concat ""

        let page1Html = $"<html>%s{links}</html>"
        server.AddHtmlRoute("/", page1Html)

        let concurrentRequests = ref 0
        let maxConcurrentObserved = ref 0

        for i in 1..3 do
            server.AddRoute(
                $"/page%d{i}",
                fun response ->
                    async {
                        let current = Interlocked.Increment(concurrentRequests)

                        lock maxConcurrentObserved (fun () ->
                            if current > !maxConcurrentObserved then
                                maxConcurrentObserved := current)

                        do! Async.Sleep 500

                        Interlocked.Decrement(concurrentRequests) |> ignore

                        let html = $"<html>Page %d{i}</html>"
                        let buffer = Encoding.UTF8.GetBytes(html)
                        response.ContentType <- "text/html; charset=utf-8"
                        response.ContentLength64 <- int64 buffer.Length
                        response.OutputStream.Write(buffer, 0, buffer.Length)
                        response.OutputStream.Close()
                    }
            )

        server.Start()

        let httpClient = createHttpClient ()
        let maxDegree = 2

        downloadWebpages httpClient maxDegree baseUrl
        |> Async.RunSynchronously
        |> ignore

        !maxConcurrentObserved |> should be (lessThanOrEqualTo maxDegree)

    [<Test>]
    member _.``Should return empty bag if start page fails to load``() =
        let port = getRandomPort ()
        use server = new TestServer(port)
        server.AddErrorRoute("/", 500)
        server.Start()

        let httpClient = createHttpClient ()
        let result = downloadWebpages httpClient 1 server.BaseUrl |> Async.RunSynchronously

        result |> Seq.isEmpty |> should equal true

    [<Test>]
    member _.``Should handle large number of links without deadlock``() =
        let port = getRandomPort ()
        use server = new TestServer(port)
        let baseUrl = server.BaseUrl

        let links =
            [ 1..20 ]
            |> List.map (fun i -> $"<a href='%s{baseUrl}page%d{i}'>Page%d{i}</a>")
            |> String.concat ""

        let mainHtml = $"<html>%s{links}</html>"
        server.AddHtmlRoute("/", mainHtml)

        for i in 1..20 do
            server.AddHtmlRoute($"/page%d{i}", "<html>Simple page</html>")

        server.Start()

        let httpClient = createHttpClient ()
        let result = downloadWebpages httpClient 5 baseUrl |> Async.RunSynchronously

        result.Count |> should equal 21
