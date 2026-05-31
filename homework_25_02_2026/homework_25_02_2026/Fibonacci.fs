module homework_25_02_2026.Fibonacci

let fibonacci n =
    let rec fibonacciInternal current preview power index =
        if index >= power then
            preview
        else
            fibonacciInternal (current + preview) current power (index + 1)

    if n = 0 || n < 0 then
        None
    else
        Some(fibonacciInternal 1 1 n 1)
