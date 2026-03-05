module homework_25_02_2026.Tests.FibonacciTests

open NUnit.Framework
open FsUnit
open homework_25_02_2026.Fibonacci

[<Test>]
let ``Fibonacci of 0 should be 0`` () = fibonacci 0 |> should equal 0

[<TestCase(1)>]
[<TestCase(2)>]
let ``Fibonacci of 1 and 2 should be 1`` n = fibonacci n |> should equal 1

[<TestCase(3, 2)>]
[<TestCase(4, 3)>]
[<TestCase(5, 5)>]
[<TestCase(6, 8)>]
[<TestCase(10, 55)>]
let ``Fibonacci for positive numbers should be correct`` input expected =
    fibonacci input |> should equal expected

[<TestCase(-1, 1)>]
[<TestCase(-2, -1)>]
[<TestCase(-3, 2)>]
[<TestCase(-4, -3)>]
[<TestCase(-6, -8)>]
let ``Fibonacci for negative numbers should follow the sequence`` input expected =
    fibonacci input |> should equal expected
