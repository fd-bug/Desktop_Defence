using UnityEngine;
using UnityEngine.Audio;

namespace fdbug
{
    public class Sound_Manager:MonoBehaviour
    {
        public static Sound_Manager instance; //宣告一個靜態變數，用於實現單例模式，確保在遊戲中只有一個Sound_Manager實例存在
        [SerializeField]
        private AudioMixer audioMixer; //宣告一個AudioMixer類型的變數，用於控制遊戲中的音量
        private string Par_Master = "MasterVolume"; //宣告一個字符串變數，表示主音量的參數名稱
        private string Par_BGM = "BGMVolume"; //宣告一個字符串變數，表示背景音樂的參數名稱
        private string Par_SFX = "SFXVolume"; //宣告一個字符串變數，表示音效的參數名稱

        private void Awake() //Awake方法在遊戲開始時被調用，用於初始化Sound_Manager實例
        {
            if (instance == null) //如果Instance為空，表示還沒有Sound_Manager實例存在
            {
                instance = this; //將當前的Sound_Manager實例賦值給Instance變數
                //DontDestroyOnLoad(gameObject); //設置當前的Sound_Manager物件在場景切換時不被銷毀，確保它在整個遊戲過程中持續存在
            }
            else
            {
                Destroy(gameObject); //如果Instance已經有值，表示已經存在一個Sound_Manager實例，則銷毀當前的物件，確保只有一個Sound_Manager存在
            }
        }


        public void Set_Master_Volume(float volume) //定義一個公共方法，用於設置主音量
        {
            audioMixer.SetFloat(Par_Master, volume); //使用AudioMixer的SetFloat方法來設置主音量參數的值
        }

        public void Set_BGM_Volume(float volume) //定義一個公共方法，用於設置背景音樂的音量
        {
            audioMixer.SetFloat(Par_BGM, volume); //使用AudioMixer的SetFloat方法來設置背景音樂參數的值
        }
        public void Set_SFX_Volume(float volume) //定義一個公共方法，用於設置音效的音量
        {
            audioMixer.SetFloat(Par_SFX, volume); //使用AudioMixer的SetFloat方法來設置音效參數的值
        }
    }
}
