namespace Quibble.Xunit

module Assert =
    
    open System

    [<Obsolete("Use JsonAssert.Equal instead.")>]
    let xJsonEqual (expectedJsonString: string, actualJsonString: string) : unit =
        JsonAssert.Equal(expectedJsonString, actualJsonString)
            
    [<Obsolete("Use JsonAssert.EqualOverrideDefault instead.")>]
    let JsonEqualOverrideDefault (expectedJsonString: string, actualJsonString: string, diffConfig : JsonDiffConfig) : unit =
        JsonAssert.EqualOverrideDefault(expectedJsonString, actualJsonString, diffConfig)
