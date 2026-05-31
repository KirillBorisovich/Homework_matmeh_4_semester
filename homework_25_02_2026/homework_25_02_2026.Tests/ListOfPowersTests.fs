module homework_25_02_2026.Tests.ListOfPowersTests

open NUnit.Framework
open FsUnit
open homework_25_02_2026.ListOfPowers

[<TestCase(0, 0)>]
let ``n=0, m=0 should return list with 1.0`` (n, m) =
    listOfPowers n m |> should equal (Some [ 1.0 ])

[<TestCase(1, 3)>]
let ``n=1, m=3 should return [2.0; 4.0; 8.0; 16.0]`` (n, m) =
    listOfPowers n m |> should equal (Some [ 2.0; 4.0; 8.0; 16.0 ])

[<TestCase(3, 2)>]
let ``n=3, m=2 should return [8.0; 16.0; 32.0]`` (n, m) =
    listOfPowers n m |> should equal (Some [ 8.0; 16.0; 32.0 ])

[<TestCase(-1, 2)>]
let ``n=-1, m=2 should work with fractions`` (n, m) =
    listOfPowers n m |> should equal (Some [ 0.5; 1.0; 2.0 ])

[<TestCase(1, -1)>]
[<TestCase(-1, -1)>]
let ``m < 0 should return None`` (n, m) = listOfPowers n m |> should equal None
