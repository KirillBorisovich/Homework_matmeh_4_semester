module LocalNetwork.Network

open System
open System.Collections.Generic

type OS =
    | MacOs
    | Linux
    | Windows

type Computer = { OS: OS; mutable IsInfected: bool }

type InfectionProbabilities =
    { MacProb: float
      LinuxProb: float
      WindowsProb: float }

    member this.ForOS(os: OS) =
        match os with
        | MacOs -> this.MacProb
        | Linux -> this.LinuxProb
        | Windows -> this.WindowsProb

type LocalNetwork private (computers: Computer[], adjacencyMatrix: bool[,], probabilities: InfectionProbabilities) =

    let random = Random(Environment.TickCount)

    let mutable currentComputers =
        computers |> Array.map (fun c -> { c with IsInfected = c.IsInfected })

    let size = computers.Length

    static member Create(computers: Computer[], adjacencyMatrix: bool[,], probabilities: InfectionProbabilities) =
        if
            adjacencyMatrix.GetLength(0) <> computers.Length
            || adjacencyMatrix.GetLength(1) <> computers.Length
        then
            invalidArg "adjacencyMatrix" "Matrix dimensions must match number of computers."

        if
            probabilities.MacProb < 0.0
            || probabilities.MacProb > 1.0
            || probabilities.LinuxProb < 0.0
            || probabilities.LinuxProb > 1.0
            || probabilities.WindowsProb < 0.0
            || probabilities.WindowsProb > 1.0
        then
            invalidArg "probabilities" "All probabilities must be between 0.0 and 1.0."

        LocalNetwork(computers, adjacencyMatrix, probabilities)

    member _.Computers =
        currentComputers |> Array.map (fun c -> { c with IsInfected = c.IsInfected })

    member private _.TryInfect(index: int) : bool =
        let computer = currentComputers[index]

        if not computer.IsInfected then
            let prob = probabilities.ForOS(computer.OS)
            random.NextDouble() < prob
        else
            false

    member private this.Step() : bool =
        let newlyInfected = HashSet<int>()

        for i in 0 .. size - 1 do
            if currentComputers[i].IsInfected then
                for j in 0 .. size - 1 do
                    if adjacencyMatrix[i, j] && not currentComputers[j].IsInfected then
                        if this.TryInfect(j) then
                            newlyInfected.Add(j) |> ignore

        for idx in newlyInfected do
            currentComputers[idx].IsInfected <- true

        newlyInfected.Count > 0

    member this.RunSimulation(printState: Computer[] -> unit) =
        printState this.Computers
        let mutable changed = true

        while changed do
            changed <- this.Step()

            if changed then
                printState this.Computers

    member this.MakeAMove() = this.Step() |> ignore
