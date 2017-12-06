using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoAdaptScollViewItem : MonoBehaviour
{
    UIRoot root;
   // Vector3 curScale = Vector3.one;

    void Awake()
    {
        if (null == root)
        {
            root = GameObject.FindObjectOfType<UIRoot>();
        }

        if (root != null)
        {
            float s = (float)Screen.height / (float)root.activeHeight;
            transform.localScale = transform.localScale * s;
            //curScale = transform.localScale * s;
        }     
    }

	// Use this for initialization
	void Start ()
    {		
	}
	
	// Update is called once per frame
	void Update ()
    {
        
	}

    private int mWidth;
    private int mHeight;

    public int GetWidth()
    {
        if (mWidth == 0)
        {
            UIRoot root = GameObject.FindObjectOfType<UIRoot>();
            if (root != null)
            {
                float s = (float)root.activeHeight / Screen.height;
                mHeight = Mathf.CeilToInt(Screen.height * s);
                mWidth = Mathf.CeilToInt(Screen.width * s);
            }
            //
        }

        return mWidth;
    }

    public int GetHeight()
    {
        if (mHeight == 0)
        {
            UIRoot root = GameObject.FindObjectOfType<UIRoot>();
            if (root != null)
            {
                float s = (float)root.activeHeight / Screen.height;
                mHeight = Mathf.CeilToInt(Screen.height * s);
                mWidth = Mathf.CeilToInt(Screen.width * s);
            }
        }
        
        return mHeight;
    }
    //float curTime = 0;
}
