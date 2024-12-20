using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadManager : MonoBehaviour
{
    private static LoadManager _instance;
    public static LoadManager Instance { get { return _instance; } }

    [SerializeField] private GameObject LoadingScreen;
    [SerializeField] private Image LoadingImage;
    
    private void Awake() 
    { 
        if (_instance != null && _instance != this)
            Destroy(this);
        else
            _instance = this;

        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        Debug.Log("[LoadManager]: Loading Main Menu");
        LoadScene((int)SceneIndexes.MAINMENU);
    }

    List<AsyncOperation> scenesLoading = new List<AsyncOperation>();
    public void LoadGame()
    {
        LoadingScreen.SetActive(true);
        
        //scenesLoading.Add(SceneManager.UnloadSceneAsync((int)SceneIndexes.MAINMENU));
        scenesLoading.Add(SceneManager.LoadSceneAsync((int)SceneIndexes.BASE, LoadSceneMode.Single));
        scenesLoading.Add(SceneManager.LoadSceneAsync((int)SceneIndexes.GROTTO, LoadSceneMode.Additive));
        scenesLoading.Add(SceneManager.LoadSceneAsync((int)SceneIndexes.CRYSTALCAVERNS, LoadSceneMode.Additive));
        scenesLoading.Add(SceneManager.LoadSceneAsync((int)SceneIndexes.FUN, LoadSceneMode.Additive));
        scenesLoading.Add(SceneManager.LoadSceneAsync((int)SceneIndexes.SMALLCAVERNS, LoadSceneMode.Additive));
        scenesLoading.Add(SceneManager.LoadSceneAsync((int)SceneIndexes.MOUNTAINS, LoadSceneMode.Additive));

        StartCoroutine(GetGameLoadProgress());
    }

    float totalLoadProgress;
    IEnumerator GetGameLoadProgress()
    {
        for (int i = 0; i < scenesLoading.Count; i++)
        {
            while (!scenesLoading[i].isDone)
            {
                totalLoadProgress = 0f;

                foreach (AsyncOperation op in scenesLoading)
                {
                    totalLoadProgress += op.progress;
                }

                totalLoadProgress = (totalLoadProgress / scenesLoading.Count);
                float progressValue = Mathf.Clamp01(totalLoadProgress);
                LoadingImage.fillAmount = progressValue;
                Debug.Log("progress = " + totalLoadProgress + "\n  progressVal = " + progressValue);

                yield return null;
            }
        }

        scenesLoading.Clear();
        LoadingScreen.SetActive(false);
    }

    private void LoadScene(int sceneID)
    {
        StartCoroutine(LoadSceneAsync(sceneID));
    }

    IEnumerator LoadSceneAsync(int sceneID)
    {
        AsyncOperation op = SceneManager.LoadSceneAsync(sceneID);

        LoadingScreen.SetActive(true);

        while (!op.isDone)
        {
            float progressValue = Mathf.Clamp01(op.progress / 0.9f);

            LoadingImage.fillAmount = progressValue;

            yield return null;
        }

        LoadingScreen.SetActive(false);
    }

    public enum SceneIndexes
    {
        LOADMANAGER = 0,
        MAINMENU = 1,
        BASE = 2,
        GROTTO = 3,
        CRYSTALCAVERNS = 4,
        FUN = 5,
        SMALLCAVERNS = 6,
        MOUNTAINS = 7
    }
}
