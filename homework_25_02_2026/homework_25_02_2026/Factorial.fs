module homework_25_02_2026.Factorial

let factorial x =
    match x with
    | _ when x < 0 -> None
    | _ ->
        let rec factorialInternal x acc =
            if x <= 1 then
                Some(acc)
            else
                factorialInternal (x - 1) (acc * x)

        factorialInternal x 1
