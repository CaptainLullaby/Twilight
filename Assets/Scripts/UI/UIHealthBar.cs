using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIHealthBar : MonoBehaviour
{
    [SerializeField] private UnityEngine.UI.Image healthbar;
    GameObject player;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("PlayerChar");
    }

    private void Update()
    {
        if (player == null)
        {
            healthbar.fillAmount = 0;
            return;
        }

        var health = player.GetComponent<Health>().currenthealth / player.GetComponent<Health>().maxhealth;

        healthbar.fillAmount = health;
    }
}
