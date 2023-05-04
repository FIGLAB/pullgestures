using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class change_font : MonoBehaviour
{
    public ScreenTouch_Stand screenTop;
    public HandData hand;
    public GameObject changeMedia;
    public GameObject sliderScreen;
    public bool moving = false;
    public bool slider = false;
    public bool finish = false;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (screenTop.num_touches == 2)
        {
            moving = true;
            changeMedia.SetActive(true);
        }
        if (moving)
        {
            this.transform.position = new Vector3(0.0f, 0.006f, hand.indexPos.z);
            if (this.transform.position.z < -0.09f)
            {
                moving = false;
                slider = true;
                changeMedia.SetActive(false);
                sliderScreen.SetActive(true);
            }
        }
        if (slider)
        {
            this.transform.position = hand.indexPos;
            // change font here
            if (hand.isPinched == false)
            {
                slider = false;
                finish = true;
                sliderScreen.SetActive(false);
            }
        }
        if (finish)
        {
            this.transform.position = new Vector3(0.0f, 1.0f, 0.0f);
        }

    }
}
