using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class RepelerLink : MonoBehaviour
{
    public Transform pointA;
    public Transform pointB;

    public float textureScrollSpeed = 2f;
    public float noiseAmplitude = 0.1f;
    public float noiseFrequency = 4f;

    private LineRenderer lr;
    private Material mat;

    private void Awake()
    {
        lr = GetComponent<LineRenderer>();
        mat = lr.material;
    }

    void Update()
    {
        if (pointA == null || pointB == null)
        {
            lr.enabled = false;
            return;
        }

        lr.enabled = true;

        Vector3 start = pointA.position;
        Vector3 end = pointB.position;

        lr.SetPosition(0, start);
        lr.SetPosition(1, end);

        AnimateTexture();
        AnimateWidth();
    }

    private void AnimateTexture()
    {
        float offset = Time.time * textureScrollSpeed;
        mat.mainTextureOffset = new Vector2(-offset, 0);
    }

    private void AnimateWidth()
    {
        float pulse = 1.5f * Mathf.Sin(Time.time * 5f) * 0.02f + 0.5f;
        lr.startWidth = pulse;
        lr.endWidth = pulse;
    }
}
