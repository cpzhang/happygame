/********************************************************************
	created:	2014/06/20
	author:		凌炜楹
    company:    深圳自游网络有限公司
	purpose:	主城界面右下角功能显示伸收功能
*********************************************************************/
using UnityEngine;
using System.Collections;

public class StrecthFuntions : MonoBehaviour {
	//本对象的transform
	//private Transform transform_;
	//伸缩速度
	private float strecth_speed_ = 0.01f;
	//伸缩状态枚举
	private enum EStrecthStatus
	{
		StrecthStatus_Ined = 0,			//完全缩入
		StrecthStatus_Outing,			//正在伸展
		StrecthStatus_Outed,			//完全伸展
		StrecthStatus_Ining,			//正在缩入
	}
	//伸缩状态
	private EStrecthStatus strecth_status_ = EStrecthStatus.StrecthStatus_Outed;
	//伸缩缓冲像素
	private float strecth_cache_ = 10f;
	//记录各功能按钮的展开状态下的位置
	private struct Function_Info
	{
		public Transform fun_transform;
		public Vector3 fun_position;
		public Function_Info(Transform trans ,Vector3 pos)
		{
			fun_transform = trans;
			fun_position = pos;
		}
	}
	private global::System.Collections.Generic.List<Function_Info> function_info_list_ = new System.Collections.Generic.List<Function_Info>();
	//各功能按钮的父对象
    public Transform funtions_trans_;
    public GameObject btn_object_;
	//缩入按钮
    public GameObject in_btn_object_;
	//伸展按钮
    public GameObject out_btn_object_;
    public int right_side_ = 0;
    public int strecth_in_ = 0;
	
	void Awake()
	{
		//transform_ = transform;

		//记录各功能按钮的展开状态下的位置
		for(int i = 0 ;i < funtions_trans_.childCount ;i++)
		{
			Transform info_trans = funtions_trans_.GetChild(i);
			Vector3 info_pos = info_trans.localPosition;
			Function_Info info = new Function_Info(info_trans ,info_pos);
			function_info_list_.Add(info);
		}

		UIEventListener.Get (in_btn_object_).onClick = StrecthAllIn;
		UIEventListener.Get (out_btn_object_).onClick = StrecthAllOut;
        if (null != btn_object_)
        {
            //UIEventListener.Get(btn_object_).onClick = StrecthAll;
        }

        if (strecth_in_ > 0)
        {
            strecth_status_ = EStrecthStatus.StrecthStatus_Outed;
            StrecthAllIn(null);
        }
        else
        {
            strecth_status_ = EStrecthStatus.StrecthStatus_Ined;
            StrecthAllOut(null);
        }
	}
	
	void Update()
	{
		int funtionnum = function_info_list_.Count;
		//0个功能按钮不用处理
		if(funtionnum > 0)
		{
			//strecth ining状态下才能缩入
			if(strecth_status_ == EStrecthStatus.StrecthStatus_Ining)
			{
				//按位移比例来移动
				for(int i = 0 ;i < funtionnum ;i++)
				{
					Function_Info info = function_info_list_[i];
					info.fun_transform.Translate(-Time.deltaTime*info.fun_position*strecth_speed_);
				}
				Function_Info info0 = function_info_list_[0];
				//当其中一个达到指定位置就改状态为strecth ined
                if ((right_side_ == 0 && info0.fun_transform.localPosition.x > 0 - strecth_cache_)
                    || (right_side_ > 0 && info0.fun_transform.localPosition.x < strecth_cache_))
				{
					//隐藏各个功能按钮
					for(int i = 0 ;i < funtions_trans_.childCount ;i++)
					{
						Function_Info info = function_info_list_[i];
                        UIWidget[] widgets = info.fun_transform.GetComponentsInChildren<UIWidget>();
                        int wNum = widgets.Length;
                        for (int j = 0; j < wNum;j++)
                        {
                            widgets[j].alpha = 0f;
                        }
						info.fun_transform.gameObject.SetActive(false);
						//移动时产生浮点误差，要调回正确位置
						info.fun_transform.localPosition = new Vector3(0,0,0);
					}
					strecth_status_ = EStrecthStatus.StrecthStatus_Ined;
				}
			}
			//strecth outing状态下才能伸展
			else if(strecth_status_ == EStrecthStatus.StrecthStatus_Outing)
			{
				//按位移比例来移动
				for(int i = 0 ;i < funtionnum ;i++)
				{
					Function_Info info = function_info_list_[i];
					info.fun_transform.Translate(Time.deltaTime*info.fun_position*strecth_speed_);
				}
				Function_Info info0 = function_info_list_[0];
				//当其中一个达到指定位置就改状态为strecth outed
                if ((right_side_ == 0 && info0.fun_transform.localPosition.x < info0.fun_position.x + strecth_cache_)
                    || (right_side_ > 0 && info0.fun_transform.localPosition.x > info0.fun_position.x - strecth_cache_))
				{
					//移动时产生浮点误差，要调回正确位置
					for(int i = 0 ;i < funtions_trans_.childCount ;i++)
					{
						Function_Info info = function_info_list_[i];
						info.fun_transform.localPosition = info.fun_position;
					}
					strecth_status_ = EStrecthStatus.StrecthStatus_Outed;
				}
			}
		}
	}

    void StrecthAll(GameObject go)
    {
        if (strecth_status_ == EStrecthStatus.StrecthStatus_Outed)
        {
            StrecthAllIn(go);
        }
        else if (strecth_status_ == EStrecthStatus.StrecthStatus_Ined)
        {
            StrecthAllOut(go);
        }
    }
	
	public void StrecthAllIn(GameObject go)
	{
		//strecth outed状态下,处理完相应操作后，才能改状态为strecth ining  
		if(strecth_status_ == EStrecthStatus.StrecthStatus_Outed)
		{
			//隐蔽in按钮，显示out按钮
			in_btn_object_.SetActive(false);
			out_btn_object_.SetActive(true);
			strecth_status_ = EStrecthStatus.StrecthStatus_Ining;
		}
	}

    public void HideIconWhenIn()
    {
        if (strecth_status_ == EStrecthStatus.StrecthStatus_Ined || strecth_status_ == EStrecthStatus.StrecthStatus_Ining)
        {
            //隐藏各个功能按钮
            for (int i = 0; i < funtions_trans_.childCount; i++)
            {
                Function_Info info = function_info_list_[i];
                UIWidget[] widgets = info.fun_transform.GetComponentsInChildren<UIWidget>();
                int wNum = widgets.Length;
                for (int j = 0; j < wNum; j++)
                {
                    widgets[j].alpha = 0f;
                }
            }
        }
    }

    void StrecthAllOut(GameObject go)
	{
		//strecth ined状态下,处理完相应操作后，才能改状态为strecth outing
		if(strecth_status_ == EStrecthStatus.StrecthStatus_Ined)
		{
			//隐蔽out按钮，显示in按钮
			out_btn_object_.SetActive(false);
			in_btn_object_.SetActive(true);
			//显示各个功能按钮
			for(int i = 0 ;i < funtions_trans_.childCount ;i++)
			{
				Function_Info info = function_info_list_[i];
                info.fun_transform.gameObject.SetActive(true);
                UIWidget[] widgets = info.fun_transform.GetComponentsInChildren<UIWidget>();
                int wNum = widgets.Length;
                for (int j = 0; j < wNum; j++)
                {
                    widgets[j].alpha = 1f;
                }
			}
			strecth_status_ = EStrecthStatus.StrecthStatus_Outing;
		}
	}
}
