/**
*Developer: Asema Hassan SSoe16
*Anatomy Education VR Puzzle game

*This class is handling all the functions of model,
 
1. Save model data into dictionaries in start.
2. Docking points check.
3. Snapping feature.
4. Scaling UP/Down whole model.
**/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class ModelController : MonoBehaviour
{

    #region OBJECT_DATA_CLASS
    class ObjectsDataClass
    {
        public string objHandle;
        public string parentName;
        public Vector3 position;
        public float relativeDistance;
        public Quaternion rotation;
        public Vector3 forward;
        public float angleBtwObjects;
        public Color matColor;

        public ObjectsDataClass(string data, string handleName)
        {
            objHandle = handleName;
            //split string on basis of # symbol
            string[] objStrArray = data.Split('#');
            if (objStrArray.Length > 0)
            {
                //getting parent
                parentName = objStrArray[0];
                //converting position
                string[] pos = objStrArray[1].Split(',');
                    float x = float.Parse(pos[0]);
                    float y = float.Parse(pos[1]);
                    float z = float.Parse(pos[2]);
                    position = new Vector3(x, y, z);
              

                //converting rotation
                string[] rot = objStrArray[2].Split(',');
                float xR = float.Parse(rot[0]);
                float yR = float.Parse(rot[1]);
                float zR = float.Parse(rot[2]);
                float wR = float.Parse(rot[3]);
                rotation = new Quaternion(xR, yR, zR, wR);

                //converting forward
                string[] fwd = objStrArray[3].Split(',');
                float xF = float.Parse(fwd[0]);
                float yF = float.Parse(fwd[1]);
                float zF = float.Parse(fwd[2]);
                forward = new Vector3(xF, yF, zF);

                //converting color
                string[] col = objStrArray[4].Split(',');
                float r = float.Parse(col[0]);
                float g = float.Parse(col[1]);
                float b = float.Parse(col[2]);
                float a = float.Parse(col[3]);
                matColor = new Color(r, g, b, a);

                //relative distance between two objects
                relativeDistance = float.Parse(objStrArray[5]);

                //get correct angle 
                angleBtwObjects = float.Parse(objStrArray[6]);
            }
        } 
    }
    #endregion

    #region PUBLIC_ VARIABLES
    public List<Transform> _modelAllChildTransformObjects = new List<Transform>();
    public List<Transform> _snappedObjectsList = new List<Transform>();

    public float lastDistance = 0f;
    public float scalingFactor = 100.0f;
    public float scalingMinThreshold = 0.1f;
    public float scalingMaxThreshold = 1.25f; //This is set according to max value needed for each model. So, it doesnt crash the app

    public float snappingRangeThreshold = 0.5f;
    public float scalingMinDistanceThreshold = 0.25f;
    public float angleDiffThreshold = 15.0f;
    public float snapDiffThreshold = 0.01f;
    //for beam between objects
    public Material beamMatColor;
    #endregion

    #region PRIVATE_VARIABLES
    //Saving all objects transform data as a string into dictionary in order to check distance and  get rotation and global position.
    Dictionary<string, Dictionary<string, string>> allObjectsTransformData = new Dictionary<string, Dictionary<string, string>>();

    private string left_To_Right_refData = "";
    private string right_To_Left_refData = "";
    private ObjectsDataClass _rightObjData;
    private ObjectsDataClass _leftObjData;

    //for beam between objects
    private float sizeOfLine = 0.05f;
    private Color _farColor = new Color(1, 0, 0, 1.0f);
    private Color _closeColor = new Color(0, 1, 0, 1.0f);
    private Color _idleBeamColor = new Color(0, 0, 1, 1.0f);
    private LineRenderer lines = null;
    private GameObject _beamObject = null;

    public bool _modelSplitted = false;
    #endregion


    void Awake()
    {
        _modelSplitted = false;
    }

    #region INITIALIZATION
    /// <summary>
    /// This method should be called after the SaveAllModelTransformDataIntoDicitonariesWithParents();
    /// Because it will use the updated list of _modelAllChildTransformObjects.
    /// isKinematic true in start of puzzle
    /// </summary>
    public void AddComponentsToChildrenWithMesh()
    {
        if (_modelAllChildTransformObjects.Count > 0)
        {
            for (int mIndex = 0; mIndex < _modelAllChildTransformObjects.Count; mIndex++)
            {
                GameObject obj = _modelAllChildTransformObjects[mIndex].gameObject;

                MeshFilter meshFilter = obj.GetComponent<MeshFilter>();
                if (meshFilter != null)
                {
                    obj.AddComponent<Rigidbody>();
                    //disable use of gravity
                    if (obj.GetComponent<Rigidbody>() != null)
                    {
                        obj.GetComponent<Rigidbody>().mass = 2.5f;
                        obj.GetComponent<Rigidbody>().isKinematic = true;
                        obj.GetComponent<Rigidbody>().useGravity = false;
                    }
                    obj.AddComponent<MeshCollider>().convex = false;
                    //adding script to make it interactable
                    obj.AddComponent<InteractableItemWithIsKinematic>();
                }
            }
        }
    }

    public void EnableDisableIsKinematic(Transform leftObj, Transform rightObj, bool isKinematic)
    {
        return;
        //disable use of gravity, and disable or enable isKinematic
        //When isKinematic is true there is no physics effect, no force; only isTrigger/Collision is called
        //if (leftObj != null)
        //        leftObj.GetComponent<Rigidbody>().isKinematic = isKinematic;

        //if(rightObj != null)
        //        rightObj.GetComponent<Rigidbody>().isKinematic = isKinematic;
    }

    public void EnableIsKinematicForAll()
    {
        return;
        //if (_modelAllChildTransformObjects.Count > 0)
        //{
        //    for (int mIndex = 0; mIndex < _modelAllChildTransformObjects.Count; mIndex++)
        //    {
        //        GameObject obj = _modelAllChildTransformObjects[mIndex].gameObject;
        //        obj.GetComponent<Rigidbody>().isKinematic = true;
        //        obj.GetComponent<Rigidbody>().useGravity = false;
        //    }
        //}
    }
    #endregion

    #region SAVING_ALL_TRANSFORM_DATA
    // Modified method for parent and subparent information.
    public void SaveAllModelTransformDataIntoDicitonariesWithParents()
    {
        Dictionary<string, string> parentChildRelation = new Dictionary<string, string>();
        _modelAllChildTransformObjects = GetAllFirstLevelChilds(this.transform);

        //1.  Pick all first child object and iterate over to check if they have further hierarchy, save name of parent object
        int firstObjInd = 0;
        while (firstObjInd < _modelAllChildTransformObjects.Count)
        {
            Transform selectedObj = _modelAllChildTransformObjects[firstObjInd];
            //2. If its a subhierarchy, get all childs for this object and add into final List of Transform
            if (selectedObj.childCount > 0)
            {
                // Debug.Log("Yes childs for: " + selectedObj.name);
                List<Transform> selectedObjchilds = GetAllFirstLevelChilds(selectedObj);

                for (int c = 0; c < selectedObjchilds.Count; c++)
                {
                    //adding childs of the selected object into the main List of Transform
                    _modelAllChildTransformObjects.Add(selectedObjchilds[c]);
                    // <name of selected object child>,<selected object as its a sub-hierarchy>
                    if (!parentChildRelation.ContainsKey(selectedObjchilds[c].name))
                    {
                        parentChildRelation.Add(selectedObjchilds[c].name, selectedObj.name);
                    }
                }
                //3. Removing the sub parent selected object as its not a mesh, its suppose to be a empty gameobject with transform set to zero. Only helpful in grouping
                _modelAllChildTransformObjects.Remove(selectedObj);
                firstObjInd = 0;
            }
            else // 4. IF the selected object doesnt have any child so we move to next one in list
            {
                firstObjInd++;
                // <name of selected object>,<its parent which is root in this case>
                if (!parentChildRelation.ContainsKey(selectedObj.name))
                {
                    parentChildRelation.Add(selectedObj.name, this.transform.name);
                }
            }
        }
        //5. Nested loops: Iterate over all the given objects in model and setup the dictionaries.
        for (int j = 0; j < _modelAllChildTransformObjects.Count; j++)
        {
            Dictionary<string, string> selObjData = new Dictionary<string, string>();
            Transform selectedObj = _modelAllChildTransformObjects[j];
            string parentName = "";

            if (parentChildRelation.ContainsKey(selectedObj.name))
                parentName = parentChildRelation[selectedObj.name];

            SaveDataOfSelectedObjectInDict(parentName, selectedObj, _modelAllChildTransformObjects, selObjData);
        }
    }

    void SaveDataOfSelectedObjectInDict(string parentName, Transform selectedObj, List<Transform> listOfModelObjects, Dictionary<string, string> selObjData)
    {
        for (int j = 0; j < listOfModelObjects.Count; j++)
        {
            Transform OtherObj = listOfModelObjects[j];
            if (!OtherObj.name.Equals(this.transform.name) && !OtherObj.name.Equals(selectedObj.name))
            {
                if (!selObjData.ContainsKey(OtherObj.name))
                {
                    //Save a string with following information with concatenation,
                    //string temp = "<parent name  of selected object> # <other object position> # <distance to the other object> # <orientation to the selected object> # <other object forward> # <angle between objects>"

                    // Calculate euclidean distance of obj1 with all other objects
                    float distanceToOtherObj = Vector3.Distance(selectedObj.transform.position, OtherObj.transform.position);
                   // float distanceToOtherObj = Vector3.Distance(selectedObj.transform.localPosition, OtherObj.transform.localPosition);
                    distanceToOtherObj = (float)Math.Round(distanceToOtherObj * 100f) / 100f;

                    //Calculating correct angle between two objects
                    Vector3 targetDir = selectedObj.transform.position - OtherObj.transform.position;
                    Vector3 forward = OtherObj.transform.forward;
                    float angleBtwObjects = Vector3.Angle(targetDir, forward);

                    Color orgColor = new Color(0, 0, 0, 1);
                    if (selectedObj.GetComponent<Renderer>() != null)
                        orgColor = selectedObj.GetComponent<Renderer>().material.color;
                    //save selected object color
                    string selObjColor = orgColor.r.ToString() + "," + orgColor.g.ToString() + "," + orgColor.b.ToString() + "," + orgColor.a.ToString();

                    /*//saving data of other object in Own sub dictionary??? doesnt make sense...save data of selected object its position and rotation
                    string otherObjPos = OtherObj.transform.position.x.ToString() + "," + OtherObj.transform.position.y.ToString() + "," + OtherObj.transform.position.z.ToString();
                    string otherObjRot = OtherObj.transform.rotation.x.ToString() + "," + OtherObj.transform.rotation.y.ToString() + "," + OtherObj.transform.rotation.z.ToString() + "," + OtherObj.transform.rotation.w.ToString();
                    string otherObjFwd = OtherObj.transform.forward.x.ToString() + "," + OtherObj.transform.forward.y.ToString() + "," + OtherObj.transform.forward.z.ToString();
                     string objectLocalData = parentName + "#" + otherObjPos + "#" + distanceToOtherObj.ToString() + "#" + otherObjRot + "#" + otherObjFwd + "#" + angleBtwObjects.ToString() + "#" + selObjColor;
                    */

                    //save data of selected object in relation to other object distance and relative angle
                    string selObjPos = selectedObj.transform.position.x.ToString() + "," + selectedObj.transform.position.y.ToString() + "," + selectedObj.transform.position.z.ToString();
                    string selObjRot = selectedObj.transform.rotation.x.ToString() + "," + selectedObj.transform.rotation.y.ToString() + "," + selectedObj.transform.rotation.z.ToString() + "," + selectedObj.transform.rotation.w.ToString();
                    string selObjFwd = selectedObj.transform.forward.x.ToString() + "," + selectedObj.transform.forward.y.ToString() + "," + selectedObj.transform.forward.z.ToString();

                    //changed the format/sequence of data 
                    string objectLocalData = parentName + "#" + selObjPos + "#" + selObjRot + "#" + selObjFwd + "#" + selObjColor + "#" + distanceToOtherObj.ToString() + "#" + angleBtwObjects.ToString();

                    //This dicitonary only saves a string containing all information of an object relative to the selected object
                    selObjData.Add(OtherObj.name, objectLocalData);
                }
            }
        }
        if (!allObjectsTransformData.ContainsKey(selectedObj.name))
        {
            //5. Saving dictionary of eachobject data into the main dictionary
            allObjectsTransformData.Add(selectedObj.name, selObjData);
        }
    }

    List<Transform> GetAllFirstLevelChilds(Transform rootParent)
    {
        List<Transform> allFirstChilds = new List<Transform>();

        foreach (Transform child in rootParent)
        {
            allFirstChilds.Add(child);
        }
        return allFirstChilds;
    }


    public void PrintTransformDataFromDicitonaries()
    {
        foreach (KeyValuePair<string, Dictionary<string, string>> allData in allObjectsTransformData)
        {
            //Access the key and value both separately
            Dictionary<string, string> childDictData = (Dictionary<string, string>)allData.Value;
            foreach (KeyValuePair<string, string> childData in childDictData)
            {
                Debug.Log("SelectedObject:  " + allData.Key + " RefObjName:" + childData.Key + " LocalObjData: " + childData.Value);
            }
        }
    }

    // selected obj Name and reference obj name
    public string GetDataForObjects(string objName, string refObjName)
    {
        //Debug.Log("ObjName:" + objName + " refObjData: " + refObjName);
        if (allObjectsTransformData.ContainsKey(objName))
        {
            Dictionary<string, string> objData = allObjectsTransformData[objName];
            if (objData.ContainsKey(refObjName)) // get the reference object to which the data is required
            {
              // Debug.Log("ObjName:" + objName + " relative to refObjData: " + refObjName + " ObjData: " + objData[refObjName]);
                return objData[refObjName];
            }
        }
        return "";
    }
    #endregion

    #region UI_CONTROLS

    private void ChangeModelGroupingStatus(Transform _parent)
    {
        if (_modelAllChildTransformObjects.Count > 0)
        {
            for (int mIndex = 0; mIndex < _modelAllChildTransformObjects.Count; mIndex++)
            {
                GameObject obj = _modelAllChildTransformObjects[mIndex].gameObject;

                //if the object also exists in snapped objects list then don't make this obj parent null.
                if (_snappedObjectsList.Count > 0)
                {
                    if (_snappedObjectsList.Contains(obj.transform))
                    {
                        if(obj.transform.parent!=null && obj.transform.parent.tag.Equals("Snapped"))
                            obj.transform.parent.SetParent(_parent);
                    }else
                        obj.transform.SetParent(_parent);
                }
                else
                    obj.transform.SetParent(_parent);
            }
        }
    }

    public void UnGroupModel()
    {
        ChangeModelGroupingStatus(null);
    }

    public void GroupModel()
    {
        ChangeModelGroupingStatus(this.transform);
    }

    public void SplitModelObjects()
    {
        if (_modelSplitted) //only split model once
            return;

        if (_modelAllChildTransformObjects.Count > 0)
        {
            for (int mIndex = 0; mIndex < _modelAllChildTransformObjects.Count; mIndex++)
            {
                GameObject obj = _modelAllChildTransformObjects[mIndex].gameObject;
                float leftWall_X = -1.0f;
                float rightWall_X = 1.0f;
                float frontWall_Z = 1.0f;
                float backWall_Z = -1.0f;
               
                //Ungrouping model here
                obj.transform.SetParent(null);  //set parent to null first
                Vector3 randPosition = new Vector3(UnityEngine.Random.Range(leftWall_X, rightWall_X),
                                        UnityEngine.Random.Range(0.5f, 1.5f),
                                        UnityEngine.Random.Range(frontWall_Z, backWall_Z));

                Vector3 randRotation = UnityEngine.Random.rotation.eulerAngles;

                //animate object to the new UnityEngine.Random position slowly using iTween
                iTween.RotateTo(obj, iTween.Hash("rotation", randRotation, "easeType", "easeInOutSine", "delay", 0.1f * mIndex));
                iTween.MoveTo(obj, iTween.Hash("position", randPosition, "easeType", "linear", "delay", 0.1f * mIndex));
            }
        }
        _modelSplitted = true;
    }

    //As suggested by Anatomist, explosion visualization keep distance from center same and only update position
    public void ExplosionVisualizatonOfModel()
    {
        if (_modelAllChildTransformObjects.Count > 0)
        {
            for (int mIndex = 0; mIndex < _modelAllChildTransformObjects.Count; mIndex++)
            {
                GameObject obj = _modelAllChildTransformObjects[mIndex].gameObject;
                float leftWall_X = -1.0f;
                float rightWall_X = 1.0f;
                float frontWall_Z = 1.0f;
                float backWall_Z = -1.0f;

                obj.transform.SetParent(null);  //set parent to null first
                Vector3 randPosition = new Vector3(UnityEngine.Random.Range(leftWall_X, rightWall_X),
                                        UnityEngine.Random.Range(0.5f, 1.5f),
                                        UnityEngine.Random.Range(frontWall_Z, backWall_Z));
                iTween.MoveTo(obj, iTween.Hash("position", randPosition, "easeType", "linear", "delay", 0.1f * mIndex));
            }
        }
    }

    public int GetCounterForTotalObjects()
    {
        if (_modelAllChildTransformObjects!=null && _modelAllChildTransformObjects.Count > 0)
        {
            return _modelAllChildTransformObjects.Count;
        }
        return 0;
    }
    #endregion

    #region SNAPPING
    /// <summary>
    ///If the distance between two objects is less than a threshold and is the right docking position..SNAP
    /// </summary>
    //
   public void SaveObjectsDataHandles(Transform leftHandObject, Transform rightHandObject)
   {
        if (leftHandObject != null && rightHandObject != null)
        {
             _rightObjData = new ObjectsDataClass(GetDataForObjects(rightHandObject.name, leftHandObject.name), rightHandObject.name);
            _leftObjData = new ObjectsDataClass(GetDataForObjects(leftHandObject.name, rightHandObject.name), leftHandObject.name);
        }
    }
    public void SingleObjectHandle(Transform obj, string handleName)
    {
        if (obj != null)
        {
            if(handleName.Equals("Left"))
                obj.GetComponent<Renderer>().material.color = _leftObjData.matColor;
            else if (handleName.Equals("Right"))
                obj.GetComponent<Renderer>().material.color = _rightObjData.matColor;
        }
    }


    public bool CheckIFObjectsAreCloser(Transform leftHandObject, Transform rightHandObject)
    {
        //Check if both objects are dragged closer to eachother
         if (leftHandObject != null && rightHandObject != null)
        {
            float distOfObjects = Vector3.Distance(leftHandObject.position, rightHandObject.position);
            float calculatedAngle = CalculateAngleBetweenTwoObjects(leftHandObject, rightHandObject);
            float angleDiff = calculatedAngle - _leftObjData.angleBtwObjects;
            float newRelativeDist = _leftObjData.relativeDistance * this.transform.localScale.x; // x/y/z as all are scaled proportionally
            //1. Check if they are both in range
            if (distOfObjects >= 0.0 && distOfObjects < newRelativeDist)
            {
                //old approach, doesnt work in certain cases
                /*calculatedAngle = CalculateAngleBetweenTwoObjects(leftHandObject, rightHandObject);
                angleDiff = _leftObjData.angleBtwObjects - calculatedAngle;

                float opacity = (Mathf.Abs(angleDiff) * 100.0f)/(180.0f - calculatedAngle);
                opacity = 1.0f -(opacity / 255.0f);*/

                /*Example:
				correctAngle: 90
				calculatedAngle: 115
				Diff: 25
				maxAngle = 180
					
				range = max - min (180-0)
				opacity = 1 - (25/180)
				opacity = 0.86*/

                //new approach, works well

                //Using dot product to determine direction
                float direction = CalculateDotProduct(leftHandObject, rightHandObject);

                calculatedAngle = CalculateAngleBetweenTwoObjects(leftHandObject, rightHandObject);
				angleDiff = _leftObjData.angleBtwObjects - calculatedAngle;
				float opacity = 1.0f - (Mathf.Abs(angleDiff)/180.0f);

               //GameController.UpdateSnapInfoUI("Objects Closed - angle difference: " + Mathf.Abs(angleDiff) +  " opacity: "+ opacity, leftHandObject.name, rightHandObject.name);

                if (opacity > 0.001f)
                {
                    UpdateLinePosition(leftHandObject.transform.position, rightHandObject.transform.position, new Color(_closeColor.r, _closeColor.g, _closeColor.b, opacity));
                }
                else //opacity value goes negative
                {
                    UpdateLinePosition(leftHandObject.transform.position, rightHandObject.transform.position, _farColor);
                }

                GameController.UpdateSnapInfoUI("Objects Closed: " + newRelativeDist + "  org dist: " + distOfObjects + "  with angle: " + calculatedAngle + 
                    " correct angle  " + _leftObjData.angleBtwObjects + "  opacity: " + opacity + " Direction: " + direction + "  ", leftHandObject.name, rightHandObject.name);

                // 2. Check if the rotation angle is closer to the relative angle, check for any of the object either left or right
                if ((angleDiff >= -angleDiffThreshold && angleDiff <= angleDiffThreshold))
                {
                    UpdateLinePosition(leftHandObject.transform.position, rightHandObject.transform.position, new Color(_closeColor.r, _closeColor.g, _closeColor.b, opacity));
                    return true;
                }
            } else if (distOfObjects > newRelativeDist){
                 UpdateLinePosition(leftHandObject.transform.position, rightHandObject.transform.position,_farColor);
                return false;
            }
        }
        return false;
    }

	#region ANGLE_CALCULATION
	//Dot product of normalized vectors
	private float CalculateDotProduct(Transform obj1, Transform obj2)
	{
		//left obj to right obj
		Vector3 obj1Fwd = transform.TransformDirection(Vector3.forward);
		Vector3 obj2Dir = obj2.transform.position - obj1.transform.position;
		return Vector3.Dot(obj1Fwd.normalized, obj2Dir.normalized);
	}

	private float CalculateAngleUsingDotProduct(Transform obj1, Transform obj2)
	{
		//Get the dot product
		float dot = Vector3.Dot(obj1.transform.position,obj2.transform.position);
		// Divide the dot by the product of the magnitudes of the vectors
		dot = dot/(obj1.transform.position.magnitude*obj2.transform.position.magnitude);
		//Get the arc cosin of the angle, you now have your angle in radians 
		float acos = Mathf.Acos(dot);
		//Multiply by 180/Mathf.PI to convert to degrees
		return acos*180/Mathf.PI;
	}
	//calculating with Mathf.Atan2 method, returns in radians
	private float AngleInRad(Transform obj1, Transform obj2) {
		return Mathf.Atan2(obj2.transform.position.y - obj1.transform.position.y, obj2.transform.position.x - obj1.transform.position.x);
	}
	// returning in degrees
	private float AngleInDeg(Transform obj1, Transform obj2) {
		return AngleInRad(obj1, obj2) * 180 / Mathf.PI;
	}

	//Using Vector3.Angle 
    private float CalculateAngleBetweenTwoObjects(Transform obj1, Transform obj2)
    {
        Vector3 targetDir = obj2.position - obj1.position;
        Vector3 forward = obj1.forward;
        return Vector3.Angle(targetDir, forward);
    }

	#endregion

    public void WhenCloserSnapObjects(Transform leftObj, Transform rightObj)
    {
        //Check if both objects are dragged closer to eachother
        if (leftObj != null && rightObj != null)
        {
            float distOfObjects = Vector3.Distance(leftObj.transform.position, rightObj.transform.position);

            distOfObjects = (float)Math.Round(distOfObjects * 100f) / 100f;
            //multiply distance into new scale of model parent object
            float newRelativeDist = _leftObjData.relativeDistance * this.transform.localScale.x; // x/y/z as all are scaled proportionally
            if (distOfObjects >= 0.0 && distOfObjects < newRelativeDist)
            {
               Debug.Log("SnapAnimation called at dist :" + distOfObjects);
                StartCoroutine(SnapObjectsToCorrectPoint(leftObj, rightObj));
            }
            else
            {
                StopAllCoroutines();
            }
        }
    }

	/*
	Method to find a position based on distance between objects and angle
	*/
	private Vector3 GetPositionBasedOnAngleAndDistance(Vector3 objPos, float distanceToObj, float angleToObj)
	{
        //do some magic here to find position
        /*
		X=distance*cos(angle) +x0
		Y=distance*sin(angle) +y0
		Z=distance*tan(angle) +z0
		*/

        Vector3 newPos = Vector3.zero;

        //This works to find new 2d position, except Z pos causing issue
          float angleInRad = angleToObj* Mathf.Deg2Rad;
          newPos.x = distanceToObj * Mathf.Cos(angleInRad) + objPos.x;
          newPos.y = distanceToObj * Mathf.Sin(angleInRad) + objPos.y;

        // float z = distanceToObj * Mathf.Tan(angleInRad) + objPos.z;

        //Works well for non-rotated objects
        //Quaternion tempQuat = Quaternion.AngleAxis(angleToObj, Vector3.forward);
        //newPos = objPos + tempQuat * Vector3.right * distanceToObj;


        //OR
        //This deosnt give accurate value
        //newPos.x = 1 * Mathf.Cos(angleToObj * Mathf.PI / 180) * distanceToObj;
        //newPos.z = 1 * Mathf.Sin(angleToObj * Mathf.PI / 180) * distanceToObj;

        return newPos;
	}

    IEnumerator SnapObjectsToCorrectPoint(Transform leftObj, Transform rightObj)
    {
        float rotSpeed = 50.0f;
        float moveSpeed = 1.0f;
        Vector3 tempRightObjPoss = rightObj.transform.localPosition;

        float distOfObjects = Vector3.Distance(leftObj.position, rightObj.position);
        distOfObjects = (float)Math.Round(distOfObjects * 100f) / 100f;
        
        //multiply distance into new scale of model parent object
        float newRelativeDist = _leftObjData.relativeDistance * this.transform.localScale.x; // x/y/z as all are scaled proportionally

        #region TESTING_POSITION
        //Debug.Log("Org Position of objects in space - Left:" + leftObj.transform.localPosition + " Right: " + rightObj.transform.localPosition);

        //Vector3 newPosForRightObj = GetPositionBasedOnAngleAndDistance(leftObj.transform.localPosition, newRelativeDist, _leftObjData.angleBtwObjects);
        //Debug.Log("NewPositionForRightObj: " + newPosForRightObj);


        //newPosForRightObj.y = tempRightObjPoss.y;
        // newPosForRightObj.z = tempRightObjPoss.z;

        //moving and rotating right object to new calculated position with original rotation
        /*    while (!rightObj.transform.rotation.Equals(_rightObjData.rotation))
            {
                rightObj.transform.rotation = Quaternion.RotateTowards(rightObj.transform.rotation, _rightObjData.rotation, Time.deltaTime * rotSpeed);

                if (!rightObj.transform.localPosition.Equals(newPosForRightObj))
                {
                    rightObj.transform.localPosition = Vector3.MoveTowards(rightObj.transform.localPosition, newPosForRightObj, Time.deltaTime * moveSpeed);
                }
                    yield return new WaitForSeconds(0.01f);
            }*/

        //Only moving to new position over time
       /* while (!rightObj.transform.position.Equals(newPosForRightObj))
        {
            rightObj.transform.position = Vector3.MoveTowards(rightObj.transform.position, newPosForRightObj, Time.deltaTime * moveSpeed);
            yield return new WaitForSeconds(0.01f);
        }*/

        #endregion

        //Move both objects
        while (distOfObjects != newRelativeDist)
         {
             distOfObjects = Vector3.Distance(leftObj.position, rightObj.position);
             distOfObjects = (float)Math.Round(distOfObjects * 100f) / 100f;
             float calculatedAngle = CalculateAngleBetweenTwoObjects(leftObj, rightObj);

			/*float dotProduct = CalculateDotProduct (leftObj,rightObj);

			if (dotProduct < 0) {
				Debug.Log ("Left and right object are opposite");
			}
			if (dotProduct > 0) {
				Debug.Log ("Left and right object facing eachother");
			}
            */
             //GameController.UpdateSnapInfoUI("Moving from: " + newRelativeDist + " to dist: " + distOfObjects + " angle: " + calculatedAngle +"  ", leftObj.name, rightObj.name);

             //rotate left object
             if (!leftObj.transform.rotation.Equals(_leftObjData.rotation))
             {
                 leftObj.transform.rotation = Quaternion.RotateTowards(leftObj.transform.rotation, _leftObjData.rotation, Time.deltaTime * rotSpeed);
             }

             //rotate right object as well
             if (!rightObj.transform.rotation.Equals(_rightObjData.rotation))
             {
                 rightObj.transform.rotation = Quaternion.RotateTowards(rightObj.transform.rotation, _rightObjData.rotation, Time.deltaTime* rotSpeed);
             }

             //TODO: Rotate right obj only, doesn't work well 
             //  if(Mathf.Abs(calculatedAngle) != _rightObjData.angleBtwObjects)
             //{
             //    rightObj.transform.rotation = Quaternion.RotateTowards(rightObj.transform.rotation, _rightObjData.rotation, Time.deltaTime * rotSpeed);
             //}

         //move closer if needed
         if (distOfObjects > newRelativeDist)
             { 
                 //leftObj.transform.position = Vector3.MoveTowards(leftObj.transform.position, rightObj.transform.position, Time.deltaTime* moveSpeed);
                 rightObj.transform.position = Vector3.MoveTowards(rightObj.transform.position, leftObj.transform.position, Time.deltaTime* moveSpeed);
             }
             //move away to dock to correct position
             if (distOfObjects < newRelativeDist)
             {
                leftObj.transform.position = Vector3.MoveTowards(leftObj.transform.position, rightObj.transform.position, -Time.deltaTime* moveSpeed);
                 rightObj.transform.position = Vector3.MoveTowards(rightObj.transform.position, leftObj.transform.position, -Time.deltaTime* moveSpeed);
             }

             float diffOfDistance = distOfObjects - newRelativeDist;
             if(diffOfDistance>= -snapDiffThreshold && diffOfDistance <= snapDiffThreshold)
             {
                     leftObj.transform.rotation = _leftObjData.rotation;
                     rightObj.transform.rotation = _rightObjData.rotation;
                     //GameController.UpdateSnapInfoUI("Docked with rot: " + newRelativeDist + " to dist: " + distOfObjects + " angle: " + calculatedAngle + "  ", leftObj.name, rightObj.name);
                     GameController.UpdateSnapInfoUI("Reached to the dock point with rotation ", leftObj.name, rightObj.name);
                     StopAllCoroutines();
                }

             yield return new WaitForSeconds(0.01f);
         }
    }

    private void PlaySnappingSFX()
    {

    }

    public void SetObjectSnapStatus(InteractableItemWithIsKinematic obj, bool status)
    {
        if(obj!=null)
             obj.SnapStatus(status);
    }

    public bool GetStatusOfObject(Transform obj)
    {
        if(obj!=null)
             return obj.GetComponent<InteractableItemWithIsKinematic>().GetSnapStatus();

        return false;
    }

    public void SetAsChildOfParent(Transform leftObj, Transform rightObj)
    {
        //set as child of model
        leftObj.transform.parent = GameObject.FindGameObjectWithTag("Snapped").transform;
        rightObj.transform.parent = GameObject.FindGameObjectWithTag("Snapped").transform;

        if (_snappedObjectsList.Count == 0)
        {
            _snappedObjectsList.Add(leftObj);
            _snappedObjectsList.Add(rightObj);
        }else
        {
            if (!_snappedObjectsList.Contains(leftObj) && !leftObj.tag.Equals("Snapped"))
            {
                _snappedObjectsList.Add(leftObj);
            }
            if (!_snappedObjectsList.Contains(rightObj) && !rightObj.tag.Equals("Snapped"))
            {
                _snappedObjectsList.Add(rightObj);
            }
        }
    }
    //Check if objects are snapped, then transform the parent of those two objects

    #endregion

    #region BEAM_BTW_OBJECTS

    public void CreateABeamObject()
    {
        if (_beamObject == null)
            _beamObject = new GameObject("Beam");
    
    }

    //Call this function when two objects are grabbed
    public void DrawABeamBetweenObjects(Transform obj1, Transform obj2)
    {
        if (_beamObject == null)
            CreateABeamObject();
        CreateLine(_beamObject, obj1.transform.position, obj2.transform.position, sizeOfLine, _farColor);
    }

    public void DeleteLineRendererFromBeamObjects()
    {
        if (_beamObject != null)
        {
            Destroy(_beamObject.GetComponent<LineRenderer>());
        }
    }
    private void CheckBeamBasedOnObjDistance(float dist, float relDistance, Vector3 obj1Pos, Vector3 obj2Pos)
    {
        //if going far
        if (dist > 0 && dist > relDistance)
        {
            UpdateLinePosition(obj1Pos, obj2Pos, _farColor);
        }
        //if getting closer
       else if (dist>0 && dist <= relDistance)
        {
            UpdateLinePosition(obj1Pos, obj2Pos, _closeColor);
        }
        else
            UpdateLinePosition(obj1Pos, obj2Pos, new Color(_closeColor.r, _closeColor.g, _closeColor.b, lines.material.color.a + 0.01f));
    }

    private void CreateLine(GameObject _beamObject, Vector3 start, Vector3 end, float lineSize, Color c)
    {
        if (lines == null)
        {
            lines = (LineRenderer)_beamObject.AddComponent<LineRenderer>();
            lines.material = beamMatColor;
            lines.material.color = c;
            lines.useWorldSpace = false;
            lines.SetWidth(lineSize, lineSize);
            lines.SetVertexCount(2);
            lines.SetPosition(0, start);
            lines.SetPosition(1, end);
        }
    }
    private void UpdateLinePosition(Vector3 start, Vector3 end, Color col)
    {
        if (lines != null)
        {
            lines.material.color = col;
            lines.SetPosition(0, start);
            lines.SetPosition(1, end);
        }
    }

    private void UpdateMaterialColor(Color col)
    {
        if (lines != null)
        {
            lines.material.color = col;
        }
    }


    #endregion

    #region SCALING_UP/DOWN
    /// <summary>
    /// This method handles both scaling up and down feature when two wand controllers are moved in VE
    ///while grip button is pressed on both controllers
    ///The distance is calculated every frame and is compared with the thresholds
    /// </summary>
    /// <param name="leftHand"></param>
    /// <param name="rightHand"></param>
    public void ScaleModel(Vector3 leftHand, Vector3 rightHand)
    {
        float orgDistance = Vector3.Distance(leftHand, rightHand);
        float distOfObjects = (float)Math.Round(orgDistance * 100f) / 100f;
        //Debug.Log("Distance:" + distOfObjects);

        if (distOfObjects > scalingMinDistanceThreshold)
        {
            //The positive relativeScale is used for scaling UP
            float relativeScale = distOfObjects / scalingFactor;
            //This will invert the value of relativeScale if the distance between objects is getting smaller; means scaling DOWN
            if (distOfObjects < lastDistance)
            {
                //If the localScale has reached min threshold.
                if (this.transform.localScale.x >= scalingMinThreshold || this.transform.localScale.y >= scalingMinThreshold || this.transform.localScale.z >= scalingMinThreshold)
                {
                    this.transform.localScale = new Vector3(this.transform.localScale.x + (relativeScale * -1),
                     this.transform.localScale.y + (relativeScale * -1),
                     this.transform.localScale.z + (relativeScale * -1));
                    //Debug.Log("ScaleMode down");
                }
            }
            else if (distOfObjects > lastDistance)
            {
                //If the localScale has reached max threshold.
                if (this.transform.localScale.x <= scalingMaxThreshold || this.transform.localScale.y <= scalingMaxThreshold || this.transform.localScale.z <= scalingMaxThreshold)
                {
                    this.transform.localScale = new Vector3(this.transform.localScale.x + relativeScale,
                    this.transform.localScale.y + relativeScale,
                    this.transform.localScale.z + relativeScale);
                    //Debug.Log("ScaleMode up");
                }
            }
            //set lastDistance to the new calculated distance
            lastDistance = distOfObjects;
        }
    }
    #endregion

    #region TESTING_METHODS

    void LogDepth(Transform root, int depth)
    {
        Transform[] children = root.GetComponentsInChildren<Transform>();
        foreach (Transform c in children)
        {
            if (c.parent == root)
            {
                // Debug.Log(c.name + ": " + (depth + 1));
                LogDepth(c, depth + 1);
            }
        }
    }

    void PrintFirstLevelHierarchyObjects(Transform obj)
    {
        //testing first level objects only
        foreach (Transform child in obj)
        {
            Debug.Log(child.name);
        }
    }
    #endregion
}