using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SteeringBehaviour : MonoBehaviour
{
    public abstract (float[] danger, float[] interest) GetSteering(float[] danger, float[] interest, AIData aiData);
}

public static class Directions
{
    public static List<Vector2> eightdirections = new List<Vector2>
    {
        new Vector2 (0, 1).normalized,  //top
        new Vector2 (1, 1).normalized,  //top right
        new Vector2 (1, 0).normalized,  //right
        new Vector2 (1, -1).normalized, //bottom right
        new Vector2 (0, -1).normalized, //bottom
        new Vector2 (-1, -1).normalized,//bottom left
        new Vector2 (-1, 0).normalized, //left
        new Vector2 (-1, 1).normalized, //top left 
    };
}