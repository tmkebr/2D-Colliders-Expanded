using UnityEngine;
using System.Collections;


/////////////////////////
/// 
/// SemiCircleCollider2D
/// 
/// Creates a 2D SemiCircle Collider using Bezier Points and a 2D Edge Collider
/// 
/// Params: 
///     - Smoothing Factor (how many lines are used to make the curve)
///     - Diameter of the SemiCircle
/// 
/// 
/// timothy Kebr, tmkebr@gmail.com
/// 12/8/2015
////////////////////////

// executing in edit mode allows us to see the changes in real time without running the game
[ExecuteInEditMode]
[RequireComponent(typeof(EdgeCollider2D))]
public class SemiCircleCollider2D : MonoBehaviour {

    // limit the range of the smoothing factor
    [Range(1, 999)]
    public int smoothingFactor;
    public float diameter;
    public bool isTrigger;
    public Vector2 offset;
    Vector2 startPoint, endPoint, handlerPoint1, handlerPoint2;
    EdgeCollider2D edgeCollider;

    // Begin the calculations necessary to make an approxiamte semicircle
    // calculate the start/end and handler points based on the diameter given
    // Calculations follow the formula described by Geoff Slinker @ 
    // http://digerati-illuminatus.blogspot.com/2008/05/approximating-semicircle-with-cubic.html
    //
    void Update()
    {
        if (smoothingFactor <  1)
        {
            Debug.LogError("A smoothing factor must be greater than or equal to 1");
        }
        else
        {
            edgeCollider = GetComponent<EdgeCollider2D>();

            float xValIn = diameter * .05f;
            float yValOff = (diameter / 2f) * (4f / 3f);

            startPoint = new Vector2(0, 0);
            handlerPoint1 = new Vector2(xValIn, yValOff);
            handlerPoint2 = new Vector2(diameter - xValIn, yValOff);
            endPoint = new Vector2(diameter, 0);

            edgeCollider.points = getLinePoints();
            edgeCollider.isTrigger = isTrigger;
            edgeCollider.offset = offset;
        }
    }

    // Get the Bezier point given 2 end points (p0, p3) and handle points (p1, p2) 
    // uses the definition of Cubic Bezier Curves according to Wikipedia:
    //       B(t) = (1-t)^3P0 + 3(1-t)^2tP1 + 3(1-t)^2tP2 + t^3P3 
    Vector3 CubicBezierPoint(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
    {
        float x = (1 - t) * (1 - t) * (1 - t) * p0.x + 3 * (1 - t) * (1 - t) * t * p1.x + 3 * (1 - t) * t * t * p2.x + t * t * t * p3.x;
        float y = (1 - t) * (1 - t) * (1 - t) * p0.y + 3 * (1 - t) * (1 - t) * t * p1.y + 3 * (1 - t) * t * t * p2.y + t * t * t * p3.y;

        Vector3 bezierPoint = new Vector3(x, y, 0f);
        return bezierPoint;
    }

    // Get the array of points
    // calculates smoothingfactor # ofs Bezier Points in between the end points
    public Vector2[] getLinePoints()
    {
        Vector2[] ret = new Vector2[smoothingFactor + 1];

        // set the start and end point in the Vector Array
        ret[0] = startPoint;
        ret[smoothingFactor] = endPoint;

        // now fill the inside of the array with smoothingFactor # of Cubic Bezier Points
        for (int i = 1; i < smoothingFactor; i++)
        {
            float t = (1f / smoothingFactor) * i;
            ret[i] = CubicBezierPoint(startPoint, handlerPoint1, handlerPoint2, endPoint, t);
        }
        //foreach(Vector2 i in ret)
        //    Debug.Log(i);
        return ret;
    }
}
