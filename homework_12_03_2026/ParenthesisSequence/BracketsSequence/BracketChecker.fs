module BracketChecker

let private openBrackets = set [ '{'; '['; '(' ]
let private matchingBracket = Map.ofList [ ('}', '{'); (']', '['); (')', '(') ]

let isBalanced (input: string) =
    let rec check stack chars =
        match chars with
        | [] -> List.isEmpty stack
        | ch :: rest ->
            if openBrackets.Contains ch then
                check (ch :: stack) rest
            elif matchingBracket.ContainsKey ch then
                match stack with
                | top :: remainingStack when top = matchingBracket[ch] -> check remainingStack rest
                | _ -> false
            else
                check stack rest

    check [] (List.ofSeq input)
