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
    [SerializeField] private GameObject targetObject;
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

    //Event System
    public EventSystem ev;
    public Button moveButton;
    public Button attackButton;
    public Button specialButton;
    public Button[] buttonList;

    // Update is called once per frame
    void Update()
    {
        //Allow player to move cursor if not disabled
        if (currentMode != CursorMode.Disabled)
        {
            Vector3 movement = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));
            transform.Translate(movement * speed * Time.deltaTime);
        } 

        SnapToObject();
        CheckActiveButtons();
        CheckActiveObject();
        CheckInput();
    }

    //Toggle targetObject
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Entity")) {
            targetObject = other.gameObject;
        }
    }   
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Entity")) {
            targetObject = null;
        }
    }

    //Snap onto object cursor is touching
    private void SnapToObject()
    {
        if (targetObject != null){
            Vector3 targetPosition = new Vector3(targetObject.transform.position.x,
                                                transform.position.y,
                                                targetObject.transform.position.z);
            //Snap smoothly
            transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * snapSpeed);
        }
    }

    //Update text that will be shown for the target and selected object, if applicable
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

    //Input response depends on current cursor mode
    private void CheckInput()
    {
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

    //Respond to user inputs while cursor is in Select mode
    private void CheckSelectInputs()
    {
        //Select Object and open action menu
        if (Input.GetKeyDown(KeyCode.Space) && targetObject != null)
        {
            //Disable cursor
            currentMode = CursorMode.Disabled;
            selectedObject = targetObject;
            //Reset default button to first interactable.
            SetDefaultMenuButton();
            textController.showActionText = true;
        }
    }

    //Respond to user inputs while cursor is in Move mode
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
            //Reset and reopen action menu
            SetDefaultMenuButton();
            textController.showActionText = true;
        }
    }

    //Respond to user inputs while cursor is in Disabled mode
    private void CheckDisabledInputs()
    {
        //Cancel Select
        if (Input.GetKeyDown(KeyCode.B) && selectedObject != null)
        {
            selectedObject = null;
            textController.showActionText = false;
            //Reset selected button in menu
            SetDefaultMenuButton();
            currentMode = CursorMode.Select;
        }
    }

    //Respond to user inputs while cursor is in Attack mode
    private void CheckAttackInputs()
    {
        //Attack Selected Unit
        if (Input.GetKeyDown(KeyCode.Return) && targetObject != null && targetObject != selectedObject) {
            selectedObject.GetComponent<CharacterEntityController>().InvokeAttack(targetObject);
            //Reset selections and menus
            HideAttackRange();
            selectedObject = null;
            textController.showActionText = false;
            //ev.SetSelectedGameObject(ev.firstSelectedGameObject);
            SetDefaultMenuButton();
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
            SetDefaultMenuButton();
            //Reopen action menu
            textController.ShowActionText();
        }
    }

    //Respond to user inputs while cursor is in Special mode
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
            SetDefaultMenuButton();
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
            SetDefaultMenuButton();
            //Reopen action menu
            textController.HideSpecialText();
            textController.ShowActionText();
        }
    }

    //Checks if selected character has enough AP, SP, and Movement.
    //Disables buttons if not enough stats.
    private void CheckActiveButtons()
    {
        //Check if object is selected
        if (selectedObject != null)
        {
            CharacterEntityController cec = selectedObject.GetComponent<CharacterEntityController>();
            //Move Button
            if (moveButton.interactable)
            {
                if (cec.currentMovement < cec.minMoveMeter)
                {
                    moveButton.interactable = false;
                }
            }
            else
            {
                if (cec.currentMovement >= cec.minMoveMeter) 
                {
                    moveButton.interactable = true;
                }
            }
            //Attak Button
            if (attackButton.interactable)
            {
                if (cec.currentAP < cec.actionCost)
                {
                    attackButton.interactable = false;
                }
            }
            else
            {
                if (cec.currentAP >= cec.actionCost)
                {
                    attackButton.interactable = true;
                }
            }
            //Special Button
            if (specialButton.interactable)
            {
                //Also disable button if special is currently active
                if (cec.currentSP < cec.spCost || cec.specialState)
                {
                    specialButton.interactable = false;
                }
            }
            else
            {
                if (cec.currentSP >= cec.spCost && !cec.specialState)
                {
                    specialButton.interactable = true;
                }
            }
        }
    }

    //Sets the default selected menu button to first one that is clickable (not disabled)
    private void SetDefaultMenuButton()
    {
        //Check button states before setting default
        CheckActiveButtons();

        Selectable first = buttonList[0];
        Selectable currentButton = null;
        for (int i = 0; i < buttonList.Length - 1; i++)
        {
            currentButton = buttonList[i];
            if (currentButton.interactable)
            {
                // Set the found selectable as the active selection
                EventSystem.current.SetSelectedGameObject(currentButton.gameObject);
                break;
            }
            else
            {

            }
        }
    }

    //Show range of unit's attack
    public void ShowAttackRange()
    {
        //Find selected object's child Attack Range object
        CircleDrawer cd = selectedObject.transform.Find("Attack Range").GetComponent<CircleDrawer>();
        cd.SetRadius(selectedObject.GetComponent<CharacterEntityController>().attackRange);
        cd.ShowRange();
    }

    //Show range of unit's movement
    public void ShowMovementRange()
    {
        //Find selected object's child Movement Range object
        CircleDrawer cd = selectedObject.transform.Find("Movement Range").GetComponent<CircleDrawer>();
        cd.SetRadius(selectedObject.GetComponent<CharacterEntityController>().currentMovement);
        cd.ShowRange();
    }

    //Show range of unit's special
    public void ShowSpecialRange()
    {
        //Find selected object's child Special Range object
        CircleDrawer cd = selectedObject.transform.Find("Special Range").GetComponent<CircleDrawer>();
        cd.SetRadius(selectedObject.GetComponent<CharacterEntityController>().GetSpecialRange());
        cd.ShowRange();
    }

    //Hide range of unit's attack
    public void HideAttackRange()
    {
        selectedObject.transform.Find("Attack Range").GetComponent<CircleDrawer>().HideRange();
    }

    //Hide range of unit's movement
    public void HideMovementRange()
    {
        selectedObject.transform.Find("Movement Range").GetComponent<CircleDrawer>().HideRange();
    }

    //Hide range of unit's special
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
}
