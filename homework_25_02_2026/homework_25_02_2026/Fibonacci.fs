module homework_25_02_2026.Fibonacci

let fibonacci n =
    let rec fibonacciInternal current previous count =
        match count with
        | 1 -> previous
        | _ -> fibonacciInternal (current + previous) current (count - 1)

    if n <= 0 then
        None
    else
        Some(fibonacciInternal 1 1 n)
