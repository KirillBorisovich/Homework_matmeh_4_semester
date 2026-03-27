module LocalNetwork.Network

open System

type OC =
    | MacOs
    | Linux
    | Windows

type Computer = { pcOC: OC; mutable IsInfected: bool }

let private random = Random()

let private isInfected probability = random.NextDouble() < probability

let private makeAMove infectionProbability (pcArray: Computer[]) (adjacencyMatrix: int[,]) =
    let rec infectTheNeighbors indexPC (pcArray: Computer[]) (arrayOfNeighbors: int[]) acc =
        if acc >= arrayOfNeighbors.Length then
            ()
        else
            match arrayOfNeighbors[acc] with
            | 0 -> infectTheNeighbors indexPC pcArray arrayOfNeighbors (acc + 1)
            | 1 when acc = indexPC -> infectTheNeighbors indexPC pcArray arrayOfNeighbors (acc + 1)
            | 1 ->
                match pcArray[acc] with
                | item when item.IsInfected = true -> infectTheNeighbors indexPC pcArray arrayOfNeighbors (acc + 1)
                | item when item.IsInfected = false ->
                    item.IsInfected <- item.pcOC |> infectionProbability |> isInfected
                    infectTheNeighbors indexPC pcArray arrayOfNeighbors (acc + 1)
                | _ -> ()
            | _ -> ()

    let rec makeAMoveInternal (pcArray: Computer[]) (adjacencyMatrix: int[,]) acc =
        if acc >= adjacencyMatrix.GetLength 0 then
            ()
        else
            if pcArray[acc].IsInfected then
                infectTheNeighbors acc pcArray adjacencyMatrix[acc, *] 0

            makeAMoveInternal pcArray adjacencyMatrix (acc + 1)

    makeAMoveInternal pcArray adjacencyMatrix 0

type LocalNetwork(pcArray: Computer[], adjacencyMatrix: int[,], macProb, linuxProb, windowsProb) =
    let pcArrayCopy =
        pcArray
        |> Array.map (fun c ->
            { pcOC = c.pcOC
              IsInfected = c.IsInfected })

    let adjacencyMatrixCopy = Array2D.copy adjacencyMatrix

    let infectionProbability =
        function
        | MacOs -> macProb
        | Linux -> linuxProb
        | Windows -> windowsProb

    member this.MakeAMove() =
        makeAMove infectionProbability pcArrayCopy adjacencyMatrixCopy

    member this.GetComputers() =
        pcArrayCopy
        |> Array.map (fun c ->
            { pcOC = c.pcOC
              IsInfected = c.IsInfected })
