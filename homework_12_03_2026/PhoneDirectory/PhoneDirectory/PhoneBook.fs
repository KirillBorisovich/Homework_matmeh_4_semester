module PhoneDirectory.PhoneBook

open System.IO

type Contact = { Name: string; Phone: string }

type PhoneBook =
    private
        { ByName: Map<string, list<Contact>>
          ByPhone: Map<string, list<Contact>> }

let createPhoneBook =
    { ByName = Map.empty
      ByPhone = Map.empty }

let add phoneBook name phone =
    let addAContactToASeparateMap (map: Map<string, list<Contact>>) key contact =
        map.Change(
            key,
            function
            | Some contacts ->
                let numberOfTheSameEntries =
                    contacts
                    |> List.filter (fun x -> x.Name = contact.Name && x.Phone = contact.Phone)
                    |> List.length

                if numberOfTheSameEntries = 0 then
                    Some(contact :: contacts)
                else
                    Some(contacts)

            | None -> Some [ contact ]
        )

    let contact = { Name = name; Phone = phone }

    { ByName = addAContactToASeparateMap phoneBook.ByName contact.Name contact
      ByPhone = addAContactToASeparateMap phoneBook.ByPhone contact.Phone contact }

let findPhoneByName phonebook name = phonebook.ByName.TryFind(name)

let findNameByNumber phonebook phone = phonebook.ByPhone.TryFind(phone)

let toSeq phoneBook =
    phoneBook.ByName |> Map.toList |> List.map snd |> Seq.concat

let writeToFile phoneBook path =
    let data =
        phoneBook |> toSeq |> Seq.map (fun contact -> $"{contact.Name} {contact.Phone}")

    File.WriteAllLines(path, data)

let readFromFile phoneBook path =
    if not (File.Exists path) then
        None
    else
        File.ReadAllLines(path)
        |> Array.toList
        |> List.fold
            (fun acc str ->
                let index = str.IndexOf(' ')
                add acc (str.Substring(0, index)) (str.Substring(index + 1)))
            phoneBook
        |> Some
