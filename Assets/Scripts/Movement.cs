using System.IO;
using UnityEngine;

public class Movement : MonoBehaviour
{
    public void Move(float speed)
    {
        transform.Translate(Vector3.up * Time.deltaTime * speed * 2f);
    }

    public void Follow(Vector3 target)
    {
        transform.Translate(target * Time.deltaTime * 2f);
    }
    public void Rotate( float rot)
        {
            transform.Rotate(Vector3.forward * rot * Time.deltaTime * 10f);
        }

    public void RandomWalk()
    {
        Vector3 pos = new Vector3(transform.position.x + Mathf.PerlinNoise(0f, 100f) - 0.5f, transform.position.y + Mathf.PerlinNoise(0f, 100f) - 0.5f, 0);
        //Vector3.Slerp(transform.position, pos, Time.deltaTime * 5f);
        Debug.Log(pos);
        transform.position = pos;
    }
    
}
