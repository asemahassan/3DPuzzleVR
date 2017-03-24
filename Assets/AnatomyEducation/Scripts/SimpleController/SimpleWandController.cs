using UnityEngine;
using System.Collections;
using System.Collections.Generic;
/**
 * Basic implementation of how to use a Vive controller as an input device.
 * Can only interact with items with InteractableItem component
 */
public class SimpleWandController : MonoBehaviour {
    private Valve.VR.EVRButtonId gripButton = Valve.VR.EVRButtonId.k_EButton_Grip;
    private SteamVR_Controller.Device controller { get { return SteamVR_Controller.Input((int)trackedObj.index); } }
    private SteamVR_TrackedObject trackedObj;
	public GameObject pickup;

	// Use this for initialization
	void Start () {
        trackedObj = GetComponent<SteamVR_TrackedObject>();
	}

	// Update is called once per frame
	void Update () {
	    if (controller == null) {
            Debug.Log("Controller not initialized");
            return;
        }

		if (controller.GetPressDown (gripButton) && pickup != null) {
			pickup.transform.parent = this.transform;
			Debug.Log("here");
		}
		if (controller.GetPressUp (gripButton) && pickup != null) {
			pickup.transform.parent = null;
		}
	}

    private void OnTriggerEnter(Collider collider) {
		pickup = collider.gameObject;
		Debug.Log ("Pick up object is:" + pickup.name);
    }

    private void OnTriggerExit(Collider collider) {
			pickup = null;
    }
}
