module homework_25_02_2026.NumberSearch

let numberSearch (number: int) list =
    let rec numberSearchInternal number list index =
        match list with
        | h :: t when h = number -> Some(index)
        | h :: t when h <> number -> numberSearchInternal number t (index + 1)
        | _ -> None

    numberSearchInternal number list 0
