module ComputationalExpressions.CalculationsOverRows

let tryParse (x: string) =
    match System.Int32.TryParse x with
    | (true, value) -> Some value
    | (false, _) -> None

type CalculateBuilder() =
    member this.Bind(x, f) =
        match tryParse x with
        | Some number -> number |> f
        | None -> None

    member this.Return(x) = Some x
