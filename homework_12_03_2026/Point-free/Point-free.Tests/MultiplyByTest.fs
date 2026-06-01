module Point_free.Tests

open NUnit.Framework
open PointFree.MultiplyBy
open FsCheck

let simple x l = List.map (fun y -> y * x) l
let rec AFunctionFromAHomeworkTaskTest x l =
    simple x l = multiplyBy x l

[<Test>]
let ``A function from a homework task test`` () =
    Check.QuickThrowOnFailure AFunctionFromAHomeworkTaskTest