using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeEffect : MonoBehaviour
{
    [SerializeField]
    [Range(0.01f, 10f)]
    public float fadeTime;
    private Image image;
    private Coroutine fadeCoroutine;

    private void Awake()
    {
        image = GetComponent<Image>();
    }
    public void StartFadeIn()
    {
        StartCoroutine(Fade(0, 1)); // 페이드인
    }

    public void StartFadeOut()
    {
        StartCoroutine(Fade(1, 0)); // 페이드아웃
    }

    IEnumerator Fade(float start, float end) // 알파값의 시작값과 종료값
    {
        float currentTime = 0.0f;
        float percent = 0.0f;
        while (percent < 1)
        {
            currentTime += Time.deltaTime;
            percent = currentTime / fadeTime;

            Color color = image.color;
            color.a = Mathf.Lerp(start,end,percent);
            image.color = color;
            yield return null;
        }
    }
}
