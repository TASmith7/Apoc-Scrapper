using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerController : MonoBehaviour, IDamage
{
    [Header("----- Components -----")]
    [SerializeField] CharacterController controller;

    [Header("----- Player Stats -----")]
    [Range(1, 10)][SerializeField] int HP;
    [Range(3, 8)] [SerializeField] float playerSpeed;
    [Range(10, 50)] [SerializeField] float gravityValue;

    [Header("----- Jetpack Stats -----")]
    [Range(1, 8)][SerializeField] float thrustPower;
    [Range(0.001f, 0.05f)] [SerializeField] float fuelConsumptionRate;
    [Range(0.0001f, 0.0003f)] [SerializeField] float fuelRefillRate;


    [Header("----- Gun Stats -----")]
    [Range(1, 10)] [SerializeField] int shootDamage;
    [Range(0.1f, 5)][SerializeField] float shootRate;
    [Range(1, 100)] [SerializeField] int shootDistance;

    

    private Vector3 playerVelocity;
    private bool groundedPlayer;

    bool isShooting; 
    Vector3 move;
    int HPOriginal;
    bool isThrusting;

    private void Start()
    {
        HPOriginal = HP;
        PlayerUIUpdate();
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
        }
    }

    void Movement()
    {
        groundedPlayer = controller.isGrounded;

        // if the player ison the ground and their velocity in y is less than 0
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
            // if we are not out of fuel
            if (gameManager.instance.jetpackFuelBar.fillAmount > 0)
            {
                // while player holds down space, give velocity in the y direction a value
                playerVelocity.y = thrustPower;
            }
            // reducing the fuel bar while the player is pressing space
            StartCoroutine(ReduceJetpackFuelUI());
        }

        // refilling the fuel bar when the player is not pressing space until it's full
        if (gameManager.instance.jetpackFuelBar.fillAmount < 1 && !isThrusting)
        {
            StartCoroutine(RefillJetpackFuelUI());
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

            // if the above^ has the component IDamage (i.e. it's not null)
            if(damageable != null)
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
        gameManager.instance.jetpackFuelBar.fillAmount -= fuelConsumptionRate;

        yield return new WaitForSeconds(0.25f);

        isThrusting = false;
    }

    IEnumerator RefillJetpackFuelUI()
    {

        yield return new WaitForSeconds(0.1f);

        if (!isThrusting)
        {
            // refilling the jetpack fuel bar
            gameManager.instance.jetpackFuelBar.fillAmount += fuelRefillRate;
        }
        
    }
}

