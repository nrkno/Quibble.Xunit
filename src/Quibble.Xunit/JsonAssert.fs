namespace Quibble.Xunit

module JsonAssert =
    
    let Equal (expectedJsonString: string, actualJsonString: string) : unit =
        Assert.JsonEqual (expectedJsonString, actualJsonString)
        
    let EqualOverrideDefault (expectedJsonString: string, actualJsonString: string, diffConfig : JsonDiffConfig) : unit =
        Assert.JsonEqualOverrideDefault (expectedJsonString, actualJsonString, diffConfig)
