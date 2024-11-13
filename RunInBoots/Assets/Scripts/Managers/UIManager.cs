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
            Debug.LogWarning("TimerText UI ��Ҹ� ã�� �� �����ϴ�.");
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
            Debug.LogWarning("TimerText UI ��Ұ� �������� �ʾҽ��ϴ�.");
        }
    }
}
