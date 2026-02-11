using System.Collections.Generic;
using UnityEngine;

public class IndicatorManager : MonoBehaviour
{
    private IndicatorBlock[] indicators;
    [SerializeField] private GameObject indicatorPrefab = null;
    [SerializeField] private float indicatorGap = 30f;

    void Awake()
    {
        indicatorPrefab = Resources.Load<GameObject>("Indicator");
    }

    public void GenerateIndicators(int indicatorCount)
    {
        List<IndicatorBlock> indicatorList = new() { transform.GetChild(0).GetComponent<IndicatorBlock>() };
        Vector3 startingPosition = transform.GetChild(0).GetComponent<RectTransform>().anchoredPosition;
        for (int i = 0; i < indicatorCount; i++)
        {
            var newIndicator = Instantiate(indicatorPrefab, transform);
            newIndicator.GetComponent<RectTransform>().anchoredPosition = new Vector2(startingPosition.x + (i + 1) * indicatorGap, 0);
            indicatorList.Add(newIndicator.GetComponent<IndicatorBlock>());
        }
        indicators = indicatorList.ToArray();
    }

    public void SetLevel(int level)
    {
        for (int i = 0; i < level + 1; i++)
        {
            indicators[i].isActive = true;
        }
    }
}
