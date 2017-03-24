/**
*Developer: Asema Hassan SSoe16
*Anatomy Education VR Puzzle game

 * InteractableItemWithIsKinematic are GameObjects with a rigidbody as IsKinematic,  that can be picked up using the Vive
 * controllers. Setting positions of objects as wand position and rotation.
 * Attach this script to gameobjects that you want to interact with, should have rigidbody as component also boxCollider.
 *
 */
 using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(MeshCollider))]
//[RequireComponent(typeof(BoxCollider))]
public class InteractableItemWithIsKinematic : MonoBehaviour {
    public bool isSnapped = false;
    protected bool currentlyInteracting;
    // The controller this object is picked up by
	private WandControlForItems attachedWand; 

    void Start()
    {
        this.tag = "InteractableItem";
        isSnapped = false;
    }
	public void BeginInteraction(WandControlForItems wand) {

        if (isSnapped)
        {
            attachedWand = wand;
            Transform parentOfSnappedObj = this.transform.parent;
            if(parentOfSnappedObj!=null)
                parentOfSnappedObj.transform.SetParent(wand.transform, true);
            currentlyInteracting = true;

         //   Debug.Log("after split here..check1");
        }
        else{ //not snapped objects
            attachedWand = wand;
            this.transform.SetParent(wand.transform, true);
            currentlyInteracting = true;

         //   Debug.Log("after split here..check2");
        }
        
    }

    public void EndInteraction(WandControlForItems wand)
    {
       if (wand == attachedWand)
       {

          if (isSnapped)
           {
                    attachedWand = null;
                    currentlyInteracting = false;
                    Transform parentOfSnappedObj = this.transform.parent;
                    if (parentOfSnappedObj != null)
                        parentOfSnappedObj.transform.SetParent(null);

              //  Debug.Log("end split here..check1");
            }
            else
            { //not snapped objects
                    attachedWand = null;
                    currentlyInteracting = false;
                    this.transform.SetParent(null);

            //    Debug.Log("end split here..check2");
            }   
         }
     }

    public bool IsInteracting() {
        return currentlyInteracting;
    }

    public void SnapStatus(bool status)
    {
        isSnapped = status;
        //check if snapped true then increment progress
        if (isSnapped)
            GameController.UpdateProgressUI(1);
        else //decrement progress
            GameController.UpdateProgressUI(-1);
    }

    public bool GetSnapStatus()
    {
        //to check progress of whole model
        return isSnapped;
    }
}
