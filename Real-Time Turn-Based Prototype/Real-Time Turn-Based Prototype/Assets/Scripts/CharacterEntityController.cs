using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class CharacterEntityController : MonoBehaviour
{
    //health
    public float currentHealth;
    public float maxHealth;

    //actions
    public float currentAP;
    public float maxAP;
    public float apRegenRate;
    public float actionCost = 20f;

    //SP for special abilities
    public float currentSP;
    public float maxSP;
    public float spRegenRate;
    [SerializeField] protected float spActionGain = 3f; //How much SP unit gains when landing attack
    [SerializeField] protected float spDamageGain = 5f; //How much SP unit gains when taking damage from an attack
    //inherited special
    public MonoBehaviour specialActionBehaviour;
    protected ISpecialAction special;
    public float spCost;
    public bool specialState = false;
    public string specialName;
    public string specialDescription;

    //Movement
    public bool isMovable = true;
    public float currentMovement;
    public float maxMovement;
    public float minMoveMeter = 4f;     //Amount currentMovement needs to be when moving from a resting position.
    public float movementRegenRate;
    public bool isMoving = false;
    protected Vector3 previousPosition;
    public float speed;
    public float minDistance = 5f;      //How close this object can get to another object when moving towards it
    protected Coroutine movementCoroutine; //Set which corotuine to run for movement
    protected Transform _lockOnTarget;   //Set when moving towards a target object
    public Transform lockOnTarget
    {
        get { return _lockOnTarget; }
        set
        {
            if (value != _lockOnTarget)
            {
                _lockOnTarget = value;
            }
            if (_lockOnTarget != null)
            {
                MoveTowardsTarget();
            }
        }
    }

    //Attack power, range, and speed
    public float attack;
    public float attackRange;
    public float attackSpeed;
    protected float attackTimer;  //lower timer = quicker attack comes out
    public GameObject attackTarget;
    public bool attackState = false;

    //Character attributes
    public string characterName;
    public string description;
    public string unitType; //player or enemy

    //Awake called before Start
    protected void Awake()
    {
        //Set up special action
        if (specialActionBehaviour != null)
        {
            special = specialActionBehaviour as ISpecialAction;
            special.SetSPCost(this);
            special.SetSpecialText(this);
        }
    }

    // Start is called before the first frame update
    protected virtual void Start()
    {
        lockOnTarget = null;
        attackTimer = attackSpeed;
        //Used for movement calculations
        previousPosition = transform.position;
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        //Prevent movement if action is active
        CheckMovable();

        //Check if attack is active and ready to fire
        if (attackState)
        {
            CalculateAttack();
        }

        //Check current health
        CheckHealth();

        //Update regeneration fields
        Regenerate();
    }

    protected void LateUpdate()
    {
        //Subtract Movement meter based on distance travelled from previous frame
        float distanceMovedThisFrame = Vector3.Distance(transform.position, previousPosition);
        if (distanceMovedThisFrame > 0)
        {
            currentMovement -= distanceMovedThisFrame;
            previousPosition = transform.position;
        }
    }

    //Move to cursor on Return/Enter
    public void MoveToCursor(Vector3 cursorPos)
    {
        Debug.Log("Move to cursor");
        Vector3 currentPos = transform.position;
        Vector3 targetPos = new Vector3(cursorPos.x, transform.position.y, cursorPos.z);
        if (movementCoroutine != null)
        {
            //Stop current movement coroutine and start new one
            StopCoroutine(movementCoroutine);
        }
        movementCoroutine = StartCoroutine(MoveToFixedLocation(targetPos));
    }

    //Move towards selected target
    public void MoveTowardsTarget()
    {
        Debug.Log("Move to unit");
        if (movementCoroutine != null)
        {
            //Stop current movement coroutine and start new one
            StopCoroutine(movementCoroutine);
        }
        movementCoroutine = StartCoroutine(MoveToObject(_lockOnTarget));
    }

    //Coroutine used for moving to a specified location on the field
    protected IEnumerator MoveToFixedLocation(Vector3 targetPos)
    {
        //target is cursor at time of hitting Return/Enter
        Vector3 targetHorPos = new Vector3(targetPos.x, transform.position.y, targetPos.z);
        isMoving = true;

        while (Vector3.Distance(transform.position, targetHorPos) > 0.01f)
        {
            if (isMovable)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);
            }
            // Wait until the next frame to re-evaluate the target's position
            yield return null;
        }

        //fix minor spacing at end of move
        transform.position = targetPos;
        isMoving = false;
        Debug.Log("Reached fixed location.");
    }

    //Coroutine used to move to target object, and follow that object when it moves
    protected IEnumerator MoveToObject(Transform targetObject)
    {
        while (_lockOnTarget != null) {
            //Prevent stutter stepping if object being followed is also moving.
            if (isMovable && (isMoving || (!isMoving && currentMovement >= minMoveMeter))) {
                Vector3 currentTargetHorPos = new Vector3(targetObject.position.x, transform.position.y, targetObject.position.z);

                //Move close enough to the target
                if (Vector3.Distance(transform.position, currentTargetHorPos) > minDistance)
                {
                    isMoving = true;
                    transform.position = Vector3.MoveTowards(transform.position, currentTargetHorPos, speed * Time.deltaTime);
                    Debug.Log("Follow Object");
                }
                //else don't move
                else
                {
                    isMoving = false;
                    Debug.Log("Reached Object or used all movement");
                }
            }
            yield return null;
        }
    }

    //Sets attack state to true if target is in range.
    public void InvokeAttack(GameObject targetObject)
    {
        //Check if target in range
        if (Vector3.Distance(targetObject.transform.position, transform.position) <= attackRange)
        {
            //Wait time for attack speed
            attackTarget = targetObject;
            attackState = true;
        }
        else
        {
            Debug.Log("Target out of range");
        }
    }

    //Counts down attack timer while attack state is active
    //Perform attack once timer reaches 0
    protected void CalculateAttack()
    {
        Debug.Log("Attack Timer: " + attackTimer);
        //Cancel Attack if target has been destroyed
        if (attackTarget == null) {
            attackTimer = attackSpeed;
            attackState = false;
            return;
        }

        attackTimer -= Time.deltaTime;

        //Do attack
        if (attackTimer <= 0)
        {
            CharacterEntityController cec = attackTarget.GetComponent<CharacterEntityController>();
            cec.currentHealth -= attack;
            Debug.Log("Target Current Health: " + attackTarget.GetComponent<CharacterEntityController>().currentHealth);
            //Give target SP for taking damage
            cec.currentSP = Mathf.Min(cec.currentSP + cec.spDamageGain, cec.maxSP);

            //Reset attack timer
            attackTimer = attackSpeed;
            attackState = false;

            //Reduce AP for action
            currentAP -= actionCost;
            //Grant SP for landing attack
            currentSP = Mathf.Min(currentSP + spActionGain, maxSP);
        }
    }

    //Checks if object is currently able to move
    protected void CheckMovable()
    {
        //Do not allow movement while attacking
        if (attackState)        //Special check done individually by each special since some allow movement
        {
            isMovable = false;
        }
        else
        {
            isMovable = true;
        }
    }

    //Destroy object if health reaches 0
    protected void CheckHealth()
    {
        if (currentHealth <=0)
        {
            GameObject.Destroy(this.gameObject);
        }
    }

    //Execute special action associated with this unit
    public void PerformSpecial(CharacterEntityController targetObject)
    {
        special?.Execute(this, targetObject);
    }

    //If special has a range value, update it. Otherwise use default of 1.
    public float GetSpecialRange()
    {
        //default
        float specialRange = 1f;
        if (special != null)
        {
            //Check if instance of Special Action contains a field called range
            ISpecialAction spAction = GetComponent<ISpecialAction>();
            if (spAction.GetType().GetField("range") != null)
            {
                //Get component type, field field, then cast the returned object to float.
                specialRange = (float) spAction.GetType().GetField("range").GetValue(spAction);
            }
        }

        return specialRange;
    }

    //Regenerate AP, Movement, and SP over time
    protected void Regenerate()
    {
        //Movement
        if (!isMoving && currentMovement < maxMovement)
        {
            //currentMovement += Time.deltaTime * movementRegenRate;
            currentMovement = Mathf.Min(currentMovement + (Time.deltaTime * movementRegenRate), maxMovement);
        }
        //AP
        if (currentAP < maxAP)
        {
            currentAP = Mathf.Min(currentAP + (Time.deltaTime * apRegenRate), maxAP);
        }
        //SP
        if (currentSP < maxSP)
        {
            currentSP = Mathf.Min(currentSP + (Time.deltaTime * spRegenRate), maxSP);
        }
    }
}
