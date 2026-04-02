// <copyright file="Task1Tests.fs" company="Bengya Kirill">
// Copyright (c) Bengya Kirill under MIT License.
// </copyright>

module Test.Tests.Task1Tests

open NUnit.Framework
open FsCheck
open Test.Task1

let shouldReturnMinListItem (list: int list) =
    match list with
    | [] -> minListItem list = None
    | _ -> minListItem list = Some(List.min list)

[<Test>]
let ``should return minimum list item`` () =
    Check.QuickThrowOnFailure shouldReturnMinListItem
