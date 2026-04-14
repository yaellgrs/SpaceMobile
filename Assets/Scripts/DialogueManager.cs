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

    public bool TryDialogue(string dialogue)
    {
        if (!Stats.Instance.dialogues.ContainsKey(dialogue))
        {
            Debug.Log("Dialogue " + dialogue + " doesn't exist.");
            return false;
        }
        if(!Stats.Instance.dialogues[dialogue])
        {
            ExecuteBlock(dialogue);
            Stats.Instance.dialogues[dialogue] = true;
            Stats.Instance.save();
            return true;
        }
        return false;
    }

    #region GENERAL

    public void StartDialogue()
    {

        MainUi.Instance.ShowMenu(false);
        MainUi.Instance.EnableShoot(false);
        BottomUI.Instance.Show(false);
        gameManager.instance.spawnMeteor = false;

    }


    public void EndDialogue()
    {
        MainUi.Instance.ShowMenu(true);
        MainUi.Instance.EnableShoot(true);
        BottomUI.Instance.Show(true);
        gameManager.instance.spawnMeteor = true;
    }

    public void HideMenu()
    {
        MainUi.Instance.ShowMenu(false);
        BottomUI.Instance.Show(false);
    }
    public void ShowMenu()
    {
        MainUi.Instance.ShowMenu(true);
        BottomUI.Instance.Show(true);
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
        DialogueUI.Instance.Hide(true);
    }

    public void UnHide()
    {
        DialogueUI.Instance.Hide(false);
    }

    public void IronDialogue()
    {
        MainUi.Instance.ironUI.IronClicked();
        MainUi.Instance.questUI.LoadWithDelay();
    }

    public void SetFirstBoss()
    {
        Stats.Instance.firstBoss = true;
    }

    public void OpenQuest()
    {
        MainUi.Instance.questUI.LoadWithDelay();
    }

    #endregion
}
