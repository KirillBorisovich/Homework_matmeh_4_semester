module PhoneDirectory.Tests

open System.IO
open NUnit.Framework
open FsUnit
open FsCheck
open PhoneDirectory.PhoneBook

[<TestCase("Alex", "123456")>]
[<TestCase("Maria", "+79001112233")>]
let ``should find added contact with correct data`` name phone =
    let book = add createPhoneBook name phone

    let expected = Some [ { Name = name; Phone = phone } ]

    findPhoneByName book name |> should equal expected
    findNameByNumber book phone |> should equal expected

[<TestCase("Ghost")>]
[<TestCase("Unknown")>]
let ``should return None for missing name`` name =
    findPhoneByName createPhoneBook name |> should equal None

[<TestCase("123456")>]
[<TestCase("000000")>]
let ``should return None for missing phone`` phone =
    findNameByNumber createPhoneBook phone |> should equal None

[<TestCase("Dave", "555")>]
let ``should contain exactly one correct entry on duplicate add`` name phone =
    let book =
        createPhoneBook |> (fun b -> add b name phone) |> (fun b -> add b name phone)

    let expected = Some [ { Name = name; Phone = phone } ]

    findPhoneByName book name |> should equal expected

[<Test>]
let ``should save to file and load back correctly`` () =
    let fileName = "test_database.txt"

    if File.Exists fileName then
        File.Delete fileName

    let originalBook =
        createPhoneBook
        |> fun b -> add b "Alex" "111-22-33"
        |> fun b -> add b "Bob" "+79990000000"

    try
        writeToFile originalBook fileName
        let loadedBook = readFromFile createPhoneBook fileName

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

    readFromFile createPhoneBook fileName |> should equal None

let addingAContactMakesItFindable (name: NonNull<string>) (phone: NonNull<string>) =
    let n = name.Get
    let p = phone.Get
    let book = add createPhoneBook n p

    findPhoneByName book n |> Option.isSome


let addingSameContactTwiceIsIdempotent (name: NonNull<string>) (phone: NonNull<string>) =
    let n = name.Get
    let p = phone.Get

    let book1 = add createPhoneBook n p
    let book2 = add book1 n p

    let list1 = toSeq book1 |> Seq.sort |> Seq.toList
    let list2 = toSeq book2 |> Seq.sort |> Seq.toList

    list1 = list2

[<Test>]
let ``adding a contact makes it findable`` () =
    Check.QuickThrowOnFailure addingAContactMakesItFindable

[<Test>]
let ``adding same contact twice is idempotent`` () =
    Check.QuickThrowOnFailure addingSameContactTwiceIsIdempotent
