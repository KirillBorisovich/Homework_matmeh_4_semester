module ComputationalExpressions.Tests.RoundingUpWorkflowTests

open ComputationalExpressions
open NUnit.Framework
open FsUnit

let rounding x = RoundingUpWorkflow.RoundingBuilder(x)

[<Test>]
let ``Should return the number 0.048`` () =
    rounding 3 {
        let! a = 2.0 / 12.0
        let! b = 3.5
        return a / b
    }
    |> should equal 0.048
