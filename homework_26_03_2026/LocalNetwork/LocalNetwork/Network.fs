module LocalNetwork.Network

open System
open System.Collections.Generic

type OC =
    | MacOs
    | Linux
    | Windows

type Computer = { pcOC: OC; mutable IsInfected: bool }

let private random = Random()

let private isInfected probability = random.NextDouble() < probability

let private makeAMove infectionProbability (pcArray: Computer[]) (adjacencyMatrix: int[,]) =
    let infectionSet = HashSet<int * Computer>()

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
                    infectionSet.Add((acc, item)) |> ignore
                    infectTheNeighbors indexPC pcArray arrayOfNeighbors (acc + 1)
                | _ -> ()
            | _ -> ()

    let rec makeAMoveInternal (pcArray: Computer[]) (adjacencyMatrix: int[,]) acc =
        if acc >= adjacencyMatrix.GetLength 0 then
            infectionSet
            |> Seq.iter (fun item ->
                let pc = snd item
                pc.IsInfected <- pc.pcOC |> infectionProbability |> isInfected)
        else
            if pcArray[acc].IsInfected then
                infectTheNeighbors acc pcArray adjacencyMatrix[acc, *] 0

            makeAMoveInternal pcArray adjacencyMatrix (acc + 1)

    makeAMoveInternal pcArray adjacencyMatrix 0

type LocalNetwork(pcArray: Computer[], adjacencyMatrix: int[,], macProb, linuxProb, windowsProb) =
    do
        if adjacencyMatrix.GetLength(0) <> adjacencyMatrix.GetLength(1) then
            invalidArg "adjacencyMatrix" "The matrix must be square."

        if pcArray.Length <> adjacencyMatrix.GetLength(0) then
            invalidOp "The number of computers does not match the size of the adjacency matrix."

        let hasInvalidValues =
            adjacencyMatrix |> Seq.cast<int> |> Seq.exists (fun x -> x <> 0 && x <> 1)

        if hasInvalidValues then
            invalidArg "adjacencyMatrix" "The adjacency matrix should contain only 0 or 1."

        let checkP p name =
            if p < 0.0 || p > 1.0 then
                invalidArg name $"The probability of {name} should be in the range [0, 1]."

        checkP macProb "macProb"
        checkP linuxProb "linuxProb"
        checkP windowsProb "windowsProb"

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
