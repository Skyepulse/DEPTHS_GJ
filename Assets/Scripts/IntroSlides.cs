using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.SceneManagement;

public class IntroSlides : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public GameObject[] slides; //these are objects that are children of the gameobject this script is attached to
    //to activate and deactivate
    [SerializeField] private float slideDuration = 5f; //how long each slide is shown
    private float slideTimer = 0f; //timer to keep track of how long the slide has been shown
    private int currentSlideIndex = 0; //index of the current slide
    private GameObject currentSlide; //the current slide being shown

    public float SlideDuration => slideDuration;
    void Start()
    {
        currentSlideIndex = 0;
        currentSlide = slides[currentSlideIndex];
        currentSlide.gameObject.SetActive(true);
        for (int i = 1; i < slides.Length; i++)
        {
            slides[i].gameObject.SetActive(false);
        }
        slideTimer = 0f;

    }

    // Update is called once per frame
    void Update()
    {
        //when all slides done call other scene and destroy this scene
        slideTimer += Time.deltaTime;
        if (slideTimer >= slideDuration && currentSlideIndex < slides.Length - 1)
        {
            currentSlideIndex++;
            currentSlide.gameObject.SetActive(false);
            currentSlide = slides[currentSlideIndex];
            currentSlide.gameObject.SetActive(true);
            slideTimer = 0f;
        }
        else if (currentSlideIndex == slides.Length - 1)
        {
            //call other scene and destroy this scene
            Debug.Log("All slides done");
            // Load the main menu scen
            SceneManager.LoadScene("GameSceneTest");

            Destroy(this.gameObject);
        }
    }

}
