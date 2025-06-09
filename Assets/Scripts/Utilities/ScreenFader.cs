using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class ScreenFader : MonoBehaviour
{
    private Image _fadeImage;
    public float fadeSpeed = 1.5f;

    private void Awake()
    {
        _fadeImage = GetComponent<Image>();
        _fadeImage.raycastTarget = false;
    }

    public IEnumerator FadeOut()
    {
        _fadeImage.raycastTarget = true;
        float alpha = 0;
        while (alpha < 1)
        {
            alpha += Time.unscaledDeltaTime * fadeSpeed;
            _fadeImage.color = new Color(0, 0, 0, alpha);
            yield return null;
        }
    }

    public IEnumerator FadeIn()
    {
        float alpha = 1;
        while (alpha > 0)
        {
            alpha -= Time.unscaledDeltaTime * fadeSpeed;
            _fadeImage.color = new Color(0, 0, 0, alpha);
            yield return null;
        }
        _fadeImage.raycastTarget = false;
    }
}
