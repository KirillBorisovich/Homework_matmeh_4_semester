module homework_24_02_2026.Tests.MapForTreesTests

open NUnit.Framework
open FsUnit
open homework_24_02_2026.MapForTrees

[<Test>]
let ``Map over single Leaf should apply function to its value`` () =
    let tree = Leaf 5
    mapTree (fun x -> x * 2) tree |> should equal (Leaf 10)

[<Test>]
let ``Map over simple Node should apply function to node value and its branches`` () =
    let tree = Node(10, Leaf 2, Leaf 3)

    let expected = Node(11, Leaf 3, Leaf 4)

    mapTree (fun x -> x + 1) tree |> should equal expected

[<Test>]
let ``Map can change the type of tree elements (int to string)`` () =
    let tree = Node(100, Leaf 1, Leaf 2)
    let expected = Node("100", Leaf "1", Leaf "2")

    mapTree _.ToString() tree |> should equal expected

[<Test>]
let ``Map over complex unbalanced tree with values in every node`` () =
    let tree =
        Node(1,
            Node(2, Leaf 3, Leaf 5),
            Leaf 4
        )

    let expected =
        Node(10,
            Node(20, Leaf 30, Leaf 50),
            Leaf 40
        )

    mapTree (fun x -> x * 10) tree |> should equal expected
