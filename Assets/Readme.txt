Anatomy Puzzle VR Game SS'16 - HTC Vive
Models:
1. Skull
2. Knee
3. Foot
---------------------------------
TODO:
1. Create a Virtual Environment (VE), a small room with walls to show boundary.
2. Import models for testing in VE.
3. Implementation of Basic features:
	0. Add rigidbody, box collider and InteractableItemWithIsKinematic script to all objects that are interactable in a model. (DONE)
	1. Save model initial data at start of game. (DONE)
	2. Picking and dragging of objects using Trigger button. (DONE)
	3. Rotation of objects using wand controller; can be done when an object is picked. (DONE)
	4. Scaling up and down whole model, when a gesture is made in envrionment. Holding down grip buttons on both controllers simultaneously. (Max scale fixed) (DONE)
	5. Split animation of model to start puzzle. (DONE)
	6. Group/Ungroup models objects, when a <button> is pressed on both controller/UI Button. (DONE)
	7. While scaling, if grip button is pressed for longer time and there is no apparent hand movement, scaling jitters: (SET A MIN DISTANCE THRESHOLD) (DONE)
	8. Add laser beam for picking and dragging objects.(DONE)
	9. Show information text on each object, the name of object.(DONE)
	10. Snapping feature; when at right position. When two objects are positioned right relatively, they should be snapped and grouped together,
	 if belong to the main model parent.(DONE)
	11. Relative distance works fine with  correct relative orientation.(DONE)
	12. Interactable items shouldnt go outside the boundary wall, check on trigger and animate objects in arena.(DONE)
	13. Some of the objects have big collider and hence its possible to pick them even from edges where there is no texture.
		Fixed using MeshCollider  non-convex  with rigid body isKinematic (DONE)
	14. Beam between two objects to show progress of snapping whether close or far (DONE).
	15. 3D Main Menu for the selection of models (DONE)
	16. Snapping feature conditions redefined; Snap Animation works (DONE)
	17. Add proper materials to foot model (DONE)
	18. Beam color opactiy to show correct rotation, and color to show distance. (DONE)
	19. Update text to the show difference btw relative distance and current distance, also show angles difference. (DONE)
	20. Hint added on wall with description of button used. (DONE)
	21. Hide laser pointer while scaling, unhide when grip buttons are released. (DONE)
	22. Data structure to handle, snapped objects as a single object to be dragged and snapped (DONE)
	
	23. Split animation causing issues in picking objects (reason unknown)
	_________________________________________________________________________________________
	Patrick fixed:
	* Knee model: is not assembled like skull model and while split animation it goes beyond the boundary lines of VR room. Needs to be assembled in Max/Maya/Blender to set pivot points to center.
	Also the naming convention is not correct, all objects have empty parent which has name. It should be other way around each mesh should have name. (DONE)
	* Skull model: needs to revised, specially teeth should be grouped together. (DONE)

ANATOMIST DEMO DAY(DONE)
1. Question's to take feedback of anatomist.
2. Evaluation and demo (15.09.16) at 3:00 PM
DEMO day changes:
	1. Add maximum fixed scale of models for DEMO day.
	2. Remove snapping features
	3. Set beam to color blue only to show relation.
	4. Hide text displayed on eye of camera.
	5. Add proper materials to foot model
	6. PROBLEM Diagonosed: The beam opacity wasn't working because of invalid shader -.- (Will be fixed after demo day) (DONE)
	_________________________________________________________________________________________

Future tasks/Suggestions:
1. Explosion Visualization
2. Laser beam to pick objects that are far.
3. Collision between objects, they shouldn't penetrate
4. Model puzzle completion (animation)
5. Models with less part (small parts grouped together)
6. Translating and Rotating whole model with trigger button.
7. Tutorial Session (with Human_Bot)
8. Grouping snapped objects and dragging together.
	Data structure to handle, snapped objects as a single object to be dragged and snapped to. 
	Suggestion:Only move right hand object to the left one until the relative distance has reached with the relative angle between two.
9. Unsnapping on a single object on a different controller button/gesture
	_________________________________________________________________________________________

Student:
Asema Hassan
210492

Supervised by:
Patrick Saalfeld
Prof. Preim Bernhard
