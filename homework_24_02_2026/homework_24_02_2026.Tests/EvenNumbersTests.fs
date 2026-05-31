module homework_24_02_2026.Tests.EvenNumbersTests

open FsCheck
open NUnit.Framework
open homework_24_02_2026.EvenNumbers

let allFunctionsShouldReturnTheSameResult (xs: int list) =
    let expected = countEvenFilter xs
    let mapResult = countEvenMap xs
    let foldResult = countEvenFold xs

    mapResult = expected && foldResult = expected

[<Test>]
let ``All functions should return the same result`` () =
    Check.QuickThrowOnFailure allFunctionsShouldReturnTheSameResult
