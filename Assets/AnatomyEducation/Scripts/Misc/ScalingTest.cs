using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ScalingTest : MonoBehaviour
{

    public Transform parentObj = null;
    public List<Transform> objList = new List<Transform>();
    public float ratio = 0.1f;
    public float _MAXThreshold = 5.0f;
    public float _MINThreshold = 0.25f;
    // Use this for initialization
    void Start()
    {

        CalculateDistance();
    }


    void CalculateDistance()
    {
        //calculate the distance btw objects
        for (int i = 0; i < objList.Count; i++)
        {
            Transform objToScale = objList[i];
            for (int j = 0; j < objList.Count; j++)
            {
                Transform objToScale2 = objList[j];
                float dist = Vector3.Distance(objToScale.transform.position, objToScale2.transform.position);
                Debug.Log("Distance: " + objToScale.name + " " + objToScale2.name + "is: " + dist);
            }
        }
    }
    // Update is called once per frame
    void Update()
    {


        //to scale all child objects with the same ratio, to not change the transform position
        #region SCALING_CHILDS
        if (Input.GetKeyUp(KeyCode.UpArrow))
        {
            for (int i = 0; i < objList.Count; i++)
            {
                Transform objToScale = objList[i];
                if (objToScale.transform.localScale.x < _MAXThreshold)
                {
                    objToScale.transform.localScale = new Vector3(objToScale.transform.localScale.x + ratio,
                    objToScale.transform.localScale.y + ratio,
                    objToScale.transform.localScale.z + ratio);
                }
            }
            Debug.Log("AllChilds Scaled up");
            CalculateDistance();
        }
        else if (Input.GetKeyUp(KeyCode.DownArrow))
        {
            for (int i = 0; i < objList.Count; i++)
            {
                Transform objToScale = objList[i];
                if (objToScale.transform.localScale.x > _MINThreshold)
                {
                    objToScale.transform.localScale = new Vector3(objToScale.transform.localScale.x - ratio,
                     objToScale.transform.localScale.y - ratio,
                     objToScale.transform.localScale.z - ratio);
                }
            }
            Debug.Log("AllChilds Scaled down");
            CalculateDistance();
        }
        #endregion

        #region SCALING_PARENT
        if (Input.GetKeyUp(KeyCode.RightArrow))
        {
            if (parentObj.transform.localScale.x < _MAXThreshold)
            {
                parentObj.transform.localScale = new Vector3(parentObj.transform.localScale.x + ratio,
                    parentObj.transform.localScale.y + ratio,
                    parentObj.transform.localScale.z + ratio);
            }
            Debug.Log("Parent Scaled up");
            CalculateDistance();
        }
        else if (Input.GetKeyUp(KeyCode.LeftArrow))
        {
            if (parentObj.transform.localScale.x > _MINThreshold)
            {
                parentObj.transform.localScale = new Vector3(parentObj.transform.localScale.x - ratio,
                     parentObj.transform.localScale.y - ratio,
                     parentObj.transform.localScale.z - ratio);
            }
            Debug.Log("Parent Scaled down");
            CalculateDistance();
        }
        #endregion
    }
}
