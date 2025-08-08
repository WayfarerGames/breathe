using System;
using TMPro;
using UnityEngine;

public class Timer : MonoBehaviour
{
    [SerializeField] private Interaction interaction;
    [SerializeField] private float gameplaySeconds = 60;
    
    private TextMeshProUGUI _text;
    private TimeSpan _startTime;
    
    private void Start()
    {
        interaction.OnValueChange += InteractionValueChange;
        _text = GetComponent<TextMeshProUGUI>();
        enabled = false;
    }

    private void OnEnable()
    {
        _startTime = TimeSpan.FromSeconds(gameplaySeconds);
    }

    private void Update()
    {
        _startTime -= TimeSpan.FromSeconds(Time.deltaTime);
        _text.text = _startTime.Seconds.ToString();
        if (_startTime.Seconds == 0)
        {
            interaction.enabled = false;
            enabled = false;
            _text.text = "restart";
        }
    }

    public void RestartGame()
    {
        enabled = true;
        interaction.enabled = true;
    }

    private void InteractionValueChange(float _, bool pressed)
    {
        if (pressed)
            enabled = true;
    }
}
