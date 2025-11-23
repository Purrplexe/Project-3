using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
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
    public GameObject rootObj;
    public string[] labels = { "brick_2x2", "brick_2x4", "brick_1x6", "plate_1x2", "plate_2x2", "plate_2x4"};
    public GameObject[] legoBricks = { };
    public Dictionary<string, GameObject> labelToBrick = new Dictionary<string, GameObject>();
    public float scaleFactor = 100.0f;
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
        for (int i = 0; i < labels.Length; i++)
        {
            labelToBrick[labels[i]] = legoBricks[i];
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
            //instantiate brick at root
            GameObject obj = Instantiate(labelToBrick[point.brickType], rootObj.transform);
            // scale x and y (not anymore)
            obj.transform.localPosition = new Vector3(point.x / scaleFactor, 0,point.y / scaleFactor);
            obj.transform.localRotation = Quaternion.Euler(0, point.isHorizontal ? 90 : 0, 0);
            //scale bricksize https://discussions.unity.com/t/sizing-an-object-to-unity-units/942353
            // set it to the % of space it takes up (along length)
            var desiredlengthUnits = point.length / scaleFactor;
            // https://discussions.unity.com/t/how-to-get-object-bounds-from-children/804233
            MeshRenderer[] meshes = obj.GetComponentsInChildren<MeshRenderer>();
            var bounds = new Bounds(obj.transform.position, Vector3.one);
            foreach (MeshRenderer mesh in meshes)
            {
                bounds.Encapsulate(mesh.bounds);
            }
            //scale along length
            float currentBoundsSize = point.isHorizontal ? bounds.size.x : bounds.size.y;
            var requiredTransformLocalScale = desiredlengthUnits / (currentBoundsSize * (point.isHorizontal ? obj.transform.lossyScale.x : obj.transform.lossyScale.y));
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
