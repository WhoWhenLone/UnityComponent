/************************************************************
  Copyright (C), 2007-2016,BJ Rainier Tech. Co., Ltd.
  FileName: UIController.cs
  Author:忽文龙       Version :1.0          Date: 2019-0-0
  Description:播放视频脚本
************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class VideoController : MonoBehaviour
{
    //视频名称
    public string videoName;
    //videoImage组件
    public GameObject VideoImage;
    //进度条组件
    public GameObject SliderController;
    //播放按钮
    public GameObject PlayBtn;
    //暂停按钮
    public GameObject PauseBtn;
    //public GameObject CenterPlayBtn;
    //视频纹理
    public RenderTexture renderTexture;
    //视频第一帧
    public GameObject FirstFrameImage;
    //视频时长
    public float Length;
    //
    private bool isDrag = false;

    // Use this for initialization
    private void Awake()
    {
        InitVideo(renderTexture);
    }
    void Start()
    {
        #region 注册事件
        PlayBtn.GetComponent<Button>().onClick.AddListener(PlayVideo);
        //CenterPlayBtn.GetComponent<Button>().onClick.AddListener(PlayVideo);
        PauseBtn.GetComponent<Button>().onClick.AddListener(PauseVideo);
        AddTriggersListener(SliderController, EventTriggerType.BeginDrag, OnBeginDrag);
        AddTriggersListener(SliderController, EventTriggerType.EndDrag, OnEndDrag);
        #endregion

    }

    // Update is called once per frame
    void Update()
    {
        ChangeSliderValue();
        ChangeVideoPlayTime();
        Debug.Log("Isdrag" + isDrag);
        if (!gameObject.GetComponentInChildren<VideoPlayer>().isPlaying)
        {
            PauseVideo();
        }
    }
    /// <summary>
    /// 初始化视频控制器
    /// </summary>
    public void InitVideoController()
    {
        PlayBtn.SetActive(true);
        //CenterPlayBtn.SetActive(true);
        PauseBtn.SetActive(false);
        PauseVideo();
    }
    /// <summary>
    /// 初始化videoplayer
    /// </summary>
    public void InitVideo(RenderTexture renderTexture)
    {
        //DestroyImmediate(gameObject.transform.Find("VideoImage").gameObject.GetComponent<VideoPlayer>());
        gameObject.transform.Find("VideoImage").gameObject.AddComponent<VideoPlayer>();
        VideoPlayer videoPlayer = gameObject.GetComponentInChildren<VideoPlayer>();
        videoPlayer.source = VideoSource.Url;
        videoPlayer.renderMode = VideoRenderMode.RenderTexture;
        videoPlayer.playOnAwake = false;
        videoPlayer.targetTexture = renderTexture;

        //适配平台
        if (Application.platform == RuntimePlatform.WebGLPlayer)
        {
            videoPlayer.url = Application.dataPath + "/StreamingAssets/Video/" + videoName + ".ogv";
        }
        else if (Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.WindowsEditor)
        {
            videoPlayer.url = Application.dataPath + "/StreamingAssets/Video/" + videoName + ".mp4";
        }

        videoPlayer.audioOutputMode = VideoAudioOutputMode.None;
        videoPlayer.Prepare();
        //设置slider 长度
        SliderController.GetComponent<Slider>().maxValue = Length;
        if (FirstFrameImage != null)
        {
            FirstFrameImage.SetActive(true);
        }

        PauseVideo();
    }
    /// <summary>
    /// 播放
    /// </summary>
    private void PlayVideo()
    {
        gameObject.GetComponentInChildren<VideoPlayer>().Play();
        PlayBtn.SetActive(false);
        //CenterPlayBtn.SetActive(false);
        PauseBtn.SetActive(true);
        if (FirstFrameImage != null)
        {
            FirstFrameImage.SetActive(false);
        }

    }
    /// <summary>
    /// 暂停
    /// </summary>
    public void PauseVideo()
    {
        gameObject.GetComponentInChildren<VideoPlayer>().Pause();
        PlayBtn.SetActive(true);
        //CenterPlayBtn.SetActive(true);
        PauseBtn.SetActive(false);
        FirstFrameImage.SetActive(true);
    }

    /// <summary>
    /// 修改视频播放进度
    /// </summary>
    private void ChangeVideoPlayTime()
    {
        if (isDrag == true)
        {
            gameObject.transform.Find("VideoImage").gameObject.GetComponent<VideoPlayer>().time = SliderController.GetComponent<Slider>().value;
        }
    }
    /// <summary>
    /// 修改进度条
    /// </summary>
    private void ChangeSliderValue()
    {
        if (!isDrag)
        {
            SliderController.GetComponent<Slider>().value = (float)gameObject.transform.Find("VideoImage").gameObject.GetComponent<VideoPlayer>().time;
        }
    }
    /// <summary>
    /// 开始拖拽
    /// </summary>
    /// <param name="enenvt"></param>
    private void OnBeginDrag(BaseEventData enenvt)
    {
        isDrag = true;
    }
    /// <summary>
    /// 结束拖拽
    /// </summary>
    /// <param name="enenvt"></param>
    private void OnEndDrag(BaseEventData enenvt)
    {
        isDrag = false;
        gameObject.transform.Find("VideoImage").gameObject.GetComponent<VideoPlayer>().time = SliderController.GetComponent<Slider>().value;
    }


    /// <summary>
    /// 为obj添加Eventrigger监听事件
    /// </summary>
    /// <param name="obj">添加监听的对象</param>
    /// <param name="eventID">添加的监听类型</param>
    /// <param name="action">触发的函数</param>
    private void AddTriggersListener(GameObject obj, EventTriggerType eventType, UnityAction<BaseEventData> action)
    {
        //首先判断对象是否已经有EventTrigger组件，若没有那么需要添加
        EventTrigger trigger = obj.GetComponent<EventTrigger>();
        if (trigger == null)
        {
            trigger = obj.AddComponent<EventTrigger>();
        }
        //实例化delegates
        if (trigger.triggers.Count == 0)
        {
            trigger.triggers = new List<EventTrigger.Entry>();//  
        }
        //定义所要绑定的事件类型 
        EventTrigger.Entry entry = new EventTrigger.Entry();
        //设置事件类型  
        entry.eventID = eventType;
        //定义回调函数  
        UnityAction<BaseEventData> callback = new UnityAction<BaseEventData>(action);
        //设置回调函数  
        entry.callback.AddListener(callback);
        //添加事件触发记录到GameObject的事件触发组件  
        trigger.triggers.Add(entry);
    }
}
