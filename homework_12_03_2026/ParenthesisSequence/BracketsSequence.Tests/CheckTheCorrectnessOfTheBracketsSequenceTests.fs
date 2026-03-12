module BracketsSequence.Tests

open NUnit.Framework
open FsUnit
open CheckTheCorrectnessOfTheBracketsSequence

[<TestCase("[fsd]sdf{sdf}(sdf)")>]
[<TestCase("[asfdsd{asdf()fds}]xcv")>]
[<TestCase("{[jkjkl(ljkjlk)]}")>]
[<TestCase("asdfadsfdasf")>]
[<TestCase("")>]
let ``should return true`` n =
    checkTheCorrectnessOfTheBracketsSequence n |> should be True

[<TestCase("[{}")>]
[<TestCase("[{)]")>]
[<TestCase("[{}])")>]
[<TestCase("[{}(])")>]
let ``should return false`` n =
    checkTheCorrectnessOfTheBracketsSequence n |> should be False
