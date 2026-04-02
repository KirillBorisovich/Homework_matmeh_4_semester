module ComputationalExpressions.Tests.CalculationsOverRowsTests

open ComputationalExpressions
open NUnit.Framework
open FsUnit

let calculate = CalculationsOverRows.CalculateBuilder()

[<Test>]
let ``Should return the number 3`` () =
    calculate {
        let! x = "1"
        let! y = "2"
        let z = x + y
        return z
    }
    |> should equal (Some 3)

[<Test>]
let ``Should return None`` () =
    calculate {
        let! x = "1"
        let! y = "Ъ"
        let z = x + y
        return z
    }
    |> should equal None
