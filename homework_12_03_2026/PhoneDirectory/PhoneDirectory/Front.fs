module PhoneDirectory.Front

open System
open PhoneDirectory.PhoneBook

let rec front (book: PhoneBook) =
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
            let newBook = add book name phone
            printfn "Entry added."
            front newBook
        | 2 ->
            printf "Enter name: "
            let name = Console.ReadLine()

            match findPhoneByName book name with
            | Some phoneList -> phoneList |> List.iter (fun contact -> printfn $"Phone: %s{contact.Phone}")
            | None -> printfn "Name not found."

            front book
        | 3 ->
            printf "Enter phone: "
            let phone = Console.ReadLine()

            match findNameByNumber book phone with
            | Some nameList -> nameList |> List.iter (fun contact -> printfn $"Name: %s{contact.Name}")
            | None -> printfn "Phone not found."

            front book
        | 4 ->
            let contacts = toSeq book

            if Seq.isEmpty contacts then
                printfn "The phone book is empty."
            else
                contacts |> Seq.iter (fun c -> printfn $"%s{c.Name}: %s{c.Phone}")

            front book
        | 5 ->
            printf "Enter file path to save: "
            let path = Console.ReadLine()

            try
                writeToFile book path
                printfn "Data saved to file."
            with ex ->
                printfn $"Error saving file: %s{ex.Message}"

            front book
        | 6 ->
            printf "Enter file path to load: "
            let path = Console.ReadLine()

            try
                match readFromFile book path with
                | Some(newPhoneBook) ->
                    printfn "Data loaded from file."
                    front newPhoneBook
                | None ->
                    printfn "File not found or could not be read."
                    front book
            with ex ->
                printfn $"Error reading file: %s{ex.Message}"
                front book
        | _ ->
            printfn "Unknown command. Please enter a number between 0 and 6."
            front book
    | false, _ ->
        printfn "Invalid input. Please enter a number."
        front book

let initialization () = front createPhoneBook
