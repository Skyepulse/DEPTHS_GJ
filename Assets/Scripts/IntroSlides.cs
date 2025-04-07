using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;

public class IntroSlides : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    RawImage[] slides;
    string[] slideTexts = { "Welcome to the game!", "Get ready for an adventure!", "Good luck!" };
    TextMeshProUGUI[] slideTextComponents;
    RawImage currentSlide;
    float slideDuration = 5f;
    float slideTimer = 0f;
    void Start()
    {
        slides = GetComponentsInChildren<RawImage>();
        currentSlide = slides[0];
        for (int i = 1; i < slides.Length; i++)
        {
            slides[i].gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        slideTimer += Time.deltaTime;
        if (slideTimer >= slideDuration)
        {
            currentSlide.gameObject.SetActive(false);
            int nextSlideIndex = (System.Array.IndexOf(slides, currentSlide) + 1) % slides.Length;
            currentSlide = slides[nextSlideIndex];
            currentSlide.gameObject.SetActive(true);
            slideTimer = 0f;
        }
    }
}
