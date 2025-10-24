using System;
using System.Collections.Generic;
using PythonConnection;
using UnityEngine;

[Serializable]
public class TestDataClass : DataClass
{
    [Serializable]
    public class Point
    {
        public float x;
        public float y;
    }

    public List<Point> Points = new List<Point>();
}

