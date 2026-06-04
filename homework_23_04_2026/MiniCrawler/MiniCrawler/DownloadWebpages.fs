// <copyright file="DownloadWebpages.fs" company="Bengya Kirill">
// Copyright (c) Bengya Kirill under MIT License.
// </copyright>

module MiniCrawler.DownloadWebpages

open System.Net.Http
open System.Text.RegularExpressions
open System.Collections.Concurrent
open System.Threading

let private fetchHtml (httpClient: HttpClient) (url: string) =
    async {
        try
            let! response = httpClient.GetStringAsync(url) |> Async.AwaitTask
            return Some response
        with ex ->
            printf $"error on %s{url}: %s{ex.Message}"
            return None
    }

let private extractLinks html =
    let pattern = @"<a\s+[^>]*?href\s*=\s*[""']([^""']+)[""']"

    Regex.Matches(html, pattern, RegexOptions.IgnoreCase)
    |> Seq.cast<Match>
    |> Seq.choose (fun m ->
        match m.Groups with
        | groups when groups.Count > 1 -> Some groups[1].Value
        | _ -> None)

let private parallelWithDegree maxDegree tasks =
    let semaphore = new SemaphoreSlim(maxDegree)

    tasks
    |> Seq.map (fun task ->
        async {
            do! semaphore.WaitAsync() |> Async.AwaitTask

            try
                return! task
            finally
                semaphore.Release() |> ignore
        })
    |> Async.Parallel

let downloadWebpages (httpClient: HttpClient) numberOfSimultaneousDownloads url =
    let lengths = ConcurrentBag<int>()
    let visited = ConcurrentDictionary<string, bool>()

    let rec downloadWebpagesRecursively url =
        async {
            if not (visited.TryAdd(url, true)) then
                return ()

            let! page = fetchHtml httpClient url

            match page with
            | Some html ->
                lengths.Add(html.Length)
                let asyncs = extractLinks html |> Seq.map downloadWebpagesRecursively
                do! asyncs |> parallelWithDegree numberOfSimultaneousDownloads |> Async.Ignore
            | None -> ()
        }

    async {
        do! downloadWebpagesRecursively url
        return lengths
    }
