module Tests

open Xunit
open Quibble.Xunit

[<Fact>]
let ``True vs false yields a boolean value mismatch.`` () =
    let ex = Assert.Throws<JsonAssertException>(fun () -> JsonAssert.Equal("true", "false"))
    Assert.Equal("Boolean value mismatch at $.\nExpected true but was false.", ex.UserMessage)

[<Fact>]
let ``True vs null yields a type mismatch.`` () =
    let ex = Assert.Throws<JsonAssertException>(fun () -> JsonAssert.Equal("true", "null"))
    Assert.Equal("Type mismatch at $.\nExpected the boolean true but was null.", ex.UserMessage)

[<Fact>]
let ``True vs 1 yields a type mismatch.`` () =
    let ex = Assert.Throws<JsonAssertException>(fun () -> JsonAssert.Equal("true", "1"))
    Assert.Equal("Type mismatch at $.\nExpected the boolean true but was the number 1.", ex.UserMessage)

[<Fact>]
let ``0 vs null yields a type mismatch.`` () =
    let ex = Assert.Throws<JsonAssertException>(fun () -> JsonAssert.Equal("0", "null"))
    Assert.Equal("Type mismatch at $.\nExpected the number 0 but was null.", ex.UserMessage)

[<Fact>]
let ``True vs "true" yields a type mismatch.`` () =
    let ex = Assert.Throws<JsonAssertException>(fun () -> JsonAssert.Equal("true", "\"true\""))
    Assert.Equal("Type mismatch at $.\nExpected the boolean true but was the string true.", ex.UserMessage)

[<Fact>]
let ``1 vs 2 yields a number value mismatch.`` () =
    let ex = Assert.Throws<JsonAssertException>(fun () -> JsonAssert.Equal("1", "2"))
    Assert.Equal("Number value mismatch at $.\nExpected 1 but was 2.", ex.UserMessage)

[<Fact>]
let ``[ 3 ] vs [ 3, 7 ] yields an array mismatch with one additional item.`` () =
    let ex = Assert.Throws<JsonAssertException>(fun () -> JsonAssert.Equal("[ 3 ]", "[ 3, 7 ]"))
    Assert.Equal("Array mismatch at $.\nAdditional item:\n + [1]: the number 7", ex.UserMessage)
    
[<Fact>]
let ``[ 3, 7 ] vs [ 7, 3 ] yields an array mismatch with one missing and one additional item.`` () =
    // [ 3 ] is picked as the common sublist.
    let ex = Assert.Throws<JsonAssertException>(fun () -> JsonAssert.Equal("[ 3, 7 ]", "[ 7, 3 ]"))
    Assert.Equal("Array mismatch at $.\nMissing item:\n - [1]: the number 7\nAdditional item:\n + [0]: the number 7", ex.UserMessage)

[<Fact>]
let ``[ 3, 7 ] vs [ 3, 5 ] yields a value difference at the appropriate index.`` () =
    let ex = Assert.Throws<JsonAssertException>(fun () -> JsonAssert.Equal("[ 3, 7 ]", "[ 3, 5 ]"))
    Assert.Equal("Number value mismatch at $[1].\nExpected 7 but was 5.", ex.UserMessage)

[<Fact>]
let ``[] vs [ 3, 7 ] yields an array mismatch with two additional items.`` () =
    let ex = Assert.Throws<JsonAssertException>(fun () -> JsonAssert.Equal("[]", "[ 3, 7 ]"))
    Assert.Equal("Array mismatch at $.\nAdditional items:\n + [0]: the number 3\n + [1]: the number 7", ex.UserMessage)

[<Fact>]
let ``[ 3, 7 ] vs [] yields an array mismatch with two missing items.`` () =
    let ex = Assert.Throws<JsonAssertException>(fun () -> JsonAssert.Equal("[ 3, 7 ]", "[]"))
    Assert.Equal("Array mismatch at $.\nMissing items:\n - [0]: the number 3\n - [1]: the number 7", ex.UserMessage)

[<Fact>]
let ``[] vs {} yields a type mismatch.`` () =
    let ex = Assert.Throws<JsonAssertException>(fun () -> JsonAssert.Equal("[]", "{}"))
    Assert.Equal("Type mismatch at $.\nExpected an empty array but was an object.", ex.UserMessage)

[<Fact>]
let ``Widget property mismatch example.`` () =
    let expectedJsonString = """{ "item": "widget", "price": 12.20 }"""
    let actualJsonString = """{ "item": "widget", "quantity": 88, "in stock": true }"""
    let ex = Assert.Throws<JsonAssertException>(fun () -> JsonAssert.Equal(expectedJsonString, actualJsonString))
    Assert.Equal("Object mismatch at $.\nMissing property:\n - 'price' (number)\nAdditional properties:\n + 'quantity' (number)\n + 'in stock' (bool)", ex.UserMessage)

[<Fact>]
let ``Missing property yields an object mismatch.`` () =
    let expectedJsonString = """{ "item": "widget", "price": 12.20 }"""
    let actualJsonString = """{ "item": "widget" }"""
    let ex = Assert.Throws<JsonAssertException>(fun () -> JsonAssert.Equal(expectedJsonString, actualJsonString))
    Assert.Equal("Object mismatch at $.\nMissing property:\n - 'price' (number)", ex.UserMessage)
    
[<Fact>]
let ``Additional property yields an object mismatch by default.`` () =
    let expectedJsonString = """{ "item": "widget" }"""
    let actualJsonString = """{ "item": "widget", "price": 12.20 }"""
    let ex = Assert.Throws<JsonAssertException>(fun () -> JsonAssert.Equal(expectedJsonString, actualJsonString))
    Assert.Equal("Object mismatch at $.\nAdditional property:\n + 'price' (number)", ex.UserMessage)

[<Fact>]
let ``Allowing additional properties with config override.`` () =
    let expectedJsonString = """{ "item": "widget" }"""
    let actualJsonString = """{ "item": "widget", "price": 12.20 }"""
    let diffConfig = {
        allowAdditionalProperties = true
    }
    JsonAssert.EqualOverrideDefault(expectedJsonString, actualJsonString, diffConfig)

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
    let ex = Assert.Throws<JsonAssertException>(fun () -> JsonAssert.Equal(expectedJsonString, actualJsonString))
    Assert.Equal("Found 2 differences.\n# 1: Object mismatch at $.books[0].\nAdditional property:\n + 'edition' (string)\n# 2: String value mismatch at $.books[1].author.\nExpected Leo Brodie but was Chuck Moore.", ex.UserMessage)

[<Fact>]
let ``Long array example - with single replacement``() =
    let expectedJsonString =
        """[{
    "title": "Data and Reality",
    "author": "William Kent"
}, {
    "title": "Thinking Forth",
    "author": "Leo Brodie"
}, {
    "title": "Programmers at Work",
    "author": "Susan Lammers"
}, {
    "title": "The Little Schemer",
    "authors": [ "Daniel P. Friedman", "Matthias Felleisen" ]
}, {
    "title": "Object Design",
    "authors": [ "Rebecca Wirfs-Brock", "Alan McKean" ]
}, {
    "title": "Domain Modelling made Functional",
    "author": "Scott Wlaschin"
}, {
    "title": "The Psychology of Computer Programming",
    "author": "Gerald M. Weinberg"
}, {
    "title": "Exercises in Programming Style",
    "author": "Cristina Videira Lopes"
}, {
    "title": "Land of Lisp",
    "author": "Conrad Barski"
}]"""
    let actualJsonString =
        """[{
    "title": "Data and Reality",
    "author": "William Kent"
}, {
    "title": "Thinking Forth",
    "author": "Leo Brodie"
}, {
    "title": "Coders at Work",
    "author": "Peter Seibel"
}, {
    "title": "The Little Schemer",
    "authors": [ "Daniel P. Friedman", "Matthias Felleisen" ]
}, {
    "title": "Object Design",
    "authors": [ "Rebecca Wirfs-Brock", "Alan McKean" ]
}, {
    "title": "Domain Modelling made Functional",
    "author": "Scott Wlaschin"
}, {
    "title": "The Psychology of Computer Programming",
    "author": "Gerald M. Weinberg"
}, {
    "title": "Exercises in Programming Style",
    "author": "Cristina Videira Lopes"
}, {
    "title": "Land of Lisp",
    "author": "Conrad Barski"
}]"""
    let ex = Assert.Throws<JsonAssertException>(fun () -> JsonAssert.Equal(expectedJsonString, actualJsonString))
    Assert.Equal("Found 2 differences.\n# 1: String value mismatch at $[2].title.\nExpected Programmers at Work but was Coders at Work.\n# 2: String value mismatch at $[2].author.\nExpected Susan Lammers but was Peter Seibel.", ex.UserMessage)

[<Fact>]
let ``Long array example - with modifications``() =
    let expectedJsonString =
        """[{
    "title": "Data and Reality",
    "author": "William Kent"
}, {
    "title": "Thinking Forth",
    "author": "Leo Brodie"
}, {
    "title": "Programmers at Work",
    "author": "Susan Lammers"
}, {
    "title": "The Little Schemer",
    "authors": [ "Daniel P. Friedman", "Matthias Felleisen" ]
}, {
    "title": "Object Design",
    "authors": [ "Rebecca Wirfs-Brock", "Alan McKean" ]
}, {
    "title": "Domain Modelling made Functional",
    "author": "Scott Wlaschin"
}, {
    "title": "The Psychology of Computer Programming",
    "author": "Gerald M. Weinberg"
}, {
    "title": "Exercises in Programming Style",
    "author": "Cristina Videira Lopes"
}, {
    "title": "Land of Lisp",
    "author": "Conrad Barski"
}]"""
    let actualJsonString =
        """[{
    "title": "Data and Reality",
    "author": "William Kent"
}, {
    "title": "Thinking Forth",
    "author": "Leo Brodie"
}, {
    "title": "Coders at Work",
    "author": "Peter Seibel"
}, {
    "title": "The Little Schemer",
    "authors": [ "Daniel P. Friedman", "Matthias Felleisen" ]
}, {
    "title": "Object Design",
    "authors": [ "Rebecca Wirfs-Brock", "Alan McKean" ]
}, {
    "title": "Domain Modelling made Functional",
    "author": "Scott Wlaschin"
}, {
    "title": "The Psychology of Computer Programming",
    "author": "Gerald M. Weinberg"
}, {
    "title": "Turtle Geometry",
    "authors": [ "Hal Abelson", "Andrea diSessa" ]
}, {
    "title": "Exercises in Programming Style",
    "author": "Cristina Videira Lopes"
}, {
    "title": "Land of Lisp",
    "author": "Conrad Barski"
}]"""
    let ex = Assert.Throws<JsonAssertException>(fun () -> JsonAssert.Equal(expectedJsonString, actualJsonString))
    Assert.Equal("Array mismatch at $.\nMissing item:\n - [2]: {\n  'title': 'Programmers at Work'\n  'author': 'Susan Lammers'\n}\nAdditional items:\n + [2]: {\n  'title': 'Coders at Work'\n  'author': 'Peter Seibel'\n}\n + [7]: {\n  'title': 'Turtle Geometry'\n  'authors': [ 2 items ]\n}", ex.UserMessage)

[<Fact>]
let ``Long array example - with arrays``() =
    let expectedJsonString =
        """[ 
[ "Data and Reality", "William Kent" ],
[ "Thinking Forth", "Leo Brodie" ],
[ "Programmers at Work", "Susan Lammers" ],
[ "The Little Schemer", "Daniel P. Friedman", "Matthias Felleisen" ],
[ "Object Design", "Rebecca Wirfs-Brock", "Alan McKean" ],
[ "Domain Modelling made Functional", "Scott Wlaschin" ],
[ "The Psychology of Computer Programming", "Gerald M. Weinberg" ],
[ "Exercises in Programming Style", "Cristina Videira Lopes" ],
[ "Land of Lisp", "Conrad Barski" ]
]"""
    let actualJsonString =
        """[
[ "Data and Reality", "William Kent" ],
[ "Thinking Forth", "Leo Brodie" ],
[ "Coders at Work", "Peter Seibel" ],
[ "The Little Schemer", "Daniel P. Friedman", "Matthias Felleisen" ],
[ "Object Design", "Rebecca Wirfs-Brock", "Alan McKean" ],
[ "Domain Modelling made Functional", "Scott Wlaschin" ],
[ "The Psychology of Computer Programming", "Gerald M. Weinberg" ],
[ "Turtle Geometry", "Hal Abelson", "Andrea diSessa" ],
[ "Exercises in Programming Style", "Cristina Videira Lopes" ],
[ "Land of Lisp", "Conrad Barski" ]
]"""
    let ex = Assert.Throws<JsonAssertException>(fun () -> JsonAssert.Equal(expectedJsonString, actualJsonString))
    Assert.Equal("Array mismatch at $.\nMissing item:\n - [2]: [\n  'Programmers at Work'\n  'Susan Lammers'\n]\nAdditional items:\n + [2]: [\n  'Coders at Work'\n  'Peter Seibel'\n]\n + [7]: [\n  'Turtle Geometry'\n  'Hal Abelson'\n  'Andrea diSessa'\n]", ex.UserMessage)
