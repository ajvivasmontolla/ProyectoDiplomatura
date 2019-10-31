using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] Image healthBar;
    [SerializeField] Image actionPointsBar;
    //private Transform camara;
    Fighter fighterStats;

    // Start is called before the first frame update
    void Start()
    {
        fighterStats = GetComponentInParent<Fighter>();
        //camara = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        healthBar.fillAmount = (float)fighterStats.GetLifePoints() / (float)fighterStats.GetBaseLifePoints();
        actionPointsBar.fillAmount = (float)fighterStats.GetActionPoints() / (float)fighterStats.GetBaseActionPoints();

        //transform.LookAt(camara);
    }
}
