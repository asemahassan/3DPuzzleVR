using UnityEngine;
using System.Collections;

public class WallsTrigger : MonoBehaviour {

    public Vector3 wallDirection = Vector3.zero;
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnTriggerEnter(Collider otherCol)
    {
        if (otherCol.tag.Equals("InteractableItem"))
        {
            Debug.Log("OnTriggerEnter object collided with wall");
        }
    }

    void OnTriggerStay(Collider otherCol)
    {
        if (otherCol.tag.Equals("InteractableItem"))
        {
            //if they are child of controller then set local position to zero, to avoid moving out of walls
            if (otherCol.transform.parent != null && otherCol.transform.parent.tag.Equals("WandControllers"))
            {
                Debug.Log("Object at boundary, set to Zero position again of wand");
                otherCol.gameObject.transform.localPosition = Vector3.zero;
            }else if(otherCol.transform.parent != null && otherCol.transform.parent.tag.Equals("Model") && GameController._isScaling==true) //during scaling when model is grouped
            {
                Debug.Log("Object going out of boundary, during scaling animate back in");
                otherCol.gameObject.transform.Translate(wallDirection * 0.01f, Space.World);
            }
            else if (otherCol.transform.parent==null) //split animation check
            {
                Debug.Log("During split animation touched wall");
                otherCol.gameObject.transform.Translate(wallDirection * 0.01f, Space.World);
            }
        }
    }

    void OnTriggerExit(Collider otherCol)
    {
        if (otherCol.tag.Equals("InteractableItem"))
        {
            Debug.Log("Animated out of wall");
        }
    }
}
