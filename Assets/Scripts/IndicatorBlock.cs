using UnityEngine;

public class IndicatorBlock : MonoBehaviour
{
    private GameObject fill;
    public bool isActive = false;

    private void Start() => fill = transform.GetChild(0).gameObject;

    private void Update() => fill.SetActive(isActive);

    private void OnValidate()
    {
        Start();
        Update();
    }
}
