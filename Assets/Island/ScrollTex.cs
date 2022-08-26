using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollTex : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }
    public float ScrollX = 0f;
    public float ScrollY = 0.25f;
    // Update is called once per frame
    void Update()
    {
        float OffsetX = Time.time * ScrollX;
        float OffsetY = Time.time * ScrollY;
        GetComponent<Renderer>().material.mainTextureOffset = new Vector2(OffsetX,OffsetY);
    }
}
