module homework_24_02_2026.ParsingTree

type Expr =
    | Const of int
    | Add of Expr * Expr
    | Sub of Expr * Expr
    | Mul of Expr * Expr
    | Div of Expr * Expr

let evaluate expr =
    let rec evalCPS e cont =
        match e with
        | Const x ->
            cont x
        | Add (left, right) -> 
            evalCPS left (fun lVal -> 
                evalCPS right (fun rVal -> cont (lVal + rVal)))
        | Sub (left, right) ->
            evalCPS left (fun lVal -> 
                evalCPS right (fun rVal -> cont (lVal - rVal)))
        | Mul (left, right) ->
            evalCPS left (fun lVal -> 
                evalCPS right (fun rVal -> cont (lVal * rVal)))
        | Div (left, right) ->
            evalCPS left (fun lVal -> 
                evalCPS right (fun rVal -> cont (lVal / rVal)))

    evalCPS expr id
