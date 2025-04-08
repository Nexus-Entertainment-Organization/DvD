using System.Collections.Generic;
using System.Diagnostics;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class Ability : MonoBehaviour, ISelectHandler
{
    static Ability selectedAbility;
    public string title;
    public int requiredValue;
    public bool canBeGreaterValue, canBeLowerValue;
    public int requiredQuantity = 1;
    public bool canBeGreaterQuantity = true, canBeLowerQuantity;
    public string description;
    /// <summary>
    /// If neither bools, has to be equal to the #
    /// If single bool, that and/or equal to the #
    /// If both bools, anything BUT that #
    /// </summary>

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GetComponentInChildren<TextMeshProUGUI>().text = title;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnSelect(BaseEventData eventData)
    {
        AbilitySelector.tempSelection = this;
    }

    public static void SetAbility(Ability ability)
    {
        selectedAbility = ability;
        FormatAbilityTitle();
    }

    public static void FormatAbilityTitle()
    {
        if(selectedAbility.IsUnityNull() == false)
        {
            selectedAbility.title = selectedAbility.title.ToLower().Trim();
        }
    }

    public static string GetSelectedAbility()
    {
        return selectedAbility.IsUnityNull() ? "None" : selectedAbility.title;
    }

    public static DieCode[] HandleProcs(DieCode[] dice)
    {
        List<DieCode> diceProcs = new();

        foreach (DieCode die in dice)
        {
            if (int.Parse(die.GetComponentInChildren<TextMeshProUGUI>().text) == 5)
            {
                diceProcs.Add(die);
            }
        }

        return diceProcs.ToArray();
    }

    public static void Heal(int heal)
    {
        FindFirstObjectByType<StatsController>().TakeAToll(-20 * heal);
    }
}
