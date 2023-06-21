using UnityEngine;

public class Movement : MonoBehaviour
{

    private float xOffset;
    private float yOffset;

    private void Start()
    {
        xOffset = UnityEngine.Random.Range(0f, 100f);
        yOffset = UnityEngine.Random.Range(0f, 100f);
    }
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
        Vector3 pos = new Vector3(transform.position.x + Mathf.PerlinNoise(xOffset, 0f) - 0.5f, transform.position.y + Mathf.PerlinNoise(0f, yOffset) - 0.5f, transform.position.z);
        Debug.Log(Vector3.Distance(pos, transform.position));
        Debug.Log(pos);
        transform.position = pos;
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Wall")
        {

        }
    }

    
}
