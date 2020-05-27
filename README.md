[![Build status](https://ci.appveyor.com/api/projects/status/0v6946lhh480cgbk?svg=true)](https://ci.appveyor.com/project/NRKOpensource/json-quibble-xunit)
[![NuGet Status](https://img.shields.io/nuget/v/Quibble.Xunit.svg?style=flat)](https://www.nuget.org/packages/Quibble.Xunit/)

# Quibble.Xunit

Quibble.Xunit is an extension to XUnit that does asserts on text strings with JSON content. It provides a method `Assert.JsonEqual` that compares strings with JSON content. If the strings do not contain equal JSON content, Quibble.Xunit will point you to the differences.

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
