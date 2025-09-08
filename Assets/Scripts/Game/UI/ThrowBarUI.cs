using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ThrowBarUI : MonoBehaviour
{
    public Image ForeGround;
    public Image BackGround;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void OnEnable()
    {
        OnThrowEnd();
    }

    void Start()
    {
        Player.GetInstance().OnThrowStart += OnThrowStart;
        Player.GetInstance().OnThrowEnd += OnThrowEnd;

    }

    private void OnDestroy()
    {
        Player.GetInstance().OnThrowStart -= OnThrowStart;
        Player.GetInstance().OnThrowEnd -= OnThrowEnd;
    }

    private void Update()
    {
        var maxWidth = BackGround.rectTransform.sizeDelta.x;
        var newSizeDelta = BackGround.rectTransform.sizeDelta;
        newSizeDelta.x = Mathf.Lerp(0, maxWidth, Player.GetInstance().GetThrowStrengthPercentage());
        ForeGround.rectTransform.sizeDelta = newSizeDelta;
    }
    private void OnThrowEnd()
    {
        transform.localScale = Vector3.zero;
    }

    private void OnThrowStart()
    {
        transform.localScale = Vector3.one;
    }

}
