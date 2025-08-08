using Shapes;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

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
    
    [SerializeField] private Transform pointPrefab;
    [SerializeField] private float pointSpacing;
    [SerializeField] private float pointMinSpace;
    
    private bool _breatheIn = false;
    private float _interactionTime;

    private float _t;

    private void OnEnable()
    {
        foreach (Transform child in transform)
            Destroy(child.gameObject);

        var currentRadius = pointMinSpace;
        
        for (var i = 0; i < numPoints; ++i)
        {
            var p = Instantiate(pointPrefab, transform);
            var angle = Random.Range(0, 360);
            p.position = new Vector3(
                Mathf.Cos(angle),
                Mathf.Sin(angle)) 
                * currentRadius;
            currentRadius += pointSpacing;

        }
    }

    private void Update()
    {
        if (_breatheIn)
            _interactionTime += Time.deltaTime;
        else
            _interactionTime -= Time.deltaTime;

        _interactionTime = Mathf.Max(0, _interactionTime);
        
        _t = _interactionTime / secondsToBreathe;
        _t = EaseInOutCubic(_t);

        disc.Thickness = Mathf.Lerp(discMinThickness, discMaxThickness, _t);
        foreach (var obj in text)
            obj.characterSpacing = Mathf.Lerp(textCSpaceMin, textCSpaceMax, _t);
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
