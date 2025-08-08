using System;
using UnityEngine;

public class PointBlob : MonoBehaviour
{
    [SerializeField] private float valueMax = 10f;
    [SerializeField] private float force = 1f;
    [SerializeField] private float scaleMax;
    [SerializeField] private float rateOfValueIncrease = 0.1f;
    [SerializeField] private float visibleRadius = 3f;
    
    private Rigidbody2D _rbody;
    private float _value;
    public int Value => Mathf.RoundToInt(_value * valueMax);

    private bool _pressed;
    private float _t;
    
    private Interaction _interaction;
    
    private void Start()
    {
        _rbody = GetComponent<Rigidbody2D>();
        _value = 0f;
    }

    public void Init(Interaction i)
    {
        _interaction = i;
        _interaction.OnValueChange += OnValueChanged;
    }
    
    private void OnDestroy()
    {
        if (_interaction != null)
            _interaction.OnValueChange -= OnValueChanged;
    }

    private void FixedUpdate()
    {
        if (_pressed && _t < 1)
        {
            _rbody.AddForce(-transform.position.normalized * (force * _t), ForceMode2D.Force);
        }
        else if (!_pressed)
        {
            if (_t > 0 && transform.position.sqrMagnitude < visibleRadius * visibleRadius)
                _value += (1 - _t) * Time.fixedDeltaTime * rateOfValueIncrease;
            
            if (_t > 0.5f)
                _rbody.AddForce(transform.position.normalized * (force/10f), ForceMode2D.Force);
        }
        
        transform.up = _rbody.linearVelocity;
        var scale = Vector3.one * Mathf.Lerp(1, scaleMax, _value);
        scale.y *= Mathf.Max(1, _rbody.linearVelocity.sqrMagnitude);
        transform.localScale = scale;
    }

    public void OnValueChanged(float t, bool pressed)
    {
        _t = t;
        _pressed = pressed;
    }
}