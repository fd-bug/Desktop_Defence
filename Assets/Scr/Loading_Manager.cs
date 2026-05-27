using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

namespace fdbug
{
    public class Loading_Manager : MonoBehaviour
    {
        private CanvasGroup group;
        private TMP_Text Text_percent;
        private Image Img_percent;

        public static Loading_Manager instance;

        private void Awake()
        {
            group = GetComponent<CanvasGroup>();
            Text_percent = GameObject.Find("Loading_Text_Percent").GetComponent<TMP_Text>();
            Img_percent = GameObject.Find("Loading_Bar_Percent").GetComponent<Image>();

            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }

            else Destroy(gameObject);

        }
        public void StartLoading(string sceneName)
        {
            StartCoroutine(Loading(sceneName));
        }


        private IEnumerator Loading(string sceneName)
        { 
            //開始載入指定場敬，儲存載入資料
            AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
            //載入完成前不切換場景
            operation.allowSceneActivation = false;
            while (!operation.isDone)
            { 
                Text_percent.text = sceneName;
                Img_percent.fillAmount = operation.progress;
                yield return null;
                if(operation.progress >= 0.9f) operation.allowSceneActivation = true;

            }
            yield return new WaitForSeconds(1);
            yield return StartCoroutine(Fade_System.Fade(group, false,0.02f));
        }

    }

}