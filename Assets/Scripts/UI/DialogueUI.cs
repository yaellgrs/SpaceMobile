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

    private void Start()
    {
        Hide(false);
    }

    public void Hide(bool hide)
    {
        if(VE_Hide == null) VE_Hide = document.rootVisualElement.Q<VisualElement>("Hide");
        VE_Hide.style.display = hide ? DisplayStyle.Flex : DisplayStyle.None;
    }

    public IEnumerator LaunchTransition()
    {
        var root = document.rootVisualElement;
        VisualElement container = root.Q<VisualElement>("transitionVideo");

        var image = new UnityEngine.UIElements.Image();
        image.image = RT_transition;
        image.style.flexGrow = 1;

        container.Add(image);
        VP_transition.Play();

        yield return new WaitForSeconds(2f);
        container.Clear();
        VP_transition.Stop();

    }
}
