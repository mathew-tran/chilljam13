using System;
using UnityEngine;

public class BoxSpawner : MonoBehaviour
{
    private GameObject SpawnedObject;

    public GameObject ObjectToSpawn;


    [SerializeField]
    private CustomTimer SpawnTimer;

    [SerializeField]
    private Transform SpawnPosition;

    [SerializeField]
    private TriggerButton TriggerButton;

    private void Awake()
    {
        SpawnTimer.Setup(3, false);
        SpawnTimer.OnTimerCompleted.AddListener(OnTimerCompleted);
        TriggerButton.InteractableText =  "Spawn " + ObjectToSpawn.GetComponent<Box>().GetBoxName();
        SpawnObject();
    }

    private void OnTimerCompleted()
    {
        SpawnedObject = Instantiate(ObjectToSpawn, SpawnPosition.position, Quaternion.identity);
        SpawnedObject.GetComponent<Box>().OnBoxDeath.AddListener(OnBoxDeath);
    }

    private void OnBoxDeath()
    {
        SpawnObject();
    }

    public void SpawnObject()
    {
        if (SpawnedObject != null)
        {
            SpawnedObject.GetComponent<Box>().OnBoxDeath.RemoveAllListeners();
            Destroy(SpawnedObject);
        }
        SpawnTimer.StartTimer();
    }

    private void OnDestroy()
    {
        if(SpawnedObject != null)
        {
            Destroy(SpawnedObject);
        }
    }
}
