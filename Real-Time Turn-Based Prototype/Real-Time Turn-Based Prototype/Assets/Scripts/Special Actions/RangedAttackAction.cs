using UnityEngine;

public class RangedAttackAction : MonoBehaviour, ISpecialAction
{
    //Attributes
    public string specialName = "Snipe";
    public string specialDescription = "For one attack, do increased damage and have an extended range";
    public float damageMultiplier = 1.8f;
    public float cost = 15f;
    //UI only, hide the ring to indicate infinite range
    public float range = 0f;
    public float attackTimer = 3f;
    private bool isBuffActive = false;
    private CharacterEntityController characterController;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (isBuffActive && characterController != null)
        {
            //Reset attack
            if (!characterController.attackState)
            {
                characterController.attack /= damageMultiplier;
                characterController.attackRange /= 100f;
                characterController.currentSP -= characterController.spCost;
                isBuffActive = false;
                characterController.specialState = false;
            }
        }
    }

    public void SetSPCost(CharacterEntityController character)
    {
        character.spCost = cost;
    }

    public void SetSpecialText(CharacterEntityController character)
    {
        character.specialName = specialName;
        character.specialDescription = specialDescription;
    }

    public void Execute(CharacterEntityController controller, CharacterEntityController targetObject)
    {
        Debug.Log("Ranged Special Executed");
        if (targetObject != null && targetObject != controller)
        {
            characterController = controller;
            characterController.attack *= damageMultiplier;
            characterController.attackRange *= 100f;
            characterController.InvokeAttack(targetObject.gameObject);
            isBuffActive = true;
            characterController.specialState = true;
        }
        else
        {
            Debug.Log("Invalid target!");
        }
    }
}
