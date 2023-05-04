using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class color_change : MonoBehaviour {
    private bool changing_top = false;
    private bool changing_bot = false;
    private bool revert = false;
    float m_Hue;
    float m_Saturation;
    float m_Value;
    float xLower;
    float yLower;
    float zLower;
    Color drawColor;
    private Vector3 topPosition = new Vector3(0.0253f, 0.0695f, 0.0426f);
    private Vector3 botPosition = new Vector3(0.0253f, 0.006f, -0.0582f);

    public GameObject HSpicker;
    public GameObject Vpicker;
    public GameObject topmouse;
    public GameObject botmouse;

    // Use this for initialization
    void Start () {
        xLower = HSpicker.transform.position.x - HSpicker.transform.localScale.x / 2.0f;
        yLower = HSpicker.transform.position.y - HSpicker.transform.localScale.y / 2.0f;
        zLower = Vpicker.transform.position.z - Vpicker.transform.localScale.z / 2.0f;
    }

    // Update is called once per frame
    void Update ()
    {
        if (Input.GetKeyDown("s")) { changing_top = true; }
        else if (Input.GetKeyDown("d")) { revert = true; }
        if (changing_top)
        {
            topPosition += new Vector3(0.001f, 0.0007f, (float)(-0.0001 - 0.001 * Mathf.Cos(17)));
            botPosition += new Vector3(0.001f, 0.0f, 0.0f);
            topmouse.transform.position = topPosition;
            botmouse.transform.position = botPosition;
            m_Hue = (topPosition.x - xLower) / HSpicker.transform.localScale.x;
            m_Saturation = (topPosition.y - yLower) / HSpicker.transform.localScale.y;
            m_Value = (botPosition.z - zLower) / Vpicker.transform.localScale.z;
            drawColor = Color.HSVToRGB(m_Hue, m_Saturation, m_Value);
            this.GetComponent<Renderer>().material.color = drawColor;
            if (topPosition.x > 0.06f ) { changing_top = false; changing_bot = true; }
        } else if (changing_bot) {
            botPosition += new Vector3(0.0f, 0.0f, -0.0005f);
            botmouse.transform.position = botPosition;
            m_Hue = (topPosition.x - xLower) / HSpicker.transform.localScale.x;
            m_Saturation = (topPosition.y - yLower) / HSpicker.transform.localScale.y;
            m_Value = (botPosition.z - zLower) / Vpicker.transform.localScale.z;
            drawColor = Color.HSVToRGB(m_Hue, m_Saturation, m_Value);
            this.GetComponent<Renderer>().material.color = drawColor;
            if (botPosition.z < -0.086f) { changing_bot = false; }
        } else if (revert)
        {
            this.GetComponent<Renderer>().material.color = Color.black;
            topPosition = new Vector3(0.0253f, 0.0695f, 0.0426f);
            botPosition = new Vector3(0.0253f, 0.006f, -0.0582f);
            topmouse.transform.position = topPosition;
            botmouse.transform.position = botPosition;
            revert = false;
        }
        
    }
}
