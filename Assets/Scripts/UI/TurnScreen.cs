﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts;
using UnityEngine.UI;
public class TurnScreen : MonoBehaviour
{

    Text label;
    RawImage background;
    AudioSource sound;

    private GameController gameController;
    private bool initialized = false;

    // Controls the fading in and out of Big turn counter
    public void Init()
    {
        gameController = GameObject.Find("GameController").GetComponent<GameController>();

        label = gameObject.GetComponentInChildren<Text>();
        background = gameObject.GetComponent<RawImage>();
        sound = gameObject.GetComponent<AudioSource>();
        label.color = new Vector4(255, 255, 255, 0);
        background.color = new Vector4(0, 0, 0, 0);

        initialized = true;
    }

    public void Play(string value)
    {
        if (initialized == false)
            return;

        label.text = value;
        sound.Play();
        gameObject.SetActive(true);
        StartCoroutine(FadeIn());
    }

    IEnumerator FadeIn()
    {

        float startTime = Time.time;

        while (Time.time - startTime < 1) // takes exactly 1 s. regardless of framerate
        {
            float passedTime = Time.time - startTime;
            label.color = new Vector4(1, 1, 1, passedTime);
            background.color = new Vector4(0, 0, 0, passedTime / 1.2f);
            yield return null;
        }
        label.color = new Vector4(1, 1, 1, 1);
        background.color = new Vector4(0, 0, 0, 0.83f);
        StartCoroutine(FadeOut());
    }

    IEnumerator FadeOut()
    {

        float startTime = Time.time;

        while (Time.time - startTime < 1) // takes exactly 2 s. regardless of framerate
        {
            float passedTime = Time.time - startTime;
            label.color = new Vector4(1, 1, 1, 1f- passedTime / 2f);
            background.color = new Vector4(0, 0, 0, 0.83f - passedTime / 2.4f);
            yield return null;
        }

        label.color = new Vector4(1, 1, 1, 0);
        background.color = new Vector4(0, 0, 0, 0);
        gameObject.SetActive(false);
    }
}