using UnityEngine;
using System.Collections;
using System.Collections.Generic;
//using System.Linq;


/////////////////////
//
// 2D Capsule Collider
//  - A unibody version of my 2D Capsule Collider. 
//  - Uses my 2D SemiCirlce Colliders and 2D Edge Colliders to make a capsule
//
// timothy Kebr, tmkebr@gmail.com
// Originally created 12/08/2015
/////////////////////

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
    int smoothingFactor = 30; // how smooth should the edges be when making the capsule's caps?
    Vector2 oldCenter;
    bool oldTrigger;

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
        float adjRadius = radius;
        adjRadius = Mathf.Clamp(radius, 0f, height/2);
        diameter = adjRadius * 2;
        

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
        updateTriggers();
    }


    /// <summary>
    /// makes a box collider
    /// </summary>
    void makeFirstEdge()
    {
        // creates the edge if one doesn't already exist and we need it (height > diameter)
        if (edge1 == null)
        {
            Debug.Log("Making First Edge");
            edge1Object = new GameObject();
            edge1Object.name = "2D Capsule Collider";
            edge1Object.AddComponent<EdgeCollider2D>();
            edge1 = edge1Object.GetComponent<EdgeCollider2D>();
            edge1.transform.SetParent(this.transform, false);
            //edge1.isTrigger = isTrigger;
        }
        
        else if (height > diameter)
        {
            Debug.Log("Adjusting First Edge");
            // if the collider has been disabled, enable it once again
            if (!edge1.enabled)
            {
                edge1.enabled = true;
            }
            Vector2[] edge1Points = new Vector2[2];
            edge1Points[0] = new Vector2(0, -(height - diameter));
            edge1Points[1] = new Vector2(0, 0);
            edge1.points = edge1Points;
            //edge1.isTrigger = isTrigger;
        }
            else
            {
            // else the semicircles cover the capsule shape already:
            // disable the collider since we won't need it
                if (edge1.enabled)
                {
                    edge1.enabled = false;
                }
            }
    }

    void makeSecondEdge()
    {
        // creates the box if one doesn't already exist
        if (edge2 == null)
        {
            Debug.Log("Making Second Edge");
            edge2Object = new GameObject();
            edge2Object.name = "Edge2";
            edge2Object.AddComponent<EdgeCollider2D>();
            edge2 = edge2Object.GetComponent<EdgeCollider2D>();
            edge2Object.transform.SetParent(edge1Object.transform, false);   
            //edge2.isTrigger = isTrigger;
        }
        else if (height > diameter)
        {
            Debug.Log("Adjusting Second Edge");
            // if the collider has been disabled, enable it once again
            if (!edge2.enabled)
            {
                edge2.enabled = true;
            }
            Vector2[] edge2Points = new Vector2[2];
            edge2Points[0] = new Vector2(diameter, 0);
            edge2Points[1] = new Vector2(diameter, -(height - diameter));
            edge2.points = edge2Points;
            //edge2.isTrigger = isTrigger;
        }
        else
        {
            // else the semicircles cover the capsule shape already:
            // disable the collider since we won't need it
            if (edge1.enabled)
            {
                edge2.enabled = false;
            }
        }        
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
            //topCap.isTrigger = isTrigger;
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
            //bottomCap.isTrigger = isTrigger;
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
        // center the colliders only if the collider center value has been changed
        if (oldCenter != Center)
        {
            edge1.offset = Center;
            edge2.offset = Center;
            bottomCap.offset = bottomCap.offset - Center;
            topCap.offset = Center;
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

    void updateTriggers()
    {
        // update the trigger values only if the isTrigger box has been changed
        if (oldTrigger != isTrigger)
        {
            edge1.isTrigger = isTrigger;
            edge2.isTrigger = isTrigger;
            topCap.isTrigger = isTrigger;
            bottomCap.isTrigger = isTrigger;
        }
    }

    // hack to check if the center and trigger values have changed
    void onDrawGizmos()
    {
        oldCenter = Center;
        oldTrigger = isTrigger;
    }

}
