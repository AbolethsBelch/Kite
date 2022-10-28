using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spline : MonoBehaviour
{
    public Transform[] controlPointsList;
    private bool isPlaying = false; //differentiate when to use gizmo vs linerenderer
    private LineRenderer lineRender;

    void OnDrawGizmos() //display in editor
    {
        Gizmos.color = Color.red;

        for (int i = 0; i < controlPointsList.Length; i++) //increment until we're no longer less than the amnt of pos
        {
            //don't draw a line if we're at any point other than p1 or p2; otherwise we will loop around
            if ((i == 0 || i == controlPointsList.Length - 2 || i == controlPointsList.Length - 1))
            {
                continue;
            }

            DisplaySpline(i); //draw spline between points
        }
    }

    void Start()
    {
        isPlaying = true;
        lineRender = GetComponent<LineRenderer>();
        lineRender.positionCount = controlPointsList.Length;

        for (int i = 0; i < lineRender.positionCount; i++) //increment until we're no longer less than the amnt of pos
        {
            DisplaySpline(i); //draw spline between points
        }
    }


    void DisplaySpline(int pos) //display the spline between the 2 points.
    {
        
        //points between the segments (we need 4, but we print between p1 and p2)
        Vector3 p0 = controlPointsList[pos - 1].position;
        Vector3 p1 = controlPointsList[pos].position; //our starting position
        Vector3 p2 = controlPointsList[pos + 1].position;
        Vector3 p3 = controlPointsList[pos + 2].position;

        
        Vector3 lastPos = p1; //set lastpos to our starting pos
        float resolution = 0.2f; //we need it to cover all of our points when we do this for loop- 0.1 (10) is too much, 0.3 (3.3r) is too few.
        int loops = Mathf.FloorToInt(1f / resolution); //how many times to repeat this loop (5)

        if (isPlaying == false) //Drawing in editor with Gizmos
        {
            for (int i = 1; i <= loops; i++) //go over our points
            {
                float t = i * resolution; //find our t position
                Vector3 newPos = GetCatmullRomPosition(t, p0, p1, p2, p3); //find the coordinates between end points
                Gizmos.DrawLine(lastPos, newPos); //draw line segment
                lastPos = newPos; //Save this pos so we can move onto the next segment
            }
        }
        else //Drawing in GameMode using LineRenderer
        {
            float t = 0f; //define t

            for (int i = 0; i < lineRender.positionCount; i++) //go over all our points
            {
                Vector3 catPos = GetCatmullRomPosition(t, p0, p1, p2, p3);
                lineRender.SetPosition(i, catPos); //setpos needs an int + vert3, setpositionS needs a vert array

                t += (1 / (float)(lineRender.positionCount - 1)); //increment t for next loop
            }
        }
    }

    //Returns a position between 4 Vector3 with Catmull-Rom spline algorithm
    Vector3 GetCatmullRomPosition(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
    {
        //setting up our coefficients for the formula below
        Vector3 a = 2f * p1;
        Vector3 b = p2 - p0;
        Vector3 c = 2f * p0 - 5f * p1 + 4f * p2 - p3;
        Vector3 d = -p0 + 3f * p1 - 3f * p2 + p3;


        //running this cubic polynomial: a + b * t + c * t^2 + d * t^3
        Vector3 pos = 0.5f * (a + (b * t) + (c * t * t) + (d * t * t * t));

        return pos;
    }
}