using UnityEngine;

public class BackgroundScroller : MonoBehaviour
{
    [SerializeField] private float scrollSpeed = 0.5f;
    [SerializeField] private bool verticalScroll = false;

    private Material material;
    private Vector2 offset;

    private void Start()
    {
        material = GetComponent<Renderer>().material;
        offset = material.mainTextureOffset;
    }

    private void Update()
    {
        if (verticalScroll)
        {
            offset.y += Time.deltaTime * scrollSpeed;
        }
        else
        {
            offset.x += Time.deltaTime * scrollSpeed;
        }

        material.mainTextureOffset = offset;
    }
}