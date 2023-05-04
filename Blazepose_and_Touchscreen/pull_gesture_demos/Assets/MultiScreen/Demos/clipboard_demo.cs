using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class clipboard_demo : MonoBehaviour
{
    public ScreenTouch_Stand screenTop;
    public HandData hand;
    public bool moving = false;
    public bool finish = false;
    public bool isImage = true;
    public GameObject imgHigh;
    public GameObject imgInd;
    public GameObject txtHigh;
    public GameObject txtInd;
    private GameObject highlight;
    private GameObject indicator;
    private int fcounter = 0;
    public GameObject figHighlight;
    public GameObject statHighlight;
    public GameObject quoteHighlight;
    public Collider figBounds;
    public Collider statBounds;
    public Collider quoteBounds;
    public GameObject clipOverlay;

    // Use this for initialization
    void Start ()
    {
        figBounds = figHighlight.GetComponent<Collider>();
        statBounds = statHighlight.GetComponent<Collider>();
        quoteBounds = quoteHighlight.GetComponent<Collider>();
    }
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown("s")) { isImage = !isImage;  }
        if (isImage)
        {
            highlight = imgHigh;
            indicator = imgInd;
        }
        else
        {
            highlight = txtHigh;
            indicator = txtInd;
        }
        if (screenTop.num_touches == 2)
        {
            moving = true;
            highlight.SetActive(true);
            indicator.SetActive(true);
            clipOverlay.SetActive(true);
        }
        if (moving)
        {
            this.transform.position = new Vector3(hand.indexPos.x, hand.indexPos.y, (float)(hand.indexPos.z + 0.015));
            indicator.transform.position = new Vector3(hand.indexPos.x, 0.006f, hand.indexPos.z);
            if (hand.isPinched == false) { fcounter += 1; }
            else { fcounter = 0; }
            if (fcounter > 30)
            {
                Debug.Log("FINISH!");
                fcounter = 0;
                moving = false;
                finish = true;
            }
        }
        if (figBounds.bounds.Contains(this.transform.position)) { figHighlight.GetComponent<MeshRenderer>().enabled = true; }
        else { figHighlight.GetComponent<MeshRenderer>().enabled = false; }
        if (statBounds.bounds.Contains(this.transform.position)) { statHighlight.GetComponent<MeshRenderer>().enabled = true; }
        else { statHighlight.GetComponent<MeshRenderer>().enabled = false; }
        if (quoteBounds.bounds.Contains(this.transform.position)) { quoteHighlight.GetComponent<MeshRenderer>().enabled = true; }
        else { quoteHighlight.GetComponent<MeshRenderer>().enabled = false; }
        if (finish)
        {
            if(imgInd.transform.localScale.x > 0.0f) {
                indicator.transform.localScale -= new Vector3(0.01f, 0.00f, 0.01f);
            }else {
                this.transform.position = new Vector3(1.0f, 0.0f, 0.0f);
                highlight.SetActive(false);
                indicator.SetActive(false);
                clipOverlay.SetActive(false);
                finish = false;
            }
        }
    }
}
