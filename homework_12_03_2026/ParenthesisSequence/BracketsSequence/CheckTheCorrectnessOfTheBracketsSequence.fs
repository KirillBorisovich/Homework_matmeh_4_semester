module BracketsSequence.CheckTheCorrectnessOfTheBracketsSequence

let push item stack = item :: stack

let pop stack =
    match stack with
    | item :: stack -> Some(item, stack)
    | [] -> None

let checkTheCorrectnessOfTheBracketsSequence input =
    let matches bracketForChek closingBracket =
        match closingBracket with
        | '}' when bracketForChek = '{' -> true
        | ']' when bracketForChek = '[' -> true
        | ')' when bracketForChek = '(' -> true
        | _ -> false

    let rec loop (input: string) stack index =
        if index >= input.Length then
            List.isEmpty stack
        else
            match input[index] with
            | '{' | '[' | '(' -> loop input (push input[index] stack) (index + 1)
            | '}' | ']' | ')' ->
                match pop stack with
                | Some(popResult, newStack) ->
                    if not (matches popResult input[index]) then
                        false
                    else
                        loop input newStack (index + 1)
                | None -> false
            | _ -> loop input stack (index + 1)

    loop input [] 0
