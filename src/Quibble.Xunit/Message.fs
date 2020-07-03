namespace Quibble.Xunit

module Message =

    open Quibble
    
    let private toObjectSummary (props : (string * JsonValue) list) : string =
        let toPropLine (p: string, v : JsonValue) : string =
            let valueStr =
                match v with 
                | JsonTrue -> "true"
                | JsonFalse -> "false"
                | JsonString s -> sprintf "'%s'" s
                | JsonNumber (_, t) -> t
                | JsonArray items ->
                    let itemCount = items |> List.length
                    match itemCount with
                    | 0 -> "[]"
                    | 1 -> "[ 1 item ]"
                    | n -> sprintf "[ %d items ]" n
                | JsonObject props ->
                    let propCount = props |> List.length
                    match propCount with
                    | 0 -> "{}"
                    | 1 -> "{ 1 property }"
                    | n -> sprintf "{ %d properties }" n
                | JsonNull -> "null"
                | JsonUndefined -> "undefined"
            sprintf "  '%s': %s" p valueStr
        let propLines = props |> List.map toPropLine
        let lines = [ "{" ] @ propLines @ [ "}" ]
        let result = String.concat "\n" lines
        result
        
    let private toArraySummary (items : JsonValue list) : string =
        let toItemLine (v : JsonValue) : string =
            let valueStr =
                match v with 
                | JsonTrue -> "true"
                | JsonFalse -> "false"
                | JsonString s -> sprintf "'%s'" s
                | JsonNumber (_, t) -> t
                | JsonArray items ->
                    let itemCount = items |> List.length
                    match itemCount with
                    | 0 -> "[]"
                    | 1 -> "[ 1 item ]"
                    | n -> sprintf "[ %d items ]" n
                | JsonObject props ->
                    let propCount = props |> List.length
                    match propCount with
                    | 0 -> "{}"
                    | 1 -> "{ 1 property }"
                    | n -> sprintf "{ %d properties }" n
                | JsonNull -> "null"
                | JsonUndefined -> "undefined"
            sprintf "  %s" valueStr
        let itemLines = items |> List.map toItemLine
        let lines = [ "[" ] @ itemLines @ [ "]" ]
        let result = String.concat "\n" lines
        result
    
    let private toValueDescription (e: JsonValue): string =
        match e with
        | JsonTrue -> "the boolean true"
        | JsonFalse -> "the boolean false"
        | JsonString s -> sprintf "the string %s" s
        | JsonNumber (_, t) -> sprintf "the number %s" t
        | JsonArray items ->
            let itemCount = items |> List.length
            match itemCount with
            | 0 -> "an empty array"
            | 1 -> "an array with 1 item"
            | _ -> sprintf "an array with %i items" itemCount
        | JsonObject _ -> "an object"
        | JsonNull -> "null"
        | _ -> "something else"

    let toAssertMessage (diff: Diff): string =
        match diff with
        | ObjectDiff ({ Path = path; Left = _; Right = _ }, mismatches) ->
            let propString (op: string) (p: string, v: JsonValue): string =
                let typeStr =
                    match v with
                    | JsonTrue
                    | JsonFalse -> "bool"
                    | JsonString _ -> "string"
                    | JsonNumber _ -> "number"
                    | JsonObject _ -> "object"
                    | JsonArray _ -> "array"
                    | JsonNull -> "null"
                    | JsonUndefined
                    | _ -> "undefined"

                sprintf " %s '%s' (%s)" op p typeStr

            let justMissing =
                function
                | RightOnlyProperty (n, v) -> Some (n, v)
                | LeftOnlyProperty _ -> None

            let justAdditional =
                function
                | RightOnlyProperty _ -> None
                | LeftOnlyProperty (n, v) -> Some (n, v)

            let additionals: string list =
                mismatches
                |> List.choose justAdditional
                |> List.map (propString "+")

            let missings: string list =
                mismatches
                |> List.choose justMissing
                |> List.map (propString "-")

            let maybeAdditionalsStr =
                if additionals.IsEmpty then
                    None
                else
                    let text =
                        if List.length additionals = 1 then "property" else "properties"

                    Some
                    <| sprintf "Additional %s:\n%s" text (String.concat "\n" additionals)

            let maybeMissingsStr =
                if missings.IsEmpty then
                    None
                else
                    let text =
                        if List.length missings = 1 then "property" else "properties"

                    Some
                    <| sprintf "Missing %s:\n%s" text (String.concat "\n" missings)

            let details =
                [ maybeMissingsStr
                  maybeAdditionalsStr ]
                |> List.choose id
                |> String.concat "\n"

            sprintf "Object mismatch at %s.\n%s" path details
        | ValueDiff { Path = path; Left = actual; Right = expected } ->
            match (actual, expected) with
            | (JsonString actualStr, JsonString expectedStr) -> 
                let maxStrLen =
                    max (String.length expectedStr) (String.length actualStr)
                let comparisonStr =
                    if maxStrLen > 30
                    then sprintf "Expected\n    %s\nbut was\n    %s" expectedStr actualStr
                    else sprintf "Expected %s but was %s." expectedStr actualStr
                sprintf "String value mismatch at %s.\n%s" path comparisonStr
            | (JsonNumber (_, actualNumberText), JsonNumber (_, expectedNumberText)) ->
                sprintf "Number value mismatch at %s.\nExpected %s but was %s." path expectedNumberText actualNumberText
            | _ -> sprintf "Some other value mismatch at %s." path
        | TypeDiff { Path = path; Left = actual; Right = expected } ->
            match (actual, expected) with
            | (JsonTrue, JsonFalse) ->
                sprintf "Boolean value mismatch at %s.\nExpected false but was true." path
            | (JsonFalse, JsonTrue) ->
               sprintf "Boolean value mismatch at %s.\nExpected true but was false." path
            | (_, _) ->
                let expectedMessage = toValueDescription expected
                let actualMessage = toValueDescription actual
                sprintf "Type mismatch at %s.\nExpected %s but was %s." path expectedMessage actualMessage
        | ArrayDiff ({ Path = path; Left = _; Right = _ }, mismatches) ->
            let itemString (op : string) (ix: int, v: JsonValue): string =
                let typeStr jv =
                    match jv with
                    | JsonTrue -> "the boolean true"
                    | JsonFalse -> "the boolean false"
                    | JsonString s ->
                        let truncate (maxlen : int) (str : string) =
                            let len = String.length str
                            if len > maxlen then
                                let truncInfo = sprintf "[%d more]" (len - maxlen) 
                                sprintf "%s %s" (str.Substring(0, maxlen)) truncInfo
                            else str                                
                        sprintf "the string %s" (truncate 30 s)
                    | JsonNumber (_, t) -> sprintf "the number %s" t
                    | JsonObject props -> toObjectSummary props
                    | JsonArray items -> toArraySummary items
                    | JsonNull -> "null"
                    | JsonUndefined
                    | _ -> "undefined"                
                sprintf " %s [%d]: %s" op ix (typeStr v)

            let additionals: string list =
                let justAdditional =
                    function
                    | RightOnlyItem _ -> None
                    | LeftOnlyItem (n, v) -> Some (n, v)
                mismatches
                |> List.choose justAdditional
                |> List.map (itemString "+")

            let missings: string list =
                let justMissing =
                    function
                    | RightOnlyItem (n, v) -> Some (n, v)
                    | LeftOnlyItem _ -> None
                mismatches
                |> List.choose justMissing
                |> List.map (itemString "-")

            let maybeAdditionalsStr =
                if additionals.IsEmpty then
                    None
                else
                    let text =
                        if List.length additionals = 1 then "item" else "items"
                    Some
                    <| sprintf "Additional %s:\n%s" text (String.concat "\n" additionals)

            let maybeMissingsStr =
                if missings.IsEmpty then
                    None
                else
                    let text =
                        if List.length missings = 1 then "item" else "items"

                    Some
                    <| sprintf "Missing %s:\n%s" text (String.concat "\n" missings)

            let details =
                [ maybeMissingsStr
                  maybeAdditionalsStr ]
                |> List.choose id
                |> String.concat "\n"

            sprintf "Array mismatch at %s.\n%s" path details

    let toUserMessage (messages : string list) : string =
        match messages with
        | [] -> "No differences"
        | [ m ] -> m
        | _ ->
            let numberOfDifferences = List.length messages
            let numberedMessages = messages |> List.mapi (fun i m -> sprintf "# %d: %s" (i + 1) m)
            let header = sprintf "Found %d differences." numberOfDifferences
            let lines = header :: numberedMessages
            String.concat "\n" lines
            