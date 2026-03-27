module LocalNetwork.Tests.NetworkTests

open NUnit.Framework
open FsUnit
open LocalNetwork.Network

[<Test>]
let ``Constructor should throw if adjacency matrix is not square`` () =
    let pcs = [| { pcOC = Windows; IsInfected = false } |]
    let matrix = array2D [ [ 0; 1 ] ]

    (fun () -> LocalNetwork(pcs, matrix, 0.5, 0.5, 0.5) |> ignore)
    |> should throw typeof<System.ArgumentException>

[<Test>]
let ``Constructor should throw if pcArray length does not match matrix size`` () =
    let pcs = [| { pcOC = Windows; IsInfected = false } |]
    let matrix = array2D [ [ 0; 0 ]; [ 0; 0 ] ]

    (fun () -> LocalNetwork(pcs, matrix, 0.5, 0.5, 0.5) |> ignore)
    |> should throw typeof<System.InvalidOperationException>

[<Test>]
let ``Constructor should throw if adjacency matrix contains invalid values`` () =
    let pcs =
        [| { pcOC = Windows; IsInfected = false }
           { pcOC = Linux; IsInfected = false } |]

    let matrix = array2D [ [ 0; 2 ]; [ 2; 0 ] ]

    (fun () -> LocalNetwork(pcs, matrix, 0.5, 0.5, 0.5) |> ignore)
    |> should throw typeof<System.ArgumentException>

[<TestCase(-0.1)>]
[<TestCase(1.1)>]
let ``Constructor should throw if macProb is out of range`` (prob: float) =
    let pcs = [| { pcOC = MacOs; IsInfected = false } |]
    let matrix = array2D [ [ 0 ] ]

    (fun () -> LocalNetwork(pcs, matrix, prob, 0.5, 0.5) |> ignore)
    |> should throw typeof<System.ArgumentException>

[<TestCase(-0.1)>]
[<TestCase(1.1)>]
let ``Constructor should throw if linuxProb is out of range`` (prob: float) =
    let pcs = [| { pcOC = Linux; IsInfected = false } |]
    let matrix = array2D [ [ 0 ] ]

    (fun () -> LocalNetwork(pcs, matrix, 0.5, prob, 0.5) |> ignore)
    |> should throw typeof<System.ArgumentException>

[<TestCase(-0.1)>]
[<TestCase(1.1)>]
let ``Constructor should throw if windowsProb is out of range`` (prob: float) =
    let pcs = [| { pcOC = Windows; IsInfected = false } |]
    let matrix = array2D [ [ 0 ] ]

    (fun () -> LocalNetwork(pcs, matrix, 0.5, 0.5, prob) |> ignore)
    |> should throw typeof<System.ArgumentException>

[<Test>]
let ``No infection should spread if no computer is infected`` () =
    let pcs =
        [| { pcOC = Windows; IsInfected = false }
           { pcOC = Linux; IsInfected = false } |]

    let matrix = array2D [ [ 0; 1 ]; [ 1; 0 ] ]
    let network = LocalNetwork(pcs, matrix, 1.0, 1.0, 1.0)
    network.MakeAMove()

    network.GetComputers()
    |> Array.forall (fun c -> not c.IsInfected)
    |> should equal true

[<Test>]
let ``With probability 1.0 all neighbors of infected computer should become infected`` () =
    let pcs =
        [| { pcOC = Windows; IsInfected = true }
           { pcOC = Windows; IsInfected = false }
           { pcOC = Windows; IsInfected = false } |]

    let matrix = array2D [ [ 0; 1; 1 ]; [ 1; 0; 0 ]; [ 1; 0; 0 ] ]
    let network = LocalNetwork(pcs, matrix, 1.0, 1.0, 1.0)
    network.MakeAMove()

    network.GetComputers()
    |> Array.forall (fun c -> c.IsInfected)
    |> should equal true

[<Test>]
let ``With probability 0.0 no neighbor should become infected`` () =
    let pcs =
        [| { pcOC = Windows; IsInfected = true }
           { pcOC = Windows; IsInfected = false }
           { pcOC = Windows; IsInfected = false } |]

    let matrix = array2D [ [ 0; 1; 1 ]; [ 1; 0; 0 ]; [ 1; 0; 0 ] ]
    let network = LocalNetwork(pcs, matrix, 0.0, 0.0, 0.0)
    network.MakeAMove()
    let result = network.GetComputers()
    result[0].IsInfected |> should equal true
    result[1].IsInfected |> should equal false
    result[2].IsInfected |> should equal false

[<Test>]
let ``Disconnected computer should not be infected even with probability 1.0`` () =
    let pcs =
        [| { pcOC = Windows; IsInfected = true }
           { pcOC = Windows; IsInfected = false } |]

    let matrix = array2D [ [ 0; 0 ]; [ 0; 0 ] ]
    let network = LocalNetwork(pcs, matrix, 1.0, 1.0, 1.0)
    network.MakeAMove()
    let result = network.GetComputers()
    result[0].IsInfected |> should equal true
    result[1].IsInfected |> should equal false

[<Test>]
let ``Already infected computer should stay infected`` () =
    let pcs = [| { pcOC = Windows; IsInfected = true } |]
    let matrix = array2D [ [ 0 ] ]
    let network = LocalNetwork(pcs, matrix, 0.0, 0.0, 0.0)
    network.MakeAMove()
    (network.GetComputers()[0]).IsInfected |> should equal true

[<Test>]
let ``Infection should propagate through a chain one step at a time`` () =
    let pcs =
        [| { pcOC = Windows; IsInfected = true }
           { pcOC = Windows; IsInfected = false }
           { pcOC = Windows; IsInfected = false } |]

    let matrix = array2D [ [ 0; 1; 0 ]; [ 1; 0; 1 ]; [ 0; 1; 0 ] ]
    let network = LocalNetwork(pcs, matrix, 1.0, 1.0, 1.0)

    network.MakeAMove()
    let after1 = network.GetComputers()
    after1[0].IsInfected |> should equal true
    after1[1].IsInfected |> should equal true
    after1[2].IsInfected |> should equal false

    network.MakeAMove()
    let after2 = network.GetComputers()
    after2[2].IsInfected |> should equal true

[<Test>]
let ``Only Windows neighbor should be infected when only windowsProb is 1.0`` () =
    let pcs =
        [| { pcOC = Linux; IsInfected = true }
           { pcOC = Windows; IsInfected = false }
           { pcOC = Linux; IsInfected = false }
           { pcOC = MacOs; IsInfected = false } |]

    let matrix =
        array2D [ [ 0; 1; 1; 1 ]; [ 1; 0; 0; 0 ]; [ 1; 0; 0; 0 ]; [ 1; 0; 0; 0 ] ]

    let network = LocalNetwork(pcs, matrix, 0.0, 0.0, 1.0)
    network.MakeAMove()
    let result = network.GetComputers()
    result[1].IsInfected |> should equal true
    result[2].IsInfected |> should equal false
    result[3].IsInfected |> should equal false

[<Test>]
let ``Only Linux neighbor should be infected when only linuxProb is 1.0`` () =
    let pcs =
        [| { pcOC = Windows; IsInfected = true }
           { pcOC = Linux; IsInfected = false }
           { pcOC = Windows; IsInfected = false }
           { pcOC = MacOs; IsInfected = false } |]

    let matrix =
        array2D [ [ 0; 1; 1; 1 ]; [ 1; 0; 0; 0 ]; [ 1; 0; 0; 0 ]; [ 1; 0; 0; 0 ] ]

    let network = LocalNetwork(pcs, matrix, 0.0, 1.0, 0.0)
    network.MakeAMove()
    let result = network.GetComputers()
    result[1].IsInfected |> should equal true
    result[2].IsInfected |> should equal false
    result[3].IsInfected |> should equal false

[<Test>]
let ``Only MacOs neighbor should be infected when only macProb is 1.0`` () =
    let pcs =
        [| { pcOC = Windows; IsInfected = true }
           { pcOC = MacOs; IsInfected = false }
           { pcOC = Windows; IsInfected = false }
           { pcOC = Linux; IsInfected = false } |]

    let matrix =
        array2D [ [ 0; 1; 1; 1 ]; [ 1; 0; 0; 0 ]; [ 1; 0; 0; 0 ]; [ 1; 0; 0; 0 ] ]

    let network = LocalNetwork(pcs, matrix, 1.0, 0.0, 0.0)
    network.MakeAMove()
    let result = network.GetComputers()
    result[1].IsInfected |> should equal true
    result[2].IsInfected |> should equal false
    result[3].IsInfected |> should equal false

[<Test>]
let ``Modifying original array after construction should not affect the network`` () =
    let pcs = [| { pcOC = Windows; IsInfected = false } |]
    let matrix = array2D [ [ 0 ] ]
    let network = LocalNetwork(pcs, matrix, 0.5, 0.5, 0.5)
    pcs[0].IsInfected <- true
    (network.GetComputers()[0]).IsInfected |> should equal false

[<Test>]
let ``GetComputers should return a defensive copy`` () =
    let pcs = [| { pcOC = Windows; IsInfected = false } |]
    let matrix = array2D [ [ 0 ] ]
    let network = LocalNetwork(pcs, matrix, 0.5, 0.5, 0.5)
    let copy = network.GetComputers()
    copy[0].IsInfected <- true
    (network.GetComputers()[0]).IsInfected |> should equal false

[<Test>]
let ``Fully connected network with probability 1.0 should infect all in one move`` () =
    let pcs =
        [| { pcOC = Linux; IsInfected = true }
           { pcOC = Linux; IsInfected = false }
           { pcOC = Linux; IsInfected = false }
           { pcOC = Linux; IsInfected = false } |]

    let matrix =
        array2D [ [ 0; 1; 1; 1 ]; [ 1; 0; 1; 1 ]; [ 1; 1; 0; 1 ]; [ 1; 1; 1; 0 ] ]

    let network = LocalNetwork(pcs, matrix, 1.0, 1.0, 1.0)
    network.MakeAMove()
    network.GetComputers() |> Array.forall _.IsInfected |> should equal true

[<Test>]
let ``Single uninfected computer should stay uninfected`` () =
    let pcs = [| { pcOC = MacOs; IsInfected = false } |]
    let matrix = array2D [ [ 0 ] ]
    let network = LocalNetwork(pcs, matrix, 1.0, 1.0, 1.0)
    network.MakeAMove()
    (network.GetComputers()[0]).IsInfected |> should equal false
