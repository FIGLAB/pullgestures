using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class map_shrink : MonoBehaviour {
    private Vector3 big;
    private Vector3 small;
    private Vector3 topPos;
    private Vector3 botPos;
    private bool shrinking = false;
    private bool growing = false;
    public GameObject indicator;

	// Use this for initialization
	void Start () {
        big = new Vector3(0.66816f, 0.37584f, 0.012f);
        small = new Vector3(0.29696f, 0.16704f, 0.012f);
        topPos = new Vector3(0.0f, 0.0065f, -0.033f);
        botPos = new Vector3(0.0f, 0.0065f, -0.13f);
    }
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown("s")) { shrinking = true; }
        if (shrinking)
        {
            indicator.transform.localPosition = indicator.transform.localPosition - new Vector3(0.0f, 0.0f, 0.0035f);
            this.transform.localScale = Vector3.Scale(this.transform.localScale, new Vector3(0.97f, 0.97f, 1.0f));
            if (this.transform.localScale.x < small.x) {
                shrinking = false;
                this.transform.localScale = small;
                indicator.transform.localPosition = botPos;
            }
        }
        if (Input.GetKeyDown("d")) { shrinking = false; growing = true; }
        if (growing)
        {
            indicator.transform.localPosition = indicator.transform.localPosition + new Vector3(0.0f, 0.0f, 0.0035f);
            this.transform.localScale = Vector3.Scale(this.transform.localScale, new Vector3(1.03f, 1.03f, 1.0f));
            if (this.transform.localScale.y > big.y) {
                growing = false;
                this.transform.localScale = big;
                indicator.transform.localPosition = topPos;
            }
        }

    }
}
