using UnityEngine;

//This class allows enemy units to automatically take actions
public class EnemyAiController : CharacterEntityController
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    protected override void Update()
    {
        //lock onto closest player unit
        LockOnPlayer();
        //Decide on Action
        CheckAction();
        base.Update();
    }

    //Have enemy follow closest player unit
    private void LockOnPlayer()
    {
        if (lockOnTarget == null)
        {
            GameObject[] targetList = GameObject.FindGameObjectsWithTag("Entity");
            GameObject closestObject = null;
            float closestDistance = maxMovement;

            foreach (GameObject go in targetList)
            {
                //Ignore self and other enemies. Also do nothig if there is no object in range
                if (go != null && go.GetComponent<CharacterEntityController>().unitType != "Enemy")
                {
                    Vector3 objectLocation = go.transform.position;
                    float currentDistance = Vector3.Distance(transform.position, objectLocation);

                    if (currentDistance < closestDistance)
                    {
                        closestObject = go;
                        closestDistance = currentDistance;
                    }
                }
            }

            //lock on if valid target exists
            if (closestObject != null)
            {
                lockOnTarget = closestObject.transform;
            }
        }
        //Ensure enemy follows target if it moves after initial coroutine finishes
        else
        {
            MoveTowardsTarget();
        }
    }

    //Decide which action the enemy should take
    private void CheckAction()
    {
        //Only perform action if none currently in progress
        if (!isMoving && !attackState && !specialState)
        {
            //Special takes priority over attack
            if (currentSP >= spCost)
            {
                PerformSpecial(gameObject.GetComponent<CharacterEntityController>());
            }
            else if (lockOnTarget != null && currentAP >= actionCost)
            {
                InvokeAttack(lockOnTarget.gameObject);
            }
        }  
    }
}
