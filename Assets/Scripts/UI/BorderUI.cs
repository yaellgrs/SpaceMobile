using UnityEngine;

public class BorderUI : MonoBehaviour
{
    public static BorderUI Instance;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void OnEnable()
    {
        //recup les 4 border : savoir leur état, pas pris, temp, permanent;
    }

    private void OnDisable()
    {
        
    }
}
