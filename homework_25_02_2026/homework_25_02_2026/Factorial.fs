module homework_25_02_2026.Factorial

let factorial x =
    let rec factorialInternal x acc =
        match x with
        | _ when x < 0 -> None
        | 0 | 1 -> Some acc
        | _ -> factorialInternal (x - 1) (acc * x)

    factorialInternal x 1
