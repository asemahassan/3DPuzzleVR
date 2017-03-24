using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using VRTK;

public class SteamVR_ControllerPointerEvents_ObjectNamePopUp : MonoBehaviour {

    private Transform objData = null;
    public Transform selectedTarget = null;

    private void Start()
    {
        if (GetComponent<VRTK_SimplePointer>() == null)
        {
            Debug.LogError("VRTK_ControllerPointerEvents_ListenerExample is required to be attached to a SteamVR Controller that has the VRTK_SimplePointer script attached to it");
            return;
        }

        //Setup controller event listeners
        GetComponent<VRTK_SimplePointer>().DestinationMarkerEnter += new DestinationMarkerEventHandler(DoPointerIn);
        GetComponent<VRTK_SimplePointer>().DestinationMarkerExit += new DestinationMarkerEventHandler(DoPointerOut);
        GetComponent<VRTK_SimplePointer>().DestinationMarkerSet += new DestinationMarkerEventHandler(DoPointerDestinationSet);

        if (objData == null)
        {
            //instantiate one as child of controller
            objData = this.transform.FindChild("ObjectsData");
            if (objData != null)
                objData.gameObject.SetActive(false);
        }
    }

    private void DebugLogger(uint index, string action, Transform target, float distance, Vector3 tipPosition)
    {
        // string targetName = (target ? target.name : "<NO VALID TARGET>");
        // Debug.Log("Controller on index '" + index + "' is " + action + " at a distance of " + distance + " on object named " + targetName + " - the pointer tip position is/was: " + tipPosition);
        //if (target.tag.Equals("UI"))
        //{
        //    Debug.Log("UI object:" + target.name);
        //}
        if (target.tag.Equals("InteractableItem"))
        {
            string objHandleName = string.Format("[{0}]WorldPointer_SimplePointer_Holder", this.gameObject.name);
            string objPointerName = string.Format("[{0}]WorldPointer_SimplePointer_PointerTip", this.gameObject.name);
            Transform pointerHandle = this.transform.FindChild(objHandleName);
            if (pointerHandle != null)
            {
                Transform pointerTip = pointerHandle.transform.FindChild(objPointerName);
                if (objData != null && pointerTip != null)
                {
                    objData.gameObject.SetActive(true);
                    objData.SetParent(pointerHandle,false);
                   // objData.transform.parent = pointerHandle;
                    objData.transform.localPosition = new Vector3(pointerTip.transform.localPosition.x,
                    pointerTip.transform.localPosition.y + 0.1f,
                    pointerTip.transform.localPosition.z - 0.1f);
                    objData.transform.localRotation = Quaternion.identity;
                    Transform bgRef = objData.transform.FindChild("BG");
                    bgRef.transform.FindChild("Info").GetComponent<Text>().text = target.name;
                    //Debug.Log("Interactacle Item: " + target.name);
                    selectedTarget = target;
                }
            }
        }
        else
        {
            if (objData != null)
            {
                objData.gameObject.SetActive(false);
            }
        }

    }
    private void DoPointerIn(object sender, DestinationMarkerEventArgs e)
    {
        DebugLogger(e.controllerIndex, "POINTER IN", e.target, e.distance, e.destinationPosition);
    }

    private void DoPointerOut(object sender, DestinationMarkerEventArgs e)
    {
        //DebugLogger(e.controllerIndex, "POINTER OUT", e.target, e.distance, e.destinationPosition);
        selectedTarget = null;
    }

    private void DoPointerDestinationSet(object sender, DestinationMarkerEventArgs e)
    {
        //DebugLogger(e.controllerIndex, "POINTER DESTINATION", e.target, e.distance, e.destinationPosition);
        selectedTarget = null;
    }
}
