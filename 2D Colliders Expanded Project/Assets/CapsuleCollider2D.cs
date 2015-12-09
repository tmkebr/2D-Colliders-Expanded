using UnityEngine;
using System.Collections;
using System.Collections.Generic;
//using System.Linq;

// executing in edit mode allows us to see the changes in real time without running the game
[ExecuteInEditMode]
public class CapsuleCollider2D : MonoBehaviour {

    // These variables can be set in the editor
    public bool isTrigger;
    public Vector2 Center;
    public float radius;
    public float height;
    public enum Direction { X_Axis, Y_Axis };
    public Direction direction = Direction.X_Axis;

    float diameter;
    int smoothingFactor = 30;
    Vector2 oldCenter;

    SemiCircleCollider2D topCap, bottomCap;
    EdgeCollider2D edge1, edge2;

    GameObject edge1Object, edge2Object, topCapObject, bottomCapObject;

    List<SemiCircleCollider2D> caps;
    List<EdgeCollider2D> edges;

    void OnValidate()
    {
        //// create the lists used for storing our collider objects
        //circles = new List<CircleCollider2D>(GetComponentsInChildren<CircleCollider2D>());
        //box = GetComponentInChildren<BoxCollider2D>();

        //caps = new List<SemiCircleCollider2D>(GetComponentsInChildren<SemiCircleCollider2D>());
        //edges = new List<EdgeCollider2D>(GetComponentsInChildren<EdgeCollider2D>());
        diameter = radius * 2;

        if (transform.childCount > 0)
        {
            edge1Object = transform.GetChild(0).gameObject;
            edge2Object = edge1Object.transform.GetChild(0).gameObject;
            topCapObject = edge1Object.transform.GetChild(0).gameObject;
            bottomCapObject = edge1Object.transform.GetChild(0).gameObject;
        }

        makeFirstEdge();
        makeSecondEdge();
        makeTopCap();
        makeBottomCap();

        centerColliders();
        rotateColliders();
    }


    /// <summary>
    /// makes a box collider
    /// </summary>
    void makeFirstEdge()
    {
        // creates the box if one doesn't already exist
        if (edge1 == null)
        {
            Debug.Log("Making First Edge");
            edge1Object = new GameObject();
            edge1Object.AddComponent<EdgeCollider2D>();
            edge1 = edge1Object.GetComponent<EdgeCollider2D>();
        }

        Debug.Log("Adjusting First Edge");
        if (height > diameter)
        {
            Vector2[] edge1Points = new Vector2[2];
            edge1Points[0] = new Vector2(0, -(height - diameter));
            edge1Points[1] = new Vector2(0, 0);
            edge1.points = edge1Points;
        }

        edge1.isTrigger = isTrigger;
        edge1.transform.SetParent(this.transform, false);
        edge1Object.name = "2D Capsule Collider";
    }

    void makeSecondEdge()
    {
        // creates the box if one doesn't already exist
        if (edge2 == null)
        {
            Debug.Log("Making Second Edge");
            edge2Object = new GameObject();
            edge2Object.AddComponent<EdgeCollider2D>();
            edge2 = edge2Object.GetComponent<EdgeCollider2D>();
        }

        Debug.Log("Adjusting Second Edge");
        if (height > diameter)
        {
            Vector2[] edge2Points = new Vector2[2];
            edge2Points[0] = new Vector2(diameter, 0);
            edge2Points[1] = new Vector2(diameter, -(height - diameter));
            edge2.points = edge2Points;
        }

        edge2.isTrigger = isTrigger;
        edge2Object.transform.SetParent(edge1Object.transform, false);
        edge2Object.name = "Edge2";
    }

    void makeTopCap()
    {
        // creates the box if one doesn't already exist
        if (topCap == null)
        {
            Debug.Log("Making Top Cap");
            topCapObject = new GameObject();
            topCapObject.AddComponent<SemiCircleCollider2D>();
            topCap = topCapObject.GetComponent<SemiCircleCollider2D>();

            Debug.Log("Adjusting Top Cap");
            topCap.smoothingFactor = smoothingFactor;
            topCap.diameter = diameter;
            topCap.isTrigger = isTrigger;
            topCapObject.transform.SetParent(edge1Object.transform, false);
            topCapObject.name = "Top Cap";
        }
        else
        {
            topCap.diameter = diameter;
        }
        
    }

    void makeBottomCap()
    {
        // creates the box if one doesn't already exist
        if (bottomCap == null)
        {
            Debug.Log("Making Bottom Cap");
            bottomCapObject = new GameObject();
            bottomCapObject.AddComponent<SemiCircleCollider2D>();
            bottomCap = bottomCapObject.GetComponent<SemiCircleCollider2D>();

            Debug.Log("Adjusting Bottom Cap");
            bottomCap.smoothingFactor = smoothingFactor;
            bottomCap.diameter = diameter;
            bottomCapObject.transform.localEulerAngles = new Vector3(0, 0, 180);
            bottomCap.offset = new Vector2(-diameter, height - diameter);
            bottomCap.isTrigger = isTrigger;
            bottomCapObject.transform.SetParent(edge1Object.transform, false);
            bottomCapObject.name = "Bottom Cap";
        }
        else
        {
            bottomCap.diameter = diameter;
            bottomCap.offset = new Vector2(-diameter, height - diameter);
        }
    }

    /// <summary>
    /// Centers the colliders
    /// </summary>
    void centerColliders()
    {
        if (oldCenter != Center)
        {
            edge1.offset = Center;
            edge2.offset = Center;
            bottomCap.offset = bottomCap.offset + Center;
            topCap.offset = topCap.offset + Center;
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
    void onDrawGizmos()
    {
        oldCenter = Center;
    }

}
