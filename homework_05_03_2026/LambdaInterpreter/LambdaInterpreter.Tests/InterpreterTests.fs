module LambdaInterpreter.Tests

open LambdaInterpreter.Interpreter
open NUnit.Framework
open FsUnit

let S =
    Lam("x", Lam("y", Lam("z", App(App(Var "x", Var "z"), App(Var "y", Var "z")))))

let K = Lam("x", Lam("y", Var "x"))
let I = Lam("x", Var "x")

[<Test>]
let ``Identity combinator reduces to argument`` () =
    let expr = App(I, Var "a")
    normalize expr |> should equal (Var "a")

[<Test>]
let ``S K K x reduces to x`` () =
    let expr = App(App(App(S, K), K), Var "x")
    normalize expr |> should equal (Var "x")

[<Test>]
let ``Alpha conversion prevents variable capture`` () =
    let expr = App(Lam("x", Lam("y", App(Var "x", Var "y"))), Var "y")
    let result = normalize expr

    match result with
    | Lam(yPrime, App(Var "y", Var yPrime2)) when yPrime = yPrime2 && yPrime <> "y" -> ()
    | _ -> Assert.Fail $"Expected alpha-converted term, got %A{result}"

[<TestCase("a", "b")>]
[<TestCase("var1", "var2")>]
[<TestCase("foo", "bar")>]
let ``K combinator returns first argument`` (arg1: string, arg2: string) =
    let expr = App(App(K, Var arg1), Var arg2)
    normalize expr |> should equal (Var arg1)
