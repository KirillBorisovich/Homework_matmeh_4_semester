module BracketsSequence.Tests

open NUnit.Framework
open FsUnit
open BracketChecker

[<TestCase("[fsd]sdf{sdf}(sdf)")>]
[<TestCase("[asfdsd{asdf()fds}]xcv")>]
[<TestCase("{[jkjkl(ljkjlk)]}")>]
[<TestCase("asdfadsfdasf")>]
[<TestCase("")>]
let ``should return true`` input = isBalanced input |> should be True

[<TestCase("[{}")>]
[<TestCase("[{)]")>]
[<TestCase("[{}])")>]
[<TestCase("[{}(])")>]
let ``should return false`` input = isBalanced input |> should be False
