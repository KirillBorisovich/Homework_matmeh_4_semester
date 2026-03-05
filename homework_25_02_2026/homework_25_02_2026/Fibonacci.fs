module homework_25_02_2026.Fibonacci

let fibonacci n =
    let rec fibonacciInternal (a, b) power index =
        if index >= power then
            b
        else
            fibonacciInternal (a + b, a) power (index + 1)

    match n with
    | 0 -> 0
    | _ ->
        let result = fibonacciInternal (1, 1) (abs n) 1
        if n < 0 && n % 2 = 0 then -result else result
