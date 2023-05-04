using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HSV_display : MonoBehaviour {
    public GameObject HSpicker;
    public GameObject Vpicker;
    public GameObject topmouse;
    public GameObject botmouse;
    public GameObject colorCircle;
    public ScreenTouch_Stand screenTop;
    public HandData hand;

    private Collider HSbounds;
    private Collider Vbounds;
    Queue<Vector3> smoothedBots = new Queue<Vector3>();
    Queue<Vector3> smoothedTops = new Queue<Vector3>();
    private Vector3 topPosition;
    private Vector3 botPosition;

    float m_Hue;
    float m_Saturation;
    float m_Value;
    float xLower;
    float yLower;
    float zLower;
    Color drawColor;

    // Use this for initialization
    void Start () {
        HSbounds = HSpicker.GetComponent<Collider>();
        Vbounds = Vpicker.GetComponent<Collider>();
        xLower = HSpicker.transform.position.x - HSpicker.transform.localScale.x / 2.0f;
        yLower = HSpicker.transform.position.y - HSpicker.transform.localScale.y / 2.0f;
        zLower = Vpicker.transform.position.z - Vpicker.transform.localScale.z / 2.0f;
    }
	
	// Update is called once per frame
	void Update () {
        if (screenTop.num_touches == 1)
        {
            colorCircle.SetActive(true);
        }
        if (hand.isPinched == false)
        {
            topmouse.SetActive(true);
            botmouse.SetActive(true);
            smoothedTops.Enqueue(new Vector3(hand.indexPos.x, hand.indexPos.y, (float)(0.025 - hand.indexPos.y*Mathf.Cos(17))));
            smoothedBots.Enqueue(new Vector3(hand.indexPos.x, 0.0055f, hand.indexPos.z));
            if (smoothedTops.Count < 6) { return; }
            smoothedTops.Dequeue();
            smoothedBots.Dequeue();
            topPosition = avgPos(smoothedTops);
            botPosition = avgPos(smoothedBots);
            if (HSbounds.bounds.Contains(topPosition) && Vbounds.bounds.Contains(botPosition))
            {
                topmouse.transform.position = topPosition;
                botmouse.transform.position = botPosition;
                m_Hue = (topPosition.x - xLower) / HSpicker.transform.localScale.x;
                m_Saturation = (topPosition.y - yLower) / HSpicker.transform.localScale.y;
                m_Value = (botPosition.z - zLower) / Vpicker.transform.localScale.z;
                drawColor = Color.HSVToRGB(m_Hue, m_Saturation, m_Value);
                colorCircle.GetComponent<Renderer>().material.color = drawColor;
            }
        }
        else
        {
            topmouse.SetActive(false);
            botmouse.SetActive(false);
            colorCircle.SetActive(false);
        }
	}

    private Vector3 smoothPos;
    public Vector3 avgPos(Queue<Vector3> positions)
    {
        smoothPos = Vector3.zero;
        foreach (Vector3 pos in positions)
        {
            smoothPos += pos;
        }
        return smoothPos/5.0f;
    }
}
