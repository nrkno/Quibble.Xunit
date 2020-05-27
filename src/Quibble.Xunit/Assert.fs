namespace Quibble.Xunit

open Xunit.Sdk

type JsonAssertException(expected: obj, actual: obj, message: string) =
    inherit AssertActualExpectedException(expected, actual, message)

module Assert =
    
    open Quibble
    
    let JsonEqual (expectedJsonString: string, actualJsonString: string) : unit =
        let diffs : Diff list = JsonStrings.diff actualJsonString expectedJsonString
        let messages : string list = diffs |> List.map DiffMessage.toDiffMessage
        if not (List.isEmpty messages) then
            let singleMessage = messages |> String.concat "\n"
            let ex = JsonAssertException(expectedJsonString, actualJsonString, singleMessage)
            raise ex
