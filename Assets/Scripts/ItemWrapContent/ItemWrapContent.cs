using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using LuaInterface;
using LuaFramework;
using System.Linq;

public class ItemWrapContent : WrapContentBase
{    
    public class WrapData
    {
        public long sortKey;
        public int falgValue;
    }
 
    private Dictionary<string, List<WrapData>> mFilterList = new Dictionary<string, List<WrapData>>();
    private string mFilterFlag;
    private LuaFunction mLuaCall;

    void OnDestroy()
    {
        if (null != mLuaCall)
        {
            mLuaCall.Dispose();
            mLuaCall = null;
        }
    }

    //初始化
    public void Init(int maxCreateObjCount, GameObject templete)
    {
        InitSettings(maxCreateObjCount, templete);  
    }

    //清理数据
    public void ClearAllFilterData()
    {
        mFilterList.Clear();
    }
    

    //数据是否存在
    public bool IsDataExist(string filterFlag, int data)
    {
        List<WrapData> dataList = null;
        if (mFilterList.TryGetValue(filterFlag, out dataList))
        {
            for (int i = 0; i < dataList.Count; ++i)
            {
                if (dataList[i].falgValue == data)
                {
                    return true;
                }
            }
        }
        return false;
    }


    //添加数据
    public void AddFilterData(string filterFlag, int data, long sortKey, bool bForeUpdate = false)
    {
        WrapData wpData = new WrapData();
        wpData.sortKey = sortKey;
        wpData.falgValue = data;
        
        if (!mFilterList.ContainsKey(filterFlag))
        {
            List<WrapData> dataList = new List<WrapData>();            
            mFilterList.Add(filterFlag, dataList);
        }
        mFilterList[filterFlag].Add(wpData);

        if (bForeUpdate)
        {
            ReInitGameOjbects(mFilterList[mFilterFlag].Count);
        }
    }

    //清除固定过滤类型data的数据
    public void RemoveFilterData(string filterFlag, int data)
    {
        if (mFilterList.ContainsKey(filterFlag))
        {
            foreach (var v in mFilterList[filterFlag])
            {
                if (v.falgValue == data)
                {
                    mFilterList[filterFlag].Remove(v);

                    ReInitGameOjbects(mFilterList[filterFlag].Count);
                    break;
                }
            }           
        }
    }

    //清除所有过滤类型data的数据
    public void RemoveFilterDataCycle(int data)
    {
        bool bCurRemove = false;
        foreach (var v in mFilterList)
        {
            foreach (var d in v.Value)
            {
                if (d.falgValue == data)
                {
                    v.Value.Remove(d);

                    if (v.Key == mFilterFlag)
                    {
                        bCurRemove = true;
                    }
                    break;
                }
            }            
        }

        if (bCurRemove)
        {
            ReInitGameOjbects(mFilterList[mFilterFlag].Count);
        }
    }

    //显示数据
    public void ShowContent(string filterFlag, bool bDescOrder)
    {
        if (!mFilterList.ContainsKey(filterFlag))
            return;

        mFilterFlag = filterFlag;
        List<WrapData> showList = mFilterList[filterFlag];

        //先把ShowList排序
        if (bDescOrder)
        {
            mFilterList[filterFlag] = showList.OrderByDescending(o => o.sortKey).ToList<WrapData>();   
        }
        else
        {
            mFilterList[filterFlag] = showList.OrderBy(o => o.sortKey).ToList<WrapData>();
        }

        InitGameOjbects(showList.Count);
    }

    public void SetWrapCall(LuaFunction call)
    {
        mLuaCall = call;
    }

    //显示回调
    public override void OnUpdateItemImp(GameObject go, int wrapIndex, int realIndex)
    {
        int dataIndex = Mathf.Abs(realIndex);
        List<WrapData> showList = mFilterList[mFilterFlag];
        if (dataIndex >= showList.Count)
        {
            //go.SetActive(false);
            return;
        }

        object flag = showList[dataIndex].falgValue;
        go.SetActive(true);

        if (null != mLuaCall)
        {
            mLuaCall.Call(go, flag, wrapIndex, realIndex);
        }        
    }
}
