module homework_25_02_2026.Tests.FactorialTests

open NUnit.Framework
open FsUnit
open homework_25_02_2026.Factorial

[<TestCase(0)>]
[<TestCase(1)>]
let ``the factorial for 0 and 1 should return the value 1`` n = factorial n |> should equal (Some 1)

[<TestCase(-1)>]
[<TestCase(-5)>]
[<TestCase(-10)>]
let ``the factorial for negative numbers should return None`` n = factorial n |> should equal None

[<TestCase(3, 6)>]
[<TestCase(5, 120)>]
[<TestCase(6, 720)>]
[<TestCase(8, 40320)>]
let ``the factorial for positive numbers must be correct`` input expected =
    factorial input |> should equal (Some expected)
