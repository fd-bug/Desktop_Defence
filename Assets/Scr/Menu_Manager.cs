using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace fdbug
{
    public class Menu_Manager : MonoBehaviour
    {
        #region 變數
        private Button Btn_Continue;
        private Button Btn_StartGame;
        private Button Btn_Option;
        private Button Btn_Option_Ingame;
        private Button Btn_Credit;
        private Button Btn_Exit;
        private Button Btn_Back_Option, Btn_Back_Credit;
        private CanvasGroup  Group_Option, Group_Credit, Group_GameUI;
        private Slider Slider_Master, Slider_BGM, Slider_Music;
        #endregion

        [Header("🎯 轉場動態設定")]
        public CanvasGroup Group_MainMenu;     // 整個主選單的 CanvasGroup (用於淡出)
        public Transform mainCamera;           // 主攝影機
        public Transform cameraGamePosition;   // 攝影機在書桌前的終點位置 (在場景中放空物件定位)
        public float cameraMoveDuration = 3.0f;// 攝影機走過去要花幾秒

        [Header("🚪 門的設定")]
        public Transform doorTransform;        // 門的 Transform
        public Vector3 doorOpenRotation;       // 門打開後的歐拉角 (例如 0, 90, 0)
        public float doorOpenDuration = 1.5f;  // 門打開要花幾秒

        [Header("🎮 遊戲核心對接")]
        public GameObject enemySpawner;        // 你的 Enemy_Spawner 物件 (一開始在場景中設為關閉)

        //private Sound_Manager Sound_Mng;
        //private Loading_Manager Loading_Mng;
        //喚醒
        private void Start()
        {

            #region 獲取介面物件
            //尋找名稱為"Btn_Continue"的物件，並取得其Button元件，類推
            Btn_StartGame=GameObject.Find("BTN_Startgame").GetComponent<Button>();
            Btn_Option=GameObject.Find("BTN_Options").GetComponent<Button>();
            Btn_Option_Ingame = GameObject.Find("BTN_Options_Ingame").GetComponent<Button>();
            Btn_Credit =GameObject.Find("BTN_Credit").GetComponent<Button>();
            Btn_Exit=GameObject.Find("BTN_Exit").GetComponent<Button>();
            Btn_Back_Option=GameObject.Find("BTN_Back_Option").GetComponent<Button>();
            //Btn_Back_Credit=GameObject.Find("BTN_Back_Credit").GetComponent<Button>();

            //Group_Continue=GameObject.Find("Button_Continue").GetComponent<CanvasGroup>(); //該button有使用canvasGroup元件來達成條件隱藏。
            Group_Option =GameObject.Find("Canvas_Options").GetComponent<CanvasGroup>();
            
            //Group_Credit=GameObject.Find("Group_Credit").GetComponent<CanvasGroup>();
            Group_GameUI= GameObject.Find("Canvas_UI").GetComponent<CanvasGroup>();
            Slider_Master =GameObject.Find("Slider_MS").GetComponent<Slider>();
            Slider_BGM=GameObject.Find("Slider_B").GetComponent<Slider>();
            Slider_Music=GameObject.Find("Slider_M").GetComponent<Slider>();
            #endregion

            //為按鈕添加點擊事件，當按鈕被點擊時，會呼叫方法
            Btn_StartGame.onClick.AddListener(StartGame);
            Btn_Option.onClick.AddListener(Option);
            Btn_Option_Ingame.onClick.AddListener(Option);
            Btn_Credit.onClick.AddListener(Credit);
            Btn_Exit.onClick.AddListener(Exit); //點擊退出按鈕時，呼叫Application.Quit方法來退出遊戲

            Btn_Back_Option.onClick.AddListener(() =>StartCoroutine(Fade_System.Fade(Group_Option, false))); //啟動淡出效果，讓選項介面逐漸隱藏起來
            //Btn_Back_Credit.onClick.AddListener(() => StartCoroutine(Fade_System.Fade(Group_Credit, false))); //啟動淡出效果，讓製作人員介面逐漸隱藏起來

            //Sound_Mng = GameObject.Find("Sound_Manager").GetComponent<Sound_Manager>(); //獲取聲音管理器物件
            //Loading_Mng = GameObject.Find("Canvas_Loading_Manager").GetComponent<Loading_Manager>(); //獲取載入管理器物件

            Slider_Master.onValueChanged.AddListener(Sound_Manager.instance.Set_Master_Volume); //當主音量滑動條的值改變時，呼叫Sound_Manager的SetMasterVolume方法來調整主音量
            Slider_BGM.onValueChanged.AddListener(Sound_Manager.instance.Set_BGM_Volume); //當背景音樂滑動條的值改變時，呼叫Sound_Manager的SetBGMVolume方法來調整背景音樂音量
            Slider_Music.onValueChanged.AddListener(Sound_Manager.instance.Set_SFX_Volume); //當音效滑動條的值改變時，呼叫Sound_Manager的SetSFXVolume方法來調整音效音量
        }

        //宣告方法:函式、函數、功能(Method / Function)

        #region 事件

        
        private void StartGame()
        {
            Debug.Log("🚀 玩家點擊開始遊戲，啟動無縫運鏡轉場！");
            StartCoroutine(GameStartTransitionRoutine());
        }

        private IEnumerator GameStartTransitionRoutine()
        {
            // 1. 🔥 主选单 UI 淡出
            if (Group_MainMenu != null)
            {
                Group_MainMenu.interactable = false;
                Group_MainMenu.blocksRaycasts = false;
                float elapsed = 0f;
                while (elapsed < 0.5f)
                {
                    elapsed += Time.deltaTime;
                    Group_MainMenu.alpha = Mathf.Lerp(1f, 0f, elapsed / 0.5f);
                    yield return null;
                }
                Group_MainMenu.alpha = 0f;
            }

            // 2. 🚪 門打開
            if (doorTransform != null)
            {
                Quaternion startRot = doorTransform.localRotation;
                Quaternion endRot = Quaternion.Euler(doorOpenRotation);
                float elapsed = 0f;
                while (elapsed < doorOpenDuration)
                {
                    elapsed += Time.deltaTime;
                    doorTransform.localRotation = Quaternion.Slerp(startRot, endRot, elapsed / doorOpenDuration);
                    yield return null;
                }
                doorTransform.localRotation = endRot;
            }

            // 3. 🎥 攝影機走到書桌前
            if (mainCamera != null && cameraGamePosition != null)
            {
                Vector3 startPos = mainCamera.position;
                Quaternion startRot = mainCamera.rotation;

                Vector3 endPos = cameraGamePosition.position;
                Quaternion endRot = cameraGamePosition.rotation;

                float elapsed = 0f;
                while (elapsed < cameraMoveDuration)
                {
                    elapsed += Time.deltaTime;
                    float t = elapsed / cameraMoveDuration;

                    // 使用 SmoothStep 讓攝影機移動有緩衝（前慢-中快-後慢），看起來更像真人走路
                    t = Mathf.SmoothStep(0f, 1f, t);

                    mainCamera.position = Vector3.Lerp(startPos, endPos, t);
                    mainCamera.rotation = Quaternion.Slerp(startRot, endRot, t);
                    yield return null;
                }
                mainCamera.position = endPos;
                mainCamera.rotation = endRot;
            }

            // 4. 🏁 抵達戰場，正式開啟遊戲功能
            Debug.Log("✨ 攝影機已定位！怪物生成器已啟動，Day 1 開始！");
            if (enemySpawner != null)
            {
                enemySpawner.SetActive(true); // 喚醒生怪器，UI 的 Day 1 就會正確跳出來！
                StartCoroutine(Fade_System.Fade(Group_GameUI, interval: 0.02f));
            }
        }

        private void Option()
        {
            Debug.Log("選項");
            StartCoroutine(Fade_System.Fade(Group_Option, interval:0.02f)); //啟動淡入效果，讓選項介面逐漸顯示出來
            /*Group_Option.alpha = 1; //顯示選項介面
            Group_Option.interactable = true; //允許互動
            Group_Option.blocksRaycasts = true; //允許射線檢測*/
          
        }

        private void Credit()
        {
            Debug.Log("製作人員");
            StartCoroutine(Fade_System.Fade(Group_Credit, interval: 0.02f));
            /*Group_Credit.alpha = 1; //顯示製作人員介面
            Group_Credit.interactable = true; //允許互動
            Group_Credit.blocksRaycasts = true; //允許射線檢測*/
        }

        private void Exit()
        {
            Debug.Log("退出遊戲");
            Application.Quit(); //退出遊戲
        }
        #endregion

    }
}
