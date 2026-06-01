module PointFree.MultiplyBy

let multiplyByOriginal x l = List.map (fun y -> y * x) l

let multiplyByStep1 x = List.map (fun y -> y * x)

let multiplyByStep2 x = List.map ((*) x)

let multiplyBy = (*) >> List.map
