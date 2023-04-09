using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerControllerBasic : MonoBehaviour /*IDamage*/
{
    [Header("----- Components -----")]
    [SerializeField] CharacterController controller;

    [Header("----- Player Stats -----")]
    [Range(1, 10)][SerializeField] int HP;
    [Range(3, 8)] [SerializeField] float playerSpeed;
    [Range(8, 25)] [SerializeField] float jumpHeight;
    [Range(10, 50)] [SerializeField] float gravityValue;
    [Range(1, 3)] [SerializeField] int jumpsMax;

    [Header("----- Gun Stats -----")]
    [Range(1, 10)] [SerializeField] int shootDamage;
    [Range(0.1f, 5)][SerializeField] float shootRate;
    [Range(1, 100)] [SerializeField] int shootDistance;

    int jumpedTimes;
    private Vector3 playerVelocity;
    private bool groundedPlayer;

    bool isShooting; 
    Vector3 move;
    int HPOriginal;

    private void Start()
    {
        //HPOriginal = HP;
        //PlayerUIUpdate();
    }

    void Update()
    {
        //if (gameManager.instance.activeMenu == null)
        //{
        //    Movement();

        //    if (!isShooting && Input.GetButton("Shoot"))
        //    {
        //        StartCoroutine(Shoot());
        //    }
        //}
        Movement();
    }

    void Movement()
    {
        groundedPlayer = controller.isGrounded;
        if (groundedPlayer && playerVelocity.y < 0)
        {
            playerVelocity.y = 0f;
            jumpedTimes = 0;
        }


        move = (transform.right * Input.GetAxis("Horizontal")) + 
               (transform.forward * Input.GetAxis("Vertical"));

        controller.Move(playerSpeed * Time.deltaTime * move);

        if (Input.GetButtonDown("Jump") && jumpedTimes < jumpsMax)
        {
            jumpedTimes++;
            playerVelocity.y = jumpHeight;
        }

        playerVelocity.y -= gravityValue * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);
    }

    //IEnumerator Shoot()
    //{
    //    isShooting = true;
        
    //    // we use this raycast to return the position of where our raycast hits
    //    RaycastHit hit; 
        
    //    // If the ray going from the middle of our screen hits something, "out" the position of where it hits in our 'hit' variable,
    //    // and it will shoot the specified distance via our variable
    //    if(Physics.Raycast(Camera.main.ViewportPointToRay(new Vector2(0.5f, 0.5f)), out hit, shootDistance))
    //    {
    //        // if the object we hit contains the IDamage interface
    //        IDamage damageable = hit.collider.GetComponent<IDamage>();

    //        // if the above^ has the component IDamage (i.e. it's not null)
    //        if(damageable != null)
    //        {
    //            // take damage from the damageable object
    //            damageable.TakeDamage(shootDamage);
    //        }
    //    }

    //    // The yield return will wait for the specified amount of seconds
    //    // before moving on to the next line. It does NOT exit the method. 
    //    yield return new WaitForSeconds(shootRate);
    //    isShooting = false;

    //}

    //public void TakeDamage(int amount)
    //{
    //    HP -= amount;
    //    PlayerUIUpdate();

    //    if(HP <= 0)
    //    {
    //        gameManager.instance.PlayerDead();
    //    }
    //}

//    void PlayerUIUpdate()
//    {
//        gameManager.instance.HPBar.fillAmount = (float) HP / (float) HPOriginal;
//    }
}

