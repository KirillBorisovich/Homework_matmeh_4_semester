module PhoneDirectory.Tests

open System.IO
open NUnit.Framework
open FsUnit
open FsCheck
open PhoneDirectory.PhoneBook

[<TestCase("Alex", "123456")>]
[<TestCase("Maria", "+79001112233")>]
let ``should find added contact with correct data`` (name: string) (phone: string) =
    let book = add (createPhoneBook ()) (Name name) (PhoneNumber phone)

    findPhoneByName book (Name name)
    |> should equal (Some(set [ PhoneNumber phone ]))

    findNameByNumber book (PhoneNumber phone)
    |> should equal (Some(set [ Name name ]))

[<TestCase("Ghost")>]
[<TestCase("Unknown")>]
let ``should return None for missing name`` name =
    findPhoneByName (createPhoneBook ()) (Name name) |> should equal None

[<TestCase("123456")>]
[<TestCase("000000")>]
let ``should return None for missing phone`` phone =
    findNameByNumber (createPhoneBook ()) (PhoneNumber phone) |> should equal None

[<TestCase("Dave", "555")>]
let ``should contain exactly one correct entry on duplicate add`` name phone =
    let book =
        createPhoneBook ()
        |> fun b -> add b (Name name) (PhoneNumber phone)
        |> fun b -> add b (Name name) (PhoneNumber phone)

    findPhoneByName book (Name name)
    |> should equal (Some(set [ PhoneNumber phone ]))

[<Test>]
let ``should save to file and load back correctly`` () =
    let fileName = "test_database.txt"

    if File.Exists fileName then
        File.Delete fileName

    let originalBook =
        createPhoneBook ()
        |> fun b -> add b (Name "Alex") (PhoneNumber "111-22-33")
        |> fun b -> add b (Name "Bob") (PhoneNumber "+79990000000")

    try
        writeToFile originalBook fileName
        let loadedBook = readFromFile (createPhoneBook ()) fileName
        loadedBook |> should not' (equal None)

        let originalList = toSeq originalBook |> Seq.sort |> Seq.toList
        let loadedList = toSeq loadedBook.Value |> Seq.sort |> Seq.toList
        loadedList |> should equal originalList
    finally
        if File.Exists fileName then
            File.Delete fileName

[<Test>]
let ``should return None if file does not exist`` () =
    let fileName = "non_existent_file_12345.txt"

    if File.Exists fileName then
        File.Delete fileName

    readFromFile (createPhoneBook ()) fileName |> should equal None

let addingAContactMakesItFindable (name: string) (phone: string) =
    let book = add (createPhoneBook ()) (Name name) (PhoneNumber phone)
    findPhoneByName book (Name name) |> Option.isSome

let addingSameContactTwiceIsIdempotent (name: string) (phone: string) =
    let book1 = add (createPhoneBook ()) (Name name) (PhoneNumber phone)
    let book2 = add book1 (Name name) (PhoneNumber phone)
    (toSeq book1 |> Seq.sort |> Seq.toList) = (toSeq book2 |> Seq.sort |> Seq.toList)

[<Test>]
let ``adding a contact makes it findable`` () =
    Check.QuickThrowOnFailure addingAContactMakesItFindable

[<Test>]
let ``adding same contact twice is idempotent`` () =
    Check.QuickThrowOnFailure addingSameContactTwiceIsIdempotent
