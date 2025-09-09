using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "OrderData_", menuName = "Custom/OrderData", order = 1)]
public class OrderData : ScriptableObject
{
    public List<BoxData> data = new List<BoxData>();

    public List<string> GetStringList()
    {
        List<string> list = new List<string>();
        foreach(BoxData box in data)
        {
            list.Add(box.BoxName);
        }

        return list;
    }
}
