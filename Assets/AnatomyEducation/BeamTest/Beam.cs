using UnityEngine;
using System.Collections;

public class Beam : MonoBehaviour {

    public GameObject obj1 = null;
    public GameObject obj2 = null;
    public Material matColor;
    private float sizeOfLine = 0.1f;
    private Color _farColor = new Color(0, 1, 0, 0.1f);
    private Color _closeColor = new Color(0, 1, 0, 1.0f);
    private LineRenderer lines = null;
    private GameObject beamObj = null;

   public float relDistance = 0.0f;
    // Use this for initialization
    void Start () {

             beamObj = new GameObject();
    }
	
	// Update is called once per frame
	void Update () {

        if (Input.GetKeyUp(KeyCode.D))
        {
            //draw a beam between two objects
            relDistance = Vector3.Distance(obj1.transform.position, obj2.transform.position);
            Debug.Log("Distance: " + relDistance);
            createLine(beamObj, obj1.transform.position, obj2.transform.position, sizeOfLine, _closeColor);
        }

        if (Input.GetKey(KeyCode.LeftArrow)) 
        {
            //obj1 is moving far
            obj1.transform.position = new Vector3(obj1.transform.position.x - 0.1f, obj1.transform.position.y, obj1.transform.position.z);
            float dist = Vector3.Distance(obj1.transform.position, obj2.transform.position);
            if(dist > relDistance)
                UpdateLinePosition(obj1.transform.position, obj2.transform.position, _farColor);
            else
                UpdateLinePosition(obj1.transform.position, obj2.transform.position, new Color(_farColor.r, _farColor.g, _farColor.b, lines.material.color.a - 0.01f));
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            //obj1 is moving close
            obj1.transform.position = new Vector3(obj1.transform.position.x + 0.1f, obj1.transform.position.y, obj1.transform.position.z);
            float dist = Vector3.Distance(obj1.transform.position, obj2.transform.position);
            if (dist <= relDistance)
            {
                UpdateLinePosition(obj1.transform.position, obj2.transform.position, _closeColor);
            }else
                UpdateLinePosition(obj1.transform.position, obj2.transform.position, new Color(_closeColor.r, _closeColor.g, _closeColor.b, lines.material.color.a + 0.01f));
        }
    }
    private void createLine(GameObject beamObj,Vector3 start, Vector3 end, float lineSize, Color c)
    {
        if (lines == null)
        {
            lines = (LineRenderer)beamObj.AddComponent<LineRenderer>();
            lines.material = matColor; // new Material(shader);
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
}
