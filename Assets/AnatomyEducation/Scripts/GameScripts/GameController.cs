/**
*Developer: Asema Hassan SSoe16
*Anatomy Education VR Puzzle game

 *GameController;
 * Controls every call during gameplay.
 * It  also handle animations, particle effects, UI messages, training and voice overs
 *Scaling wand controller; call
*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
	#region PRIVATE_VARIABLES

	private WandControlForItems wandController_Left = null;
	private WandControlForItems wandController_Right = null;
	private GameObject modelObj = null;
	private bool _objectsAreInRange = false;

	#endregion

	#region PUBLIC_VARIABLES

	//Set these two variables from inspector,otherwise scaling won't work
	public GameObject leftWand = null;
	public GameObject rightWand = null;
	// public bool isGrouped = false;
	public bool hasSnapped = false;

	public GameObject snapUICanvas = null;
	public static Text snapUIText = null;
	public static Text snapProgress = null;
	public bool hasReadObjData = false;

	public static int totalNumberOfObjects = 0;
	public static ModelController mainModelController = null;
	public List<GameObject> listOfAllModels = new List<GameObject> ();

	public InteractableItemWithIsKinematic _refObjectLeft = null;
	public InteractableItemWithIsKinematic _refObjectRight = null;

	public static bool _isScaling = false;

	#endregion

	#region INITIALIZATION

	void Start ()
	{
		//the model name is selected from MainMenu
		EnableTheSelectedModel ();

		if (leftWand == null || rightWand == null)
			Debug.LogError ("Please assign from inspector, Controller (left) and Controller (right) to the leftWand and rightWand respectively");


		if ((leftWand != null && leftWand.activeSelf) && (rightWand != null && rightWand.activeSelf)) {
			wandController_Left = leftWand.GetComponent<WandControlForItems> ();
			wandController_Right = rightWand.GetComponent<WandControlForItems> ();
		}

		//Call all methods to setup selected model
		SaveAllModelData (); // 1
		AddComponentsToAllModelObjects (); // 2
	}

	private void EnableTheSelectedModel ()
	{
		//As their is no other model in the scene force to only use Human_Bot
		string currentModel = "Human_Bot"; //PlayerPrefs.GetString ("CurrentModelName");
		Debug.Log (currentModel);
		if (listOfAllModels != null && listOfAllModels.Count > 0) {
			if (currentModel.Equals ("Human_Bot")) {
				modelObj = listOfAllModels [0] as GameObject;
			}
//            else if (currentModel.Equals("Skull_Model"))
//            {
//                modelObj = listOfAllModels[1] as GameObject;
//            }
//            else if (currentModel.Equals("Foot_Model"))
//            {
//                modelObj = listOfAllModels[2] as GameObject;
//            }
//            else if (currentModel.Equals("Knee_Model"))
//            {
//                modelObj = listOfAllModels[3] as GameObject;
//            }
		}

		if (modelObj == null) {
			Debug.LogError ("No! Model found in scene");
		} else {
			modelObj.SetActive (true);
			mainModelController = modelObj.GetComponent<ModelController> ();
		}

	}

	#endregion

	#region MODEL_SETUP_INITIALIZE

	void SaveAllModelData ()
	{
		if (mainModelController != null) {
			mainModelController.SaveAllModelTransformDataIntoDicitonariesWithParents ();
			mainModelController.PrintTransformDataFromDicitonaries ();
			totalNumberOfObjects = mainModelController.GetCounterForTotalObjects ();
			PlayerPrefs.SetInt ("ProgressCounter", 0);
			Debug.Log ("All data saved");
		}
	}

	/// <summary>
	/// This method will add components to all models objects which are interactable,
	/// BoxCollider, RigidBody as isKinematic, InteractableItemWithIsKinematic.cs
	/// </summary>
	void AddComponentsToAllModelObjects ()
	{
		if (mainModelController != null)
			mainModelController.AddComponentsToChildrenWithMesh ();
	}

	#endregion

	#region WAND_GESTURES_FOR_SCALE_SNAP

	void Update ()
	{
		#region INITIALIZATION
		if ((leftWand != null && leftWand.activeSelf) && (rightWand != null && rightWand.activeSelf)) {
			if (wandController_Left == null)
				wandController_Left = leftWand.GetComponent<WandControlForItems> ();
			if (wandController_Right == null)
				wandController_Right = rightWand.GetComponent<WandControlForItems> ();
		}
		#endregion

		#region ACTIVE_WANDS_CONTROLLER
		if ((leftWand != null && leftWand.activeSelf) && (rightWand != null && rightWand.activeSelf)) {
			#region SCALING
			//When  grip buttons are pressed together on controllers, scale up or down model
			if (wandController_Left.gripButtonPressed == true && wandController_Right.gripButtonPressed == true) {
				if (mainModelController != null) {
					//group model here
					_isScaling = true;
					mainModelController.GroupModel ();
					wandController_Left.ChangePointersStatus (false);
					wandController_Right.ChangePointersStatus (false);
					mainModelController.ScaleModel (wandController_Left.controllerPosition, wandController_Right.controllerPosition);
				}
			} else {
				if (mainModelController._modelSplitted) {
					mainModelController.UnGroupModel ();
				}
				wandController_Left.ChangePointersStatus (true);
				wandController_Right.ChangePointersStatus (true);
				_isScaling = false;
			}
			#endregion

			#region ONE_HAND_UNSNAP
			//one handed interaction while two objects are already snapped, unsnap them???
			/*   if (wandController_Left.grabbedObject == true && wandController_Left.interactingItem.GetSnapStatus() == true)
            {
                //mainModelController.SetObjectSnapStatus(wandController_Left.interactingItem, false);
                // mainModelController.SingleObjectHandle(wandController_Left.interactingItem.transform, "Left");
                // mainModelController.EnableDisableIsKinematic(wandController_Left.interactingItem.transform, null, false);
                //Debug.Log("for left hand isKinematic set to false");
            }
            if (wandController_Right.grabbedObject == true && wandController_Right.interactingItem.GetSnapStatus() == true)
            {
                //mainModelController.SetObjectSnapStatus(wandController_Right.interactingItem, false);
                //mainModelController.SingleObjectHandle(wandController_Right.interactingItem.transform, "Right");
                // mainModelController.EnableDisableIsKinematic(null, wandController_Right.interactingItem.transform, false);
                // Debug.Log("for right hand isKinematic set to false");
            }*/
			#endregion

			#region TWO_HAND_SNAP
			//When  both controllers hold some objects and those are dragged closely
			if (wandController_Left.grabbedObject == true && wandController_Right.grabbedObject == true) {
				#region CP1_NEW_SNAP
				if (wandController_Left.interactingItem.GetSnapStatus () == false || wandController_Right.interactingItem.GetSnapStatus () == false) {
					// if the picked object is not snapped
					if (mainModelController != null) {
						if (hasReadObjData == false) {
							mainModelController.SaveObjectsDataHandles (wandController_Left.interactingItem.transform, wandController_Right.interactingItem.transform);
							hasReadObjData = true;
						}
						//  mainModelController.EnableDisableIsKinematic(wandController_Left.interactingItem.transform, wandController_Right.interactingItem.transform, true);
						mainModelController.DrawABeamBetweenObjects (wandController_Left.interactingItem.transform, wandController_Right.interactingItem.transform);

						_objectsAreInRange = mainModelController.CheckIFObjectsAreCloser (wandController_Left.interactingItem.transform, wandController_Right.interactingItem.transform);
						if (_objectsAreInRange) {
							_refObjectLeft = wandController_Left.interactingItem;
							_refObjectRight = wandController_Right.interactingItem;
							//Debug.Log("To snap: " + _refObjectLeft.transform.name + " " + _refObjectRight.transform.name);
						} else if (!_objectsAreInRange) {
							_refObjectLeft = null;
							_refObjectRight = null;
						}
					}
				}
                #endregion
                
                #region CP2_ALREADY_SNAPPED
                //check if initially any object is snapped and is child of a empty parent with tag "Snapped"
                //update position of parent transform instead of child object
                //but compare the new object with the first picked snapped object for relative distance and orientation?
                else if (wandController_Left.interactingItem.GetSnapStatus () == true || wandController_Right.interactingItem.GetSnapStatus () == true) {
					if (wandController_Left.interactingItem.GetSnapStatus () == true && wandController_Left.interactingItem.transform.parent != null && wandController_Left.interactingItem.transform.parent.tag.Equals ("Snapped")) {
						//save reference of the touched object but transform the parent
						Debug.Log ("Left object is already snapped");
						_refObjectLeft = wandController_Left.interactingItem;
					}
					if (wandController_Right.interactingItem.GetSnapStatus () == true && wandController_Right.interactingItem.transform.parent != null && wandController_Right.interactingItem.transform.parent.tag.Equals ("Snapped")) {
						//save reference of the touched object but transform the parent
						Debug.Log ("Right object is already snapped");
						_refObjectRight = wandController_Right.interactingItem;
					}
					// Transform parent objects position and rotation
					// ReferenceObject relative distance with the other object in wand which is not snapped yet.
					// If all conditions are true then snap it too
				}
				#endregion
                
			}

            #region CP3_FREE_TO_SNAP
            else if (wandController_Left.grabbedObject == false && wandController_Right.grabbedObject == false) {
				//when both objects are set free to be snapped, execute only when 2 objects are in range
				if (_objectsAreInRange) {
					if (_refObjectRight != null && _refObjectLeft != null) {
						Debug.Log ("Snap objects; " + _refObjectLeft.transform.name + "  " + _refObjectRight.transform.name);
						mainModelController.WhenCloserSnapObjects (_refObjectLeft.transform, _refObjectRight.transform);
						mainModelController.SetObjectSnapStatus (_refObjectLeft, true);
						mainModelController.SetObjectSnapStatus (_refObjectRight, true);
						mainModelController.SetAsChildOfParent (_refObjectLeft.transform, _refObjectRight.transform);

						_refObjectLeft = null;
						_refObjectRight = null;
						hasReadObjData = false;
					}
					_refObjectLeft = null;
					_refObjectRight = null;
					hasReadObjData = false;
					_objectsAreInRange = false;
				} else {
					//there exist any line renderer on beam object delete it
					if (mainModelController != null) {
						mainModelController.DeleteLineRendererFromBeamObjects ();
						hasReadObjData = false;
						_refObjectLeft = null;
						_refObjectRight = null;
						_objectsAreInRange = false;
					}
				}
			}
			#endregion
			#endregion
		}
		#endregion
	}

	#endregion

	#region UI_CONTROLS

	/* public void ControlGroupingOfModel()
    {
        Text groupCtrl = GameObject.Find("GroupControl").transform.FindChild("groupText").GetComponent<Text>();

        if (isGrouped)
        {
            if (mainModelController != null)
                mainModelController.UnGroupModel();
            isGrouped = false;
            groupCtrl.text = "Group Model";
            ShowLogMessagesUI("Model is ungrouped");
        }
        else
        {
            if (mainModelController != null)
                mainModelController.GroupModel();
            isGrouped = true;
            groupCtrl.text = "UnGroup Model";
            ShowLogMessagesUI("Model is grouped");
        }
    }*/

	//public static void UnGroupAllModel()
	//{
	//    if (mainModelController != null)
	//        mainModelController.UnGroupModel();
	//}

	public static void UpdateSnapInfoUI (string msg, string leftObjName, string rightObjName)
	{
		if (snapUIText == null) {
			snapUIText = GameObject.Find ("ObjectSnapUI").transform.FindChild ("SnapInfo").GetComponent<Text> ();
		}
		snapUIText.text = msg + leftObjName + " with " + rightObjName;
	}

	public static void UpdateProgressUI (int count)
	{
		//return;
		int progress = PlayerPrefs.GetInt ("ProgressCounter");
		progress += count;
		PlayerPrefs.SetInt ("ProgressCounter", progress);
		if (snapProgress == null) {
			snapProgress = GameObject.Find ("ObjectSnapUI").transform.FindChild ("Progress").GetComponent<Text> ();
		}

		float totalProgress = (100.0f / totalNumberOfObjects) * progress;
		snapProgress.text = "Progress: " + totalProgress + " %";
	}

	public void StartModelPuzzle ()
	{
		Button puzzleStartBtn = GameObject.Find ("StartPuzzle").GetComponent<Button> ();

		if (puzzleStartBtn.enabled) {
			//then explode/split all model gameobjects into random positions and rotations
			if (mainModelController != null) {
				mainModelController.SplitModelObjects ();
				//mainModelController.ExplosionVisualizatonOfModel(); //TODO: Future task
				//Create beam object once the puzzle has initiated
				mainModelController.CreateABeamObject ();
				Debug.Log ("Split model");
			}
			ColorBlock colors = puzzleStartBtn.colors;
			colors.normalColor = Color.black; // shows that this button is disable
			puzzleStartBtn.colors = colors;
			puzzleStartBtn.enabled = false;
		}

		//Show log Msg
		ShowLogMessagesUI ("Model is split; ready to be puzzled");
	}

	public void Restart ()
	{
		SceneManager.LoadScene ("PuzzleVRGame");
	}

	public void Menu ()
	{
		SceneManager.LoadScene ("MainMenu");
	}

	private void ShowLogMessagesUI (string msg)
	{
		Text logMsgText = GameObject.Find ("LogMsg").GetComponent<Text> ();
		if (logMsgText != null)
			logMsgText.text = msg;
	}

	#endregion

}

