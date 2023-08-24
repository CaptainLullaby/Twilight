using UnityEngine.Events; 
using UnityEngine;
using UnityEngine.SceneManagement;

public class Health : MonoBehaviour
{
    [SerializeField] private bool isDead;
    public float currenthealth;
    public int maxhealth;

    public UnityEvent<GameObject> OnHitWithReference, OnDeathWithRefernce;

    public void Start()
    {
        if (maxhealth == 0)
            InitHealth(5);
    }

    public void InitHealth(int healthVal)
    {
        currenthealth = healthVal;
        maxhealth = healthVal;
        isDead = false;
    }

    public void iHit(int amount, GameObject target)
    {
        if (isDead)
            return;
        if (target.layer == gameObject.layer)
            return;

        currenthealth -= amount;

        if(currenthealth > 0)
            OnHitWithReference?.Invoke(target);
        else
        {
            OnDeathWithRefernce?.Invoke(target);
            isDead = true;
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
