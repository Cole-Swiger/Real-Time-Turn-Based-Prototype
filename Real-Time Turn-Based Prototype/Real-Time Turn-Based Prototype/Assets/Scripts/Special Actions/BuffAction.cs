using UnityEngine;

public class BuffAction : MonoBehaviour, ISpecialAction
{
    //Attributes
    public string specialName = "Buff";
    public string specialDescription = "Increase your speed, attack, action regeneration rate, and receive less damage for 20 seconds";
    public float speedBuff = 1.5f; //times
    public float damageBuff = 1.1f; //times
    public float regenBuff = 2f; //times
    public float damageReceived = 0.75f; //times
    public float buffTimer = 20f; //Seconds
    public float cost = 12f;
    public bool isBuffActive = false;
    private CharacterEntityController characterController;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (isBuffActive)
        {
            Debug.Log("Buff Timer: " + buffTimer);
            if (buffTimer <= 0)
            {
                characterController.speed /= speedBuff;
                characterController.attack /= damageBuff;
                characterController.apRegenRate /= regenBuff;
                isBuffActive = false;
                characterController.specialState = false;
                return;
            }
            buffTimer -= Time.deltaTime;
        }
    }

    public void Execute(CharacterEntityController controller, CharacterEntityController targetObject)
    {
        Debug.Log("Buff Special Executed");
        characterController = controller;
        //Buff all stats for certain amount of time.
        characterController.speed *= speedBuff;
        characterController.attack *= damageBuff;
        characterController.apRegenRate *= regenBuff;
        characterController.currentSP -= characterController.spCost;

        isBuffActive = true;
        characterController.specialState = true;
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
}
