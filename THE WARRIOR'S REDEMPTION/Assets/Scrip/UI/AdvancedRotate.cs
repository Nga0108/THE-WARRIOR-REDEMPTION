using UnityEngine;
using System.Collections;

public class AdvancedRotate : MonoBehaviour
{
    [Header("Cài đặt Xoay")]
    public float rotateSpeed = 100f;
    public bool clockwise = true;

    [Header("Hiệu ứng Nhịp thở (Pulse)")]
    public bool usePulse = true;
    public float pulseSpeed = 2f;
    public float pulseAmount = 0.15f;

    [Header("Hiệu ứng Mờ ảo (Flicker)")]
    public bool useFlicker = false;
    public float flickerSpeed = 5f;

    private Vector3 baseScale;
    private SpriteRenderer sr;
    private float baseAlpha;

    void Awake()
    {
       
        baseScale = transform.localScale;
        sr = GetComponent<SpriteRenderer>();
        if (sr != null) baseAlpha = sr.color.a;
    }

    void OnEnable()
    {
        
        transform.localScale = Vector3.zero;
        StartCoroutine(ScaleUp());
    }

    void Update()
    {
        
        float direction = clockwise ? -1f : 1f;
        transform.Rotate(0, 0, direction * rotateSpeed * Time.deltaTime);

        
        if (usePulse)
        {
            float scaleMod = Mathf.Sin(Time.time * pulseSpeed) * pulseAmount;
            transform.localScale = baseScale + new Vector3(scaleMod, scaleMod, 0);
        }

        
        if (useFlicker && sr != null)
        {
            float alphaMod = Mathf.PingPong(Time.time * flickerSpeed, baseAlpha * 0.5f);
            Color c = sr.color;
            c.a = baseAlpha - alphaMod;
            sr.color = c;
        }
    }

   
    IEnumerator ScaleUp()
    {
        float t = 0;
        while (t < 1)
        {
            
            t += Time.unscaledDeltaTime * 2f;
            transform.localScale = Vector3.Lerp(Vector3.zero, baseScale, t);
            yield return null;
        }
        transform.localScale = baseScale;
    }
}