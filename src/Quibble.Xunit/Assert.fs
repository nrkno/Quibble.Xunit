namespace Quibble.Xunit

open Xunit.Sdk
open Quibble    

type JsonAssertException(expected: obj, actual: obj, messages: string list) =
    inherit AssertActualExpectedException(expected, actual, Message.toUserMessage messages)
    
    member self.DiffMessages = messages

type JsonDiffConfig = {
    allowAdditionalProperties: bool
}

module Assert =
    
    let JsonEqual (expectedJsonString: string, actualJsonString: string) : unit =        
        let diffs : Diff list = JsonStrings.diff actualJsonString expectedJsonString
        let messages : string list = diffs |> List.map Message.toAssertMessage
        if not (List.isEmpty messages) then
            let ex = JsonAssertException(expectedJsonString, actualJsonString, messages)
            raise ex
            
    let JsonEqualOverrideDefault (expectedJsonString: string, actualJsonString: string, diffConfig : JsonDiffConfig) : unit =
        let checkDiffConfig (diffConfig : JsonDiffConfig) (diff : Diff) =
            if diffConfig.allowAdditionalProperties then
                match diff with
                | Properties (diffPoint, mismatches) ->
                    // Left JSON <-> Actual JSON (Left-only properties are additional properties.)
                    // Right JSON <-> Expected JSON (Right-only properties are missing properties.)
                    let keepRightOnly =
                        function
                        | LeftOnlyProperty _ -> false
                        | RightOnlyProperty _ -> true
                    let remaining = mismatches |> List.filter keepRightOnly
                    match remaining with
                    | [] -> None
                    | _ -> Some <| Properties (diffPoint, remaining)
                | _ -> Some diff
            else
                Some diff
        let diffs : Diff list = JsonStrings.diff actualJsonString expectedJsonString
        let diffs' = diffs |> List.choose (checkDiffConfig diffConfig)
        let messages : string list = diffs' |> List.map Message.toAssertMessage
        if not (List.isEmpty messages) then
            let ex = JsonAssertException(expectedJsonString, actualJsonString, messages)
            raise ex
