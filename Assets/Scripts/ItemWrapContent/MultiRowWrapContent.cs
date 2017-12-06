using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiRowWrapContent : UIWrapContent  
{
    public virtual int GetRowNum()
    {
        return 3;
    }

    public virtual void SetTemplete(GameObject templete)
    {
        InitTempleteScale(templete);
    }

    protected virtual void InitTempleteScale(GameObject uiTemplete)
    {
    }

    protected virtual float GetTempleteWidth()
    {
        return 0;
    }

    protected override void ResetChildPositions()
    {
        int RowNum = GetRowNum();

        float fItemWidth = GetTempleteWidth();
        for (int i = 0, imax = mChildren.Count; i < imax; ++i)
        {
            Transform t = mChildren[i];
            t.localPosition = mHorizontal ? new Vector3(i * itemSize, 0f, 0f) : new Vector3((i % RowNum) * fItemWidth, -(i / RowNum) * itemSize, 0f);
            
            UpdateItem(t, i);
        }
    }

    protected override void UpdateItem(Transform item, int index)
    {
        if (onInitializeItem != null)
        {
            int RowNum = GetRowNum();
            float fItemWidth = GetTempleteWidth();

            int yIndex = Mathf.Abs(Mathf.RoundToInt(item.localPosition.y / itemSize));
            int xIndex = Mathf.Abs(Mathf.RoundToInt(item.localPosition.x / fItemWidth));
            int realIndex = RowNum * yIndex + xIndex;
            onInitializeItem(item.gameObject, index, realIndex);
        }
    }

    protected override void SortBasedOnScrollMovementImp()
    {
        if (!CacheScrollView()) return;

        // Cache all children and place them in order  
        mChildren.Clear();
        for (int i = 0; i < mTrans.childCount; ++i)
        {
            Transform t = mTrans.GetChild(i);
            if (null == t || t.gameObject == null)
            {
                continue;
            }
            //if (hideInactive && !t.gameObject.activeInHierarchy) continue;
            if (!t.gameObject.activeInHierarchy) continue;
            mChildren.Add(t);
        }

        ResetChildPositions();
    }
     
    protected override float GetExtentsCount()
    {
        int RowNum = GetRowNum();
        return itemSize * (mChildren.Count / RowNum) * 0.5f;  
    }

    public void ForceWrapContent()
    {
        mFirstTime = true;
        WrapContent();
        mFirstTime = false;
    }

    public override void WrapContent()
    {
        float extents = GetExtentsCount();
        Vector3[] corners = mPanel.worldCorners;

        for (int i = 0; i < 4; ++i)
        {
            Vector3 v = corners[i];
            v = mTrans.InverseTransformPoint(v);
            corners[i] = v;
        }

        Vector3 center = Vector3.Lerp(corners[0], corners[2], 0.5f);
        bool allWithinRange = true;
        float ext2 = extents * 2f;

        if (mHorizontal)
        {
            float min = corners[0].x - itemSize;
            float max = corners[2].x + itemSize;

            for (int i = 0, imax = mChildren.Count; i < imax; ++i)
            {
                Transform t = mChildren[i];
                float distance = t.localPosition.x - center.x;

                if (distance < -extents)
                {
                    Vector3 pos = t.localPosition;
                    pos.x += ext2;
                    distance = pos.x - center.x;
                    int realIndex = Mathf.RoundToInt(pos.x / itemSize);

                    if (minIndex == maxIndex || (minIndex <= realIndex && realIndex <= maxIndex))
                    {
                        t.localPosition = pos;
                        UpdateItem(t, i);
                    }
                    else allWithinRange = false;
                }
                else if (distance > extents)
                {
                    Vector3 pos = t.localPosition;
                    pos.x -= ext2;
                    distance = pos.x - center.x;
                    int realIndex = Mathf.RoundToInt(pos.x / itemSize);

                    if (minIndex == maxIndex || (minIndex <= realIndex && realIndex <= maxIndex))
                    {
                        t.localPosition = pos;
                        UpdateItem(t, i);
                    }
                    else allWithinRange = false;
                }
                else if (mFirstTime) UpdateItem(t, i);

                if (cullContent)
                {
                    distance += mPanel.clipOffset.x - mTrans.localPosition.x;
                    if (!UICamera.IsPressed(t.gameObject))
                        NGUITools.SetActive(t.gameObject, (distance > min && distance < max), false);
                }
            }
        }
        else
        {
            float min = corners[0].y - itemSize;
            float max = corners[2].y + itemSize;

            for (int i = 0, imax = mChildren.Count; i < imax; ++i)
            {
                Transform t = mChildren[i];
                float distance = t.localPosition.y - center.y;

                if (distance < -extents)
                {
                    Vector3 pos = t.localPosition;
                    pos.y += ext2;
                    distance = pos.y - center.y;
                    int realIndex = Mathf.RoundToInt(pos.y / itemSize);

                    if (minIndex == maxIndex || (minIndex <= realIndex && realIndex <= maxIndex))
                    {
                        t.localPosition = pos;
                        UpdateItem(t, i);
                    }
                    else allWithinRange = false;
                }
                else if (distance > extents)
                {
                    Vector3 pos = t.localPosition;
                    pos.y -= ext2;
                    distance = pos.y - center.y;
                    int realIndex = Mathf.RoundToInt(pos.y / itemSize);

                    if (minIndex == maxIndex || (minIndex <= realIndex && realIndex <= maxIndex))
                    {
                        t.localPosition = pos;
                        UpdateItem(t, i);
                    }
                    else allWithinRange = false;
                }
                else if (mFirstTime) UpdateItem(t, i);

                if (cullContent)
                {
                    distance += mPanel.clipOffset.y - mTrans.localPosition.y;
                    if (!UICamera.IsPressed(t.gameObject))
                        NGUITools.SetActive(t.gameObject, (distance > min && distance < max), false);
                }
            }
        }
        mScroll.restrictWithinPanel = !allWithinRange; 
        mScroll.InvalidateBounds();  

        //说明：目前拖到最后动的太厉害可能会回复不了，先这么着
       // mScroll.restrictWithinPanel = true;       
       
    }  
}
