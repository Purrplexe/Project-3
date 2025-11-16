using System;
using System.Collections;
using System.Collections.Generic;
using PythonConnection;
using Unity.VisualScripting;
using UnityEngine;

public class ConnectionTest : MonoBehaviour
{


    [Serializable]
    private class SendingData
    {
        public SendingData(int testValue0, List<float> testValue1)
        {
            this.testValue0 = testValue0;
            this.testValue1 = testValue1;
        }

        public int testValue0;

        [SerializeField]
        private List<float> testValue1;
    }
    public GameObject legoObj;
    public GameObject rootObj;
    public float scaleFactor = 0.1f;
    void Start()
    {
        PythonConnector.instance.RegisterAction(typeof(TestDataClass), OnDataReceived);

        if (PythonConnector.instance.StartConnection())
        {
            Debug.Log("Connected");
        }
        else
        {
            Debug.Log("Connection Failed");
        }
    }

    void Update()
    {

    }

    public void OnTimeout()
    {
        Debug.Log("Timeout");
    }

    public void OnStop()
    {
        Debug.Log("Stopped");
    }

    public void OnDataReceived(DataClass data)
    {
        TestDataClass testData = data as TestDataClass;
        for (int i = 0; i < rootObj.transform.childCount; i++)
            Destroy(rootObj.transform.GetChild(i).gameObject);
        foreach (TestDataClass.Point point in testData.Points)
        {
            Debug.Log("x: " + point.x + " y: " + point.y);
            GameObject obj = Instantiate(legoObj, rootObj.transform);
            // scale x and y (not anymore)
            obj.transform.localPosition = new Vector3(point.x / scaleFactor, 0,point.y / scaleFactor);
            obj.transform.localRotation = Quaternion.Euler(0, point.isHorizontal ? 90 : 0, 0);
            //scale bricksize https://discussions.unity.com/t/sizing-an-object-to-unity-units/942353
            // set it to the % of space it takes up (along length)
            var desiredlengthUnits = point.length / scaleFactor;
            var currentBoundsSize = obj.GetComponent<MeshRenderer>().bounds.size.x;
            var requiredTransformLocalScale = desiredlengthUnits / (currentBoundsSize * obj.transform.lossyScale.x);
            //apply transformation
            obj.transform.localScale *= requiredTransformLocalScale;


        }

        /* List<float> v2 = new List<float>()
        {
            UnityEngine.Random.Range(0.1f, 0.9f),
            UnityEngine.Random.Range(0.1f, 0.9f)
        };
        SendingData sendingData = new SendingData(v1, v2);

        Debug.Log("Sending Data: " + v1 + ", " + v2[0] + ", " + v2[1]);

        PythonConnector.instance.Send("test", sendingData);
        */
    }
}
