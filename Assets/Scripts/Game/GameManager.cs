using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Events;

public class OrderInfo
{
    public string BoxName;
    public int BoxesToGet = 1;
    public int CurrentBoxes = 0;
    public OrderInfo(string boxName, int boxesToGet)
    {
        BoxName = boxName;
        BoxesToGet = boxesToGet;
        CurrentBoxes = 0;
    }
    public bool IsCompleted()
    {
        return CurrentBoxes >= BoxesToGet;
    }

    public string GetString()
    {
        return String.Format("[{0}/{1}] - {2}", CurrentBoxes, BoxesToGet, BoxName);
    }
}
public class GameManager : MonoBehaviour
{
    public static GameManager mInstance;

    public Dictionary<string, OrderInfo> ObjectsToGet = new Dictionary<string, OrderInfo>();

    public UnityEvent<Dictionary<string, OrderInfo>> OnObjectsToGetChanged;
    public UnityEvent OnCompletedOrder;
    public UnityEvent OnCorrectBoxAdded;
    public UnityEvent OnIncorrectBoxAdded;
    public UnityEvent<string> OnSignalSent;

    public CustomTimer AfterCompleteTimer;
    private void GetNextOrder()
    {
        ObjectsToGet.Clear();
        var boxData = Resources.Load<OrderData>("Orders/OrderData_1");
        if (boxData == null)
        {
            return;
        }
        
        foreach(var boxName in boxData.GetStringList())
        {
            if (ObjectsToGet.ContainsKey(boxName))
            {
                ObjectsToGet[boxName].BoxesToGet += 1;
            }
            else
            {
                ObjectsToGet.Add(boxName, new OrderInfo(boxName, 1));
            }
               
        }

        foreach(var obj in ObjectsToGet)
        {
            Debug.Log(obj);
        }
        OnObjectsToGetChanged?.Invoke(ObjectsToGet);
    }

    public bool CanAddItem(string boxName)
    {
        if (ObjectsToGet.ContainsKey(boxName))
        {
            return ObjectsToGet[boxName].IsCompleted() == false;
        }
        return false;
    }

    public void AttemptToAddItem(string boxName)
    {
        if (CanAddItem(boxName) == false)
        {
            OnIncorrectBoxAdded?.Invoke();
            return;
        }
        ObjectsToGet[boxName].CurrentBoxes += 1;
        OnCorrectBoxAdded?.Invoke();

        if (HasCompletedAllOrders())
        {
            OnCompletedOrder?.Invoke();
            AfterCompleteTimer.StartTimer();
        }
    }

    public bool HasCompletedAllOrders()
    {
        foreach(var boxName in ObjectsToGet.Keys)
        {
            if (ObjectsToGet[boxName].IsCompleted() == false)
            {
                return false;
            }
        }
        return true;
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
        AfterCompleteTimer.Setup(2.0f, false);
        AfterCompleteTimer.OnTimerCompleted.AddListener(OnAfterCompleteTimerCompleted);
        GetNextOrder();
    }

    private void OnAfterCompleteTimerCompleted()
    {
        GetNextOrder();
    }
}
