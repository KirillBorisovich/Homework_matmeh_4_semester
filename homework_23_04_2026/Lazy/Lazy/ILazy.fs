// <copyright file="ILazy.fs" company="Bengya Kirill">
// Copyright (c) Bengya Kirill under MIT License.
// </copyright>

namespace Lazy

open System.Threading

type ILazy<'T> =
    abstract Get: unit -> 'T

module LazyFactory =
    type Box<'T> = { Value: 'T }

    let singleThreaded supplier =
        let mutable result = Unchecked.defaultof<'T>
        let mutable isCalculated = false

        { new ILazy<'T> with
            member this.Get() =
                if not isCalculated then
                    result <- supplier ()
                    isCalculated <- true

                result }

    let multiThreadLazyWithoutLock supplier =
        let mutable result: Box<'T> = Unchecked.defaultof<Box<'T>>

        { new ILazy<'T> with
            member this.Get() =
                let current = result

                if not (obj.ReferenceEquals(current, Unchecked.defaultof<Box<'T>>)) then
                    current.Value
                else
                    let computed = { Value = supplier () }

                    let swapped =
                        Interlocked.CompareExchange(&result, computed, Unchecked.defaultof<Box<'T>>)

                    if obj.ReferenceEquals(swapped, Unchecked.defaultof<Box<'T>>) then
                        computed.Value
                    else
                        swapped.Value }


    let SingleThreadedLazy supplier =
        let mutable result = Unchecked.defaultof<'T>
        let mutable isCalculated = false
        let lockObject = obj ()

        { new ILazy<'T> with
            member this.Get() =
                if not isCalculated then
                    lock lockObject (fun () ->
                        if not isCalculated then
                            result <- supplier ()
                            isCalculated <- true)


                result }
