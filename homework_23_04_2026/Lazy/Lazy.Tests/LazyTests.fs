// <copyright file="LazyTests.fs" company="Bengya Kirill">
// Copyright (c) Bengya Kirill under MIT License.
// </copyright>

module Lazy.Tests

open System
open System.Threading
open System.Threading.Tasks
open FsUnit
open NUnit.Framework

let threadCount = Environment.ProcessorCount * 4

let createMockSupplier =
    let mutable callCount = 0

    let supplier =
        fun () ->
            Thread.Sleep(50)
            Interlocked.Increment(&callCount) |> ignore
            Guid.NewGuid()

    supplier, (fun () -> callCount)

let tasks (barrier: Barrier) (lazyValue: ILazy<'T>) =
    [| for _ in 1..threadCount ->
           Task.Run(fun () ->
               barrier.SignalAndWait()
               lazyValue.Get()) |]

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
let ``multiThreadLazyWithLock Get should return correct value`` () =
    let lazyValue = LazyFactory.multiThreadLazyWithLock (fun () -> "test")
    lazyValue.Get() |> should equal "test"

[<Test>]
let ``multiThreadLazyWithLock supplier should be called only once in multithreaded environment`` () =
    task {
        let supplier, getCallCount = createMockSupplier
        let lazyValue = LazyFactory.multiThreadLazyWithLock supplier
        use barrier = new Barrier(threadCount)

        let! results = tasks barrier lazyValue |> Task.WhenAll

        getCallCount () |> should equal 1
        (results |> Array.distinct).Length |> should equal 1
    }

[<Test>]
let ``multiThreadLazyWithoutLock Get should return correct value in single-threaded environment`` () =
    let lazyValue = LazyFactory.multiThreadLazyWithoutLock (fun () -> 123)
    lazyValue.Get() |> should equal 123

[<Test>]
let ``multiThreadLazyWithoutLock should return same value for all threads`` () =
    task {
        let supplier, getCallCount = createMockSupplier
        let lazyValue = LazyFactory.multiThreadLazyWithoutLock supplier

        use barrier = new Barrier(threadCount)

        let! results = tasks barrier lazyValue |> Task.WhenAll

        (results |> Array.distinct).Length |> should equal 1
        getCallCount () |> should be (greaterThanOrEqualTo 1)
    }
