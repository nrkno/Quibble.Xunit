module Tests

open Xunit
open Quibble.Xunit

[<Fact>]
let ``True vs false is a boolean value mismatch.`` () =
    let ex = Assert.Throws<JsonAssertException>(fun () -> Assert.JsonEqual("true", "false"));
    Assert.Equal("Boolean value mismatch at $.\nExpected true but was false.", ex.UserMessage)

[<Fact>]
let ``True vs null is a kind mismatch.`` () =
    let ex = Assert.Throws<JsonAssertException>(fun () -> Assert.JsonEqual("true", "null"));
    Assert.Equal("Kind mismatch at $.\nExpected the boolean true but was null.", ex.UserMessage)

[<Fact>]
let ``True vs 1 is a kind mismatch.`` () =
    let ex = Assert.Throws<JsonAssertException>(fun () -> Assert.JsonEqual("true", "1"));
    Assert.Equal("Kind mismatch at $.\nExpected the boolean true but was the number 1.", ex.UserMessage)

[<Fact>]
let ``True vs "true" is a kind mismatch.`` () =
    let ex = Assert.Throws<JsonAssertException>(fun () -> Assert.JsonEqual("true", "\"true\""));
    Assert.Equal("Kind mismatch at $.\nExpected the boolean true but was the string true.", ex.UserMessage)