module Tests

open Xunit
open Quibble.Xunit

[<Fact>]
let ``True vs false yields a boolean value mismatch.`` () =
    let ex = Assert.Throws<JsonAssertException>(fun () -> Assert.JsonEqual("true", "false"))
    Assert.Equal("Boolean value mismatch at $.\nExpected true but was false.", ex.UserMessage)

[<Fact>]
let ``True vs null yields a type mismatch.`` () =
    let ex = Assert.Throws<JsonAssertException>(fun () -> Assert.JsonEqual("true", "null"))
    Assert.Equal("Type mismatch at $.\nExpected the boolean true but was null.", ex.UserMessage)

[<Fact>]
let ``True vs 1 yields a type mismatch.`` () =
    let ex = Assert.Throws<JsonAssertException>(fun () -> Assert.JsonEqual("true", "1"))
    Assert.Equal("Type mismatch at $.\nExpected the boolean true but was the number 1.", ex.UserMessage)

[<Fact>]
let ``0 vs null yields a type mismatch.`` () =
    let ex = Assert.Throws<JsonAssertException>(fun () -> Assert.JsonEqual("0", "null"))
    Assert.Equal("Type mismatch at $.\nExpected the number 0 but was null.", ex.UserMessage)

[<Fact>]
let ``True vs "true" yields a type mismatch.`` () =
    let ex = Assert.Throws<JsonAssertException>(fun () -> Assert.JsonEqual("true", "\"true\""))
    Assert.Equal("Type mismatch at $.\nExpected the boolean true but was the string true.", ex.UserMessage)

[<Fact>]
let ``1 vs 2 yields a number value mismatch.`` () =
    let ex = Assert.Throws<JsonAssertException>(fun () -> Assert.JsonEqual("1", "2"))
    Assert.Equal("Number value mismatch at $.\nExpected 1 but was 2.", ex.UserMessage)

[<Fact>]
let ``[ 3 ] vs [ 3, 7 ] yields an array length mismatch.`` () =
    let ex = Assert.Throws<JsonAssertException>(fun () -> Assert.JsonEqual("[ 3 ]", "[ 3, 7 ]"))
    Assert.Equal("Array length mismatch at $.\nExpected 1 item but was 2.", ex.UserMessage)
    
[<Fact>]
let ``[ 3, 7 ] vs [ 7, 3 ] yields two number value mismatches.`` () =
    let ex = Assert.Throws<JsonAssertException>(fun () -> Assert.JsonEqual("[ 3, 7 ]", "[ 7, 3 ]"))
    Assert.Equal("Found 2 differences.\n# 1: Number value mismatch at $[0].\nExpected 3 but was 7.\n# 2: Number value mismatch at $[1].\nExpected 7 but was 3.", ex.UserMessage)
    Assert.Equal("Number value mismatch at $[0].\nExpected 3 but was 7.", ex.DiffMessages.[0])
    Assert.Equal("Number value mismatch at $[1].\nExpected 7 but was 3.", ex.DiffMessages.[1])

[<Fact>]
let ``[] vs {} yields a type mismatch.`` () =
    let ex = Assert.Throws<JsonAssertException>(fun () -> Assert.JsonEqual("[]", "{}"))
    Assert.Equal("Type mismatch at $.\nExpected an empty array but was an object.", ex.UserMessage)

[<Fact>]
let ``Widget property mismatch example.`` () =
    let expectedJsonString = """{ "item": "widget", "price": 12.20 }"""
    let actualJsonString = """{ "item": "widget", "quantity": 88, "in stock": true }"""
    let ex = Assert.Throws<JsonAssertException>(fun () -> Assert.JsonEqual(expectedJsonString, actualJsonString))
    Assert.Equal("Object mismatch at $.\nAdditional properties:\n - 'quantity' (number)\n - 'in stock' (bool)\nMissing property:\n - 'price' (number)", ex.UserMessage)

[<Fact>]
let ``Missing property yields an object mismatch.`` () =
    let expectedJsonString = """{ "item": "widget", "price": 12.20 }"""
    let actualJsonString = """{ "item": "widget" }"""
    let ex = Assert.Throws<JsonAssertException>(fun () -> Assert.JsonEqual(expectedJsonString, actualJsonString))
    Assert.Equal("Object mismatch at $.\nMissing property:\n - 'price' (number)", ex.UserMessage)
    
[<Fact>]
let ``Additional property yields an object mismatch.`` () =
    let expectedJsonString = """{ "item": "widget" }"""
    let actualJsonString = """{ "item": "widget", "price": 12.20 }"""
    let ex = Assert.Throws<JsonAssertException>(fun () -> Assert.JsonEqual(expectedJsonString, actualJsonString))
    Assert.Equal("Object mismatch at $.\nAdditional property:\n - 'price' (number)", ex.UserMessage)
    
[<Fact>]
let ``Books example``() =
    let expectedJsonString =
        """{
    "books": [{
        "title": "Data and Reality",
        "author": "William Kent"
    }, {
        "title": "Thinking Forth",
        "author": "Leo Brodie"
    }]
}"""
    let actualJsonString =
        """{
    "books": [{
        "title": "Data and Reality",
        "author": "William Kent",
        "edition": "2nd"
    }, {
        "title": "Thinking Forth",
        "author": "Chuck Moore"
    }]
}"""
    let ex = Assert.Throws<JsonAssertException>(fun () -> Assert.JsonEqual(expectedJsonString, actualJsonString))
    Assert.Equal("Found 2 differences.\n# 1: Object mismatch at $.books[0].\nAdditional property:\n - 'edition' (string)\n# 2: String value mismatch at $.books[1].author.\nExpected Leo Brodie but was Chuck Moore.", ex.UserMessage)

