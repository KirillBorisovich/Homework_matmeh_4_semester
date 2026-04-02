// <copyright file="Task2Tests.fs" company="Bengya Kirill">
// Copyright (c) Bengya Kirill under MIT License.
// </copyright>

module Test.Tests.Task2Tests

open NUnit.Framework
open FsUnit
open Test.Task2


[<Test>]
let ``getSquare 1 should return single star`` () = getSquare 1 |> should equal "*\n"

[<Test>]
let ``getSquare 2 should return 2x2 square`` () = getSquare 2 |> should equal "**\n**"

[<Test>]
let ``getSquare 4 should return 4x4 square`` () =
    getSquare 4 |> should equal "****\n*  *\n*  *\n****"

[<Test>]
let ``getSquare 5 should return 5x5 square`` () =
    getSquare 5 |> should equal "*****\n*   *\n*   *\n*   *\n*****"
