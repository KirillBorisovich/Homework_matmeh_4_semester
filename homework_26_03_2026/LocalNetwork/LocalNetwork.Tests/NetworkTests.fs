module LocalNetwork.Tests.NetworkTests

open NUnit.Framework
open FsUnit
open LocalNetwork.Network

[<Test>]
let ``Constructor should throw if adjacency matrix is not square`` () =
    let computers = [| { OS = Windows; IsInfected = false } |]
    let matrix = array2D [ [ true; false ] ]

    (fun () ->
        LocalNetwork.Create(
            computers,
            matrix,
            { MacProb = 0.5
              LinuxProb = 0.5
              WindowsProb = 0.5 }
        )
        |> ignore)
    |> should throw typeof<System.ArgumentException>

[<Test>]
let ``Constructor should throw if pcArray length does not match matrix size`` () =
    let computers = [| { OS = Windows; IsInfected = false } |]
    let matrix = array2D [ [ true; false ]; [ false; true ] ]

    (fun () ->
        LocalNetwork.Create(
            computers,
            matrix,
            { MacProb = 0.5
              LinuxProb = 0.5
              WindowsProb = 0.5 }
        )
        |> ignore)
    |> should throw typeof<System.ArgumentException>

[<TestCase(-0.1)>]
[<TestCase(1.1)>]
let ``Constructor should throw if macProb is out of range`` (prob: float) =
    let computers = [| { OS = MacOs; IsInfected = false } |]
    let matrix = array2D [ [ false ] ]

    (fun () ->
        LocalNetwork.Create(
            computers,
            matrix,
            { MacProb = prob
              LinuxProb = 0.5
              WindowsProb = 0.5 }
        )
        |> ignore)
    |> should throw typeof<System.ArgumentException>

[<TestCase(-0.1)>]
[<TestCase(1.1)>]
let ``Constructor should throw if linuxProb is out of range`` (prob: float) =
    let computers = [| { OS = Linux; IsInfected = false } |]
    let matrix = array2D [ [ false ] ]

    (fun () ->
        LocalNetwork.Create(
            computers,
            matrix,
            { MacProb = 0.5
              LinuxProb = prob
              WindowsProb = 0.5 }
        )
        |> ignore)
    |> should throw typeof<System.ArgumentException>

[<TestCase(-0.1)>]
[<TestCase(1.1)>]
let ``Constructor should throw if windowsProb is out of range`` (prob: float) =
    let computers = [| { OS = Windows; IsInfected = false } |]
    let matrix = array2D [ [ false ] ]

    (fun () ->
        LocalNetwork.Create(
            computers,
            matrix,
            { MacProb = 0.5
              LinuxProb = 0.5
              WindowsProb = prob }
        )
        |> ignore)
    |> should throw typeof<System.ArgumentException>

[<Test>]
let ``No infection should spread if no computer is infected`` () =
    let computers =
        [| { OS = Windows; IsInfected = false }; { OS = Linux; IsInfected = false } |]

    let matrix = array2D [ [ false; true ]; [ true; false ] ]

    let network =
        LocalNetwork.Create(
            computers,
            matrix,
            { MacProb = 1.0
              LinuxProb = 1.0
              WindowsProb = 1.0 }
        )

    network.MakeAMove()

    network.Computers
    |> Array.forall (fun c -> not c.IsInfected)
    |> should equal true

[<Test>]
let ``With probability 1.0 all neighbors of infected computer should become infected`` () =
    let computers =
        [| { OS = Windows; IsInfected = true }
           { OS = Windows; IsInfected = false }
           { OS = Windows; IsInfected = false } |]

    let matrix =
        array2D [ [ false; true; true ]; [ true; false; false ]; [ true; false; false ] ]

    let network =
        LocalNetwork.Create(
            computers,
            matrix,
            { MacProb = 1.0
              LinuxProb = 1.0
              WindowsProb = 1.0 }
        )

    network.MakeAMove()

    network.Computers |> Array.forall _.IsInfected |> should equal true

[<Test>]
let ``With probability 0.0 no neighbor should become infected`` () =
    let computers =
        [| { OS = Windows; IsInfected = true }
           { OS = Windows; IsInfected = false }
           { OS = Windows; IsInfected = false } |]

    let matrix =
        array2D [ [ false; true; true ]; [ true; false; false ]; [ true; false; false ] ]

    let network =
        LocalNetwork.Create(
            computers,
            matrix,
            { MacProb = 0.0
              LinuxProb = 0.0
              WindowsProb = 0.0 }
        )

    network.MakeAMove()
    let result = network.Computers
    result[0].IsInfected |> should equal true
    result[1].IsInfected |> should equal false
    result[2].IsInfected |> should equal false

[<Test>]
let ``Disconnected computer should not be infected even with probability 1.0`` () =
    let computers =
        [| { OS = Windows; IsInfected = true }; { OS = Windows; IsInfected = false } |]

    let matrix = array2D [ [ false; false ]; [ false; false ] ]

    let network =
        LocalNetwork.Create(
            computers,
            matrix,
            { MacProb = 1.0
              LinuxProb = 1.0
              WindowsProb = 1.0 }
        )

    network.MakeAMove()
    let result = network.Computers
    result[0].IsInfected |> should equal true
    result[1].IsInfected |> should equal false

[<Test>]
let ``Already infected computer should stay infected`` () =
    let computers = [| { OS = Windows; IsInfected = true } |]
    let matrix = array2D [ [ false ] ]

    let network =
        LocalNetwork.Create(
            computers,
            matrix,
            { MacProb = 0.0
              LinuxProb = 0.0
              WindowsProb = 0.0 }
        )

    network.MakeAMove()
    (network.Computers[0]).IsInfected |> should equal true

[<Test>]
let ``Infection should propagate through a chain one step at a time`` () =
    let computers =
        [| { OS = Windows; IsInfected = true }
           { OS = Windows; IsInfected = false }
           { OS = Windows; IsInfected = false } |]

    let matrix =
        array2D [ [ false; true; false ]; [ true; false; true ]; [ false; true; false ] ]

    let network =
        LocalNetwork.Create(
            computers,
            matrix,
            { MacProb = 1.0
              LinuxProb = 1.0
              WindowsProb = 1.0 }
        )

    network.MakeAMove()
    let after1 = network.Computers
    after1[0].IsInfected |> should equal true
    after1[1].IsInfected |> should equal true
    after1[2].IsInfected |> should equal false

    network.MakeAMove()
    let after2 = network.Computers
    after2[2].IsInfected |> should equal true

[<Test>]
let ``Only Windows neighbor should be infected when only windowsProb is 1.0`` () =
    let computers =
        [| { OS = Linux; IsInfected = true }
           { OS = Windows; IsInfected = false }
           { OS = Linux; IsInfected = false }
           { OS = MacOs; IsInfected = false } |]

    let matrix =
        array2D
            [ [ false; true; true; true ]
              [ true; false; false; false ]
              [ true; false; false; false ]
              [ true; false; false; false ] ]

    let network =
        LocalNetwork.Create(
            computers,
            matrix,
            { MacProb = 0.0
              LinuxProb = 0.0
              WindowsProb = 1.0 }
        )

    network.MakeAMove()
    let result = network.Computers
    result[1].IsInfected |> should equal true
    result[2].IsInfected |> should equal false
    result[3].IsInfected |> should equal false

[<Test>]
let ``Only Linux neighbor should be infected when only linuxProb is 1.0`` () =
    let computers =
        [| { OS = Windows; IsInfected = true }
           { OS = Linux; IsInfected = false }
           { OS = Windows; IsInfected = false }
           { OS = MacOs; IsInfected = false } |]

    let matrix =
        array2D
            [ [ false; true; true; true ]
              [ true; false; false; false ]
              [ true; false; false; false ]
              [ true; false; false; false ] ]

    let network =
        LocalNetwork.Create(
            computers,
            matrix,
            { MacProb = 0.0
              LinuxProb = 1.0
              WindowsProb = 0.0 }
        )

    network.MakeAMove()
    let result = network.Computers
    result[1].IsInfected |> should equal true
    result[2].IsInfected |> should equal false
    result[3].IsInfected |> should equal false

[<Test>]
let ``Only MacOs neighbor should be infected when only macProb is 1.0`` () =
    let computers =
        [| { OS = Windows; IsInfected = true }
           { OS = MacOs; IsInfected = false }
           { OS = Windows; IsInfected = false }
           { OS = Linux; IsInfected = false } |]

    let matrix =
        array2D
            [ [ false; true; true; true ]
              [ true; false; false; false ]
              [ true; false; false; false ]
              [ true; false; false; false ] ]

    let network =
        LocalNetwork.Create(
            computers,
            matrix,
            { MacProb = 1.0
              LinuxProb = 0.0
              WindowsProb = 0.0 }
        )

    network.MakeAMove()
    let result = network.Computers
    result[1].IsInfected |> should equal true
    result[2].IsInfected |> should equal false
    result[3].IsInfected |> should equal false

[<Test>]
let ``Modifying original array after construction should not affect the network`` () =
    let computers = [| { OS = Windows; IsInfected = false } |]
    let matrix = array2D [ [ false ] ]

    let network =
        LocalNetwork.Create(
            computers,
            matrix,
            { MacProb = 0.5
              LinuxProb = 0.5
              WindowsProb = 0.5 }
        )

    computers[0].IsInfected <- true
    (network.Computers[0]).IsInfected |> should equal false

[<Test>]
let ``GetComputers should return a defensive copy`` () =
    let computers = [| { OS = Windows; IsInfected = false } |]
    let matrix = array2D [ [ false ] ]

    let network =
        LocalNetwork.Create(
            computers,
            matrix,
            { MacProb = 0.5
              LinuxProb = 0.5
              WindowsProb = 0.5 }
        )

    let copy = network.Computers
    copy[0].IsInfected <- true
    (network.Computers[0]).IsInfected |> should equal false

[<Test>]
let ``Fully connected network with probability 1.0 should infect all in one move`` () =
    let computers =
        [| { OS = Linux; IsInfected = true }
           { OS = Linux; IsInfected = false }
           { OS = Linux; IsInfected = false }
           { OS = Linux; IsInfected = false } |]

    let matrix =
        array2D
            [ [ false; true; true; true ]
              [ true; false; true; true ]
              [ true; true; false; true ]
              [ true; true; true; false ] ]

    let network =
        LocalNetwork.Create(
            computers,
            matrix,
            { MacProb = 1.0
              LinuxProb = 1.0
              WindowsProb = 1.0 }
        )

    network.MakeAMove()
    network.Computers |> Array.forall _.IsInfected |> should equal true

[<Test>]
let ``Single uninfected computer should stay uninfected`` () =
    let computers = [| { OS = MacOs; IsInfected = false } |]
    let matrix = array2D [ [ false ] ]

    let network =
        LocalNetwork.Create(
            computers,
            matrix,
            { MacProb = 1.0
              LinuxProb = 1.0
              WindowsProb = 1.0 }
        )

    network.MakeAMove()
    (network.Computers[0]).IsInfected |> should equal false
