using UnityEngine;
using UnityEngine.UIElements;

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

    private void Start()
    {
        Hide(false);
    }

    public void Hide(bool hide)
    {
        if(VE_Hide == null) VE_Hide = document.rootVisualElement.Q<VisualElement>("Hide");
        VE_Hide.style.display = hide ? DisplayStyle.Flex : DisplayStyle.None;
    }
}
