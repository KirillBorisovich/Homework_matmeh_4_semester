module ComputationalExpressions.RoundingUpWorkflow

type RoundingBuilder(precision: int) =
    member this.Bind (x: float, f) = System.Math.Round(x, precision) |> f

    member this.Return(x: float) = System.Math.Round(x, precision)
