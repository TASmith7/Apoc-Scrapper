using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.AI;

public class ratAI : MonoBehaviour, IDamage
{
    [Header("----- Components -----")]
    [SerializeField] Renderer model;
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Transform headPos;
    [SerializeField] Transform shootPos;
    [SerializeField] SphereCollider ratCollWake;
    /*[SerializeField] SphereCollider ratCollBite*/
    //[SerializeField] Rigidbody rb;

    [Header("----- Rat Stats -----")]
    [SerializeField] int HP;
    [SerializeField] int playerFaceSpeed;
    [SerializeField] int sightAngle;
    [Range(10, 200)][SerializeField] float radiusSleep;
    [Range(10,1000)][SerializeField] float radiusActive;
    [SerializeField] float activeRadius;

    [Header("----- Rat Bite Stats -----")]
    [Range(1, 10)][SerializeField] int biteDamage;
    [Range(0.1f, 5)][SerializeField] float biteRate;
    [SerializeField] float biteDistance;
    //[SerializeField] GameObject attack;
    //[SerializeField] float attackSpeed;

    //[Header("----- Rat Jump Stats (WIP)-----")]
    //[SerializeField] float jumpHeight;
    //[SerializeField] float jumpDistance;
    //[SerializeField] float jumpAmt;

    Vector3 playerDirection;
    bool playerInRange;
    float angleToPlayer;
    float distance;
    bool isShooting;
    
     
     
    




    void Start()
    {
        
        activeRadius = radiusSleep;
        agent.stoppingDistance = biteDistance;
        
        //rb = GetComponent<Rigidbody>();

    }


    void Update()
    {
        // we only want to activate our AI if the player is within our enemy's range (or trigger)
        if (playerInRange)
        {
            CanSeePlayer();
            
        }

    }
    

    bool CanSeePlayer()
    {
        

        // this tells us what direction our player is in relative to our enemy
        playerDirection = (gameManager.instance.player.transform.position - headPos.position);

        // this calculates the angle between where our player is and where we (the enemy) are looking
        angleToPlayer = Vector3.Angle(new Vector3(playerDirection.x,0, playerDirection.z), transform.forward);

       

        // this returns the info of WHAT is HIT by the raycast
        RaycastHit hit;

        // this will shoot the raycast in the direction of the player at all times. Our 'out' variable is what object is getting hit
        if (Physics.Raycast(headPos.position, playerDirection, out hit))
        {
            // if the object we are hitting is the player, AND the angle to our player is within our sight angle
            if (hit.collider.CompareTag("Player") && angleToPlayer <= sightAngle)
            {

                

                // this gets the enemy to move in the direction of our player
                agent.SetDestination(gameManager.instance.player.transform.position);

                if (agent.remainingDistance <agent.stoppingDistance)
                    FacePlayer();

                if (!isShooting)
                    StartCoroutine(Shoot());

                return true;
            }
        }

        return false;
    }

    IEnumerator Shoot()
    {
        isShooting = true;

        // we use this raycast to return the position of where our raycast hits
        RaycastHit hit;

        // If the ray going from the middle of our screen hits something, "out" the position of where it hits in our 'hit' variable,
        // and it will shoot the specified distance via our variable
        if (Physics.Raycast(headPos.position, playerDirection, out hit))
        {
            // if the object we hit contains the IDamage interface
            IDamage damageable = hit.collider.GetComponent<IDamage>();

            // if the above^ has the component IDamage (i.e. it's not null), and it is not the player
            if (damageable != null && hit.collider.tag != "Enemy")
            {
                //take damage if in range
                if (playerDirection.magnitude<biteDistance)
                    damageable.TakeDamage(biteDamage);
            }
        }

        yield return new WaitForSeconds(biteRate);
        isShooting = false;
    }

    // any object that ENTERS the collider
    void OnTriggerEnter(Collider other)
    {


        if (other.CompareTag("Player"))
        {
            ratCollWake.radius = radiusActive;
            activeRadius = ratCollWake.radius;
            playerInRange = true;
        }
        
            
        
    }
    //void Jump(SphereCollider jumpColl)
    //{
    //    if (jumpColl.CompareTag("Player"))
    //    {
    //        // Calculate the direction towards the player
    //        Vector3 targetDirection = (jumpColl.transform.position - transform.position).normalized;

    //        // Calculate the force vector considering the jump height, jump distance, and target direction
    //        Vector3 force = new Vector3(targetDirection.x * jumpDistance, jumpHeight, targetDirection.z * jumpDistance);

    //        // Apply the force to the Rigidbody
    //        rb.AddForce(force, ForceMode.Impulse);
    //    }
    //}
    // any object that EXITS the collider
    public void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            
            playerInRange = false;
        }
    }
    public void TakeDamage(int amount)
    {
        HP -= amount;

        // if we (the enemy) gets shot, we should know where the player shot us from
        agent.SetDestination(gameManager.instance.player.transform.position);

        // remove the stopping distance so that the enemy goes right to the spot where we shot him from, rather than stopping with the stopping distance
        agent.stoppingDistance = 0;

        StartCoroutine(FlashColor());

        if (HP <= 0)
        {
            
            Destroy(gameObject);
        }
    }

    IEnumerator FlashColor()
    {
        model.material.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        model.material.color = Color.white;
    }
   
    void FacePlayer()
    {
        Quaternion rot = Quaternion.LookRotation(new Vector3(playerDirection.x, playerDirection.y, playerDirection.z));
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * playerFaceSpeed);
    }
}
