using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

///// 
// Unity 2D Capsule Collider
//   - Creates a capsule shaped collider much like the 3D capsule collider
//   - Uses 2D Circle Colliders and 2D Box Collider to make compound collider
//   - Provides in-editor control to make it feel as native as possible
//
// timothy Kebr, tmkebr@gmail.com
// Originally created October 2015
/////

// executing in edit mode allows us to see the changes in real time without running the game
[ExecuteInEditMode]
public class Capsule2D : MonoBehaviour
{

    // These variables can be set in the editor
    public bool isTrigger;
    public Vector2 Center;
    public float radius;
    public float height;
    public enum Direction { X_Axis, Y_Axis };
    public Direction direction = Direction.X_Axis;


    // These variables will be used to manage the colliders
    List<CircleCollider2D> circles;
    BoxCollider2D box;
    Vector2 oldCenter;
    GameObject boxObject, circle0Object, circle1Object;


    // Use this for initialization
    void OnValidate()
    {
        // create the lists used for storing our collider objects
        circles = new List<CircleCollider2D>(GetComponentsInChildren<CircleCollider2D>());
        box = GetComponentInChildren<BoxCollider2D>();


        // if there aren't any game objects created, then make them as a compound game object
        if (transform.childCount > 0)
        {
            boxObject = transform.GetChild(0).gameObject;
            circle0Object = boxObject.transform.GetChild(0).gameObject;
            circle1Object = boxObject.transform.GetChild(0).gameObject;
        }

        ////
        // Make and position the box, and update the offset value based on the box size
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
        // creates the box if one doesn't already exist
        if (box == null)
        {
            boxObject = new GameObject();
            boxObject.AddComponent<BoxCollider2D>();
            box = boxObject.GetComponent<BoxCollider2D>();
        }
        
        // Adjusts the box's size, nesting, name, and trigger values
        box.size = new Vector2((height - radius), radius);
        box.isTrigger = isTrigger;
        boxObject.transform.SetParent(this.transform, false);
        boxObject.name = "Collider";
    }

    /// <summary>
    /// makes a circle collider
    /// </summary>
    /// <param name="i"> the index in the circle list (Circle#) </param>
    /// <param name="offset"> how far to offset the circle based on box size</param>
    void makeCircle(int i, float offset)
    {
        // checks if circles have already been created
        // if they have been, just update the radius and offset values, don't bother making new circles
        if (circles.ElementAtOrDefault(i) != null)
        {
            circles[i].radius = radius / 2;
            circles[i].offset = new Vector2(offset, 0);
        }

        // else no circles have been made yet, so we will make them here
        else
        {

            // Create the circles
            if (i == 0) {
                // if the index is 0, make the first circle and add it to the list, set up nesting, and naming
                circle0Object = new GameObject();
                circles.Insert(i, circle0Object.AddComponent<CircleCollider2D>());
                circle0Object.transform.SetParent(boxObject.transform, false);
                circle0Object.name = ("circle" + i);
            }
            else {
                // if the index is 1, make the second circle and add it to the list, set up nesting, and naming
                circle1Object = new GameObject();
                circles.Insert(i, circle1Object.AddComponent<CircleCollider2D>());
                circle1Object.transform.SetParent(boxObject.transform, false);
                circle1Object.name = ("circle" + i);
            }

            // set the radii and the offsets of the circles
            circles[i].radius = radius / 2;
            circles[i].offset = new Vector2(offset, 0);
        }

        // update the trigger values
        circles[i].isTrigger = isTrigger;       
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
