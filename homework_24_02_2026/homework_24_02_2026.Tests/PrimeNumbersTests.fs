module homework_24_02_2026.Tests.PrimeNumbersTests

open NUnit.Framework
open FsUnit
open homework_24_02_2026.PrimeNumbers

[<Test>]
let ``First 5 primes should be [2; 3; 5; 7; 11]`` () =
    primes |> Seq.take 5 |> Seq.toList |> should equal [ 2; 3; 5; 7; 11 ]

[<Test>]
let ``Taking 0 primes should return empty list`` () =
    primes |> Seq.take 0 |> Seq.toList |> should be Empty

[<TestCase(0, 2)>]
[<TestCase(1, 3)>]
[<TestCase(2, 5)>]
[<TestCase(3, 7)>]
[<TestCase(4, 11)>]
[<TestCase(9, 29)>]
[<TestCase(99, 541)>]
let ``N-th element in sequence should be the correct prime number`` index expected =
    primes |> Seq.item index |> should equal expected
