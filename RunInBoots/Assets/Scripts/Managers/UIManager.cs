using UnityEngine;
using TMPro;

public class UIManager : MonoSingleton<UIManager>
{
    private TextMeshProUGUI _timerText;

    private void FindTimerText()
    {
        GameObject timerObject = GameObject.Find("TimerText");
        if (timerObject != null)
        {
            _timerText = timerObject.GetComponent<TextMeshProUGUI>();
        }
        else
        {
            Debug.LogWarning("TimerText UI 요소를 찾을 수 없습니다.");
        }
    }

    public void UpdateTimerUI(float remainingTime)
    {
        if (_timerText == null)
        {
            FindTimerText();
        }
        if (_timerText != null)
        {
            int minutes = Mathf.FloorToInt(remainingTime / 60f);
            int seconds = Mathf.FloorToInt(remainingTime % 60f);
            _timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }
        else
        {
            Debug.LogWarning("TimerText UI 요소가 설정되지 않았습니다.");
        }
    }
}
