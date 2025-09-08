using System;
using UnityEngine;

public class SortBox : MonoBehaviour
{
    [SerializeField]
    private BoxTriggerChecker KillCollider;

    
    void Start()
    {
        KillCollider.OnBoxEntered.AddListener(OnBoxEntered);
    }

    private void OnBoxEntered(GameObject box)
    {
        Debug.Log("Box killed");
        Destroy(box.gameObject);
    }

}
