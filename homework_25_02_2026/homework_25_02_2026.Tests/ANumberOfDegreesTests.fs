module homework_25_02_2026.Tests.ANumberOfDegreesTests

open NUnit.Framework
open FsUnit
open homework_25_02_2026.ANumberOfDegrees

[<TestCase(0, 0)>]
let ``n=0, m=0 should return list with 1`` (n, m) =
    aNumberOfDegrees n m |> should equal [ 1 ]

[<TestCase(1, 3)>]
let ``n=1, m=3 should return [2; 4; 8; 16]`` (n, m) =
    aNumberOfDegrees n m |> should equal [ 2; 4; 8; 16 ]

[<TestCase(3, 2)>]
let ``n=3, m=2 should return [8; 16; 32]`` (n, m) =
    aNumberOfDegrees n m |> should equal [ 8; 16; 32 ]

[<TestCase(1, -1)>]
[<TestCase(-1, 1)>]
[<TestCase(-1, -1)>]
let ``n < 0 or m < 0 should return empty list`` (n, m) = aNumberOfDegrees n m |> should be Empty
