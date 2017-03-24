/**
*Developer: Asema Hassan SSoe16
*Anatomy Education VR Puzzle game

 * Anatomy Education VR Puzzle game
 * Basic implementation of how to use a Vive controller as an input device.
 * Can only interact with items with InteractableItem component.
 *  Also scale up the whole model all together when two-handed grip button is pressed onto two objects simultanoeuly
 */
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class WandControlForItems : MonoBehaviour
{

    #region PRIVATE_VARIABLES
    private Valve.VR.EVRButtonId triggerButton = Valve.VR.EVRButtonId.k_EButton_SteamVR_Trigger;
    private Valve.VR.EVRButtonId menuButton = Valve.VR.EVRButtonId.k_EButton_ApplicationMenu;
    private Valve.VR.EVRButtonId gripButton = Valve.VR.EVRButtonId.k_EButton_Grip;
  
    private SteamVR_Controller.Device controller { get { return SteamVR_Controller.Input((int)trackedObj.index); } }
    private SteamVR_TrackedObject trackedObj;
    HashSet<InteractableItemWithIsKinematic> objectsHoveringOver = new HashSet<InteractableItemWithIsKinematic>();

    private InteractableItemWithIsKinematic closestItem = null;

    #endregion

    #region PUBLIC_VARIABLES
    public InteractableItemWithIsKinematic interactingItem = null;
    //Set objects name in UI for reference
    public Text rightHandObject_InfoUI = null;
    public Text leftHandObject_InfoUI = null;

    public bool gripButtonPressed = false;
    public bool grabbedObject = false;
    public Vector3 controllerPosition = Vector3.zero;

    public bool islaserController = false;

    public GameObject pointerObjHandler = null;
    #endregion

    #region INITIALIZATION
    // Use this for initialization
    void Start()
    {
        GetChildObjectsRef();
    }
    #endregion


    void GetChildObjectsRef()
    {
        trackedObj = GetComponent<SteamVR_TrackedObject>();
        if (this.transform.name.Equals("Controller (right)"))
        {
            rightHandObject_InfoUI = GameObject.Find("RightHandObjectsInfo").GetComponent<Text>();
            pointerObjHandler = GameObject.Find("[Controller (right)]WorldPointer_SimplePointer_Holder");
        }
        else if (this.transform.name.Equals("Controller (left)"))
        {
            leftHandObject_InfoUI = GameObject.Find("LeftHandObjectsInfo").GetComponent<Text>();
            pointerObjHandler = GameObject.Find("[Controller (left)]WorldPointer_SimplePointer_Holder");
        }
    }
    #region WAND_CONTROL
    // Update is called once per frame
    void Update()
    {
        if (controller == null)
        {
            Debug.Log("Controller not initialized");
            return;
        }
        //--------------Changes after 12.07.16----------------------
        #region ON_MENU_BUTTON
            /* if (controller.GetPressUp(menuButton))
                   {
                       if (!islaserController)
                       {
                           //enable the laser pointer to select UI buttons e-g group or ungroup
                           this.gameObject.AddComponent<Wacki.ViveUILaserPointer>();
                           islaserController = true;
                       }else if(islaserController)
                       {
                           Destroy(this.gameObject.GetComponent<Wacki.ViveUILaserPointer>());
                           islaserController = false;
                           Destroy(this.transform.FindChild("Cube").gameObject);
                           Destroy(this.transform.FindChild("Sphere").gameObject);
                       }
                    }*/
            #endregion

            #region TRANSLATE_ROTATE_ON_TRIGGER
            //For picking and dragging objects
            if (controller.GetPressDown(triggerButton))
            {
                // Find the closest item to the hand in case there are multiple and interact with it
                float minDistance = float.MaxValue;
                float distance;

                foreach (InteractableItemWithIsKinematic item in objectsHoveringOver)
                {
                    distance = (item.transform.position - transform.position).sqrMagnitude;

                    if (distance < minDistance)
                    {
                        minDistance = distance;
                        closestItem = item;
                    }
                }
                interactingItem = closestItem;
                closestItem = null;

                if (interactingItem)
                {
                    //set object name in UI
                    if (this.transform.name.Equals("Controller (right)") && rightHandObject_InfoUI != null)
                    {
                        rightHandObject_InfoUI.text = "Right-hand object: " + interactingItem.name;
                    }
                    if (this.transform.name.Equals("Controller (left)") && leftHandObject_InfoUI != null)
                    {
                        leftHandObject_InfoUI.text = "Left-hand object: " + interactingItem.name;
                    }

                    if (interactingItem.IsInteracting())
                    {
                        interactingItem.EndInteraction(this);
                    }

                    interactingItem.BeginInteraction(this);
                    grabbedObject = true;
                }
            }

        //For dropping objects
        if (controller.GetPressUp(triggerButton) && interactingItem != null)
        {
            //check to snap if two gameobjects are in range
            interactingItem.EndInteraction(this);
            //set object name in UI
            if (this.transform.name.Equals("Controller (right)") && rightHandObject_InfoUI != null)
            {
                rightHandObject_InfoUI.text = "Right hand free";
            }
            if (this.transform.name.Equals("Controller (left)") && leftHandObject_InfoUI != null)
            {
                leftHandObject_InfoUI.text = "Left hand free";
            }
            grabbedObject = false;
        }
        #endregion

        //--------------Changes after 12.07.16----------------------
        #region SCALING_ON_GRIP
        controllerPosition = this.transform.position;
        if (controller.GetPressDown(gripButton))
        {
            gripButtonPressed = true;
        }
        else if (controller.GetPressUp(gripButton))
        {
            gripButtonPressed = false;
        }
        #endregion
    }
    #endregion


    public void ChangePointersStatus(bool status)
    {
        if (pointerObjHandler == null)
        {
            GetChildObjectsRef();
        }
       pointerObjHandler.SetActive(status);
    }

    #region PUBLIC_METHODS
    public void EndInteractionWhenObjectsSnapped()
    {
        if (interactingItem != null)
        {
            //check to snap if two gameobjects are in range
            interactingItem.EndInteraction(this);
            //set object name in UI
            if (this.transform.name.Equals("Controller (right)") && rightHandObject_InfoUI != null)
            {
                rightHandObject_InfoUI.text = "Right hand free";
            }
            if (this.transform.name.Equals("Controller (left)") && leftHandObject_InfoUI != null)
            {
                leftHandObject_InfoUI.text = "Left hand free";
            }
            grabbedObject = false;
        }
    }

    #endregion

    #region TRIGGER_EVENTS
    // Adds all colliding items to a HashSet for processing which is closest
    private void OnTriggerEnter(Collider collider)
    {
        InteractableItemWithIsKinematic collidedItem = collider.GetComponent<InteractableItemWithIsKinematic>();
        if (collidedItem)
        {
            objectsHoveringOver.Add(collidedItem);
        }
    }

    // Remove all items no longer colliding with to avoid further processing
    private void OnTriggerExit(Collider collider)
    {
        InteractableItemWithIsKinematic collidedItem = collider.GetComponent<InteractableItemWithIsKinematic>();
        if (collidedItem)
        {
            objectsHoveringOver.Remove(collidedItem);
        }
    }
    #endregion
}
