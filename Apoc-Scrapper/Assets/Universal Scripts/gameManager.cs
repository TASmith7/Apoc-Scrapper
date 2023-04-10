using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class gameManager : MonoBehaviour
{
    public static gameManager instance;

    [Header("----- Player Stuff -----")]
    public GameObject player;
    public playerController playerScript;

    [Header("----- UI Stuff -----")]
    public GameObject activeMenu;
    public GameObject pauseMenu;
    public GameObject winMenu;
    public GameObject loseMenu;
    public Image HPBar;
    public TextMeshProUGUI enemiesRemainingText;
    public Image jetpackFuelBar;
    public GameObject jetpackFuelBarParent;
    public GameObject mainReticle;
    public GameObject salvageableItemReticle;

    [Header("-----Turret Stuff-----")]
    public GameObject turret;

    public int enemiesRemaining;

    public bool isPaused;
    float timeScaleOriginal;

    void Awake()
    {
        instance = this;
        player = GameObject.FindGameObjectWithTag("Player");
        playerScript = player.GetComponent<playerController>();
        timeScaleOriginal = Time.timeScale;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Cancel") && activeMenu == null)
        {
            isPaused = !isPaused;
            activeMenu = pauseMenu;
            pauseMenu.SetActive(isPaused);

            if (isPaused)
            {
                PauseState();
            }
            else
            {
                UnpauseState();
            }
        }


    }

    public void PauseState()
    {
        Time.timeScale = 0;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
    }

    public void UnpauseState()
    {
        Time.timeScale = timeScaleOriginal;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        activeMenu.SetActive(false);
        activeMenu = null;
    }

    public void UpdateGameGoal(int amount)
    {
        enemiesRemaining += amount;
        enemiesRemainingText.text = enemiesRemaining.ToString("F0");

        if(enemiesRemaining <= 0)
        {
            activeMenu = winMenu;
            activeMenu.SetActive(true);
            PauseState();
        }
    }

    public void PlayerDead()
    {
        PauseState();
        activeMenu = loseMenu;
        activeMenu.SetActive(true);
    }

    public void TurnOffJetpackUI()
    {
        jetpackFuelBarParent.SetActive(false);
    }

    public void TurnOnJetpackUI()
    {
        jetpackFuelBarParent.SetActive(true);
    }

    public void CueSalvageableReticle()
    {
        mainReticle.SetActive(false);
        salvageableItemReticle.SetActive(true);
    }

    public void CueMainReticle()
    {
        salvageableItemReticle.SetActive(false);
        mainReticle.SetActive(true);
    }
}
