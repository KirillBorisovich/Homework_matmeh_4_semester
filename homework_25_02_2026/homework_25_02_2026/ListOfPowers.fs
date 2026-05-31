module homework_25_02_2026.ListOfPowers

let listOfPowers n m =
    match m with
    | _ when m < 0 -> None
    | _ ->
        let firstNumberInList = pown 2.0 n

        let rec resultList acc index =
            match index with
            | 0 -> List.rev acc
            | _ -> resultList ((2.0 * List.head acc) :: acc) (index - 1)

        Some(resultList [ firstNumberInList ] m)
