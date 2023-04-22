using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using DG.Tweening;
/// <summary>
/// 设定窗口类型
/// </summary>
public enum WindowsType : byte
{
    AnimatorWin,        //带动画机的窗口 开关效果由动画机控制
    BoxMsgWin,          //提示性的窗口类 同上
    DescInfoWin,        //描述说明类 位置由Btn抛出 需要赋值Btn的位置position
    Null,               //没有效果
    AlphaFade,          //show弹框出现动画：时间0~5帧，缩放1.1~1，渐变出现（不透明度）0~1  hide 弹框消失动画：时间0~8帧，缩放1~1.05，渐变消失（不透明度）1~0
}


/// <summary>
/// 通用单例窗口基类
/// 
/// 窗口类应当只适用于对于UI的控制,尽量避免处理数据。
/// 1. 每创建一个窗口类 （应当）同时创建一个针对于本UI窗体交互的数据类。如InfoWin，应该存在一个InfoWinData。 后简称为Data。
/// 2. Data 应当不继承任何基类。 如MonoBehaviour，仅作为对该窗体的数据交互，用来保存解析的客户端数据，获取的服务器数据。
/// 3. UI窗体所需的数据在Data进行处理后，以(结构体)的数据类型传入UI窗体，或以封装好的状态行为传给UI窗体。
/// 4. UI窗体根据行为状态来编写相应的表现，尽量避免在UI窗体类进行功能逻辑。
/// 5. 基本流程为：UI获取组件、注册事件 -> 事件发送请求到服务器(或客户端Data,自己处理) -> Data经过服务器(或客户端)数据改变或者状态改变 -> UI窗体监听到数据和状态进行表现
///
///  时间顺序: BaseWin: Awake() -> InitUI() -> InitLanguage() -> InitEvent()
///         -> ChildWin: Awake() -> new  ChildData -> InitData() -> InitUI() -> InitLanguage() -> InitEvent()
///         -> UserClick -> Show() -> GetData() -> 状态UI效果;
/// 
///  简单的UI窗体则可以在窗体类进行逻辑编写， 但逻辑和UI表现尽量分开。
/// </summary>
/// <typeparam name="T">泛型窗口实体单例</typeparam>

public abstract class WindowsBase<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance = null;
    
    private static bool applicationIsQuitting = false;

    //遮罩关闭按钮
    protected Button maskCloseBtn;

    //窗口Panel
    protected Transform mainPanel;

    //窗口类型
    private WindowsType windowsType;
    
    //规则 Show Hide动画，开放给特效去做。 
    [HideInInspector]
    public Animator animator;

    //开关窗口的位置  
    [HideInInspector]
    public Vector3 startPoint;
    
    //tweening speed 
    private const float animatorSpeed = 0.2f;
    private const float alphaOpenFadeTime = 0.1f;  //5帧
    private const float alphaCloseFadeTime = 0.15f;  //5帧

    //单例窗口是否打开
    private bool isOpen;

    
    /// <summary>
    /// 用来手动GC释放该窗口实体类 
    /// Destroy   
    /// </summary>
    public static void Destroy()
    {
        if (_instance != null)
        {
            WindowsManager.Instance.RemoveWindow(_instance.gameObject);
            GameObject.Destroy(_instance.gameObject);
            _instance = null;
        }
    }
    
    /// <summary>
    /// 自动创建 该窗口类到指定位置下 
    /// </summary>
    public static T Instance
    {
        get
        {
            if (applicationIsQuitting)
            {
                return null;
            }

            if (_instance == null)
            {
                GameObject canvas = GameObject.Find("Canvas");
                if (canvas == null)
                {
                    Debug.Log("Can't find Canvas，Create it now");
                    canvas = new GameObject("Canvas");
                    canvas.AddComponent<Canvas>();
                    canvas.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
                    canvas.GetComponent<Canvas>().sortingOrder = 1;
                }
                else
                {
                    canvas.GetComponent<Canvas>().sortingOrder = 1;
                }
                GameObject chatCanvas = GameObject.Find("ChatCanvas");
                if (chatCanvas == null)
                {
                    Debug.Log("Can't find ChatCanvas，Create it now");
                    chatCanvas = new GameObject("ChatCanvas");  //聊天界面父节点位置
                    chatCanvas.AddComponent<Canvas>();
                    chatCanvas.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
                    chatCanvas.GetComponent<Canvas>().sortingOrder = 2;
                }
                else
                {
                    chatCanvas.GetComponent<Canvas>().sortingOrder = 2;
                }
                
                Type t = typeof(T);
                if (t.Name.Contains("Chat"))   //聊天界面
                {
                    Transform chatTrans = chatCanvas.transform.Find(t.Name);
                    if (chatTrans == null)
                    {
                        GameObject go = GameObject.Instantiate(Resources.Load(string.Format("Prefabs/WindowsChatPrefabs/{0}", t.Name))) as GameObject;
                        go.transform.SetParent(chatCanvas.transform);
                        go.name = t.Name;
                        SetTransformPosition(go);
                    }
                    else
                    {
                        _instance = chatTrans.GetComponent<T>();
                    }
                }
                else                            //一般界面
                {
                    Transform trans = canvas.transform.Find(t.Name);
                    if (trans == null)
                    {
                        GameObject go = GameObject.Instantiate(Resources.Load(string.Format("Prefabs/WindowsPrefabs/{0}", t.Name))) as GameObject;
                        go.transform.SetParent(canvas.transform);
                        go.name = t.Name;
                        SetTransformPosition(go);
                    }
                    else
                    {
                        _instance = trans.GetComponent<T>();
                    }
                }
                //都没找到
                if (_instance == null)
                {
                    Debug.Log("Can't find Windows: " + t.Name);
                }
            }

            return _instance;
        }
    }

    /// <summary>
    /// 初始化窗体位置状态信息
    /// </summary>
    /// <param name="obj"></param>
    private static void SetTransformPosition(GameObject obj)
    {
        RectTransform rectTrans = obj.GetComponent<RectTransform>();
        Vector3 localPosition = rectTrans.localPosition;
        obj.gameObject.SetActive(false);
        localPosition.z = 0f;
        rectTrans.localPosition = localPosition;
        rectTrans.localEulerAngles = Vector3.zero;
        rectTrans.anchoredPosition = Vector3.zero;
        rectTrans.anchorMax = Vector2.one;
        rectTrans.anchorMin = Vector2.zero;
        rectTrans.offsetMax = Vector2.zero;
        rectTrans.offsetMin = Vector2.zero;
        rectTrans.localScale = Vector3.one;
        _instance = obj.GetComponent<T>();
    }
    

    /*******************************************子类重写**************************************/
    
    protected virtual void Awake()
    {
        if (transform.Find("UIBtn"))
            maskCloseBtn = transform.Find("UIBtn").GetComponent<Button>();
        if (transform.Find("Panel"))
            mainPanel = transform.Find("Panel");
        
        this.InitUI();
        this.InitLanguage();
        this.InitEvent();
    }
    
    /// <summary>
    /// 子类开放：用来获取UI组件
    /// 初始子类赋值：
    ///  windowsType -> WindowsType.   if DescInfoWin -> startPoint;
    /// </summary>
    public virtual void InitUI() { }

    /// <summary>
    /// 子类开放：初始化语言赋值
    /// </summary>
    public virtual void InitLanguage() { }
    
    /// <summary>
    /// 子类开放：注册Btn等事件
    /// 需要子类注册 maskCloseBtn.onClick.AddListener(T); T->Hide();  T->HideImmediate(); T->HideDelay(float);
    /// </summary>
    public virtual void InitEvent() { }

    /// <summary>
    /// 子类开放：用来进行UI初始化，数据初始化
    /// </summary>
    public virtual void OnShow() { }
    
    /// <summary>
    /// 子类开放：用来进行手动GC，或者窗口关闭时的内存释放。
    /// </summary>
    public virtual void OnHide() { }

    
    /******************************************窗口单例调用****************************************/
    
    /// <summary>
    /// 窗口基类：动画打开窗口 
    /// </summary>
    /// <param name="_type">type为窗口类型</param>
    public void Show(WindowsType _type)
    {
        if (isOpen)
            return;
        windowsType = _type;
        SetGameObjectActive(true);
        if (windowsType == WindowsType.AnimatorWin)
        {
            if (this.animator == null)
                this.animator = this.GetComponent<Animator>();
            if (this.animator != null)
                this.animator.SetBool("isOpen", true);
        }
        else if (windowsType == WindowsType.BoxMsgWin)
        {
            mainPanel.localScale = new Vector3(0.3f, 0.3f, 0.3f);
            mainPanel.DOScale(Vector3.one, 0.2f);
        }
        else if(windowsType == WindowsType.DescInfoWin)
        {
            mainPanel.transform.localPosition = startPoint;
            mainPanel.transform.DOLocalMove(new Vector3(transform.localPosition.x, transform.localPosition.y , 0),animatorSpeed);
            mainPanel.localScale = new Vector3(0.2f, 0.2f, 0.2f);
            mainPanel.DOScale(Vector3.one, animatorSpeed);
        }
        else if (windowsType == WindowsType.Null)
        {
            mainPanel.localScale = Vector3.one;
        }
        else if (windowsType == WindowsType.AlphaFade)
        {
            mainPanel.localScale = new Vector3(1.1f, 1.1f, 1f);
            if (!this.transform.GetComponent<CanvasGroup>())
                this.transform.gameObject.AddComponent<CanvasGroup>();
            var canvas = this.transform.GetComponent<CanvasGroup>();
            canvas.alpha = 0;
            
            mainPanel.DOScale(Vector3.one, alphaOpenFadeTime);
            canvas.DOFade(1, alphaOpenFadeTime);
        }
        
        OnShow();

    }

    /// <summary>
    /// 窗口基类：动画关闭窗口 或 简单关闭窗口
    /// </summary>
    public void Hide()
    {
        OnHide();
        SetGameObjectActive(false);
        if (windowsType == WindowsType.AnimatorWin)
        {
            if (this.animator == null)
                this.animator = this.GetComponent<Animator>();
            if (this.gameObject.activeSelf)
            {
                if (this.animator != null)
                {
                    this.animator.SetBool("isOpen", false);
                    HideDelay(this.animator.GetCurrentAnimatorClipInfo(0).Length);
                }
            }
        }
        else if (windowsType == WindowsType.BoxMsgWin)
        {
            mainPanel.DOScale(new Vector3(0.2f, 0.2f, 0.2f), 0.2f).OnComplete(()=> this.gameObject.SetActive(false));
        }
        else if(windowsType == WindowsType.DescInfoWin)
        {
            mainPanel.transform.position = new Vector3(transform.localPosition.x, transform.localPosition.y , 0);
            mainPanel.transform.DOLocalMove(startPoint,animatorSpeed);
            mainPanel.DOScale(new Vector3(0.2f, 0.2f, 0.2f), animatorSpeed).OnComplete(()=> this.gameObject.SetActive(false));
        }
        else if (windowsType == WindowsType.Null)
        {
            this.gameObject.SetActive(false);
        }
        else if (windowsType == WindowsType.AlphaFade)
        {
            mainPanel.localScale = new Vector3(1, 1, 1f);
            if (!this.transform.GetComponent<CanvasGroup>())
                this.transform.gameObject.AddComponent<CanvasGroup>();
            var canvas = this.transform.GetComponent<CanvasGroup>();
            canvas.alpha = 1;
            mainPanel.DOScale(new Vector3(1.05f, 1.05f, 1f), alphaCloseFadeTime);
            canvas.DOFade(0, alphaCloseFadeTime).OnComplete(()=> this.gameObject.SetActive(false));
        }
        
    }
    
    /// <summary>
    /// 窗口基类：立即隐藏窗口
    /// </summary>
    public void HideImmediate()
    {
        SetGameObjectActive(false);
        this.gameObject.SetActive(false);
    }
    
    /// <summary>
    /// 窗口基类：延迟关闭窗口
    /// </summary>
    /// <param name="delayTime"></param>
    public void HideDelay(float delayTime)
    {
        SetGameObjectActive(false);
        StartCoroutine(IEDelayHide(delayTime));
    }
    
    
    /**********************父类基类函数******************************/
    
    /// <summary>
    /// 基类：延迟关闭窗口
    /// </summary>
    /// <param name="delayTime">计时关闭窗口</param>
    /// <returns></returns>
    IEnumerator IEDelayHide(float delayTime)
    {
        yield return new WaitForSeconds(delayTime);
        this.gameObject.SetActive(false);
    }



    /// <summary>
    /// 记录当前窗口的开启关闭状态
    /// </summary>
    /// <param name="_isOpen"></param>
    private void SetGameObjectActive(bool _isOpen)
    {
        if (_isOpen)
        {
            this.isOpen = true;
            WindowsManager.Instance.SaveWindow(this.gameObject);
            this.gameObject.SetActive(true);
        }
        else
        {
            this.isOpen = false;
            WindowsManager.Instance.RemoveWindow(this.gameObject);
        }
    }





    /**************************如果有常用的方法，自己添加,尽量保持数据的单向性****************************************/


    /*翻页*/
    private int pageindex = 0;   
    private int pageLength = 0;
    private Button pageLeftBtn;
    private Button pageNextBtn;
    /// <summary>
    ///  初始化: 序号 page长度 左右Btn override直接写状态
    /// </summary>
    /// <param name="isLeft">是否左翻页</param>
    public virtual void OnTurnPage(bool isLeft)
    {
        if (isLeft)
        {
            pageindex = pageindex == 0 ? 0 : pageindex--;
        }
        else
        {
            pageindex = pageindex == pageLength - 1 ? pageLength : pageindex++;
        }
        
        //Override
        
    }
    
    
    
    
    /*页面单计时器*/
    private int timerMin = 0;
    private int timeCur;
    
    /// <summary>
    /// 计时器开始 初始化赋值timeCur的值  update调用text  finish回调
    /// </summary>
    public void TimerStart()
    {
        if (timeCur > timerMin)
            InvokeRepeating("TimerMeterEvent", 1f, 1f);
        else
            OnTimerFinish();
        
        OnTimerStart();
    }
    /// <summary>
    /// 外置改变时间 不停止定时器
    /// </summary>
    /// <param name="change"></param>
    public void TimerUpdate(int change)
    {
        timeCur = change;
    }
    /// <summary>
    /// 强制停止定时器
    /// </summary>
    public void TimerStop()
    {
        CancelInvoke("TimerMeterEvent");
        OnTimerFinish();
    }
    
    //子类开放
    public virtual void OnTimerStart() { }
    public virtual void OnTimerUpdate() { }
    public virtual void OnTimerFinish() { }
    
    //基类方法
    private void TimerMeterEvent()
    {
        if (timeCur <= timerMin)
        {
            CancelInvoke("TimerMeterEvent");
            OnTimerFinish();
        }
        else
        {
            timeCur -= 1;
            OnTimerUpdate();
        }
    }

    
    
    
    
    /**/













}


