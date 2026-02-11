using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RocketShop : MonoBehaviour, IInteractable
{
    [SerializeField] private GameObject mainUI;
    [SerializeField] private GameObject shopUI;
    [SerializeField] private ShopUpgrades shopUpgradeValues;
    private readonly Dictionary<UpgradeType, float[]> upgradeValues = new();
    private readonly Dictionary<UpgradeType, int[]> upgradePrices = new();
    private readonly Dictionary<UpgradeType, int> upgradeLevels = new();
    private readonly Dictionary<UpgradeType, Button> upgradeButtons = new();

    [SerializeField] private TextMeshProUGUI[] upgradeTexts = new TextMeshProUGUI[4];
    [SerializeField] private TextMeshProUGUI[] costTexts = new TextMeshProUGUI[4];
    [SerializeField] private TextMeshProUGUI hullRepairText;

    [SerializeField] private IndicatorManager[] indicatorManagers = new IndicatorManager[4];

    [SerializeField] private Button damageButton;
    [SerializeField] private Button cooldownButton;
    [SerializeField] private Button overheatButton;
    [SerializeField] private Button maxHealthButton;
    [SerializeField] private Button hullRepairButton;

    public enum UpgradeType
    {
        Damage      = 0,
        Cooldown    = 1,
        Overheat    = 2,
        MaxHealth   = 3
    }

    private PlayerController player;

    private void Start()
    {
        player = PlayerController.Instance;

        upgradeValues.Add(UpgradeType.Damage, shopUpgradeValues.damageValues);
        upgradeValues.Add(UpgradeType.Cooldown, shopUpgradeValues.cooldownValues);
        upgradeValues.Add(UpgradeType.Overheat, shopUpgradeValues.overheatValues);
        upgradeValues.Add(UpgradeType.MaxHealth, shopUpgradeValues.maxHealthValues);

        upgradeLevels.Add(UpgradeType.Damage, 0);
        upgradeLevels.Add(UpgradeType.Cooldown, 0);
        upgradeLevels.Add(UpgradeType.Overheat, 0);
        upgradeLevels.Add(UpgradeType.MaxHealth, 0);

        upgradeButtons.Add(UpgradeType.Damage, damageButton);
        upgradeButtons.Add(UpgradeType.Cooldown, cooldownButton);
        upgradeButtons.Add(UpgradeType.Overheat, overheatButton);
        upgradeButtons.Add(UpgradeType.MaxHealth, maxHealthButton);

        upgradePrices.Add(UpgradeType.Damage, shopUpgradeValues.damagePrice);
        upgradePrices.Add(UpgradeType.Cooldown, shopUpgradeValues.cooldownPrice);
        upgradePrices.Add(UpgradeType.Overheat, shopUpgradeValues.overheatPrice);
        upgradePrices.Add(UpgradeType.MaxHealth, shopUpgradeValues.maxHealthPrice);

        CreateIndicators();
    }

    private void Update()
    {
        UpdateButtons();
        UpdateText();
    }

    public void Upgrade(int enumValue)
    {
        var type = (UpgradeType)enumValue;
        player.ApplyUpgrade(type, upgradeValues[type][upgradeLevels[type]]);
        indicatorManagers[enumValue].SetLevel(upgradeLevels[type] + 1);
        player.AddMoney(-upgradePrices[type][upgradeLevels[type]]);
        Debug.Log(upgradeLevels[type]);
        upgradeLevels[type]++;
        Debug.Log(upgradeLevels[type]);
    }

    private void CreateIndicators()
    {
        for(int i = 0; i < indicatorManagers.Length; i++)
        {
            if (indicatorManagers[i] == null) continue;
            indicatorManagers[i].GenerateIndicators(upgradeValues[(UpgradeType)i].Length);
        }
    }

    public void RepairHull() 
    {
        player.AddMoney(-GetHullRepairPrice());
        player.Heal(player.MaxHealth);
    }
    

    private void UpdateText()
    {
        for (int i = 0; i < upgradeTexts.Length; i++)
        {
            if (upgradeTexts[i] == null) continue;
            upgradeTexts[i].text = "Lv. " + upgradeLevels[(UpgradeType)i];
        }
        for (int i = 0; i < costTexts.Length; i++)
        {
            if (costTexts[i] == null) continue;
            costTexts[i].text = "Cost: " + (upgradeLevels[(UpgradeType)i] == upgradePrices[(UpgradeType)i].Length ? "Max" : upgradePrices[(UpgradeType)i][upgradeLevels[(UpgradeType)i]]);
        }
        hullRepairText.text = "Cost: " + GetHullRepairPrice();
    }

    private void UpdateButtons()
    {
        for(int i = 0; i < upgradeButtons.Count; i++)
        {
            if (upgradeButtons[(UpgradeType)i] == null) continue;
            var type = (UpgradeType)i;
            upgradeButtons[type].interactable = upgradeLevels[type] < upgradeValues[type].Length && (upgradeLevels[type] < upgradePrices[type].Length) && player.CurrentMoney >= upgradePrices[type][upgradeLevels[type]];
        }
        hullRepairButton.interactable = player.Health < player.MaxHealth && player.CurrentMoney >= GetHullRepairPrice();
    }

    private int GetHullRepairPrice() 
    {
        float missingHealthRatio = (player.MaxHealth - player.Health) / (float)player.MaxHealth;
        return (int)Mathf.Lerp(0, shopUpgradeValues.maxHullRepairPrice, missingHealthRatio * missingHealthRatio);
    }

    public void Interact()
    {
        Debug.Log("Interacting with RocketShop");
        PauseManager.SetGameState(true);
        PauseManager.allowStateChange = false;
        mainUI.SetActive(false);
        shopUI.SetActive(true);
    }

    public void StopInteract()
    {
        Debug.Log("Stop Interacting with RocketShop");
        PauseManager.SetGameState(false);
        PauseManager.allowStateChange = true;
        mainUI.SetActive(true);
        shopUI.SetActive(false);
    }
}
