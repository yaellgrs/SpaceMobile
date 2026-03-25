using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UIElements;

//public enum MarkerType { Xp, Diamand, Damage, Iron, Uranium, Critique, Prestige };
public enum MarkerType { Xp, Diamand, Damage, Iron, Uranium, Critique, Prestige };
[UxmlElement]
public partial class markerElement : Label
{
    #region UI Elements
    private VisualElement VE_logo;
    #endregion


    #region constantes
    private static Color[] COLORS ={
        Utility.Hex("#00F9FF"), //Xp
        Utility.Hex("#FF0000"), //Diamand
        Utility.Hex("#FF0E00"), //Damage
        Utility.Hex("#FFA300"), //Iron
        Utility.Hex("#00FF05"),  //Uranium
        Utility.Hex("#Damage"),  //Crtique
        Utility.Hex("#FAFF00"),  //Prestige
    };

    private const float DISPLAY_TIME = 2f;
    private const float MOVE_Y = 2f;
    private const float BASE_FONT_SIZE = 70f;
    #endregion

    #region variables


    public MarkerType type;

    private float timer = 0f;

    public float alpha_decrease = 0.95f;
    public float speed = 1f;

    private float move_x = 1f;

    private bool active = false;
    #endregion
    public markerElement()
    {
        Init();
        //Load(Vector3.zero, "1", MarkerType.Damage);
    }

    private void Init()
    {
        StyleSheet style = Resources.Load<StyleSheet>("styles/markerStyle");
        styleSheets.Add(style);

        VE_logo = new VisualElement();

        VE_logo.name = "logo";

        AddToClassList("markerText");
        VE_logo.AddToClassList("markerLogo");

        pickingMode = PickingMode.Ignore;


        this.schedule.Execute(Update).Every(16); // 16 ms ~ 60 fps

        Add(VE_logo);
    }

    public void Load(Vector2 pos, string text, MarkerType type, float speed, float alpha_decrease, float fontFactor)
    {
        timer = 0f;
        active = true;
        style.translate = new Translate(0, 0, 0);
        style.color = new Color(resolvedStyle.color.r, resolvedStyle.color.g, resolvedStyle.color.b, 1f);
        SetAlpha(1f);

        this.text = text;
        this.type = type;
        this.speed = speed;
        this.alpha_decrease = alpha_decrease;

        //reset le inline style du font 
        style.fontSize = BASE_FONT_SIZE * fontFactor;
        style.height = Length.Percent(3f * fontFactor);
        

        style.left = pos.x;
        style.top = pos.y;

        if (speed > 0.5f) move_x = Random.Range(-0.025f, 0.025f);
        else move_x = 0.025f;

        SetLabelColor();
        SetLogo();
    }

    private void SetLabelColor()
    {
        if (type == MarkerType.Iron)
            style.color = Utility.GetShipColor();
        else
            style.color = COLORS[(int)type];
    }
    
    private void SetLogo()
    {
        if(type == MarkerType.Iron)
            VE_logo.style.backgroundImage = Utility.GetMainRessourceLogo();
        else
        {
            string path = "markers/" + type.ToString();
            VE_logo.style.backgroundImage = Resources.Load<Texture2D>(path);
        }
    }

    public void Update()
    {
        if (!active) return;
        timer += Time.deltaTime;

        style.translate = resolvedStyle.translate + new Vector3(move_x, -MOVE_Y * speed * 50 * Time.deltaTime, 0);

        float alphaNormalized = Mathf.Pow(alpha_decrease, Time.deltaTime * 60f);
        SetAlpha(resolvedStyle.color.a * alphaNormalized);
        if (timer > DISPLAY_TIME)
        {
            timer = 0f;
            active = false;
            RemoveFromHierarchy();

            if (MarkersUI.Instance != null)
                MarkersUI.Instance.markers.Add(this);
            //PoolManager.Instance.returnPrefab(this);
        }
    }

    private void SetAlpha(float alpha)
    {
        style.color = new Color(resolvedStyle.color.r, resolvedStyle.color.g, resolvedStyle.color.b, alpha);
        VE_logo.style.unityBackgroundImageTintColor = new Color(VE_logo.resolvedStyle.unityBackgroundImageTintColor.r, VE_logo.resolvedStyle.unityBackgroundImageTintColor.g, VE_logo.resolvedStyle.unityBackgroundImageTintColor.b, alpha);

    }


}
