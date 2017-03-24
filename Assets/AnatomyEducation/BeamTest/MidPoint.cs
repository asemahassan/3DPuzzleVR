using UnityEngine;
using System.Collections;

public class MidPoint : MonoBehaviour
{

    public Transform OtherObjectA; // First object of pair
    public Transform OtherObjectB; // Second object of pair

    public void Update()
    {
        Vector3 directionCtoA = OtherObjectA.position - transform.position; // directionCtoA = positionA - positionC
        Vector3 directionCtoB = OtherObjectB.position - transform.position; // directionCtoB = positionB - positionC
        Vector3 directionAtoB = OtherObjectB.position - OtherObjectA.position; // directionAtoB = target.position - source.position 
        Vector3 midpointAtoB = new Vector3((directionCtoA.x + directionCtoB.x) / 2.0f, (directionCtoA.y + directionCtoB.y) / 2.0f, (directionCtoA.z + directionCtoB.z) / 2.0f); // midpoint between A B

        Debug.DrawRay(OtherObjectA.position, directionAtoB, Color.red); // line between A and 
        Debug.DrawRay(transform.position, directionCtoA, Color.green); // line between C and A
        Debug.DrawRay(transform.position, directionCtoB, Color.green); // line between C and B
        Debug.DrawRay(transform.position, midpointAtoB, Color.blue); // line midway between C and B

    }
}
