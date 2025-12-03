using UnityEngine;

public class AOEAttackAction : MonoBehaviour, ISpecialAction
{
    //Action attributes
    public string specialName = "Crush";
    public string specialDescription = "Charge up, then do a weak AOE attack on anyone in range.";
    public float range = 5f;
    public float damage = 4f;
    public float cost = 10f;
    public float attackTimer = 2.5f;
    private float currentTimer;
    private bool specialActive = false;
    private CharacterEntityController characterController;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentTimer = attackTimer;
    }

    // Update is called once per frame
    void Update()
    {
        if (specialActive) 
        {
            //Countdown to attack
            Debug.Log("Current Timer: " + currentTimer);
            if (currentTimer <= 0)
            {
                Collider[] hitColliders = Physics.OverlapSphere(transform.position, range);
                foreach (Collider hitCollider in hitColliders)
                {
                    Debug.Log("Hit by attack: " + hitCollider.gameObject.name);
                    if (hitCollider.gameObject.GetComponent<CharacterEntityController>() != null && hitCollider.gameObject.GetComponent<CharacterEntityController>() != characterController)
                    {
                        CharacterEntityController character = hitCollider.gameObject.GetComponent<CharacterEntityController>();
                        character.currentHealth -= damage;
                        Debug.Log($"{character.currentHealth}");
                    }
                }
                specialActive = false;
                characterController.specialState = true;
                currentTimer = attackTimer;
            }
            else
            {
                currentTimer -= Time.deltaTime;
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
        Debug.Log("AOE Special Executed");
        characterController = controller;
        specialActive = true;
        characterController.specialState = true;
    }
}
