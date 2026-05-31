module homework_25_02_2026.Tests.NumberSearchTests

open NUnit.Framework
open FsUnit
open FsCheck
open homework_25_02_2026.NumberSearch

let searchingForANumberShouldReturnTheIndexOfTheFirstOccurrence (list: list<int>) number =
    List.tryFindIndex (fun x -> x = number) list = numberSearch number list

[<Test>]
let ``searching for a number should return the index of the first occurrence`` () =
    Check.QuickThrowOnFailure searchingForANumberShouldReturnTheIndexOfTheFirstOccurrence

[<Test>]
let ``searching for a number in an empty list should be None`` () = numberSearch 2 [] |> should equal None
