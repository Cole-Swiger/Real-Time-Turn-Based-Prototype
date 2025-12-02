using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CursorController : MonoBehaviour
{
    public float size;
    public float speed = 5f;

    //lock onto hovered object
    public float snapSpeed = 5f;
    private GameObject targetObject;
    public UnitTextController textController;

    //Pick character action
    public GameObject selectedObject;

    //Cursor Mode
    public enum CursorMode
    {
        Select,
        Move,
        Attack,
        Special,
        Disabled
    }
    public CursorMode currentMode = CursorMode.Select;
    //public bool disabled = false;

    //Event System
    public EventSystem ev;
    public Button specialButton;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (currentMode != CursorMode.Disabled)
        {
            Vector3 movement = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));
            transform.Translate(movement * speed * Time.deltaTime);
        } 

        SnapToObject();
        CheckActiveObject();
        CheckSpecialActive();
        CheckInput();
    }

    //Snap onto object
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Entity")) {
            targetObject = other.gameObject;
            //textController.showText = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Entity")) {
            targetObject = null;
            //textController.showText = false;
        }
    }

    private void SnapToObject()
    {
        if (targetObject != null){
            Vector3 targetPosition = new Vector3(targetObject.transform.position.x,
                                                transform.position.y,
                                                targetObject.transform.position.z);

            transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * snapSpeed);
        }
    }

    private void CheckActiveObject()
    {
        //Show text for highlighted object
        if (targetObject != null)
        {
            textController.UpdateText(targetObject);
            textController.showText = true;
            textController.showStats = true;
        }

        //Hide Text
        if (targetObject == null)
        {
            textController.showText = false;
            textController.showStats = false;
        }

        //Update special text for selected object
        if (selectedObject != null)
        {
            textController.UpdateSpecialText(selectedObject);
        }
    }

    private void CheckInput()
    {
        //Input response depends on current mode
        //Select Mode
        if (currentMode == CursorMode.Select)
        {
            CheckSelectInputs();
        }
        //Move Mode
        else if (currentMode == CursorMode.Move)
        {
            CheckMoveInputs();
        }
        //Disabled Mode
        else if (currentMode == CursorMode.Disabled)
        {
            CheckDisabledInputs();
        }
        //Attack Mode
        else if (currentMode == CursorMode.Attack) {
            CheckAttackInputs();
        }
        //Special Mode
        else if (currentMode == CursorMode.Special)
        {
            CheckSpecialInputs();
        }
    }

    private void CheckSelectInputs()
    {
        //Select Object and open action menu
        if (Input.GetKeyDown(KeyCode.Space) && targetObject != null)
        {
            //Disable cursor
            currentMode = CursorMode.Disabled;
            selectedObject = targetObject;
            textController.showActionText = true;
        }
    }

    private void CheckMoveInputs()
    {
        //Upodate Move Range each frame
        ShowMovementRange();

        //Move Selected Object to target location
        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (targetObject != null)
            {
                //Check if target within range
                if (Vector3.Distance(targetObject.transform.position, selectedObject.transform.position) <= selectedObject.GetComponent<CharacterEntityController>().currentMovement)
                {
                    //target in range
                    //Move selected object to target object. Change trajectory if target object moves.
                    selectedObject.GetComponent<CharacterEntityController>().lockOnTarget = targetObject.transform;
                    //Hide movement range
                    HideMovementRange();
                    //Reset cursor mode to allow new action
                    selectedObject = null;
                    currentMode = CursorMode.Select;
                }
               else
                {
                    //target out of range
                    Debug.Log("Target out of movement range");
                }
            }
            else
            {
                //Move selected object to cursor. Do not change trajectory if cursor moves after the fact.
                Vector3 currentPos = transform.position;
                //Check if cursor is in movement range
                if (Vector3.Distance(currentPos, selectedObject.transform.position) <= selectedObject.GetComponent<CharacterEntityController>().currentMovement)
                {
                    //cursor in range
                    selectedObject.GetComponent<CharacterEntityController>().MoveToCursor(currentPos);
                    //Reset cursor mode to allow new action
                    HideMovementRange();
                    selectedObject = null;
                    currentMode = CursorMode.Select;
                }
                else
                {
                    //cursor out of range
                    Debug.Log("Cursor out of movement range");
                }
            }
        }

        //Cancel move and return to action menu
        if (Input.GetKeyDown(KeyCode.B))
        {
            //Set mode to disabled
            currentMode = CursorMode.Disabled;
            //return cursor to selected object
            HideMovementRange();
            Vector3 selectedUnitPos = selectedObject.transform.position;
            transform.position = new Vector3(selectedUnitPos.x, transform.position.y, selectedUnitPos.z);
            //Reopen action menu
            textController.showActionText = true;
        }
    }

    private void CheckDisabledInputs()
    {
        //Cancel Select
        if (Input.GetKeyDown(KeyCode.B) && selectedObject != null)
        {
            selectedObject = null;
            textController.showActionText = false;
            //Reset selected button in menu
            ev.SetSelectedGameObject(ev.firstSelectedGameObject);
            currentMode = CursorMode.Select;
        }
    }

    private void CheckAttackInputs()
    {
        //Attack Selected Unit
        if (Input.GetKeyDown(KeyCode.Return) && targetObject != null && targetObject != selectedObject) {
            selectedObject.GetComponent<CharacterEntityController>().InvokeAttack(targetObject);
            //Reset selections and menus
            HideAttackRange();
            selectedObject = null;
            textController.showActionText = false;
            ev.SetSelectedGameObject(ev.firstSelectedGameObject);
            currentMode = CursorMode.Select;
        }

        //Cancel attack and return to action menu
        if (Input.GetKeyDown(KeyCode.B))
        {
            //Set mode to disabled
            currentMode = CursorMode.Disabled;
            //return cursor to selected object
            Vector3 selectedUnitPos = selectedObject.transform.position;
            //Hide attack range circle
            HideAttackRange();
            transform.position = new Vector3(selectedUnitPos.x, transform.position.y, selectedUnitPos.z);
            //Reset selected button in menu
            ev.SetSelectedGameObject(ev.firstSelectedGameObject);
            //Reopen action menu
            textController.ShowActionText();
        }
    }

    private void CheckSpecialInputs()
    {
        //Initiate special
        if (Input.GetKeyDown(KeyCode.Return))
        {
            CharacterEntityController targetController;
            if (targetObject != null)
            {
                targetController = targetObject.GetComponent<CharacterEntityController>();
            }
            else
            {
                targetController = null;
            }
            selectedObject.GetComponent<CharacterEntityController>().PerformSpecial(targetController);

            //Reset selections and menus
            Debug.Log("Start Special Reset");
            HideSpecialRange();
            selectedObject = null;
            textController.HideSpecialText();
            ev.SetSelectedGameObject(ev.firstSelectedGameObject);
            currentMode = CursorMode.Select;
        }

        //Cancel special and return to action menu
        if (Input.GetKeyDown(KeyCode.B))
        {
            //Set mode to disabled
            currentMode = CursorMode.Disabled;
            HideSpecialRange();
            //return cursor to selected object
            Vector3 selectedUnitPos = selectedObject.transform.position;
            transform.position = new Vector3(selectedUnitPos.x, transform.position.y, selectedUnitPos.z);
            //Reset selected button in menu
            ev.SetSelectedGameObject(ev.firstSelectedGameObject);
            //Reopen action menu
            textController.HideSpecialText();
            textController.ShowActionText();
        }
    }

    private void CheckSpecialActive()
    {
        if (selectedObject != null && specialButton.interactable)
        {
            CharacterEntityController cec = selectedObject.GetComponent<CharacterEntityController>();
            if (cec.currentSP < cec.spCost)
            {
                specialButton.interactable = false;
            }
        }
        else if (selectedObject != null && !specialButton.interactable) 
        {
            CharacterEntityController cec = selectedObject.GetComponent<CharacterEntityController>();
            if (cec.currentSP >= cec.spCost)
            {
                specialButton.interactable = true;
            }
        }
    }

    public void ShowAttackRange()
    {
        //Find selected object's child Attack Range object
        CircleDrawer cd = selectedObject.transform.Find("Attack Range").GetComponent<CircleDrawer>();
        cd.SetRadius(selectedObject.GetComponent<CharacterEntityController>().attackRange);
        cd.ShowRange();
    }

    public void ShowMovementRange()
    {
        //Find selected object's child Movement Range object
        CircleDrawer cd = selectedObject.transform.Find("Movement Range").GetComponent<CircleDrawer>();
        cd.SetRadius(selectedObject.GetComponent<CharacterEntityController>().currentMovement);
        cd.ShowRange();
    }

    public void ShowSpecialRange()
    {
        //Find selected object's child Special Range object
        CircleDrawer cd = selectedObject.transform.Find("Special Range").GetComponent<CircleDrawer>();
        cd.SetRadius(selectedObject.GetComponent<CharacterEntityController>().GetSpecialRange());
        cd.ShowRange();
    }

    public void HideAttackRange()
    {
        selectedObject.transform.Find("Attack Range").GetComponent<CircleDrawer>().HideRange();
    }

    public void HideMovementRange()
    {
        selectedObject.transform.Find("Movement Range").GetComponent<CircleDrawer>().HideRange();
    }

    public void HideSpecialRange()
    {
        selectedObject.transform.Find("Special Range").GetComponent<CircleDrawer>().HideRange();
    }

    public void UpdateCursorModeMove()
    {
        currentMode = CursorMode.Move;
    }

    public void UpdateCursorModeAttack()
    {
        currentMode = CursorMode.Attack;
    }

    public void UpdateCursorModeSpecial()
    {
        currentMode = CursorMode.Special;
    }

    /*
    TODO:
    Add ranges and other limitations
    Implement Regeneration
    */
}
