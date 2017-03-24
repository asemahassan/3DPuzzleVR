using UnityEngine;
using System.Collections;

public class EuclideanDistance {
	
	public Transform object1 = null;
	public Transform object2 = null;
	// Use this for initialization
	void Start () {
            Debug.Log(GetDistance(object1,object2));

    }
    public static float GetDistance(Transform object1, Transform object2)
    {
        float distance = Vector3.Distance(object1.transform.position, object2.transform.position);
       // float distanceSqr = (object1.position - object2.position).sqrMagnitude;
       //Debug.Log("Distance: " + distance);
       // Debug.Log("DistanceSqr: " + distanceSqr);
        return distance;
    }
}
