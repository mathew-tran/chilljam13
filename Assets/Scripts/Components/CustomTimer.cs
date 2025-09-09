using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;

public class CustomTimer : MonoBehaviour
{
    public float WaitTime = 0.0f;
    
    public bool bIsRunning = false;
    public bool bIsLooping = false;

    [Header("Internal")]
    public float CurrentTime = 0.0f;

    public UnityEvent OnTimerCompleted;

    public void Setup(float waitTime, bool bLooping)
    {
        WaitTime = waitTime;
        bIsLooping = bLooping;
        bIsRunning = false;
    }
    public void StartTimer()
    {
        CurrentTime = 0.0f;
        bIsRunning = true;
    }
    public void PauseTimer()
    {
        bIsRunning = false;
    }

    public void StopTimer()
    {
        CurrentTime = 0.0f;
        bIsRunning = false; 
    }

    private void Update()
    {
        if (bIsRunning)
        {
            CurrentTime += Time.deltaTime;
            if(CurrentTime >= WaitTime)
            {
                OnTimerCompleted?.Invoke();
                if (bIsLooping)
                {
                    CurrentTime = 0.0f;
                }
                else
                {
                    bIsRunning = false;
                }
            }
        }

    }

}
