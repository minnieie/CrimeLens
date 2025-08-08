using UnityEngine;
using System.Collections;

public class FlickeringLight : MonoBehaviour
{
    [Header("Light Settings")]
    public Light pointLight;
    
    [Header("Flicker Settings")]
    public float minIntensity = 0.2f;
    public float maxIntensity = 2.5f;
    public float flickerSpeed = 0.5f;
    public float flickerAmount = 1.0f;
    
    [Header("Random Flicker")]
    public bool enableRandomFlicker = true;
    public float randomFlickerMinInterval = 0.05f;
    public float randomFlickerMaxInterval = 0.2f;
    public float randomFlickerAmount = 0.8f;
    
    [Header("Burst Flicker")]
    public bool enableBurstFlicker = true;
    public float burstIntervalMin = 3f;
    public float burstIntervalMax = 8f;
    public float burstDuration = 0.5f;
    
    private float baseIntensity;
    private float nextRandomFlickerTime;
    private float nextBurstTime;
    
    void Start()
    {
        if (pointLight == null)
        {
            pointLight = GetComponent<Light>();
        }
        
        baseIntensity = pointLight.intensity;
        nextBurstTime = Time.time + Random.Range(burstIntervalMin, burstIntervalMax);
    }
    
    void Update()
    {
        // Base flicker effect
        float flicker = Mathf.PerlinNoise(Time.time * flickerSpeed, 0) * flickerAmount;
        pointLight.intensity = Mathf.Clamp(baseIntensity + flicker, minIntensity, maxIntensity);
        
        // Random intense flicker
        if (enableRandomFlicker && Time.time > nextRandomFlickerTime)
        {
            StartCoroutine(RandomFlicker());
            nextRandomFlickerTime = Time.time + Random.Range(randomFlickerMinInterval, randomFlickerMaxInterval);
        }
        
        // Burst flicker effect
        if (enableBurstFlicker && Time.time > nextBurstTime)
        {
            StartCoroutine(FlickerBurst());
            nextBurstTime = Time.time + Random.Range(burstIntervalMin, burstIntervalMax);
        }
    }
    
    IEnumerator RandomFlicker()
    {
        float originalIntensity = pointLight.intensity;
        
        if (Random.value > 0.7f)
        {
            pointLight.intensity = 0f;
        }
        else
        {
            pointLight.intensity = Mathf.Clamp(originalIntensity - randomFlickerAmount, minIntensity, maxIntensity);
        }
        
        yield return new WaitForSeconds(Random.Range(0.05f, 0.1f));
        
        pointLight.intensity = originalIntensity;
        
        if (Random.value > 0.8f)
        {
            pointLight.intensity = maxIntensity * 1.5f;
            yield return new WaitForSeconds(0.03f);
            pointLight.intensity = originalIntensity;
        }
    }
    
    IEnumerator FlickerBurst()
    {
        float elapsed = 0f;
        float originalIntensity = pointLight.intensity;
        
        while (elapsed < burstDuration)
        {
            pointLight.intensity = Random.Range(minIntensity, maxIntensity * 1.5f);
            elapsed += Time.deltaTime;
            yield return null;
        }
        
        pointLight.intensity = originalIntensity;
    }
}