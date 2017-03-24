 /*using UnityEngine;
 using System.Collections;
 public class DragDropMouse : MonoBehaviour
{

 private Vector3 screenPoint;
 private Vector3 offset;

 void OnMouseDown()
{
      screenPoint = Camera.main.WorldToScreenPoint(gameObject.transform.position);
     offset = gameObject.transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z));

}
 
void OnMouseDrag()
    {
        Vector3 curScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z);
        Vector3 curPosition = Camera.main.ScreenToWorldPoint(curScreenPoint) +offset;
        transform.position = curPosition;
    }
}
*/

using System.Collections;
using UnityEngine;

public class DragDropMouse : MonoBehaviour
{
  private Color mouseOverColor = Color.blue;
  private Color originalColor = Color.yellow;
  private bool dragging = false;
  private float distance;

   void OnMouseEnter()
   {
         this.GetComponent<Renderer>().material.color = mouseOverColor;
    }
   void OnMouseExit()
  {
        this.GetComponent<Renderer>().material.color = originalColor;
   }

   void OnMouseDown()
   {
     distance = Vector3.Distance(transform.position, Camera.main.transform.position);
     dragging = true;
   }
  void OnMouseUp()
   {
      dragging = false;
   }
    void Update()
   {
       if (dragging)
       {
         Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
         Vector3 rayPoint = ray.GetPoint(distance);
          transform.position = rayPoint;
       }
   }
}
