using Fungus;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;

    public GameObject inputSystem;
    public Flowchart flowChart;

    private void Awake()
    {
        if(Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        if (inputSystem != null)
            inputSystem.SetActive(false);
    }

    public void ExecuteBlock(string blockName)
    {
        flowChart.ExecuteBlock(blockName);
    }



    public void EndDialogue()
    {
        MainUi.Instance.ShowMenu(true);
        BottomUI.Instance.Show(true);
        gameManager.instance.spawnMeteor = true;
        if (inputSystem != null)
            inputSystem.SetActive(false);
    }

    public void StartDialogue()
    {
        MainUi.Instance.ShowMenu(false);
        BottomUI.Instance.Show(false);
        gameManager.instance.spawnMeteor = false;
        if (inputSystem != null)
            inputSystem.SetActive(true);
    }

    public void StartWarning()
    {
        gameManager.instance.activeWarning(true);
        StartCoroutine(SoundManager.Instance.PlaySoundWithTime(SoundEffectType.BossWarning, 3f));
    }

    public void StopWarning()
    {
        gameManager.instance.activeWarning(false);
    }
}
