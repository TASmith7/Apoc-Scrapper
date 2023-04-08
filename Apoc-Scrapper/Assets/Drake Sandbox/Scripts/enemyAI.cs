using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.AI;


public class enemyAI : MonoBehaviour, IDamage
{
    [Header("-----Components-----")]
    [SerializeField] Renderer model;
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Transform headPos;
    [SerializeField] Transform shootPos;

    [Header("-----Enemy Stats-----")]
    [SerializeField] int HP;
    [SerializeField] int playerFaceSpeed;
    [SerializeField] int sightLine;
    Vector3 playerDir;
    Vector3 runTo;
    [Header("-----Gun Stats-----")]
    [Range(1, 10)][SerializeField] int shootDamage;
    [Range(.1f, 5)][SerializeField] float shootRate;
    [Range(1, 100)][SerializeField] int shootDistance;
    [SerializeField] int bulletSpeed;
    [SerializeField] GameObject bullet;
    bool playerInRange;
    float angleToPlayer;
    bool isShooting;
    float stoppingDistanceOrig;
    



    // Start is called before the first frame update
    void Start()
    {
        gameManager.instance.UpdateGameGoal(1);
        stoppingDistanceOrig = agent.stoppingDistance;
    }

    // Update is called once per frame
    void Update()
    {
        if(playerInRange)
        {

            CanSeePlayer();
            
        }
        
        
    }
    IEnumerator shoot()
    {
        isShooting = true;
        GameObject bulletClone =Instantiate(bullet, shootPos.position, bullet.transform.rotation);
        bulletClone.GetComponent<Rigidbody>().velocity = transform.forward * bulletSpeed;
        yield return new WaitForSeconds(shootRate);
        isShooting = false;
    }
    void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            playerInRange = true;

        }
    }
    bool CanSeePlayer()
    {
        playerDir = (gameManager.instance.player.transform.position - transform.position);
        angleToPlayer=Vector3.Angle(new Vector3(playerDir.x,0,playerDir.z),transform.forward);
        Debug.DrawRay(transform.position, playerDir);
        Debug.Log(angleToPlayer);
        RaycastHit hit;
        if (Physics.Raycast(transform.position, playerDir,out hit))
        {
            if(hit.collider.CompareTag("Player")&&angleToPlayer<=sightLine)
            {
                agent.stoppingDistance = stoppingDistanceOrig;
                agent.SetDestination(gameManager.instance.player.transform.position);
               
                if (agent.remainingDistance <= agent.stoppingDistance)
                {
                    FacePlayerAlways();
                }
                if (!isShooting)
                {
                    StartCoroutine(shoot());
                }

                return true;
            }
        }
        return false;
    }
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false ;

        }
    }
    public void TakeDamage(int dmg)
    {
        HP -= dmg;
        agent.SetDestination(gameManager.instance.player.transform.position);
        agent.stoppingDistance = 0;
        StartCoroutine(Hurt());
        if (HP <= 0)
        {

            Destroy(gameObject);
            gameManager.instance.UpdateGameGoal(-1);

        }
    }
    IEnumerator Hurt()
    {
        model.material.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        model.material.color = Color.white;
    }
    void FacePlayerAlways()
    {

        Quaternion rot = Quaternion.LookRotation(new Vector3(playerDir.x,0,playerDir.z));
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * playerFaceSpeed);
    }
    public void Flee()
    {
    }
}
