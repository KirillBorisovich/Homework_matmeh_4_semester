module PhoneDirectory.PhoneBook

open System.IO

type Name = Name of string
type PhoneNumber = PhoneNumber of string
type Contact = { Name: Name; Phone: PhoneNumber }

type PhoneBook =
    private
        { ByName: Map<Name, Set<PhoneNumber>>
          ByPhone: Map<PhoneNumber, Set<Name>> }

let createPhoneBook () =
    { ByName = Map.empty
      ByPhone = Map.empty }

let add phoneBook (name: Name) (phone: PhoneNumber) =
    let updatedByName =
        phoneBook.ByName
        |> Map.change name (fun existing -> Some(existing |> Option.defaultValue Set.empty |> Set.add phone))

    let updatedByPhone =
        phoneBook.ByPhone
        |> Map.change phone (fun existing -> Some(existing |> Option.defaultValue Set.empty |> Set.add name))

    { ByName = updatedByName
      ByPhone = updatedByPhone }

let findPhoneByName phonebook name = phonebook.ByName.TryFind(name)

let findNameByNumber phonebook phone = phonebook.ByPhone.TryFind(phone)

let toSeq phoneBook =
    phoneBook.ByName
    |> Map.toSeq
    |> Seq.collect (fun (name, phones) -> phones |> Seq.map (fun phone -> { Name = name; Phone = phone }))

let writeToFile phoneBook path =
    let data =
        phoneBook
        |> toSeq
        |> Seq.map (fun contact ->
            let (Name nameStr) = contact.Name
            let (PhoneNumber phoneStr) = contact.Phone
            $"{nameStr} {phoneStr}")

    File.WriteAllLines(path, data)

let readFromFile phoneBook path =
    if not (File.Exists path) then
        None
    else
        File.ReadAllLines(path)
        |> Array.toList
        |> List.fold
            (fun acc str ->
                let parts = str.Split([| ' ' |], 2)

                if parts.Length >= 2 then
                    let name = Name parts[0]
                    let phone = PhoneNumber parts[1]
                    add acc name phone
                else
                    acc)
            phoneBook
        |> Some
