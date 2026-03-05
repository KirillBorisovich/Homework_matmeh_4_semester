module homework_25_02_2026.Tests.ReverseListTests

open NUnit.Framework
open FsCheck
open homework_25_02_2026.ReverseList

let reverseListShouldReturnTheReverseList list = List.rev list = reverseList list

[<Test>]
let ``reverseList should return the reverse list`` () =
    Check.QuickThrowOnFailure reverseListShouldReturnTheReverseList
