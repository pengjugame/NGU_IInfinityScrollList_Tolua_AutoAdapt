using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoMultiRowWrapContent : MultiRowWrapContent
{
    public int mItemWidth = 100;
    public UIScrollView mViewFrame;

    private int rowNum;

    public override int GetRowNum()
    {
        if (rowNum == 0)
        {
            InitRowNum();
        }
        return rowNum;
    }

    protected override float GetTempleteWidth()
    {
        return mItemWidth;
    }

    void InitRowNum()
    {
        mViewFrame.panel.Update(); //必须刷新一次

        float viewWidth = mViewFrame.panel.finalClipRegion.z;

        int col = Mathf.FloorToInt(viewWidth / mItemWidth);
        if (col < 1)
        {
            col = 1;
        }

        rowNum = col;

        float center =  mViewFrame.panel.finalClipRegion.x;
        float posX = 0;
        if (rowNum % 2 == 0)
        {
            posX = center -(rowNum / 2) * mItemWidth + mItemWidth / 2;
        }
        else
        {
            posX = center -(rowNum / 2) * mItemWidth;
        }

        transform.localPosition = new Vector3(posX, itemSize / 2, transform.localPosition.z);
    }
    
}
