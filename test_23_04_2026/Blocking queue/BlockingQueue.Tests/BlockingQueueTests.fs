// <copyright file="BlockingQueueTests.fs" company="Bengya Kirill">
// Copyright (c) Bengya Kirill under MIT License.
// </copyright>

module BlockingQueue.Tests

open System.Threading.Tasks
open FsUnit
open NUnit.Framework

[<Test>]
let ``Enqueue and Dequeue single item returns that item`` () =
    let queue = BlockingQueue<int>()
    queue.Enqueue 42
    queue.Dequeue() |> should equal 42

[<Test>]
let ``Sequential access maintains order`` () =
    let queue = BlockingQueue<int>()
    [ 1..5 ] |> List.iter queue.Enqueue

    [ 1..5 ]
    |> List.map (fun _ -> queue.Dequeue())
    |> should equal [ 1; 2; 3; 4; 5 ]

[<Test>]
let ``Dequeue on empty queue blocks until an item is enqueued`` () =
    let queue = BlockingQueue<string>()
    let task = Task.Run(fun () -> queue.Dequeue())
    System.Threading.Thread.Sleep 100
    task.IsCompleted |> should equal false
    queue.Enqueue "test"
    let tasks: Task[] = [| task :> Task |]
    Task.WaitAll(tasks, 1000) |> should equal true
    task.Result |> should equal "test"

[<Test>]
let ``Multiple consumers wake up exactly one per enqueue`` () =
    let queue = BlockingQueue<int>()
    let received = System.Collections.Concurrent.ConcurrentBag<int>()

    let consumers =
        [ for _ in 1..5 -> Task.Run(fun () -> received.Add(queue.Dequeue())) ]

    System.Threading.Thread.Sleep 100
    [ 10..14 ] |> List.iter queue.Enqueue
    Task.WaitAll(consumers |> Seq.map id |> Seq.toArray)
    received |> Seq.sort |> List.ofSeq |> should equal [ 10..14 ]

[<Test>]
let ``Concurrent access maintains data integrity`` () =
    let queue = BlockingQueue<int>()
    let produced = Array.init 1000 id
    let consumed = System.Collections.Concurrent.ConcurrentBag<int>()

    let producers =
        Task.Run(fun () ->
            for i in produced do
                queue.Enqueue i)

    let consumer =
        Task.Run(fun () ->
            for _ in produced do
                consumed.Add(queue.Dequeue()))

    Task.WaitAll(producers, consumer)
    consumed |> Seq.length |> should equal 1000

    consumed
    |> Seq.sort
    |> List.ofSeq
    |> should equal (produced |> Seq.sort |> List.ofSeq)

[<Test>]
let ``Enqueue when no waiting consumers does not block`` () =
    let queue = BlockingQueue<int>()
    [ 1..100 ] |> List.iter queue.Enqueue
    let results = [ for _ in 1..100 -> queue.Dequeue() ]
    results |> should equal [ 1..100 ]

[<Test>]
let ``Multiple producers and consumers process all items exactly once`` () =
    let queue = BlockingQueue<string>()

    let producers =
        [ for i in 0..9 ->
              Task.Run(fun () ->
                  for j in 0..9 do
                      queue.Enqueue $"{i}-{j}") ]

    Task.WaitAll(producers |> Seq.map id |> Seq.toArray)
    let results = System.Collections.Concurrent.ConcurrentBag<string>()

    let consumers =
        [ for _ in 0..9 ->
              Task.Run(fun () ->
                  for _ in 0..9 do
                      results.Add(queue.Dequeue())) ]

    Task.WaitAll(consumers |> Seq.map id |> Seq.toArray)
    results.Count |> should equal 100
    results |> Seq.distinct |> Seq.length |> should equal 100
