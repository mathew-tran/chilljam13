using NUnit.Framework.Interfaces;
using System;
using UnityEngine;

public class BoxSpawner : IInteractable
{
    private GameObject SpawnedObject;

    public GameObject ObjectToSpawn;


    [SerializeField]
    private CustomTimer SpawnTimer;

    [SerializeField]
    private Transform SpawnPosition;

    [SerializeField]
    private TriggerButton TriggerButton;

    private bool bCanInteract = true;

    private void Awake()
    {
        SpawnTimer.Setup(3, false);
        SpawnTimer.OnTimerCompleted.AddListener(OnTimerCompleted);
        TriggerButton.InteractableText =  "Spawn " + ObjectToSpawn.GetComponent<Box>().GetBoxName();
        SpawnObject();
    }

    private void OnTimerCompleted()
    {
        GetComponent<Rigidbody>().isKinematic = true;
        SpawnedObject = Instantiate(ObjectToSpawn, SpawnPosition.position, Quaternion.identity);
        SpawnedObject.GetComponent<Box>().OnBoxDeath.AddListener(OnBoxDeath);
    }

    private void OnBoxDeath()
    {
        SpawnObject();
    }

    public void SpawnObject()
    {
        KillSpawnedObject();
        SpawnTimer.StartTimer();
    }

    private void KillSpawnedObject()
    {
        if (SpawnedObject != null)
        {
            SpawnedObject.GetComponent<Box>().OnBoxDeath.RemoveAllListeners();
            Destroy(SpawnedObject);
        }
    }

    private void OnDestroy()
    {
        KillSpawnedObject();
    }

    public override string GetInteractableName()
    {
        return "Set Pedestal";
    }

    public override bool CanInteract()
    {
        return bCanInteract;
    }

    public override void Interact(Player playerRef)
    {
        bCanInteract = false;
        playerRef.PickupItem(gameObject);
        KillSpawnedObject();
        SpawnTimer.StopTimer();

    }

    public override void Drop()
    {
        bCanInteract = true;
        SpawnTimer.StartTimer();
    }
}
