module homework_25_02_2026.ANumberOfDegrees

let aNumberOfDegrees n m =
    if n < 0 || m < 0 then
        []
    else
        let firstNumberInList = pown 2 n

        let rec resultList currentPower acc index =
            if index < 0 then
                List.rev acc
            else
                resultList (currentPower * 2) (currentPower :: acc) (index - 1)

        resultList firstNumberInList [] m
