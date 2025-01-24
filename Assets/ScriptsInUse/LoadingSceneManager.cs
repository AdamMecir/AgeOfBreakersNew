using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class LoadingSceneManager : MonoBehaviour
{
    [SerializeField] private Slider progressBar;
    [SerializeField] private TextMeshProUGUI loadingText;
    [SerializeField] private float additionalLoadingTime = 2f;


    public static string NextSceneName;

    private void Start()
    {
        StartCoroutine(LoadSceneAsync(NextSceneName));

    }

    private IEnumerator LoadSceneAsync(string sceneName)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
        operation.allowSceneActivation = false;

        float timer = 0f;

        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / 0.9f);

            progressBar.value = progress;

            if (loadingText != null)
            {
                loadingText.text = (progress * 100f).ToString("F0") + "%";
            }

            if (operation.progress >= 0.9f)
            {
                timer += Time.deltaTime;

                if (timer >= additionalLoadingTime)
                {
                    operation.allowSceneActivation = true;
                }
            }

            yield return null;
        }
    }
}
