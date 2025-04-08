using TMPro;
using Unity.VisualScripting;
using UnityEditor.Playables;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AbilitySelector : MonoBehaviour
{
    public string selectedAbility;
    public static Ability tempSelection;
    public TextMeshProUGUI rollTextbox;
    static Canvas selectionUI;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        selectionUI = transform.parent.GetComponent<Canvas>();
        selectionUI.enabled = true;
    }

    // Update is called once per frame
    void Update()
    {
        selectedAbility = Ability.GetSelectedAbility();
        rollTextbox.text = selectedAbility;

        transform.GetChild(0).GetComponent<Button>().interactable = !tempSelection.IsUnityNull();
    }

    public void SelectAbility()
    {
        if(tempSelection.IsUnityNull() == false)
        {
            Ability.SetAbility(tempSelection);
            selectionUI.enabled = false;
        }
    }

    public static void ChooseAbility()
    {
        tempSelection = null;
        selectionUI.enabled = true;
    }
}
