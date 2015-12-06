using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[ExecuteInEditMode]
public class Capsule2D : MonoBehaviour
{

    public bool isTrigger;
    public Vector2 Center;
    public float radius;
    public float height;
    public enum Direction { X_Axis, Y_Axis };
    public Direction direction = Direction.X_Axis;

    List<CircleCollider2D> circles;
    BoxCollider2D box;
    Vector2 oldCenter;
    GameObject boxObject, circle0Object, circle1Object;

    // Use this for initialization
    void OnValidate()
    {
        circles = new List<CircleCollider2D>(GetComponentsInChildren<CircleCollider2D>());
        box = GetComponentInChildren<BoxCollider2D>();


        if (transform.childCount > 0)
        {
            boxObject = transform.GetChild(0).gameObject;
            circle0Object = boxObject.transform.GetChild(0).gameObject;
            circle1Object = boxObject.transform.GetChild(0).gameObject;
        }

        ////
        // Make the box
        ////
        makeBox();

        float offset = ((box.size.x) / 2);

        ////
        // Make the circles
        ////
        makeCircle(0, -offset);
        makeCircle(1, offset);


        ////
        // Center the colliders
        ////
        centerColliders();

        ////
        // Rotate the colliders
        ////
        rotateColliders();
    }



    /// <summary>
    /// makes a box collider
    /// </summary>
    void makeBox()
    {
        if (box == null)
        {
            boxObject = new GameObject();
            boxObject.AddComponent<BoxCollider2D>();
            box = boxObject.GetComponent<BoxCollider2D>();
        }
        //boxObject = transform.GetChild(0).gameObject;
        box.size = new Vector2((height - radius), radius);
        box.isTrigger = isTrigger;
        boxObject.transform.SetParent(this.transform, false);
        boxObject.name = "box";
    }

    /// <summary>
    /// makes a circle collider
    /// </summary>
    /// <param name="i"> the index in the circle list (Circle#) </param>
    /// <param name="offset"> how far to offset the circle based on box size</param>
    void makeCircle(int i, float offset)
    {
        if (circles.ElementAtOrDefault(i) != null)
        {
            circles[i].radius = radius / 2;
            circles[i].offset = new Vector2(offset, 0);
        }
        else
        {

            if (i == 0) {
                circle0Object = new GameObject();
            }
            else {
                circle1Object = new GameObject();
            }


            if (i == 0) {
                circles.Insert(i, circle0Object.AddComponent<CircleCollider2D>());
            }
            else {
                circles.Insert(i, circle1Object.AddComponent<CircleCollider2D>());
            }


            circles[i].radius = radius / 2;
            circles[i].offset = new Vector2(offset, 0);
        }

        circles[i].isTrigger = isTrigger;

        if (i == 0)
        {
            circle0Object.transform.SetParent(boxObject.transform, false);
            circle0Object.name = ("circle" + i);
        }
        else
        {
            circle1Object.transform.SetParent(boxObject.transform, false);
            circle1Object.name = ("circle" + i);
        }
        
    }

    /// <summary>
    /// Centers the colliders
    /// </summary>
    void centerColliders()
    {
        if (oldCenter != Center)
        {
            box.offset = Center;
            circles[0].offset = circles[0].offset + Center;
            circles[1].offset = circles[1].offset + Center;
        }
    }

    /// <summary>
    /// rotates the colliders
    /// </summary>
    void rotateColliders()
    {
        if (direction == Direction.Y_Axis)
        {
            gameObject.transform.localEulerAngles = new Vector3(0, 0, 90);
        }
        else if (direction == Direction.X_Axis)
        {
            gameObject.transform.localEulerAngles = new Vector3(0, 0, 0);
        }
    }

    // hack to check if the center value has changed
    void onDrawGizmos() {
        oldCenter = Center;
    }
}
