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
    public bool showText = false;

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
    }

    public void UpdateText(GameObject unit)
    {
        nameText.text = unit.GetComponent<CharacterEntityController>().characterName;
        descText.text = unit.GetComponent<CharacterEntityController>().description;
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
