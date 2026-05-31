module homework_24_02_2026.MapForTrees

type Tree<'a> =
    | Leaf of 'a
    | Node of 'a * Tree<'a> * Tree<'a>

let mapTree f tree =
    let rec mapCPS t cont =
        match t with
        | Leaf x ->
            cont (Leaf (f x))

        | Node (value, left, right) ->
            let mappedValue = f value

            mapCPS left (fun leftMapped ->
                mapCPS right (fun rightMapped ->
                    cont (Node (mappedValue, leftMapped, rightMapped))))

    mapCPS tree id
