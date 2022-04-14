using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeartImage : MonoBehaviour
{
    public Sprite[] heartSprites;

    GameObject tap;
    TapController tapcontroller;
    Image heartUI;

    private void Start()
    {

        tap = GameObject.FindGameObjectWithTag("TapController");
        tapcontroller = tap.GetComponent<TapController>();

    }

    private void Update()
    {
        heartUI = GetComponent<Image>();
        heartUI.sprite = heartSprites[tapcontroller.curHealth];
    }
}
