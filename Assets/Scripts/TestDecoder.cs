using System;
using System.Collections.Generic;
using PythonConnection;

public class TestDecoder : DataDecoder
{
    protected override Dictionary<string, Type> DataToType()
    {
        return new Dictionary<string, Type>() { { "box_coordinates", typeof(TestDataClass) } };
    }
}
