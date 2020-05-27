[![Build status](https://ci.appveyor.com/api/projects/status/0v6946lhh480cgbk?svg=true)](https://ci.appveyor.com/project/NRKOpensource/json-quibble-xunit)
[![NuGet Status](https://img.shields.io/nuget/v/Quibble.Xunit.svg?style=flat)](https://www.nuget.org/packages/Quibble.Xunit/)

# Quibble.Xunit

Quibble.Xunit is an extension to XUnit that does asserts on text strings with JSON content. It provides a method `Assert.JsonEqual` that compares strings with JSON content. If the strings do not contain equal JSON content, Quibble.Xunit will point you to the differences.

# Why Quibble.Xunit?

We often want to verify that some JSON text matches our expectations. A typical use case is writing tests for a web api that serves JSON responses. Without a JSON diff tool, we have two options: compare the JSON text as strings or deserialize the JSON into a data structure and compare the data structure with your expectations. 

Comparing JSON text as strings may be acceptable for very small JSON documents, but it quickly becomes a very poor experience as JSON documents have more than a couple of properties. Calling `Assert.Equal` on two strings will point you to the exact character position where the strings first deviate, but that's not really a helpful way of navigating a JSON document. In addition, calling `Assert.Equal` on strings will not help you in identifying multiple differences between the JSON documents at the same time.

Deserializing the response before comparing means that you have to write deserialization code (which may or may not be trivial) and in addition means you're comparing something else than what you really wanted to compare. In addition, you essentially have the same problem as with comparing strings. It is easy enough to check whether or not two objects deserialized from JSON are equal, but harder to figure out exactly how they're different if they are. Unless you want to peek into properties in your debugger, you must typically implement a suitable comparison mechanism if you want more detailed information. 

In contrast, Quibble.Xunit understands JSON and will point you directly to the differences in your JSON documents. Quibble.Xunit uses [JsonPath](https://goessner.net/articles/JsonPath/) syntax to point you to the right locations. In JsonPath syntax, `$` indicates the root of the document, whereas something like `$.books[1].author` means "the author property of the second element of the books array".

# Examples 

## F#

```
open Quibble.Xunit
```

### Comparing numbers

```
Assert.JsonEqual("1", "2")
```

throws a `JsonAssertException` and offers the following explanation:

```
Boolean value mismatch at $.
Expected true but was false.
Expected: true
Actual:   false
   at Quibble.Xunit.Assert.JsonEqual(String expectedJsonString, String actualJsonString)
```

### Comparing arrays

```
Assert.JsonEqual("[ 1 ]", "[ 2, 1 ]")
```

throws a `JsonAssertException` and offers the following explanation:

```
Array length mismatch at $.
Expected 1 item but was 2.
Expected: [ 1 ]
Actual:   [ 2, 1 ]
   at Quibble.Xunit.Assert.JsonEqual(String expectedJsonString, String actualJsonString)
```

### Comparing objects

```
let expected = """{ "item": "widget", "price": 12.20 }"""
let actual = """{ "item": "widget" }"""
Assert.JsonEqual(expected, actual)
```

throws a `JsonAssertException` and offers the following explanation:

```
Object mismatch at $.
Missing property:
price (number).
Expected: { "item": "widget", "price": 12.20 }
Actual:   { "item": "widget" }
   at Quibble.Xunit.Assert.JsonEqual(String expectedJsonString, String actualJsonString)
```

### Composite example

```
let expected = """{ "books": [ { "title": "Data and Reality", "author": "William Kent" }, { "title": "Thinking Forth", "author": "Leo Brodie" } ] }"""
let actual = """{ "books": [ { "title": "Data and Reality", "author": "William Kent" }, { "title": "Thinking Forth", "author": "Chuck Moore" } ] }"""
Assert.JsonEqual(expected, actual)
```

throws a `JsonAssertException` and offers the following explanation:

```
String value mismatch at $.books[1].author.
Expected Leo Brodie but was Chuck Moore.
Expected: { "books": [ { "title": "Data and Reality", "author": "William Kent" }, { "title": "Thinking Forth", "author": "Leo Brodie" } ] }
Actual:   { "books": [ { "title": "Data and Reality", "author": "William Kent" }, { "title": "Thinking Forth", "author": "Chuck Moore" } ] }
   at Quibble.Xunit.Assert.JsonEqual(String expectedJsonString, String actualJsonString)
```
