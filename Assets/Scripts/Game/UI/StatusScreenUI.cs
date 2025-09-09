using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StatusScreenUI : MonoBehaviour
{
    [SerializeField]
    private TMP_Text Text;

    [SerializeField] 
    private CustomTimer Timer;

    [SerializeField]
    private Sprite CorrectTexture;

    [SerializeField]
    private Sprite IncorrectTexture;

    [SerializeField]
    private Image ImageReference;

    private void Start()
    {
        GameManager.GetInstance().OnObjectsToGetChanged.AddListener(OnObjectsToGetChanged);
        GameManager.GetInstance().OnCorrectBoxAdded.AddListener(OnCorrectBoxAdded);
        GameManager.GetInstance().OnIncorrectBoxAdded.AddListener(OnIncorrectBoxAdded);
        GameManager.GetInstance().OnCompletedOrder.AddListener(OnCompletedOrder);
        Timer.OnTimerCompleted.AddListener(OnTimerCompleted);
        Timer.Setup(.25f, false);
    }

    private void OnCompletedOrder()
    {
        
        OnTimerCompleted();
        Timer.StopTimer();
        Text.text = "ORDER COMPLETED";
    }

    private void OnCorrectBoxAdded()
    {
        ImageReference.sprite = CorrectTexture;
        ImageReference.gameObject.SetActive(true);
        Text.text = "";
        Timer.StartTimer();
    }

    private void OnIncorrectBoxAdded()
    {
        ImageReference.sprite = IncorrectTexture;
        ImageReference.gameObject.SetActive(true);
        Text.text = "";
        Timer.StartTimer();
    }

    private void OnTimerCompleted()
    {
        OnObjectsToGetChanged(GameManager.GetInstance().ObjectsToGet);
        ImageReference.gameObject.SetActive(false);
    }

    private void OnObjectsToGetChanged(Dictionary<string, OrderInfo> arg0)
    {
        string text = "ITEMS REQUIRED:\n";
        foreach(string key in arg0.Keys)
        {
            text += arg0[key].GetString() + "\n";
        }
        Text.text = text;
    }
}
