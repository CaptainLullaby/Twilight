using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject playerCamera;
    [SerializeField] private GameObject map;

    public bool backup = false;
    public GameObject Target;

    private float isBackupReady = 0f;
    private float isTargetorReady = 0f;

    private void Start()
    {
        GameObject kamera = Instantiate(playerCamera, transform.position, Quaternion.identity);
        kamera.transform.SetParent(this.transform);
        map = GameObject.FindGameObjectWithTag("Map");
        Target = null;
    }

    private void Update()
    {
        if (Target != null && isTargetorReady <= 0)
        {
            foreach (var enemy in GameObject.FindGameObjectsWithTag("AI"))
            {
                if (enemy == null)
                    break;
                if (enemy.GetComponent<AIBrute>())
                    enemy.GetComponent<AIBrute>().SetTarget(Target.transform);
                if (enemy.GetComponent<AISpotter>())
                    enemy.GetComponent<AISpotter>().SetTarget(Target.transform);
                if (enemy.GetComponent<AISoldier>())
                    enemy.GetComponent<AISoldier>().SetTarget(Target.transform);
            }
            isTargetorReady = 10f;
        }

        if (backup && isBackupReady <= 0)
        {
            map.GetComponent<ItemPlacer>().Spawner(GameObject.FindGameObjectWithTag("PlayerChar").transform);
            isBackupReady = 10f;
        }


        backup = false;
        Target = null;
        isBackupReady -= Time.deltaTime;
        isTargetorReady -= Time.deltaTime;
    }
}
