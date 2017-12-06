using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FixedMultiRowWrapContent : MultiRowWrapContent
{
    public enum ScaleType
    {
        FixBackground, //基于底板长度
        FixObjectScale, //子obj整体缩放
    }

    public UIScrollView mViewFrame;
    public int mRowNum = 1;
    public float mItemGap;
    public ScaleType mScaleType = ScaleType.FixBackground;
    
    private int templeteWidth = 100;

    public override int GetRowNum()
    {
        return mRowNum;
    }

    protected override float GetTempleteWidth()
    {
        return templeteWidth + mItemGap;
    }

    protected override void InitTempleteScale(GameObject uiTemplete)
    {
        if (mScaleType == ScaleType.FixBackground)
        {
            UISprite sp = uiTemplete.GetComponent<UISprite>();
            UITexture tex = null;

            if (null == sp)
            {
                tex = uiTemplete.GetComponent<UITexture>();
                if (null == tex)
                {
                    return;
                }
            }

            mViewFrame.panel.Update(); //必须刷新一次
            templeteWidth = Mathf.FloorToInt((mViewFrame.panel.finalClipRegion.z - (mItemGap * (mRowNum + 1))) / mRowNum);

            if (null != sp)
            {
                sp.width = templeteWidth;
            }
            else if (null != tex)
            {
                tex.width = templeteWidth;
            }
            else
            {
                return;
            }           
        }
        else if (mScaleType == ScaleType.FixObjectScale)
        {
            float preWidth = mViewFrame.panel.finalClipRegion.z;
            mViewFrame.panel.Update(); 
            float curWidth = mViewFrame.panel.finalClipRegion.z;
                    
            float scale = curWidth / preWidth;     
            Bounds b = NGUIMath.CalculateRelativeWidgetBounds(uiTemplete.transform, true);        
            templeteWidth = Mathf.FloorToInt(b.size.x * scale);

            uiTemplete.transform.localScale = scale * Vector3.one;

            //高度缩放
            itemSize = Mathf.FloorToInt(itemSize * scale);
         
            //间距也要缩放
            mItemGap = mItemGap * scale;
        }

        if (templeteWidth == 0)
        {
            return;
        }

        float center = mViewFrame.panel.finalClipRegion.x;
        float posX = 0;
        if (mRowNum % 2 == 0)
        {
            posX = center - (mRowNum / 2) * templeteWidth + templeteWidth / 2 - mItemGap / 2;
        }
        else
        {
            posX = center - (mRowNum / 2) * templeteWidth - mItemGap;
        }

        transform.localPosition = new Vector3(posX, itemSize / 2, transform.localPosition.z);
    }

}
