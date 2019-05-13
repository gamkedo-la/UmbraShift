using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour
{
    public Transform target;
    public float visualRange = 30.0f;

    public int currentHealth;
    private BaseCharacterClass baseClass;
    private ActionManager myManager;

    NavMeshAgent agent;

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;

        baseClass = GetComponent<BaseCharacterClass>();
        currentHealth = baseClass.maxHealth;
        myManager = GetComponent<ActionManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (myManager == InputManager.instance.actionManager)
        {
            Vector3 travelDiff = target.position - transform.position;
            travelDiff.y = 0.0f;

            Vector3 moveDiff = agent.velocity;
            moveDiff.y = 0.0f;

            if (travelDiff.magnitude > 0.5 && moveDiff.magnitude > 0.1)
            {
                transform.rotation = Quaternion.LookRotation(moveDiff);
            }
            agent.SetDestination(target.position);
        }

        CheckLineOfSite();
    }

    void CheckLineOfSite()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, visualRange);
        for (int i = 0; i < hitColliders.Length; i++)
        {
            if (hitColliders[i].tag == "NPC")
            {
                //Debug.Log("NPC is in the overlapsphere");
                Vector3 rayFromMeToNPC = hitColliders[i].gameObject.transform.position - transform.position;
                RaycastHit rhinfo;
                if (Physics.Raycast(transform.position, rayFromMeToNPC, out rhinfo, visualRange))
                {
                    //Debug.Log(rhinfo.collider.name);
                    if (rhinfo.collider.tag == "NPC")
                    {
                        EnemyController enemyCtrl = hitColliders[i].GetComponent<EnemyController>();
                        if (enemyCtrl != null)
                        {
                            enemyCtrl.PlayerCanSee(true);
                        }
                    }
                    else
                    {
                        EnemyController enemyCtrl = hitColliders[i].GetComponent<EnemyController>();
                        if (enemyCtrl != null)
                        {
                            enemyCtrl.PlayerCanSee(false);
                        }
                    }
                }
            }
        }
    }
}
