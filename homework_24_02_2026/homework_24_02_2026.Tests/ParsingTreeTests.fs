module homework_24_02_2026.Tests.ParsingTreeTests

open NUnit.Framework
open FsUnit
open homework_24_02_2026.ParsingTree

[<Test>]
let ``Evaluate simple constant should return its value`` () =
    evaluate (Const 5) |> should equal 5

[<Test>]
let ``Evaluate simple addition: 2 + 3 = 5`` () =
    evaluate (Add(Const 2, Const 3)) |> should equal 5

[<Test>]
let ``Evaluate simple subtraction: 10 - 4 = 6`` () =
    evaluate (Sub(Const 10, Const 4)) |> should equal 6

[<Test>]
let ``Evaluate simple multiplication: 3 * 4 = 12`` () =
    evaluate (Mul(Const 3, Const 4)) |> should equal 12

[<Test>]
let ``Evaluate simple division: 20 / 4 = 5`` () =
    evaluate (Div(Const 20, Const 4)) |> should equal 5

[<Test>]
let ``Evaluate complex expression: ((10 + 5) * (8 - 4)) / 2 = 30`` () =
    let expr = 
        Div(
            Mul(
                Add(Const 10, Const 5), 
                Sub(Const 8, Const 4)
            ),
            Const 2
        )
    evaluate expr |> should equal 30
