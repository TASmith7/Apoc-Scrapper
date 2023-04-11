using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class droneAI : MonoBehaviour, IDamage
{
    [Header("----- Components -----")]
    [SerializeField] Renderer model;
    [SerializeField] NavMeshAgent agent;
    //[SerializeField] Rigidbody rb;
    // allows us to cast the ray from anywhere but we choice to cast it from the head
    [SerializeField] Transform headPos;
    [SerializeField] Transform shootPos;
    //[SerializeField] Transform shootPos2;
    //[SerializeField] Transform speed;

    [Header("----- Enemy Stats -----")]
    // Health Points
    [SerializeField] int HP;
    [SerializeField] int playerFaceSpeed;
    [SerializeField] int sightAngle;



    //[Header("----- Enemy Gun -----")]

    //Lecture three
    [Header("----- Gun Stats -----")]
    [Range(1, 10)][SerializeField] int shootDamage;
    [Range(0.01f, 5)][SerializeField] float shootRate;
    [Range(1, 100)][SerializeField] int shootDist;
    [SerializeField] GameObject bullet;
    [SerializeField] int bulletSpeed;
   // [SerializeField] SphereCollider radius;

    //direction of the player is in
    Vector3 playerDir;
    bool playerInRange;
    float angleToPlayer;
    bool isShooting;
    float stoppingDistOrig;
    

    // Start is called before the first frame update
    void Start()
    {

        //gameManager.instance.updatGameGoal(1);
        stoppingDistOrig = agent.stoppingDistance;
    }

    // Update is called once per frame
    void Update()
    {
        if (playerInRange)
        {
            canSeePlayer();
        }
    }

    bool canSeePlayer()
    {
        playerDir = (gameManager.instance.player.transform.position - headPos.position);
        angleToPlayer = Vector3.Angle(new Vector3(playerDir.x, playerDir.y, playerDir.z), gameManager.instance.player.transform.position);

        Debug.DrawRay(headPos.position, playerDir, Color.red);
        Debug.Log(angleToPlayer);

        RaycastHit hit;
        if (Physics.Raycast(headPos.position, playerDir, out hit))
        {
            if (hit.collider.CompareTag("Player") && angleToPlayer <= sightAngle)
            {
                // if enemy see you he stopping distance will go back to original value
                agent.stoppingDistance = stoppingDistOrig;

                // add force to the drone to make it fly towards the player
                // changes the velocity by the value of force * DT / mass
                // playerDir help the object move in the air by 5 units at a time in the direction of the player with ForceMode.Force adding velocity over time 
                //if(playerDir.magnitude > 5 )
                //{
                //   rb.AddForce(playerDir * 5f, ForceMode.Acceleration);
                //}

                // has enemy following player
                // maybe this one will turn his head
                agent.SetDestination(gameManager.instance.player.transform.position);

                if (agent.remainingDistance <= agent.stoppingDistance)
                {
                    facePlayer();
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

    IEnumerator shoot()
    {
        isShooting = true;
        GameObject bulletClone = Instantiate(bullet, shootPos.position, bullet.transform.rotation);
        //GameObject bulletClone2 = Instantiate(bullet, shootPos2.position, bullet.transform.rotation);
        bulletClone.GetComponent<Rigidbody>().velocity = transform.forward * bulletSpeed;
        yield return new WaitForSeconds(shootRate);
        isShooting = false;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }

    public void TakeDamage(int amount)
    {
        HP -= amount;
        //rb.AddForce(playerDir * 5f, ForceMode.Impulse);
        agent.SetDestination(gameManager.instance.player.transform.position);
        agent.stoppingDistance = 0;


        StartCoroutine(flashColor());

        if (HP <= 0)
        {
            //gameManager.instance.updatGameGoal(-1);
            Destroy(gameObject);
        }
    }

    IEnumerator flashColor()
    {
        model.material.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        model.material.color = Color.white;
    }

    void facePlayer()
    {
        Quaternion rot = Quaternion.LookRotation(new Vector3(playerDir.x, playerDir.y, playerDir.z));
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * playerFaceSpeed);
    }
}