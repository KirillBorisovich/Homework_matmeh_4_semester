// <copyright file="BlockingQueue.fs" company="Bengya Kirill">
// Copyright (c) Bengya Kirill under MIT License.
// </copyright>

namespace BlockingQueue

open System.Collections.Generic
open System.Threading

/// <summary>
/// Represents a thread-safe blocking queue.
/// If a thread attempts to dequeue from an empty queue, it blocks until an item becomes available.
/// </summary>
/// <typeparam name="'T">The type of elements stored in the queue.</typeparam>
type BlockingQueue<'T>() =
    let queue = Queue<'T>()

    /// <summary>
    /// Adds an item to the queue and notifies one waiting consumer.
    /// </summary>
    /// <param name="item">The item to enqueue.</param>
    member this.Enqueue item =
        lock queue (fun () ->
            queue.Enqueue item
            Monitor.Pulse queue)

    /// <summary>
    /// Removes and returns the first element of the queue.
    /// If the queue is empty, the calling thread blocks until an item is enqueued.
    /// </summary>
    /// <returns>The element removed from the front of the queue.</returns>
    member this.Dequeue() =
        lock queue (fun () ->
            while queue.Count = 0 do
                Monitor.Wait queue |> ignore

            queue.Dequeue())
