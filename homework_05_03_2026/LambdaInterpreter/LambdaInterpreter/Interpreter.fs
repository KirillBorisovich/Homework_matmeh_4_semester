module LambdaInterpreter.Interpreter

type Term =
    | Var of string
    | App of Term * Term
    | Lam of string * Term

let rec fv =
    function
    | Var y -> Set.singleton y
    | App(t1, t2) -> Set.union (fv t1) (fv t2)
    | Lam(y, t) -> Set.remove y (fv t)

let rec fresh y avoidSet =
    if Set.contains y avoidSet then
        fresh (y + "'") avoidSet
    else
        y

let rec subst x e m =
    match m with
    | Var y -> if x = y then e else m
    | App(t1, t2) -> App(subst x e t1, subst x e t2)
    | Lam(y, t) ->
        if x = y then
            m
        else if Set.contains y (fv e) && Set.contains x (fv t) then
            let z = fresh y (Set.union (fv e) (fv t))
            let t' = subst y (Var z) t
            Lam(z, subst x e t')
        else
            Lam(y, subst x e t)

let rec step =
    function
    | App(Lam(x, body), arg) -> Some(subst x arg body)
    | App(t1, t2) ->
        match step t1 with
        | Some t1' -> Some(App(t1', t2))
        | None ->
            match step t2 with
            | Some t2' -> Some(App(t1, t2'))
            | None -> None
    | Lam(x, t) ->
        match step t with
        | Some t' -> Some(Lam(x, t'))
        | None -> None
    | Var _ -> None

let rec normalize t =
    match step t with
    | Some t' -> normalize t'
    | None -> t
