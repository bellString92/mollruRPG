using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterUI : MonoBehaviour
{
    [SerializeField] private Player player;
    [SerializeField] private Transform hpBarParent;
    [SerializeField] private GameObject orgHpBar;
    [SerializeField] private MonsterHpBar hpBar;


    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player").GetComponent<Player>();
        hpBarParent = transform.GetChild(0);
        orgHpBar = Resources.Load("Prefabs/UI/MonsterHpBar") as GameObject;
    }

    // Update is called once per frame
    void Update()
    {
        /*if (player.myTargetmonster.Count > 0)
        {
            foreach (Transform t in player.myTargetmonster)
            {
                if (t.GetComponent<Enemy>().monsterHpBar == null)
                {
                    GameObject obj = Instantiate(orgHpBar, hpBarParent);
                    hpBar = obj.GetComponent<MonsterHpBar>();
                    hpBar.monster = t.GetComponent<Enemy>();
                    t.GetComponent<Enemy>().monsterHpBar = hpBar;
                }
                else
                {
                    t.GetComponent<Enemy>().monsterHpBar.hpBarAliveTime = 0.0f;
                }
            }
        }*/
    }

 }
