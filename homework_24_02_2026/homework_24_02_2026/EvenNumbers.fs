module homework_24_02_2026.EvenNumbers

let countEvenFilter xs =
    xs |> List.filter (fun x -> x % 2 = 0) |> List.length

let countEvenMap xs =
    xs |> List.map (fun x -> if x % 2 = 0 then 1 else 0) |> List.sum

let countEvenFold xs =
    xs |> List.fold (fun acc x -> if x % 2 = 0 then acc + 1 else acc) 0
