/**
*Developer: Asema Hassan SSoe16
*Anatomy Education VR Puzzle game

TEST Class
*This class is handling snapping of two objects with mouse drag.
**/
using UnityEngine;
using System.Collections;

public class Snapping : MonoBehaviour
{

    float closeVPDist = 0.1f;
    float moveSpeed = 40.0f;
    float rotateSpeed = 80.0f;

    Color closeColor = new Color(0, 1, 0, 1);
    Color correctColor = new Color(1,0,0,1);

    private float dist = Mathf.Infinity;
    private Color normalColor;
  

    private Vector3 screenPoint;
    private Vector3 offset;

    private GameObject sphere;

    public Vector3 cubeDefaultPos = Vector3.zero;
    public Vector3 sphereDefaultPos = Vector3.zero;


    public float cubeRelDistance = 0;
    public Quaternion cubeDefaultRot = Quaternion.identity;
    public Vector3 cubeFwdDirection = Vector3.zero;
    public float correctAngle = 0;

    public bool positionChanged = false;
    // Use this for initialization

    void Start()
    {
        normalColor = GetComponent<Renderer>().material.color;
        sphere = GameObject.Find("Hub");
        cubeDefaultPos = this.transform.position;
        sphereDefaultPos = sphere.transform.position;

        RelativeDistanceAndOrientationStored();

    }
    void Update()
    {

        if (Input.GetKey(KeyCode.R))
        {
            this.transform.Rotate(new Vector3(this.transform.rotation.eulerAngles.x, this.transform.rotation.eulerAngles.y + 0.01f, this.transform.rotation.eulerAngles.z));
            sphereDefaultPos = sphere.transform.position;
            cubeDefaultPos = this.transform.position;
        }

        if (Input.GetKey(KeyCode.S))
        {
            this.transform.position = new Vector3(this.transform.position.x - (0.01f/2.0f),
                this.transform.position.y - (0.01f / 2.0f),
                this.transform.position.z - (0.01f / 2.0f));
            this.transform.localScale=new Vector3(this.transform.localScale.x + 0.01f,
                this.transform.localScale.y + 0.01f,
                this.transform.localScale.z + 0.01f);
            this.transform.position = new Vector3(this.transform.position.x + (0.01f / 2.0f),
            this.transform.position.y + (0.01f / 2.0f),
             this.transform.position.z + (0.01f / 2.0f));


            sphere.transform.position = new Vector3(sphere.transform.position.x - 0.01f,
               sphere.transform.position.y - 0.01f,
               sphere.transform.position.z - 0.01f);

            sphere.transform.localScale = new Vector3(sphere.transform.localScale.x + 0.01f,
                sphere.transform.localScale.y + 0.01f,
                sphere.transform.localScale.z + 0.01f);

            sphere.transform.position = new Vector3(sphere.transform.position.x + 0.01f,
              sphere.transform.position.y +0.01f,
              sphere.transform.position.z + 0.01f);

            float dist = Vector3.Distance(cubeDefaultPos, sphereDefaultPos);
            Debug.Log("Distance: " + dist);
            Debug.Log("Position: " + this.transform.position + " " + sphere.transform.position);
        }
    }

    void RelativeDistanceAndOrientationStored()
    {
        cubeRelDistance= Vector3.Distance(cubeDefaultPos, sphereDefaultPos);
        cubeDefaultRot = this.transform.rotation;
        cubeFwdDirection = this.transform.forward;
        correctAngle = CalculateAngleBetweenTwoObjects(this.transform, sphere.transform);
    }

    void OnMouseDown()
    {
        screenPoint = Camera.main.WorldToScreenPoint(gameObject.transform.position);
        offset = gameObject.transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z));
    }

    void OnMouseDrag()
    {
        Vector3 curScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z);
        Vector3 curPosition = Camera.main.ScreenToWorldPoint(curScreenPoint) + offset;
        transform.position = curPosition;

        Vector3 partnerPos = Camera.main.WorldToViewportPoint(sphereDefaultPos);
        Vector3 myPos = Camera.main.WorldToViewportPoint(transform.position);
        dist = Vector2.Distance(partnerPos, myPos);
   
        GetComponent<Renderer>().material.color = (dist <= closeVPDist) ? closeColor : normalColor;
    }

    void OnMouseUp()
    {
        if ((dist>= 0.01f) && dist <= closeVPDist)
        {
           // Debug.Log("Distance:" + dist);
            StartCoroutine(InstallPart());
        }else
        {
            StopAllCoroutines();
        }
    }

    float CalculateAngleBetweenTwoObjects(Transform obj1, Transform obj2)
    {
        // float angle = Mathf.Acos(Vector3.Dot(obj1.normalized, obj2.normalized));
        Vector3 targetDir = obj2.position - obj1.position;
        Vector3 forward = obj1.forward;
        float angle = Vector3.Angle(targetDir, forward);
        return angle;
    }

    IEnumerator InstallPart()
    {
        float relDistance = Vector3.Distance(this.transform.position, sphere.transform.position);
        while (relDistance >= cubeRelDistance)
        {
            relDistance= Vector3.Distance(this.transform.position, sphere.transform.position);
            float calculatedAngle = CalculateAngleBetweenTwoObjects(this.transform, sphere.transform);
           // Debug.Log("Calculated angle:" + calculatedAngle);

            float diff = calculatedAngle - correctAngle;
            if(diff >= -5.0f && diff <= 5.0f)
            {
                transform.position = Vector3.MoveTowards(this.transform.position, sphere.transform.position, Time.deltaTime * moveSpeed);
                transform.rotation = Quaternion.RotateTowards(this.transform.rotation, cubeDefaultRot, Time.deltaTime * rotateSpeed);
                //transform.rotation = new Quaternion(cubeDefaultRot.x,cubeDefaultRot.y,cubeDefaultRot.z,cubeDefaultRot.w);

                GetComponent<Renderer>().material.color = correctColor;
            }
          
            yield return new WaitForSeconds(0.01f);
        }
   
       // Debug.Log("Reached a distance:" + relDistance);
    }
}

