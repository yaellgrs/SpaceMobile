
using System.Reflection.Emit;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class XpMarker : MonoBehaviour
{
    public TextMeshProUGUI label;
    public Image img;
    private float timer = 0f;

    private const float DISPLAY_TIME = 2f;  
    private const float MOVE_Y = 0.025f;  
    public float alpha_decrease = 0.975f;
    public float speed = 1f;

    public MarkerType type;

    public void init(Vector3 position, string xp)
    {
        if (type == MarkerType.Damage || type == MarkerType.Critique)
        {
            label.text = "-" + xp;
            position.y += 0.3f;
        }
        else if (type == MarkerType.Iron || type == MarkerType.Uranium || type == MarkerType.Prestige || type == MarkerType.Xp)
        {
            label.text = "+" + xp;
            position.y += 0.3f;
        }
            transform.position = position;
        timer = 0f;
        if (label != null)
        {

            label.color = new Color(label.color.r, label.color.g, label.color.b, 1f);
        }
        if(img != null)
        {
            img.color = new Color(img.color.r, img.color.g, img.color.b, 1f);
        }


    }
    private void Update()
    {
        timer += Time.deltaTime;

        transform.position = transform.position + new Vector3(0f, MOVE_Y * speed * 50 * Time.deltaTime, 0);
        float alphaNormalized = Mathf.Pow(alpha_decrease, Time.deltaTime * 60f);
        if (label != null)
        {
            label.color = new Color(label.color.r, label.color.g, label.color.b, label.color.a * alphaNormalized);
        }
        if (img != null)
        {
            img.color = new Color(img.color.r, img.color.g, img.color.b, img.color.a * alphaNormalized);
        }

        if(timer > DISPLAY_TIME)
        {
            timer = 0f;
            PoolManager.Instance.returnPrefab(this);
        }
    }
}
