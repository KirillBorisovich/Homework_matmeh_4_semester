// <copyright file="LazyTests.fs" company="Bengya Kirill">
// Copyright (c) Bengya Kirill under MIT License.
// </copyright>

module Lazy.Tests

open System
open System.Threading
open System.Threading.Tasks
open FsUnit
open NUnit.Framework

[<Test>]
let ``singleThreaded Get should return correct value`` () =
    let lazyValue = LazyFactory.singleThreaded (fun () -> 42)
    lazyValue.Get() |> should equal 42

[<Test>]
let ``singleThreaded Get should return same value on repeated calls`` () =
    let lazyValue = LazyFactory.singleThreaded (fun () -> Guid.NewGuid())
    let firstResult = lazyValue.Get()
    let secondResult = lazyValue.Get()
    firstResult |> should equal secondResult

[<Test>]
let ``singleThreaded supplier should be called only once`` () =
    let mutable callCount = 0

    let supplier =
        fun () ->
            callCount <- callCount + 1
            "test"

    let lazyValue = LazyFactory.singleThreaded supplier

    callCount |> should equal 0

    lazyValue.Get() |> ignore
    callCount |> should equal 1

    lazyValue.Get() |> ignore
    callCount |> should equal 1

[<Test>]
let ``SingleThreadedLazy Get should return correct value`` () =
    let lazyValue = LazyFactory.SingleThreadedLazy(fun () -> "test")
    lazyValue.Get() |> should equal "test"

[<Test>]
let ``SingleThreadedLazy supplier should be called only once in multithreaded environment`` () =
    let mutable callCount = 0

    let supplier =
        fun () ->
            Thread.Sleep(50)
            Interlocked.Increment(&callCount) |> ignore
            Guid.NewGuid()

    let lazyValue = LazyFactory.SingleThreadedLazy supplier

    let tasks =
        [| for _ in 1..100 -> Task.Run(fun () -> lazyValue.Get()) |] |> Task.WhenAll

    let results = tasks.Result

    callCount |> should equal 1

    let distinctResults = results |> Array.distinct
    distinctResults.Length |> should equal 1

[<Test>]
let ``multiThreadLazyWithoutLock Get should return correct value in single-threaded environment`` () =
    let lazyValue = LazyFactory.multiThreadLazyWithoutLock (fun () -> 123)
    lazyValue.Get() |> should equal 123

[<Test>]
let ``multiThreadLazyWithoutLock should return same value for all threads`` () =
    let mutable callCount = 0

    let supplier =
        fun () ->
            Thread.Sleep(50)
            Interlocked.Increment(&callCount) |> ignore
            Guid.NewGuid()

    let lazyValue = LazyFactory.multiThreadLazyWithoutLock supplier

    let tasks =
        [| for _ in 1..100 -> Task.Run(fun () -> lazyValue.Get()) |] |> Task.WhenAll

    let results = tasks.Result

    let distinctResults = results |> Array.distinct
    distinctResults.Length |> should equal 1

    callCount |> should be (greaterThanOrEqualTo 1)
