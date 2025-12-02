using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UnitTextController : MonoBehaviour
{
    public GameObject characterText;
    //public GameObject characterName;
    public TMP_Text nameText;
    public TMP_Text descText;

    //Stats
    public GameObject statsText;
    public TMP_Text hpText;
    public TMP_Text apText;
    public TMP_Text spText;
    public TMP_Text movementText;
    public TMP_Text specialNameTextStats;
    public TMP_Text specialCostText;
    public TMP_Text attackText;
    public bool showText = false;
    public bool showStats = false;

    //attributes for menu that appears when selecintg unit
    public GameObject actionText;
    public bool showActionText;

    //attributes for menu that appears when clicking the Special button for a unit.
    public GameObject specialText;
    public TMP_Text specialNameText;
    public TMP_Text specialDescText;
    public bool showSpecialText;

    // Start is called before the first frame update
    void Start()
    {
        characterText.SetActive(false);
        actionText.SetActive(false);
        specialText.SetActive(false);
        statsText.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (showText)
        {
            characterText.SetActive(true);
        }
        else
        {
            characterText.SetActive(false);
        }
        if (showActionText)
        {
            actionText.SetActive(true);
        }
        else
        {
            actionText.SetActive(false);
        }
        if (showSpecialText)
        {
            specialText.SetActive(true);
        }
        else
        {
            specialText.SetActive(false);
        }
        if (showStats)
        { 
            statsText.SetActive(true);
        }
        else
        {
            statsText.SetActive(false);
        }
    }

    public void UpdateText(GameObject unit)
    {
        CharacterEntityController cet = unit.GetComponent<CharacterEntityController>();
        nameText.text = cet.characterName;
        descText.text = cet.description;
        hpText.text = Mathf.FloorToInt(cet.currentHealth) + "/" + cet.maxHealth;
        apText.text = Mathf.FloorToInt(cet.currentAP) + "/" + cet.maxAP;
        spText.text = Mathf.FloorToInt(cet.currentSP) + "/" + cet.maxSP;
        movementText.text = Mathf.FloorToInt(cet.currentMovement) + "/" + cet.maxMovement;
        attackText.text = Mathf.FloorToInt(cet.attack).ToString();
        specialNameTextStats.text = cet.specialName;
        specialCostText.text = "Cost: " + cet.spCost;
    }

    public void UpdateSpecialText(GameObject unit)
    {
        specialNameText.text = unit.GetComponent<CharacterEntityController>().specialName;
        specialDescText.text = unit.GetComponent<CharacterEntityController>().specialDescription;
    }

    public void ShowDescriptionText()
    {
        showText = true;
    }

    public void HideDescriptionText()
    {
        showText = false;
    }

    public void ShowActionText()
    {
        showActionText = true;
    }

    public void HideActionText()
    {
        showActionText = false;
    }

    public void ShowSpecialText() 
    { 
        showSpecialText = true;
    }

    public void HideSpecialText()
    {
        showSpecialText = false;
    }
}
