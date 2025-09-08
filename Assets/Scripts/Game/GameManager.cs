using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager mInstance;

    public List<string> ObjectsToGet = new List<string>();

    private void GetNextOrder()
    {
        var boxData = Resources.Load<OrderData>("Orders/OrderData_1");
        if (boxData == null)
        {
            return;
        }
        ObjectsToGet = boxData.GetStringList();

        foreach(var obj in ObjectsToGet)
        {
            Debug.Log(obj);
        }
    }
    public static GameManager GetInstance()
    {
        return mInstance;
    }

    private void Awake()
    {
        if (mInstance != null)
        {
            Debug.Log("GameManager:: there should only be one!");
        }
        mInstance = this;
    }

    private void Start()
    {
        GetNextOrder();
    }

}
