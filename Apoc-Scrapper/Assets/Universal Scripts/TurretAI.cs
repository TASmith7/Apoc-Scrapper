using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TurretAI : MonoBehaviour, IDamage
{
    [Header("----- Components -----")]
    [SerializeField] Renderer model;
    [SerializeField] Transform headPos;
    [SerializeField] Transform shootPos;
    //[SerializeField] Rigidbody rigidBody;

    [Header("----- Enemy Stats -----")]
    [SerializeField] int HP;
    [SerializeField] int playerFaceSpeed;
    [SerializeField] int sightAngle;

    [Header("----- Gun Stats -----")]
    
    [Range(0.1f, 5)][SerializeField] float shootRate;
    [Range(1, 100)][SerializeField] int shootDistance;
    [SerializeField] GameObject TurretBullet;
    [SerializeField] int bulletSpeed;

    Vector3 playerDirection;
    bool playerInRange;
    float angleToPlayer;
    bool isShooting;
    
    

    void Start()
    {
        

        // caching the original stopping distance that we set
        
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
        angleToPlayer = Vector3.Angle(new Vector3(playerDirection.x, playerDirection.y, playerDirection.z), transform.forward);

        Debug.DrawRay(headPos.position, playerDirection, Color.red);

        // this returns the info of WHAT is HIT by the raycast
        RaycastHit hit;

        // this will shoot the raycast in the direction of the player at all times. Our 'out' variable is what object is getting hit
        if(Physics.Raycast(headPos.position, playerDirection, out hit))
        {
            // if the object we are hitting is the player, AND the angle to our player is within our sight angle
            if(hit.collider.CompareTag("Player") && angleToPlayer <= sightAngle)
            {
                
                

                // this gets the enemy to move in the direction of our player
                //agent.SetDestination(gameManager.instance.player.transform.position);

               
                
                
                    FacePlayer();
                

                if (!isShooting)
                {
                    // if the player is within the sight range, which we check in update, and we are not already shooting (just so we don't shoot multiple times at once), start shooting
                    StartCoroutine(Shoot());
                }


                return true;
            }
        }

        return false;
    }

    IEnumerator Shoot()
    {
        isShooting = true;
        
        // this creates a reference to an instantiated bullet, first parameter = what youre instantiating, second = where it's instantiating from on the enemy
        // (which we'll set in unity), third = the bullets orientation (doesn't really matter but it's necessary)
        GameObject bulletClone = Instantiate(TurretBullet, shootPos.position, TurretBullet.transform.rotation);

        // this will set the bullets velocity via the rigidbody component of the game object
        bulletClone.GetComponent<Rigidbody>().velocity = transform.forward * bulletSpeed;

        // our wait time which is going to be our defined shootRate
        yield return new WaitForSeconds(shootRate);

        isShooting = false; 
    }

    // any object that ENTERS the collider
    void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

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
        //agent.SetDestination(gameManager.instance.player.transform.position);

        // remove the stopping distance so that the enemy goes right to the spot where we shot him from, rather than stopping with the stopping distance
        //agent.stoppingDistance = 0;

        StartCoroutine(FlashColor());

        if(HP <= 0)
        {
            
            Destroy(gameObject);
            //Rigidbody.Instantiate(rigidBody);
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
