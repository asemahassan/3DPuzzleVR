using UnityEngine;
using System.Collections;

public class AngleTest : MonoBehaviour {


    public float AngleCalculated = 0.0f;
    public Transform target = null;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

        AngleWithVector3();
    }

    void AngleWithVector3()
    {
        AngleCalculated = Vector3.Angle(this.transform.position, target.transform.position);
        Debug.Log("Vector3Angle: " + AngleCalculated);

    }

    void AngleWithQuaternion()
    {
        AngleCalculated = Quaternion.Angle(this.transform.rotation, target.transform.rotation);
        Debug.Log("QuaternionAngle: " + AngleCalculated);
    }
}
