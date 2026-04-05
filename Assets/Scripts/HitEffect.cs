using System.Collections;
using UnityEngine;
using DG.Tweening;

public class HitEffect : MonoBehaviour
{
    private static float duration = 0.35f;
    private static float maxIntensity = 0.25f;

    private int hitEffectAmount = Shader.PropertyToID("_HitEffectAmount");

    private SpriteRenderer[] spriteRenderers;
    private Material[] materials;

    private float lerpAmount; 

    private void Awake()
    {
        spriteRenderers = GetComponentsInChildren<SpriteRenderer>();

        materials = new Material[spriteRenderers.Length];
        for(int i = 0; i < spriteRenderers.Length; i++)
        {
            materials[i] = spriteRenderers[i].material;
        }
    }

    public void ActiveHitEffect()
    {
        lerpAmount = 0f;
        DOTween.To(GetLerpValue, SetLerpValue, maxIntensity, duration).SetEase(Ease.OutExpo).OnUpdate(OnlerpUpdate).OnComplete(OnlerpCompleted);
    }

    private void OnlerpUpdate()
    {
        foreach (Material mat in materials)
        {
            mat.SetFloat(hitEffectAmount, GetLerpValue());
        }
    }

    private void OnlerpCompleted()
    {
        DOTween.To(GetLerpValue, SetLerpValue, 0f, duration).OnUpdate(OnlerpUpdate);
    }
    
    private float GetLerpValue()
    {
        return lerpAmount;
    }

    private void SetLerpValue(float newValue)
    {
        lerpAmount = newValue;
    }

}
