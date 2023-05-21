using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;
using TMPro;

public class LoadingScreen : MonoBehaviour
{
    public static LoadingScreen instance;

    //[Line("Progress")]
    public Image ProgressBar;
    public TMP_Text ProgressText;
    //[Line("backgroud")]
    public bool LoopBackground = true;
    public ReorderableList<GameObject> BackgroundList;
    public float backgroundLoopTime;

    AsyncOperation async;

    public void loadingScreen(string sceneName)
    {
        this.transform.Find("Panel").gameObject.SetActive(true);
        if (LoopBackground) StartCoroutine(transitionImage());
        StartCoroutine(Loading(sceneName));
    }
    public void loadingScreen(int sceneNo)
    {
        this.transform.Find("Panel").gameObject.SetActive(true);
        if (LoopBackground) StartCoroutine(transitionImage());
        StartCoroutine(Loading(sceneNo));
    }

    private void Awake()
    {
        instance = this;
        this.transform.Find("Panel").gameObject.SetActive(false);
    }

    private void Start()
    {
        
    }

    IEnumerator transitionImage()
    {
        for (int i = 0; i < BackgroundList.List.Count; i++)
        {
            yield return new WaitForSeconds(backgroundLoopTime);

            for (int j = 0; j < BackgroundList.List.Count; j++) BackgroundList.List[j].SetActive(false);

            BackgroundList.List[i].SetActive(true);
        }
    }

    IEnumerator Loading(int sceneNo)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneNo);
        while (!operation.isDone)
        {
            float progressValue = Mathf.Clamp01(operation.progress / 0.9f);
            ProgressBar.fillAmount = progressValue;
            ProgressText.text = (progressValue * 100) + " %";
            yield return null;
        }
    }
    IEnumerator Loading(string sceneName)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
        while (!operation.isDone)
        {
            float progressValue = Mathf.Clamp01(operation.progress / 0.9f);
            ProgressBar.fillAmount = progressValue;
            ProgressText.text = (progressValue * 100) + " %";
            yield return null;
        }
    }

}

#if UNITY_EDITOR

[CustomEditor(typeof(LoadingScreen))]
public class LoadingScreenEditor : Editor
{
    public override void OnInspectorGUI()
    {
        EditorUtility.SetDirty(target);
        LoadingScreen screen = (LoadingScreen)target;

        EditorGUILayout.BeginVertical("box");
        EditorGUILayout.BeginHorizontal("Toolbar", GUILayout.Height(30));
        EditorGUILayout.LabelField($"Progress", EditorStyles.boldLabel);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.PropertyField(serializedObject.FindProperty("ProgressBar"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("ProgressText"));

        EditorGUILayout.EndVertical();

        EditorGUILayout.BeginVertical("box");
        EditorGUILayout.BeginHorizontal("Toolbar", GUILayout.Height(30));
        EditorGUILayout.LabelField($"Background", EditorStyles.boldLabel);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.PropertyField(serializedObject.FindProperty("LoopBackground"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("backgroundLoopTime"));
        EditorGUILayout.Space(5);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("BackgroundList"));

        EditorGUILayout.EndVertical();

        serializedObject.ApplyModifiedProperties();
    }
}

#endif
