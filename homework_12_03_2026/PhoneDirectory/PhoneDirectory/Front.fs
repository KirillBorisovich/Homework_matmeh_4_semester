module PhoneDirectory.Front

open System
open PhoneDirectory.PhoneBook

let rec run (book: PhoneBook) =
    printfn
        """
Phone directory

Enter number for:
0: exit
1: add an entry
2: find a phone number by name
3: find a name by phone
4: display all the current contents of the database
5: save current data to a file
6: read data from a file
"""

    printf "> "

    let input = Console.ReadLine()

    match Int32.TryParse(input) with
    | true, command ->
        match command with
        | 0 ->
            printfn "Goodbye!"
            ()
        | 1 ->
            printf "Enter name: "
            let name = Console.ReadLine()
            printf "Enter phone: "
            let phone = Console.ReadLine()
            let newBook = add book (Name name) (PhoneNumber phone)
            printfn "Entry added."
            run newBook
        | 2 ->
            printf "Enter name: "
            let name = Console.ReadLine()

            match findPhoneByName book (Name name) with
            | Some phones ->
                let count = Set.count phones
                printfn $"Found %d{count} entries:"
                phones |> Set.iter (fun (PhoneNumber p) -> printfn $"%s{p}")
            | None -> printfn "Name not found."

            run book
        | 3 ->
            printf "Enter phone: "
            let phone = Console.ReadLine()

            match findNameByNumber book (PhoneNumber phone) with
            | Some names ->
                let count = Set.count names
                printfn $"Found %d{count} entries:"
                names |> Set.iter (fun (Name n) -> printfn $"%s{n}")
            | None -> printfn "Phone not found."

            run book
        | 4 ->
            let contacts = toSeq book

            if Seq.isEmpty contacts then
                printfn "The phone book is empty."
            else
                contacts
                |> Seq.iter (fun c ->
                    let (Name n) = c.Name
                    let (PhoneNumber p) = c.Phone
                    printfn $"%s{n}: %s{p}")

            run book
        | 5 ->
            printf "Enter file path to save: "
            let path = Console.ReadLine()

            try
                writeToFile book path
                printfn "Data saved to file."
            with ex ->
                printfn $"Error saving file: %s{ex.Message}"

            run book
        | 6 ->
            printf "Enter file path to load: "
            let path = Console.ReadLine()

            try
                match readFromFile book path with
                | Some newPhoneBook ->
                    printfn "Data loaded from file."
                    run newPhoneBook
                | None ->
                    printfn "File not found or could not be read."
                    run book
            with ex ->
                printfn $"Error reading file: %s{ex.Message}"
                run book
        | _ ->
            printfn "Unknown command. Please enter a number between 0 and 6."
            run book
    | false, _ ->
        printfn "Invalid input. Please enter a number."
        run book

let start () = run (createPhoneBook ())
