using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class playerController : MonoBehaviour, IDamage
{
    [Header("-----Components-----")]
    [SerializeField] CharacterController controller;
    [Header("-----Player Stats-----")]
    [Range(3, 8)][SerializeField] float playerSpeed ;
    [Range(1, 10)][SerializeField] int playerHP;
    int HPOrig;
    [Range(5, 15)] [SerializeField] float jumpHeight;
    [Range(10,50)][SerializeField] float gravityValue ;
    [Range(1, 3)][SerializeField] int jumpMax;
    int JumpTimes;
    [Header("-----Gun Stats-----")]
    [Range(1, 10)][SerializeField] int shootDamage;
    [Range(.1f, 5)][SerializeField] float shootRate;
    [Range(1, 100)][SerializeField] int shootDistance;
    
   
    private Vector3 playerVelocity;
    private bool groundedPlayer;
    Vector3 move;
    bool isShooting;
    

    private void Start()
    {
        HPOrig = playerHP;
        PlayerUIUpdate();
    }

    void Update()
    {
        if (gameManager.instance.activeMenu == null)
        {


            movement();
            if (!isShooting & Input.GetButton("Shoot"))
                StartCoroutine(shoot());
        }
    }
    void movement()
    {
        groundedPlayer = controller.isGrounded;
        if (groundedPlayer && playerVelocity.y < 0)
        {
            playerVelocity.y = 0f;
            JumpTimes = 0;
        }

        move = (transform.right * Input.GetAxis("Horizontal")) + 
               (transform.forward * Input.GetAxis("Vertical"));

        controller.Move(move * Time.deltaTime * playerSpeed);

        

        // Changes the height position of the player..
        if (Input.GetButtonDown("Jump") && JumpTimes<jumpMax)
        {
            JumpTimes++;
            playerVelocity.y = jumpHeight;
        }

        playerVelocity.y -= gravityValue * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);
    }

    IEnumerator shoot()
    {
        
        isShooting = true;
        RaycastHit hit;
        if(Physics.Raycast(Camera.main.ViewportPointToRay(new Vector2(0.5f,0.5f)), out hit,shootDistance))
        {
            IDamage damageable = hit.collider.GetComponent<IDamage>();

            if(damageable != null)
            {
                damageable.TakeDamage(shootDamage);
            }
            

        }




        yield return new WaitForSeconds(shootRate);
        isShooting = false;
    }
    public void TakeDamage(int amt)
    {
        playerHP -= amt;
        PlayerUIUpdate();
        if(playerHP <= 0)
        {
            gameManager.instance.playerLost();
        }
    }
    void PlayerUIUpdate()
    {
        gameManager.instance.HPBar.fillAmount = (float)playerHP / (float)HPOrig;
    }
}
