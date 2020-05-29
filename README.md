[![Build status](https://ci.appveyor.com/api/projects/status/0v6946lhh480cgbk?svg=true)](https://ci.appveyor.com/project/NRKOpensource/quibble-xunit)
[![NuGet Status](https://img.shields.io/nuget/v/Quibble.Xunit.svg?style=flat)](https://www.nuget.org/packages/Quibble.Xunit/)

# Quibble.Xunit

Quibble.Xunit is an extension to [xUnit.net](https://xunit.net/) that does asserts on text strings with JSON content. It provides a method `JsonAssert.Equal` that compares two strings with JSON content - one with the JSON string we want to check, and one with the JSON we expect to see. If the strings do not contain the same JSON content, Quibble.Xunit will throw an exception and point you to the differences. It uses [Quibble](https://github.com/nrkno/Quibble) to provide the JSON diff.

## TL;DR 

![Comparing JSON](https://user-images.githubusercontent.com/1174441/83284025-6b1a8a80-a1dc-11ea-84db-9e69528a3385.png)

## Jump to examples

* [F# Examples](#f-examples)
* [C# Examples](#c-examples)

# Why Quibble.Xunit?

We often want to verify that some JSON text matches our expectations. A typical use case is writing tests for a web api that serves JSON responses. Without a JSON diff tool, we have two options: compare the JSON text as strings or deserialize the JSON into a data structure and compare the data structure with your expectations. 

Comparing JSON text as strings may be acceptable for very small JSON documents, but it quickly becomes a very poor experience as JSON documents have more than a couple of properties. Calling `Assert.Equal` on two strings will point you to the exact character position where the strings first deviate, but that's not really a helpful way of navigating a JSON document. In addition, calling `Assert.Equal` on strings will not help you in identifying multiple differences between the JSON documents at the same time.

Deserializing the response before comparing means that you have to write deserialization code (which may or may not be trivial) and in addition means you're comparing something else than what you really wanted to compare. In addition, you essentially have the same problem as with comparing strings. It is easy enough to check whether or not two objects deserialized from JSON are equal, but harder to figure out exactly how they're different if they are. Unless you want to peek into properties in your debugger, you must typically implement a suitable comparison mechanism if you want more detailed information. 

In contrast, Quibble.Xunit understands JSON and will point you directly to the differences in your JSON documents. Quibble.Xunit uses [JSONPath](https://goessner.net/articles/JsonPath/) syntax to indicate the right locations. In JSONPath syntax, `$` means the root of the document, whereas something like `$.books[1].author` means "the author property of the second element of the books array".

# F# Examples

```
dotnet add package Quibble.Xunit
```

```
open Quibble.Xunit
```

### Comparing numbers

#### Number example: 1 != 2

```
JsonAssert.Equal("1", "2")
```

throws a `JsonAssertException` and offers the following explanation:

```
Number value mismatch at $.
Expected 1 but was 2.
Expected: 1
Actual:   2
   at Quibble.Xunit.JsonAssert.Equal(String expectedJsonString, String actualJsonString)
```

#### Number example: 1.0 == 1

```
JsonAssert.Equal("1.0", "1")
```

does not protest, since JSON doesn't distinguish between integers and doubles. Hence `1.0` and `1` are just two different ways of writing the same number.

#### Number example: 123.4 vs 1.234E2

```
JsonAssert.Equal("123.4", "1.234E2")
```

does not protest either, since JSON supports scientific notation for numbers. Again, `123.4` and `1.234E2` are just two different ways of writing the same number.


### Comparing arrays

#### Array example: Number of items

```
JsonAssert.Equal("[ 3 ]", "[ 3, 7 ]")
```

throws a `JsonAssertException` and offers the following explanation:

```
Array length mismatch at $.
Expected 1 item but was 2.
Expected: [ 3 ]
Actual:   [ 3, 7 ]
   at Quibble.Xunit.JsonAssert.Equal(String expectedJsonString, String actualJsonString)
```

#### Array example: Item order matters

```
JsonAssert.Equal("[ 3, 7 ]", "[ 7, 3 ]")
```

throws a `JsonAssertException` and offers the following explanation:

```
Found 2 differences.
# 1: Number value mismatch at $[0].
Expected 3 but was 7.
# 2: Number value mismatch at $[1].
Expected 7 but was 3.
Expected: [ 3, 7 ]
Actual:   [ 7, 3 ]
   at Quibble.Xunit.JsonAssert.Equal(String expectedJsonString, String actualJsonString)
```

### Comparing objects

#### Object example: Property mismatch

```
let expected = """{ "item": "widget", "price": 12.20 }"""
let actual = """{ "item": "widget", "quantity": 88, "in stock": true }"""

JsonAssert.Equal(expected, actual)
```

throws a `JsonAssertException` and offers the following explanation:

```
Object mismatch at $.
Additional properties:
 - 'quantity' (number)
 - 'in stock' (bool)
Missing property:
 - 'price' (number)
Expected: { "item": "widget", "price": 12.20 }
Actual:   { "item": "widget", "quantity": 88, "in stock": true }
   at Quibble.Xunit.JsonAssert.Equal(String expectedJsonString, String actualJsonString)
```

#### Object example: Property order is irrelevant

```
let expected = """{ "item": "widget", "price": 12.20 }"""
let actual = """{ "price": 12.20, "item": "widget" }"""
JsonAssert.Equal(expected, actual)
```

does not protest, since JSON properties are unordered.

### Comparing apples and oranges 

#### Type mismatch example: number vs null

```
JsonAssert.Equal("0", "null")
```

throws a `JsonAssertException` and offers the following explanation:

```
Type mismatch at $.
Expected the number 0 but was null.
Expected: 0
Actual:   null
   at Quibble.Xunit.JsonAssert.Equal(String expectedJsonString, String actualJsonString)
```

#### Type mismatch example: array vs object

```
JsonAssert.Equal("[]", "{}")
```

throws a `JsonAssertException` and offers the following explanation:

```
Type mismatch at $.
Expected an empty array but was an object.
Expected: []
Actual:   {}
   at Quibble.Xunit.JsonAssert.Equal(String expectedJsonString, String actualJsonString)
```


### Composite example

```
let expected =
   """{
    "books": [{
        "title": "Data and Reality",
        "author": "William Kent"
    }, {
        "title": "Thinking Forth",
        "author": "Leo Brodie"
    }]
}"""

let actual =
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

JsonAssert.Equal(expected, actual)
```

throws a `JsonAssertException` and offers the following explanation:

```
Found 2 differences.
# 1: Object mismatch at $.books[0].
Additional property:
 - 'edition' (string)
# 2: String value mismatch at $.books[1].author.
Expected Leo Brodie but was Chuck Moore.
Expected: {
    "books": [{
        "title": "Data and Reality",
        "author": "William Kent"
    }, {
        "title": "Thinking Forth",
        "author": "Leo Brodie"
    }]
}
Actual:   {
    "books": [{
        "title": "Data and Reality",
        "author": "William Kent",
        "edition": "2nd"
    }, {
        "title": "Thinking Forth",
        "author": "Chuck Moore"
    }]
}
   at Quibble.Xunit.JsonAssert.Equal(String expectedJsonString, String actualJsonString)
```

# C# Examples

```
dotnet add package Quibble.Xunit
```

```
using Quibble.Xunit
```

### Comparing numbers

#### Number example: 1 != 2

```
JsonAssert.Equal("1", "2");
```

throws a `JsonAssertException` and offers the following explanation:

```
Number value mismatch at $.
Expected 1 but was 2.
Expected: 1
Actual:   2
   at Quibble.Xunit.JsonAssert.Equal(String expectedJsonString, String actualJsonString)
```

#### Number example: 1.0 == 1

```
JsonAssert.Equal("1.0", "1");
```

does not protest, since JSON doesn't distinguish between integers and doubles. Hence `1.0` and `1` are just two different ways of writing the same number.

#### Number example: 123.4 vs 1.234E2

```
JsonAssert.Equal("123.4", "1.234E2")
```

does not protest either, since JSON supports scientific notation for numbers. Again, `123.4` and `1.234E2` are just two different ways of writing the same number.


### Comparing arrays

#### Array example: Number of items

```
JsonAssert.Equal("[ 3 ]", "[ 3, 7 ]")
```

throws a `JsonAssertException` and offers the following explanation:

```
Array length mismatch at $.
Expected 1 item but was 2.
Expected: [ 3 ]
Actual:   [ 3, 7 ]
   at Quibble.Xunit.JsonAssert.Equal(String expectedJsonString, String actualJsonString)
```

#### Array example: Item order matters

```
JsonAssert.Equal("[ 3, 7 ]", "[ 7, 3 ]")
```

throws a `JsonAssertException` and offers the following explanation:

```
Found 2 differences.
# 1: Number value mismatch at $[0].
Expected 3 but was 7.
# 2: Number value mismatch at $[1].
Expected 7 but was 3.
Expected: [ 3, 7 ]
Actual:   [ 7, 3 ]
   at Quibble.Xunit.JsonAssert.Equal(String expectedJsonString, String actualJsonString)
```

### Comparing objects

#### Object example: Property mismatch

```
var expected = @"{ ""item"": ""widget"", ""price"": 12.20 }";
var actual = @"{ ""item"": ""widget"", ""quantity"": 88, ""in stock"": true }";

JsonAssert.Equal(expected, actual);
```

throws a `JsonAssertException` and offers the following explanation:

```
Object mismatch at $.
Additional properties:
 - 'quantity' (number)
 - 'in stock' (bool)
Missing property:
 - 'price' (number)
Expected: { "item": "widget", "price": 12.20 }
Actual:   { "item": "widget", "quantity": 88, "in stock": true }
   at Quibble.Xunit.JsonAssert.Equal(String expectedJsonString, String actualJsonString)
```

#### Object example: Property order is irrelevant

```
var expected = @"{ ""item"": ""widget"", ""price"": 12.20 }";
var actual = @"{  ""price"": 12.20,  ""item"": ""widget"" }";

JsonAssert.Equal(expected, actual)
```

does not protest, since JSON properties are unordered.

### Comparing apples and oranges 

#### Type mismatch example: number vs null

```
JsonAssert.Equal("0", "null");
```

throws a `JsonAssertException` and offers the following explanation:

```
Type mismatch at $.
Expected the number 0 but was null.
Expected: 0
Actual:   null
   at Quibble.Xunit.JsonAssert.Equal(String expectedJsonString, String actualJsonString)
```

#### Type mismatch example: array vs object

```
JsonAssert.Equal("[]", "{}");
```

throws a `JsonAssertException` and offers the following explanation:

```
Type mismatch at $.
Expected an empty array but was an object.
Expected: []
Actual:   {}
   at Quibble.Xunit.JsonAssert.Equal(String expectedJsonString, String actualJsonString)
```

### Composite example

```
var expected = 
    @"{
        ""books"": [{
            ""title"": ""Data and Reality"",
            ""author"": ""William Kent"" 
        }, {
            ""title"": ""Thinking Forth"",
            ""author"": ""Leo Brodie""
        }]
    }";

var actual =
    @"{
        ""books"": [{
            ""title"": ""Data and Reality"",
            ""author"": ""William Kent"",
            ""edition"": ""2nd""
        }, {
            ""title"": ""Thinking Forth"",
            ""author"": ""Chuck Moore""
        }]
    }";
JsonAssert.Equal(expected, actual);
```

throws a `JsonAssertException` and offers the following explanation:

```
Found 2 differences.
# 1: Object mismatch at $.books[0].
Additional property:
 - 'edition' (string)
# 2: String value mismatch at $.books[1].author.
Expected Leo Brodie but was Chuck Moore.
Expected: {
    "books": [{
        "title": "Data and Reality",
        "author": "William Kent"
    }, {
        "title": "Thinking Forth",
        "author": "Leo Brodie"
    }]
}
Actual:   {
    "books": [{
        "title": "Data and Reality",
        "author": "William Kent",
        "edition": "2nd"
    }, {
        "title": "Thinking Forth",
        "author": "Chuck Moore"
    }]
}
   at Quibble.Xunit.JsonAssert.Equal(String expectedJsonString, String actualJsonString)
```

