// <copyright file="ILazy.fs" company="Bengya Kirill">
// Copyright (c) Bengya Kirill under MIT License.
// </copyright>

namespace Lazy

open System.Threading

type ILazy<'T> =
    abstract Get: unit -> 'T

module LazyFactory =
    [<AllowNullLiteral>]
    type Box<'T>(value: 'T) =
        member _.Value = value

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
        let mutable result: Box<'T> = null

        { new ILazy<'T> with
            member this.Get() =
                let current = result

                if not (obj.ReferenceEquals(current, null)) then
                    current.Value
                else
                    let computed = Box(supplier ())

                    let swapped = Interlocked.CompareExchange(&result, computed, null)

                    if obj.ReferenceEquals(swapped, null) then
                        computed.Value
                    else
                        swapped.Value }


    let multiThreadLazyWithLock supplier =
        let mutable result = Unchecked.defaultof<'T>
        let mutable isCalculated = false
        let lockObject = obj ()

        { new ILazy<'T> with
            member this.Get() =
                if not (Volatile.Read(&isCalculated)) then
                    lock lockObject (fun () ->
                        if not (Volatile.Read(&isCalculated)) then
                            result <- supplier ()
                            Volatile.Write(&isCalculated, true))


                result }
