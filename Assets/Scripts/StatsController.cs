using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static DiceController;

public class StatsController : MonoBehaviour
{
    RectTransform backImage, energyImage, strainImage;
    float toll, exhaustion, stepSize, width, height;
    static bool inBattle;
    SceneStuff stuff;
    [SerializeField] GameObject dieGuy;

    private void Awake()
    {
        stuff = FindFirstObjectByType<SceneStuff>(); //Need this in awake so all code can find before inactive
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        backImage = transform.GetChild(0).GetComponent<RectTransform>();
        energyImage = transform.GetChild(1).GetComponent<RectTransform>();
        strainImage = transform.GetChild(2).GetComponent<RectTransform>();

        width = backImage.rect.width;
        height = backImage.rect.height;
        stepSize = width / 100;

        energyImage.offsetMax = backImage.offsetMax - new Vector2(toll, 0);
        strainImage.offsetMax = backImage.offsetMax - new Vector2(width, 0);

        inBattle = true;
        StartCoroutine(ExaustionBuildup());

        stuff.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

    }

    IEnumerator QueueReload(string message, bool lose)
    {
        stuff.gameObject.SetActive(true);

        TextMeshProUGUI header = stuff.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI body = stuff.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        header.text = lose ? "You Lost!" : "You Win";
        body.text = lose ? message : message.Replace("You", "They");
        header.enabled = false;
        body.enabled = false;

        Button[] buttons = stuff.GetComponentsInChildren<Button>();
        foreach (Button button in buttons)
        {
            button.gameObject.SetActive(false);
        }

        yield return new WaitForSeconds(1);

        header.enabled = true;
        body.enabled = true;

        yield return new WaitForSeconds(2);

        foreach (Button button in buttons)
        {
            button.gameObject.SetActive(true);
        }
    }

    IEnumerator ExaustionBuildup()
    {
        yield return new WaitForSeconds(10);
        if (inBattle)
        {
            AddExhaustion(1);
            StartCoroutine(ExaustionBuildup());
        }
    }

    public void TakeAToll(int amount)
    {
        if (inBattle)
        {
            toll += amount * stepSize;
            energyImage.offsetMax = backImage.offsetMax - new Vector2(toll, 0);

            if (strainImage.offsetMax.x >= energyImage.offsetMax.x)
            {
                inBattle = false;
                StartCoroutine(QueueReload("You ran out of energy", gameObject.CompareTag("Player")));
            }
        }
    }
    void AddExhaustion(int amount)
    {
        if (inBattle)
        {
            exhaustion += stepSize * amount;
            strainImage.offsetMax = backImage.offsetMax - new Vector2(width - exhaustion, 0);

            if (strainImage.offsetMax.x >= energyImage.offsetMax.x)
            {
                inBattle = false;
                StartCoroutine(QueueReload("You became exhausted", gameObject.CompareTag("Player")));
            }
        }
    }

    public void RollTotal(DiceController dice)
    {
        if (dice.errorBox.isActiveAndEnabled == false)
        {
            Button button = dice.GetComponentInChildren<Button>();
            button.interactable = false;

            DiceStats stats = dice.GetDice();
            int[] rolls = new int[stats.diceCount];
            for (int index = 0; index < rolls.Length; index++)
            {
                rolls[index] = Random.Range(0, stats.diceSize) + 1;
            }

            if (inBattle && !dice.errorBox.isActiveAndEnabled)
            {
                StartCoroutine(SpawnDice(rolls, stats.diceShape, button));
            }
        }
    }

    IEnumerator SpawnDice(int[] rolls, Sprite size, Button button)
    {
        print(button);
        foreach (int roll in rolls)
        {
            GameObject die = Instantiate(dieGuy);

            die.tag = tag;
            die.GetComponent<Image>().sprite = size;
            die.GetComponent<Image>().color = energyImage.GetComponent<Image>().color;
            die.GetComponentInChildren<TextMeshProUGUI>().text = roll.ToString();
            die.GetComponent<DieCode>().toll = roll;

            die.transform.SetParent(gameObject.transform);
            float dieWidth = die.GetComponent<RectTransform>().rect.width;
            float range = (width - dieWidth) / 2;
            float xOffset = Random.Range(-range, range);
            float yOffset = die.CompareTag("Player") ? -height : 0;
            die.transform.localPosition = new Vector2(xOffset, yOffset);

            Rigidbody2D body = die.GetComponent<Rigidbody2D>();
            if (die.CompareTag("Player"))
            {
                body.gravityScale = 50;
            }
            else
            {
                body.gravityScale = -50;
            }

            yield return new WaitForSeconds(0.05f);
        }
        button.interactable = true;
    }
}