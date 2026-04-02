// <copyright file="Task1.fs" company="Bengya Kirill">
// Copyright (c) Bengya Kirill under MIT License.
// </copyright>

module Test.Task1

/// <summary>
/// Finds and returns the minimum element of the given list.
/// Uses the built-in List.reduce function.
/// </summary>
/// <param name="list">The input list to find the minimum element in.</param>
/// <returns>Some value with the minimum element if the list is not empty; otherwise, None.</returns>
let minListItem list =
    match list with
    | [] -> None
    | _ -> Some(List.reduce min list)
