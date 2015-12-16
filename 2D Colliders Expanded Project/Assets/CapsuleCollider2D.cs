using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


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
    float maxVal = 999999999999999f;

    SemiCircleCollider2D topCap, bottomCap;
    EdgeCollider2D edge0, edge1;

    GameObject edge0Object, edge1Object, topCapObject, bottomCapObject;

    List<SemiCircleCollider2D> caps;
    List<EdgeCollider2D> edges;

    void OnValidate()
    {
        //// create the lists used for storing our collider objects
        //circles = new List<CircleCollider2D>(GetComponentsInChildren<CircleCollider2D>());
        //box = GetComponentInChildren<BoxCollider2D>();

        edges = new List<EdgeCollider2D>(GetComponentsInChildren<EdgeCollider2D>());
        caps = new List<SemiCircleCollider2D>(GetComponentsInChildren<SemiCircleCollider2D>());

        diameter = Mathf.Clamp(radius * 2, 0, maxVal);
        

        if (transform.childCount > 0)
        {
            edge0Object = transform.GetChild(0).gameObject;
            edge1Object = edge0Object.transform.GetChild(0).gameObject;
            topCapObject = edge0Object.transform.GetChild(1).gameObject;
            bottomCapObject = edge0Object.transform.GetChild(2).gameObject;
        }

        make_edges(0);
        make_edges(1);
        make_caps(0);
        make_caps(1);

        center_colliders();
        rotate_colliders();
        update_triggers();
    }


    void make_edges(int i)
    {
        
        if (edges.ElementAtOrDefault(i) != null)
        {
            EdgeCollider2D curEdge = edges[i];
            edge0 = edge0Object.GetComponent<EdgeCollider2D>();
            edge1 = edge1Object.GetComponent<EdgeCollider2D>();

            if (height > diameter)
            {
                // if the collider has been disabled, enable it once again
                if (!curEdge.enabled)
                {
                    curEdge.enabled = true;
                }
                Vector2[] edgePoints = new Vector2[2];


                // if we are adjusting the first edge
                if (i == 0)
                {
                    edgePoints[0] = new Vector2(0, -(height - diameter));
                    edgePoints[1] = new Vector2(0, 0);
                    curEdge.points = edgePoints;
                }

                // else if we are adjusting the second edge
                else
                {
                    edgePoints[0] = new Vector2(diameter, 0);
                    edgePoints[1] = new Vector2(diameter, -(height - diameter));
                    curEdge.points = edgePoints;
                }
            }
            else
            {
                // else the semicircles cover the capsule shape already:
                // disable the collider since we won't need it
                if (curEdge.enabled)
                {
                    curEdge.enabled = false;
                }
            }
        }


        else
        {
            if (i == 0)
            {
                edge0Object = new GameObject();
                edge0Object.name = "Capsule2D";
                edges.Insert(i, edge0Object.AddComponent<EdgeCollider2D>());
                edge0 = edge0Object.GetComponent<EdgeCollider2D>();
                edge0.transform.SetParent(this.transform, false);
            }
            else
            {
                edge1Object = new GameObject();
                edge1Object.name = "edge" + i;
                edges.Insert(i, edge1Object.AddComponent<EdgeCollider2D>());
                edge1 = edge1Object.GetComponent<EdgeCollider2D>();
                edge1.transform.SetParent(edge0Object.transform, false);
            }
            Vector2[] edgePoints = new Vector2[2];
            edgePoints[0] = new Vector2(0, -(height - diameter));
            edgePoints[1] = new Vector2(0, 0);
            edges[i].points = edgePoints;
        }
    }

    void make_caps(int i)
    {
        float offsetGap = Mathf.Clamp(height - diameter, 0, maxVal);

        if (caps.ElementAtOrDefault(i) != null)
        {
            // TODO: assumes that both caps are made. might be a case where the caps element isn't null, but one
            // hasn't been made
            topCap = topCapObject.GetComponent<SemiCircleCollider2D>();
            bottomCap = bottomCapObject.GetComponent<SemiCircleCollider2D>();

            if (i == 0) {
                topCap.diameter = diameter;
            }
            else
            {
                bottomCap.diameter = diameter;
                bottomCap.offset = new Vector2(-diameter, offsetGap);
            }
        }
        else
        {
            if (i == 0)
            {
                topCapObject = new GameObject();
                caps.Insert(i, topCapObject.AddComponent<SemiCircleCollider2D>());
                topCapObject.name = "Top Cap";
                topCap = topCapObject.GetComponent<SemiCircleCollider2D>();

                topCap.smoothingFactor = smoothingFactor;

                topCap.diameter = diameter;

                topCapObject.transform.SetParent(edge0Object.transform, false); 
            }
            else {
                bottomCapObject = new GameObject();
                caps.Insert(i, bottomCapObject.AddComponent<SemiCircleCollider2D>());
                bottomCapObject.name = "Bottom Cap";
                bottomCap = bottomCapObject.GetComponent<SemiCircleCollider2D>();

                bottomCap.smoothingFactor = smoothingFactor;

                bottomCap.diameter = diameter;

                bottomCapObject.transform.localEulerAngles = new Vector3(0, 0, 180); // rotate the cap

                bottomCap.offset = new Vector2(-diameter, offsetGap); // offset the cap

                bottomCapObject.transform.SetParent(edge0Object.transform, false);
            }
        }
    }


    /// <summary>
    /// Centers the colliders
    /// </summary>
    void center_colliders()
    {
        // center the colliders only if the collider center value has been changed
        if (oldCenter != Center)
        {
            edge0.offset = Center;
            edge1.offset = Center;
            bottomCap.offset = bottomCap.offset - Center;
            topCap.offset = Center;
        }
    }

    /// <summary>
    /// rotates the colliders
    /// </summary>
    void rotate_colliders()
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

    // checks if the triggers have changed and updates them accordingly
    void update_triggers()
    {
        // update the trigger values only if the isTrigger box has been changed
        if (oldTrigger != isTrigger)
        {
            edges[0].isTrigger = isTrigger;
            edges[1].isTrigger = isTrigger;
            caps[0].isTrigger = isTrigger;
            caps[1].isTrigger = isTrigger;
        }
    }

    // hack to check if the center and trigger values have changed
    void onDrawGizmos()
    {
        oldCenter = Center;
        oldTrigger = isTrigger;
    }

    //// create and adjust the top cap
    //void make_top_cap()
    //{
    //    // creates the bottom cap if one doesn't exist
    //    if (topCap == null)
    //    {
    //        // create and get the cap's game object
    //        topCapObject = new GameObject();
    //        topCapObject.AddComponent<SemiCircleCollider2D>();
    //        topCapObject.name = "Top Cap";
    //        topCapObject.tag = "topCap";
    //        topCap = topCapObject.GetComponent<SemiCircleCollider2D>();

    //        //Debug.Log("Adjusting Top Cap");
    //        topCap.smoothingFactor = smoothingFactor;

    //        topCap.diameter = diameter;

    //        topCapObject.transform.SetParent(edge0Object.transform, false); 
    //    }

    //    // else the cap has already been made and we just need to adjust it
    //    else
    //    {
    //        topCap.diameter = diameter;
    //    }

    //}

    //// create and adjust the bottom cap
    //void make_bottom_cap()
    //{
    //    // Clamp the offset so the caps don't misalign


    //    // creates the bottom cap if one doesn't exist
    //    if (bottomCap == null)
    //    {
    //        //Debug.Log("Making Bottom Cap");
    //        // create and get the cap's game object
    //        bottomCapObject = new GameObject();
    //        bottomCapObject.AddComponent<SemiCircleCollider2D>();
    //        bottomCapObject.name = "Bottom Cap";
    //        bottomCapObject.tag = "bottomCap";
    //        bottomCap = bottomCapObject.GetComponent<SemiCircleCollider2D>();

    //        //Debug.Log("Adjusting Bottom Cap");
    //        bottomCap.smoothingFactor = smoothingFactor;

    //        bottomCap.diameter = diameter;

    //        bottomCapObject.transform.localEulerAngles = new Vector3(0, 0, 180); // rotate the cap

    //        bottomCap.offset = new Vector2(-diameter, offsetGap); // offset the cap

    //        bottomCapObject.transform.SetParent(edge0Object.transform, false);


    //    }

    //    // else if the cap is already made, just adjust it
    //    else
    //    {
    //        bottomCap.diameter = diameter;
    //        bottomCap.offset = new Vector2(-diameter, offsetGap);
    //    }
    //}


    ///// <summary>
    ///// makes a box collider
    ///// </summary>
    //void make_first_edge()
    //{
    //    // creates the edge if one doesn't already exist and we need it (height > diameter)
    //    if (edge0 == null)
    //    {
    //        Debug.Log("Making First Edge");
    //        edge0Object = new GameObject();
    //        edge0Object.name = "2D Capsule Collider";
    //        edge0Object.tag = "edge1";
    //        edge0Object.AddComponent<EdgeCollider2D>();
    //        edge0 = edge0Object.GetComponent<EdgeCollider2D>();
    //        edge0.transform.SetParent(this.transform, false);
    //    }

    //    if (height > diameter)
    //    {
    //        Debug.Log("Adjusting First Edge");
    //        // if the collider has been disabled, enable it once again
    //        if (!edge0.enabled)
    //        {
    //            edge0.enabled = true;
    //        }
    //        Vector2[] edge1Points = new Vector2[2];
    //        edge1Points[0] = new Vector2(0, -(height - diameter));
    //        edge1Points[1] = new Vector2(0, 0);
    //        edge0.points = edge1Points;
    //        //edge1.isTrigger = isTrigger;
    //    }
    //        else
    //        {
    //        // else the semicircles cover the capsule shape already:
    //        // disable the collider since we won't need it
    //            if (edge0.enabled)
    //            {
    //                edge0.enabled = false;
    //            }
    //        }
    //}

    //void make_second_edge()
    //{
    //    // creates the box if one doesn't already exist
    //    if (edge1 == null)
    //    {
    //        //Debug.Log("Making Second Edge");
    //        edge1Object = new GameObject();
    //        edge1Object.name = "Edge2";
    //        edge1Object.tag = "edge2";
    //        edge1Object.AddComponent<EdgeCollider2D>();
    //        edge1 = edge1Object.GetComponent<EdgeCollider2D>();
    //        edge1Object.transform.SetParent(edge0Object.transform, false);   
    //    }

    //    // if the edge should be adjusted (height is bigger than the diameter)
    //    // then we enable it and adjust the offsets
    //    if (height > diameter)
    //    {
    //        //Debug.Log("Adjusting Second Edge");
    //        // if the collider has been disabled, enable it once again
    //        if (!edge1.enabled)
    //        {
    //            edge1.enabled = true;
    //        }
    //        Vector2[] edge2Points = new Vector2[2];
    //        edge2Points[0] = new Vector2(diameter, 0);
    //        edge2Points[1] = new Vector2(diameter, -(height - diameter));
    //        edge1.points = edge2Points;
    //    }
    //    else
    //    {
    //        // else the semicircles cover the capsule shape already:
    //        // disable the collider since we won't need it
    //        if (edge1.enabled)
    //        {
    //            edge1.enabled = false;
    //        }
    //    }        
    //}

}
