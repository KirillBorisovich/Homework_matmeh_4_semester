module homework_25_02_2026.ReverseList

let reverseList list =
    let rec reverseListInternal list acc =
        match list with
        | [] -> acc
        | h :: t -> reverseListInternal t (h :: acc)

    reverseListInternal list []