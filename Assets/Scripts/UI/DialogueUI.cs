using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Video;

public class DialogueUI : MonoBehaviour
{
    #region INSTANCE
    public static DialogueUI Instance;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    #endregion

    [SerializeField] private UIDocument document;

    private VisualElement VE_Hide;

    public VideoPlayer VP_transition;
    public RenderTexture RT_transition;
    public Button Btn_skip;

    private void Start()
    {
        Hide(false);
    }

    private void OnEnable()
    {
        Btn_skip = document.rootVisualElement.Q<Button>("skip");

        Btn_skip.clicked += SkipDialogue;
    }

    public void SetSkipButton(bool active)
    {
        Btn_skip = document.rootVisualElement.Q<Button>("skip");

        if (active)
        {
            Btn_skip.style.visibility = Visibility.Visible;
            Btn_skip.clicked -= SkipDialogue;
            Btn_skip.clicked += SkipDialogue;
        }
        else
        {
            Btn_skip.style.visibility = Visibility.Hidden;
            Btn_skip.clicked -= SkipDialogue;
        }

        Debug.Log("skip buttpn : " + active);


    }

    private void SkipDialogue()
    {
        DialogueManager.Instance.EndDialogue();
    }

    public void Hide(bool hide)
    {
        if(VE_Hide == null) VE_Hide = document.rootVisualElement.Q<VisualElement>("Hide");
        VE_Hide.style.display = hide ? DisplayStyle.Flex : DisplayStyle.None;
    }

    public IEnumerator LaunchTransition()
    {
        yield return new WaitForSeconds(0.25f);


        var root = document.rootVisualElement;
        VisualElement container = root.Q<VisualElement>("transitionVideo");

        var image = new UnityEngine.UIElements.Image();
        image.image = RT_transition;
        image.style.flexGrow = 1;

        container.Add(image);
        VP_transition.Play();

        image.style.opacity = 0;
        float time = 0;

        while(time < 0.5f)
        {
            image.style.opacity = Mathf.Lerp(0, 1, time/0.5f);

            time += Time.deltaTime;
            yield return null;
        }

        image.style.opacity = 1f;

        yield return new WaitForSeconds(2f);

        time = 0;

        while (time <0.5f)
        {
            
            image.style.opacity = Mathf.Lerp(1, 0, time/0.5f);

            time += Time.deltaTime;
            yield return null;
        }

        container.Clear();
        VP_transition.Stop();

        if(DialogueManager.Instance.TryDialogue("FirstPrestige"))
            gameManager.instance.SetPause(true);    

    }
}
