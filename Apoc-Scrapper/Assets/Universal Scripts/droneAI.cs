using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

// IDamage will show error/squigglies if takeDamage function is not set to public
public class droneAI : MonoBehaviour, IDamage
{
    [Header("----- Components -----")]
    [SerializeField] Renderer model;
    [SerializeField] NavMeshAgent agent;
    // allows us to cast the ray from anywhere but we choice to cast it from the head
    [SerializeField] Transform headPos;
    [SerializeField] Transform shootPos;

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
        // sets the stoppingDistOrig to the current stopping distance
        stoppingDistOrig = agent.stoppingDistance;
    }

    // Update is called once per frame
    void Update()
    {
        // only start following and shooting if player is in range of enemy
        if (playerInRange)
        {
            canSeePlayer();
        }
    }

    bool canSeePlayer()
    {
        //player direction
        playerDir = (gameManager.instance.player.transform.position - headPos.position);
        angleToPlayer = Vector3.Angle(new Vector3(playerDir.x, 0, playerDir.z), transform.forward); // but i had change transform.forward to headPos.forward and enemy was only able to see me on his left side and my right
        //draws the raysfrom enemy to player
        Debug.DrawRay(headPos.position, playerDir, Color.red);
        Debug.Log(angleToPlayer);

        RaycastHit hit;
        if (Physics.Raycast(headPos.position, playerDir, out hit))
        {
            //check to see if ray cast hit the player and the angle of the player is less than something we set 
            if (hit.collider.CompareTag("Player") && angleToPlayer <= sightAngle)
            {
                // if enemy see you he stopping distance will go back to original value
                agent.stoppingDistance = stoppingDistOrig;
                // has enemy following player
                // maybe this one will turn his head
                agent.SetDestination(gameManager.instance.player.transform.position);
                // how far he is from destination
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
        // to reference a bullet
        GameObject bulletClone = Instantiate(bullet, shootPos.position, bullet.transform.rotation);
        // to give bullet a velocity
        bulletClone.GetComponent<Rigidbody>().velocity = transform.forward * bulletSpeed;
        yield return new WaitForSeconds(shootRate);
        isShooting = false;
    }

    // syntax to be able to use Trigger function in the Sphere Collider
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }
    // When the player leaves the enemies range this is what the enemy will do
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }


    //needs to be public and the exact same name as IDamage script
    public void TakeDamage(int amount)
    {
        // subtracts damage taken from HP
        HP -= amount;
        // when shot the enemy will turn towards the shooter
        agent.SetDestination(gameManager.instance.player.transform.position);
        agent.stoppingDistance = 0;

        StartCoroutine(flashColor());

        if (HP <= 0)
        {
            //gameManager.instance.updatGameGoal(-1);
            //die
            Destroy(gameObject);
        }
    }

    // Function Flashes enemy red 
    IEnumerator flashColor()
    {
        // turns enemy red
        model.material.color = Color.red;
        // waits a few seconds
        yield return new WaitForSeconds(0.1f);
        // returns enemy back to white
        model.material.color = Color.white;
    }

    // fixes Bug that enemy does not turn when not moving
    void facePlayer()
    {
        //call when condition is meet 
        //dont want the enemy to take in the consideration of the players y
        // Quaternion rot = Quaternion.LookRotation(playerDir);
        // may need the y consideration for flying players
        Quaternion rot = Quaternion.LookRotation(new Vector3(playerDir.x, 0, playerDir.z));

        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * playerFaceSpeed);
    }
}
