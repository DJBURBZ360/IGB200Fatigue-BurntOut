﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FlashObject : MonoBehaviour
{
    [SerializeField] private float appearaceTime = 0.1f;
    [SerializeField] private float fadeSpeed = 3.0f;
    private Image renderer;
    private Fader fader = new Fader();

    // Start is called before the first frame update
    void Start()
    {
        renderer = this.gameObject.GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        Color color = renderer.color;
        if (color.a <= 0)
        {
            fader.ResetFader();
        }
        fader.DoFade(ref color.a, fadeSpeed, appearaceTime);
        renderer.color = color;
    }
}
