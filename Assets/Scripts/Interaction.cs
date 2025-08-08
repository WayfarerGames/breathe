using System;
using Shapes;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

public class Interaction : MonoBehaviour
{
    [SerializeField] private float secondsToBreathe;
    [SerializeField] private float discMinThickness;
    [SerializeField] private float discMaxThickness;
    [SerializeField] private float textCSpaceMin;
    [SerializeField] private float textCSpaceMax;
    
    [SerializeField] private Disc disc;
    [SerializeField] private TextMeshProUGUI[] text;
    
    [SerializeField] private int numPoints = 100;
    
    [SerializeField] private PointBlob pointPrefab;
    [SerializeField] private float pointSpacing;
    [SerializeField] private float pointMinSpace;

    [SerializeField, TextArea] private string inText;
    [SerializeField, TextArea] private string outText;
    [SerializeField] private string scoreTextFormat;
    
    [SerializeField] private float multiplierResetTime = 0.5f;

    private int _multiplier;
    public event Action<float, bool> OnValueChange;
    
    private bool _breatheIn;
    private float _interactionTime;

    private float _t;
    private float _score;
    private bool _timeFreeze = false;

    private void OnEnable()
    {
        foreach (Transform child in transform)
            Destroy(child.gameObject);

        text[1].text = "";
        text[0].gameObject.SetActive(true);
        var currentRadius = pointMinSpace;
        disc.gameObject.SetActive(true);
        
        for (var i = 0; i < numPoints; ++i)
        {
            var p = Instantiate(pointPrefab, transform);
            p.Init(this);
            var angle = Random.Range(0, 360);
            p.transform.position = new Vector3(
                Mathf.Cos(angle),
                Mathf.Sin(angle)) 
                * currentRadius;
            currentRadius += pointSpacing;
        }

        _multiplier = 1;
        _interactionTime = 0;
        _t = 0;
        _score = 0;
        OnValueChange?.Invoke(0, false);
        
    }

    private void OnDisable()
    {
        disc.gameObject.SetActive(false);
        text[0].gameObject.SetActive(false);
    }

    private void Update()
    {
        if (Time.time > multiplierResetTime)
            _multiplier = 1;
        
        text[0].text = _t < 0.5f ? inText : outText;
        
        if (_breatheIn)
            _interactionTime += Time.deltaTime;
        else
            _interactionTime -= Time.deltaTime;

        _interactionTime = Mathf.Clamp( _interactionTime, 0, secondsToBreathe);
        
        _t = _interactionTime / secondsToBreathe;
        _t = EaseInOutCubic(_t);
        
        OnValueChange?.Invoke(_t, _breatheIn);
        
        disc.Thickness = Mathf.Lerp(discMinThickness, discMaxThickness, _t);
        foreach (var obj in text)
            obj.characterSpacing = Mathf.Lerp(textCSpaceMin, textCSpaceMax, _t);
    }

    private async void OnCollisionEnter2D(Collision2D other)
    {
        ++_multiplier;
        _score += other.gameObject.GetComponent<PointBlob>().Value * _multiplier;
        text[1].text = string.Format(scoreTextFormat, _score);
        Destroy(other.gameObject);

        if (_timeFreeze) return;
        
        _timeFreeze = true;
        Time.timeScale = 0;
        var endTime = Time.realtimeSinceStartup + 0.1f;
        while (Time.realtimeSinceStartup < endTime)
            await Awaitable.NextFrameAsync();
        
        Time.timeScale = 1f;
        _timeFreeze = false;
    }

    private float EaseInOutCubic(float x)
    {
        return x < 0.5 ? 4 * x * x * x : 1 - Mathf.Pow(-2 * x + 2, 3) / 2;

    }
    
    private void OnInteract(InputValue val)
    {
        _breatheIn = val.isPressed;
    }
}
