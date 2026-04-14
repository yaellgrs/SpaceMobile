using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class Loader : MonoBehaviour
{
    public LoaderUI loaderUI;

    float progressShow = 0f;


    public void Start()
    {
        StartCoroutine(AsyncLoadGame());
    }


    IEnumerator AsyncLoadGame()
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync("MainScene");
        operation.allowSceneActivation = false;

        while (!operation.isDone)
        {
            float target = Mathf.Clamp01(operation.progress / 0.9f);
            progressShow = Mathf.MoveTowards(progressShow, target, Time.deltaTime*0.5f);

            loaderUI.UpdateLoadBarre(progressShow);

            if (operation.progress >= 0.9f && progressShow >= 0.99f)
            {
                yield return new WaitForSeconds(0.1f);
                operation.allowSceneActivation = true;
            }

            yield return null;
        }
    }
}
