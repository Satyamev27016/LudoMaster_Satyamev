// pathobject
using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class PathsObjectParent : PathPoint
{


    public PathPoint[] RedPath;
    public PathPoint[] BluePath;
    public PathPoint[] GreenPath;
    public PathPoint[] YellowPath;
    public PathPoint[] BasePoint;

    public float[] scales;
    public float[] positionDifference;

    public List<PathPoint> safePoints;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}


