using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class playerController : MonoBehaviour, IDamage, ISalvageable
{
    [Header("----- Components -----")]
    [SerializeField] CharacterController controller;

    [Header("----- Player Stats -----")]
    [Range(1, 100)][SerializeField] int HP;
    [Range(3, 8)] [SerializeField] float playerSpeed;
    [Range(10, 50)] [SerializeField] float gravityValue;
    int playerSalvageScore;
    [Range(1, 10)][SerializeField] int salvageRange;
    [Range(0.5f, 5)][SerializeField] float salvageRate;

    [Header("----- Jetpack Stats -----")]
    [Range(1, 8)][SerializeField] float thrustPower;
    [Range(0, 1)] [SerializeField] float fuelConsumptionRate;
    [Range(0, 0.5f)] [SerializeField] float fuelRefillRate;
    [Range(1, 100)] [SerializeField] int timeToTurnOffFuelBar;


    [Header("----- Gun Stats -----")]
    [Range(1, 10)] [SerializeField] int shootDamage;
    [Range(0.1f, 5)][SerializeField] float shootRate;
    [Range(1, 100)] [SerializeField] int shootDistance;

    private Vector3 playerVelocity;
    private bool groundedPlayer;

    bool isShooting;
    bool isSalvaging;
    Vector3 move;
    int HPOriginal;
    bool isThrusting;

    float timeOfLastThrust;

    private void Start()
    {
        HPOriginal = HP;
        PlayerUIUpdate();
        playerSalvageScore = 0;
    }

    void Update()
    {
        if (gameManager.instance.activeMenu == null)
        {
            Movement();

            if (!isShooting && Input.GetButton("Shoot"))
            {
                StartCoroutine(Shoot());
            }
            
            if(!isSalvaging && Input.GetButton("Salvage"))
            {
                StartCoroutine(Salvage());
            }
        }

    }

    void FixedUpdate()
    {
        RaycastHit hit;

        if (Physics.Raycast(Camera.main.ViewportPointToRay(new Vector2(0.5f, 0.5f)), out hit, shootDistance))
        {
            // if the object we are looking at is salvageable
            ISalvageable salvageable = hit.collider.GetComponent<ISalvageable>();

            // if the above^ has the component ISalvageable (i.e. it's not null)
            if (salvageable != null)
            {
                // change the reticle to salvageable reticle
                gameManager.instance.CueSalvageableReticle();
            }
            else
            {
                // else if what we are looking at isn't salvageable, change/keep the reticle to main reticle
                gameManager.instance.CueMainReticle();
            }
        }
        
    }

    void Movement()
    {
        groundedPlayer = controller.isGrounded;

        // if the player is on the ground and their velocity in y is less than 0
        if (groundedPlayer && playerVelocity.y < 0)
        {
            // set the vertical velocity to 0
            playerVelocity.y = 0f;
        }

        // movement on the x and z axes
        move = (transform.right * Input.GetAxis("Horizontal")) + 
               (transform.forward * Input.GetAxis("Vertical"));

        // calling the builtin move method on the player controller with frame rate independence
        controller.Move(playerSpeed * Time.deltaTime * move);

        if (Input.GetButton("Jump"))
        {
            // turn on our jetpack fuel bar
            gameManager.instance.TurnOnJetpackUI();

            // if we are not out of fuel
            if (gameManager.instance.jetpackFuelBar.fillAmount > 0)
            {
                // while player holds down space, give velocity in the y direction a value
                playerVelocity.y = thrustPower;

                timeOfLastThrust = Time.fixedTime;
            }
            // reducing the fuel bar while the player is pressing space
            StartCoroutine(ReduceJetpackFuelUI());
        }

        // refilling the fuel bar when the player is not pressing space until it's full
        if (gameManager.instance.jetpackFuelBar.fillAmount < 1 && !isThrusting)
        {
            StartCoroutine(RefillJetpackFuelUI());
        }

        // if the elapsed time at our last thrust minus our current time elapsed is greater than or equal to 2 seconds
        if(Time.fixedTime - timeOfLastThrust >= timeToTurnOffFuelBar)
        {
            // turn off jetpack fuel UI
            gameManager.instance.TurnOffJetpackUI();
        }


        // ensuring our players y velocity take gravity into effect
        playerVelocity.y -= gravityValue * Time.deltaTime;

        
        controller.Move(playerVelocity * Time.deltaTime);
    }

    IEnumerator Shoot()
    {
        isShooting = true;
        
        // we use this raycast to return the position of where our raycast hits
        RaycastHit hit; 
        
        // If the ray going from the middle of our screen hits something, "out" the position of where it hits in our 'hit' variable,
        // and it will shoot the specified distance via our variable
        if(Physics.Raycast(Camera.main.ViewportPointToRay(new Vector2(0.5f, 0.5f)), out hit, shootDistance))
        {
            // if the object we hit contains the IDamage interface
            IDamage damageable = hit.collider.GetComponent<IDamage>();

            // if the above^ has the component IDamage (i.e. it's not null), and it is not the player
            if(damageable != null && hit.collider.tag != "Player")
            {
                // take damage from the damageable object
                damageable.TakeDamage(shootDamage);
            }
        }

        // The yield return will wait for the specified amount of seconds
        // before moving on to the next line. It does NOT exit the method. 
        yield return new WaitForSeconds(shootRate);
        isShooting = false;

    }

    IEnumerator Salvage()
    {
        isSalvaging = true;

        RaycastHit hit;

        if(Physics.Raycast(Camera.main.ViewportPointToRay(new Vector2(0.5f, 0.5f)), out hit, salvageRange))
        {
            // if the object we clicked on contains the ISalvageable interface
            ISalvageable salvageable = hit.collider.GetComponent<ISalvageable>();

             // if the object is salvageable
            if (salvageable != null)
            {
                SalvageObject(hit.collider.gameObject);
            }
        }

        yield return new WaitForSeconds(salvageRate);

        isSalvaging = false;
    }

    public void TakeDamage(int amount)
    {
        HP -= amount;
        PlayerUIUpdate();

        if(HP <= 0)
        {
            gameManager.instance.PlayerDead();
        }
    }

    void PlayerUIUpdate()
    {
        // updating the players health bar
        gameManager.instance.HPBar.fillAmount = (float) HP / (float) HPOriginal;
    }

    IEnumerator ReduceJetpackFuelUI()
    {  
        // this bool will be helpful for future development of thrusting capabilities. It currently has no effective use
        isThrusting = true;

        // stopping the refill coroutine while thrusting
        StopCoroutine(RefillJetpackFuelUI());

        // reducing the jetpack fuel bar
        gameManager.instance.jetpackFuelBar.fillAmount -= fuelConsumptionRate * Time.deltaTime;

        yield return new WaitForSeconds(0.25f);

        isThrusting = false;
    }

    IEnumerator RefillJetpackFuelUI()
    {
        yield return new WaitForSeconds(0.1f);

        if (!isThrusting)
        {
            // refilling the jetpack fuel bar
            gameManager.instance.jetpackFuelBar.fillAmount += fuelRefillRate * Time.deltaTime;
        }
        
    }

    public void SalvageObject(GameObject objectToSalvage)
    {
        // updating salvage score based on the objects salvage value assigned in inspector
        playerSalvageScore += objectToSalvage.GetComponent<salvageableObject>().salvageValue;

        // destroying object
        Destroy(objectToSalvage);

        // updating salvage score UI
        gameManager.instance.UpdateSalvageScore(playerSalvageScore);
    }
}

