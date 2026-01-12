
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
    public float alpha_decrease = 0.96f;
    public float speed = 1f;

    public PoolManager.markerType type;

    public void init(Vector3 position, string xp)
    {
        if (type == PoolManager.markerType.Damage || type == PoolManager.markerType.Critique)
        {
            label.text = "-" + xp;
            position.y += 0.3f;
        }
        else if (type == PoolManager.markerType.Iron || type == PoolManager.markerType.Uranium || type == PoolManager.markerType.Prestige || type == PoolManager.markerType.Xp)
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

        transform.position = transform.position + new Vector3(0f, MOVE_Y * speed, 0);
        if(label != null)
        {
            label.color = new Color(label.color.r, label.color.g, label.color.b, label.color.a * alpha_decrease);
        }
        if (img != null)
        {
            img.color = new Color(img.color.r, img.color.g, img.color.b, img.color.a * alpha_decrease);
        }

        if(timer > DISPLAY_TIME)
        {
            timer = 0f;
            PoolManager.Instance.returnPrefab(this);
        }
    }
}
