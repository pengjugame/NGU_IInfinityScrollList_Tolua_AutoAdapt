using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//Warp基本类
public abstract class WrapContentBase : MonoBehaviour
{
    public UIPanel mPanel;

    private GameObject uiTemplete;
    private UIScrollView uiScollView;
    private MultiRowWrapContent mWrapContent;
    private Vector2 mInitPanelClipOffset;
    private Vector3 mInitPanelLocalPos;    
    private bool mbFirstInit = false;
    private int mMaxItemCount = 4;

    //多标签页切换时不用删除
    private List<GameObject> mTempletePool = new List<GameObject>();
    private GameObject mPoolNode;

    void OnDestroy()
    {        
        for (int i = 0; i < mTempletePool.Count; ++i)
        {
            GameObject.Destroy(mTempletePool[i]);
        }
        mTempletePool.Clear();
    }

    protected void InitSettings(int maxCount, GameObject templete)
    {    

        mMaxItemCount = maxCount;
        uiTemplete = templete;

        if (!mbFirstInit)
        {
            mWrapContent = GetComponent<MultiRowWrapContent>();
            mWrapContent.SetTemplete(uiTemplete);
          
            //绑定方法
            mWrapContent.onInitializeItem = OnUpdateItem;

            mInitPanelClipOffset = mPanel.clipOffset;
            mInitPanelLocalPos = mPanel.cachedTransform.localPosition;
            uiScollView = mPanel.GetComponent<UIScrollView>();
            mbFirstInit = true;
        }
    }

    //初始化，不赋值
    protected void InitGameOjbects(int initCount)
    {
        if (!mbFirstInit)
        {
            Debug.LogError("InitGameOjbects, Please Init WrapContent AtFirst.");
            return;
        }

        if (mWrapContent == null)
        {
            Debug.LogError("InitGameOjbects WrapContent Is Null");
            return;
        }

        int curInitCount = initCount;
        if (initCount > mMaxItemCount)
        {
            curInitCount = mMaxItemCount;
        }
     
        Vector3 localScale = Vector3.one;
        for (int i = transform.childCount; i < curInitCount; ++i)
        {
            if (i == 0)
            {
                localScale = uiTemplete.transform.localScale;
            }
            uiTemplete.transform.localScale = Vector3.one;

            GameObject obj = UsedCreateTempleteInst();
            if (null != obj)
            {
                obj.transform.localScale = localScale;
                obj.name = obj.name + i.ToString();
                obj.SetActive(true);
            }
        }
        
        //多余的移
        List<GameObject> objs = new List<GameObject>();
        for (int i = curInitCount; i < transform.childCount; ++i)
        {
            objs.Add(transform.GetChild(i).gameObject);
        }
                
        for (int i = 0; i < objs.Count; ++i )
        {
            RecycleTempleteInst(objs[i]);
        }

        mWrapContent.SortBasedOnScrollMovement();
        //reset
        mPanel.clipOffset = mInitPanelClipOffset;
        mPanel.cachedTransform.localPosition = mInitPanelLocalPos;
             

        //竖向的，向上加的为负
        int minStart = mMaxItemCount;
        if (initCount > mMaxItemCount)
        {
            minStart = initCount;
        }
        else
        {
            minStart = initCount;
        }

        mWrapContent.minIndex = 1 - minStart;
        mWrapContent.maxIndex = 0;
        mWrapContent.enabled = initCount > mMaxItemCount ? true : false;
        //mWrapContent.WrapContent();
        
        uiScollView.restrictWithinPanel = true;
        uiScollView.ResetPosition();
    }
    
    protected void ReInitGameOjbects(int initCount)
    {     
        //竖向的，向上加的为负
        int minStart = mMaxItemCount;
        if (initCount > mMaxItemCount)
        {
            minStart = initCount;
        }
        else
        {
            minStart = initCount;
        }


        mWrapContent.minIndex = 1 - minStart;
        mWrapContent.maxIndex = 0;

        mWrapContent.ForceWrapContent();     
    }


    //实现数据刷新
    public abstract void OnUpdateItemImp(GameObject go, int wrapIndex, int realIndex);

    //代理 
    private void OnUpdateItem(GameObject go, int wrapIndex, int realIndex)
    {        
        OnUpdateItemImp(go, wrapIndex, realIndex);
    }

    private GameObject UsedCreateTempleteInst()
    {
        if (mTempletePool.Count > 0)
        {
            GameObject go = mTempletePool[mTempletePool.Count - 1];

            //set parent
            Transform t = go.transform;
            t.parent = gameObject.transform;
            t.localPosition = Vector3.zero;
            t.localRotation = Quaternion.identity;
            t.localScale = Vector3.one;
            go.layer = gameObject.layer;

            mTempletePool.RemoveAt(mTempletePool.Count - 1);
            return go;
        }
        else
        {
            GameObject obj = NGUITools.AddChild(gameObject, uiTemplete);
            return obj;
        }
    }

    private void RecycleTempleteInst(GameObject go)
    {
        if (null == mPoolNode)
        {
            mPoolNode = new GameObject("Object_WrapContent_Pool");
        }

        go.SetActive(false);
        go.transform.parent = mPoolNode.transform;
        mTempletePool.Add(go);
    }
}
