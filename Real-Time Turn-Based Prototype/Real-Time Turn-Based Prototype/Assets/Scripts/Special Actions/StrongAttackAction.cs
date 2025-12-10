using System;
using Unity.VisualScripting;
using UnityEngine;

public class StrongAttackAction : MonoBehaviour, ISpecialAction
{
    //Attributes
    public string specialName = "Immense Strength";
    public string specialDescription = "Have your next attack do 3 times the normal damage.";
    public float damageMultiplier = 3f;
    public float cost = 15f;
    public float range = 1f;
    public bool isBuffActive = false;
    public CharacterEntityController characterController;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        range = characterController.attackRange;
    }

    // Update is called once per frame
    void Update()
    {
        if (isBuffActive && characterController != null)
        {
            //Reset attack after action is performed
            if (!characterController.attackState)
            {
                characterController.attack /= damageMultiplier;
                characterController.currentSP -= characterController.spCost;
                isBuffActive = false;
                characterController.specialState = false;
            }
        }   
    }

    //Set spCost field on associated units
    public void SetSPCost(CharacterEntityController character)
    {
        character.spCost = cost;
    }

    //Set special text fields for associated units
    public void SetSpecialText(CharacterEntityController character)
    {
        character.specialName = specialName;
        character.specialDescription = specialDescription;
    }

    //Perform the special action
    //Increase damage of next attack
    public void Execute(CharacterEntityController controller, CharacterEntityController targetObject)
    {
        Debug.Log("Strength Special Executed");
        if (targetObject != null && targetObject != controller)
        {
            characterController = controller;
            if (Vector3.Distance(targetObject.transform.position, controller.transform.position) <= range)
            {
                //Multiply attack for untit by 3 for 1 attack.
                characterController.attack *= damageMultiplier;
                characterController.InvokeAttack(targetObject.gameObject);
                isBuffActive = true;
                characterController.specialState = true;
            }
            else
            {
                Debug.Log("Target out of range");
            }
        }
        else
        {
            Debug.Log("Invalid target!");
        }
    }
}
