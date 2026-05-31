module homework_24_02_2026.PrimeNumbers

let isPrime n =
    if n < 2 then
        false
    else
        let bound = int (sqrt (float n))
        seq { 2..bound } |> Seq.forall (fun i -> n % i <> 0)

let primes = Seq.initInfinite id |> Seq.filter isPrime
