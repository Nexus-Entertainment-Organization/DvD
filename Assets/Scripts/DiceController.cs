using System;
using System.Linq;
using TMPro;
using UnityEngine;

public class DiceController : MonoBehaviour
{
    public TextMeshProUGUI diceTexbox;
    public TMP_Dropdown diceDropdown;
    public TMP_InputField diceInput;
    int tempSize, tempCount;
    int[] diceSizes = { 4, 6, 8, 10, 12, 20 };
    public TextMeshProUGUI errorBox;
    public Sprite[] sizeShapes;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        diceInput.onValueChanged.AddListener(ValueChanged);
        diceInput.onEndEdit.AddListener(ValueSubmitted);
        diceDropdown.onValueChanged.AddListener(SaveDiceSize);
        ResetDice();
    }

    public class DiceStats
    {
        public int diceCount;
        public int diceSize;
        public Sprite diceShape;

        public DiceStats(int diceCount, int diceSize, Sprite diceShape)
        {
            this.diceCount = diceCount;
            this.diceSize = diceSize;
            this.diceShape = diceShape;
        }
    }

    public DiceStats GetDice()
    {
        int diceSize = int.Parse(diceDropdown.options[tempSize].text);

        Sprite diceShape = null;
        switch (diceSize)
        {
            case 4:
                diceShape = sizeShapes[0];
                    break;
            case 6:
                diceShape = sizeShapes[1];
                break;
            case 8:
                diceShape = sizeShapes[2];
                break;
            case 10:
                diceShape = sizeShapes[3];
                break;
            case 12:
                diceShape = sizeShapes[4];
                break;
            case 20:
                diceShape = sizeShapes[5];
                break;
            default:
                Debug.LogError("Wrong sized Dice???");
                break;
        }

        return new DiceStats(tempCount, diceSize, diceShape);
    }


    public void ValueChanged(string input) //Updates dice on the fly
    {
        ValidateDice(input, false);
    }
    public void ValueSubmitted(string input) //Submit throws error while updating dice
    {
        ValidateDice(input, true);
    }

    /// <summary>
    /// Validates if input is in either # or #d# format. If not, returns warning message if Submit
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    void ValidateDice(string input, bool isSubmit)
    {
        string warning = null;
        input = input.Trim(); //avoid white spaces on either side
        string[] inputs = input.ToLower().Split("d"); //#d# for diceCount/diceSize


        if (inputs.Length != 2 && int.TryParse(input, out int value)) //only numbers
        {
            if (value > 0 && value < 100) //whole input is just diceCount, and valid
            {
                diceTexbox.text = value.ToString();
                tempCount = value;
            }
            else // whole input is a number, but not valid for diceCount
            {
                warning = $"Number of Dice is out of range: {value}" +
                    $"\nEx/ 1-99 in # or #d{diceDropdown.options[tempSize].text} format";
            }
        }
        else if (inputs.Length == 2 && inputs[0].Trim().Length > 0) // #no whitespace for diceCount 
        {
            for (int index = 0; index < inputs.Length; index++) //Check each at a time
            {
                if (index == 0) //If this is the diceCount
                {
                    if (int.TryParse(inputs[index], out int dice)) //If this value is a number
                    {
                        /*
                         * Can't roll zero dice
                         * 
                         * This may change at some point:
                         *      If you roll 100 dice, you take minimum 100 damage, resulting in instant loss
                         */
                        if (dice > 0 && dice < 100)
                        {
                            tempCount = dice;
                        }
                        else //number is valid, just not for the number of dice allowed
                        {
                            warning = $"Number of Dice is out of range: {dice}" +
                                $"\nEx/ 1-99 in # or #d{diceDropdown.options[tempSize].text} format";
                        }
                    }
                    else // diceCount is not a number
                    {
                        warning = $"The number of dice is not valid: {inputs[index]}" +
                            $"\nEx/ 1-99 in # or #d{diceDropdown.options[tempSize].text} format";
                    }
                }
                else if (inputs[1].Trim().Length > 0) //If this is diceSize and is not whitespace
                {
                    if (int.TryParse(inputs[index], out int dice)) //If this value is a number
                    {
                        if (diceSizes.Contains(dice)) //Check the list of dice allowed to use
                        {
                            tempSize = Array.IndexOf(diceSizes, dice);
                        }
                        else //number is valid, but not for the size of the dice
                        {
                            warning = $"Dice size/type is not valid: {dice}" +
                                $"\nEx/ 4,6,8,10,12,20 in {diceTexbox.text}d# format";
                        }
                    }
                    else // diceSize is not a number
                    {
                        warning = $"The size/type of dice is not valid: {inputs[index]}" +
                            $"\nEx/ 4,6,8,10,12,20 in {diceTexbox.text}d# format";
                    }
                }
                else //Index out of range... somehow... I check for Length == 2 in upper if
                {
                    warning = $"No size/type for dice is defined: {inputs[index]}" +
                        $"\nPlease use {diceTexbox.text}d# format";
                }
            }
        }
        else if (string.IsNullOrEmpty(input))
        {
            if (isSubmit)
            {
                diceInput.text = diceInput.text.Trim();
            }
            ResetDice();
        }
        else //Input is not in either # or #d# format
        {
            warning = $"Please use either '#' or '#d#' format: " +
                $"\nEx/: {diceTexbox.text} or {diceTexbox.text}d{diceDropdown.options[tempSize].text}";
        }

        ResetDice(warning, isSubmit);
    }

    void ResetDice(string warning = null, bool isSubmit = false)
    {
        if (tempCount <= 0)
        {
            tempCount = 1;
        }
        diceTexbox.text = tempCount.ToString();
        diceDropdown.value = tempSize;

        if (isSubmit && string.IsNullOrEmpty(warning) == false)
        {
            errorBox.transform.parent.gameObject.SetActive(true);
            errorBox.text = warning;
        }
        else if (string.IsNullOrEmpty(warning) == false)
        {
            errorBox.transform.parent.gameObject.SetActive(false);
            //print(warning);
        }
        else
        {
            errorBox.transform.parent.gameObject.SetActive(false);
        }
    }

    public void SaveDiceSize(int index)
    {
        tempSize = index;
    }
}
