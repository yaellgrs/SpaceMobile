using Fungus;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;

    public EventSystem inputSystem;
    public Flowchart flowChart;

    private void Awake()
    {
        if(Instance == null){
            Instance = this;

        }
        else
            Destroy(gameObject);

        // Supprime les EventSystems en double
        //var allEventSystems = FindObjectsByType<EventSystem>(FindObjectsSortMode.None);
        //if (allEventSystems.Length > 1)
        //{
        //    for (int i = 1; i < allEventSystems.Length; i++)
        //    {
        //        Debug.Log($"EventSystem supprimé : {allEventSystems[i].gameObject.name}");
        //        Destroy(allEventSystems[i].gameObject);
        //    }
        //}
    }

    private void Start()
    {
        if (Stats.Instance.firstConnection) DialogueManager.Instance.ExecuteBlock("FirstConnection");
    }
    public void ExecuteBlock(string blockName)
    {

        //var dialogInput = flowChart.GetComponent<DialogInput>();
        //if (dialogInput != null) dialogInput.enabled = true;

        //StartDialogue();

        flowChart.ExecuteBlock(blockName);
    }

    #region GENERAL

    public void StartDialogue()
    {

        MainUi.Instance.ShowMenu(false);
        BottomUI.Instance.Show(false);
        gameManager.instance.spawnMeteor = false;
        Debug.Log("start dialogue");

    }


    public void EndDialogue()
    {
        MainUi.Instance.ShowMenu(true);
        BottomUI.Instance.Show(true);
        gameManager.instance.spawnMeteor = true;
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

    public void Hide()
    {
        BottomUI.Instance.Hide(true);
    }

    public void UnHide()
    {
        BottomUI.Instance.Hide(false);
    }

    public void IronDialogue()
    {
        MainUi.Instance.ironUI.IronClicked();
        MainUi.Instance.questUI.LoadWithDelay();
    }

    #endregion
}
