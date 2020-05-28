namespace Quibble.Xunit

open Xunit.Sdk

type JsonAssertException(expected: obj, actual: obj, messages: string list) =
    inherit AssertActualExpectedException(expected, actual, String.concat "\n" messages)
    
    member self.DiffMessages = messages

module Assert =
    
    open Quibble
    
    let JsonEqual (expectedJsonString: string, actualJsonString: string) : unit =
        let diffs : Diff list = JsonStrings.diff actualJsonString expectedJsonString
        let messages : string list = diffs |> List.map AssertMessage.toDiffMessage
        if not (List.isEmpty messages) then
            let ex = JsonAssertException(expectedJsonString, actualJsonString, messages)
            raise ex
