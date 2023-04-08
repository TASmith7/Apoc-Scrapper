using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class gameManager : MonoBehaviour
{

    public static gameManager instance;
    [Header("-----Player Stuff-----")]
    public GameObject player;
    public playerController playerScript;
    [Header("-----UI Stuff-----")]
    public GameObject pauseMenu;
    public GameObject activeMenu;
    public GameObject winMenu;
    public GameObject loseMenu;
    public Image HPBar;
    public int enemyRem;
    public bool isPaused;
    float timeScaleOrig;
    public TextMeshProUGUI enemyRemText;


    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
        player = GameObject.FindGameObjectWithTag("Player");
        playerScript = player.GetComponent<playerController>();
        timeScaleOrig = Time.timeScale;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && activeMenu == null)
        {
            isPaused = !isPaused;
            activeMenu = pauseMenu;


            activeMenu.SetActive(isPaused);
            if (isPaused)
            {
                PauseState();
            }
            else
            {
                PlayState();
            }

        }
    }
    public void PauseState()
    {
        Time.timeScale = 0;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;

    }
    public void PlayState()
    {
        Time.timeScale = timeScaleOrig;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        activeMenu.SetActive(false);
        activeMenu = null;
    }


    public void UpdateGameGoal(int amount)
    {
        enemyRem += amount;
        enemyRemText.text = enemyRem.ToString("F0");
        if (enemyRem <= 0)
        {
            activeMenu = winMenu;
            activeMenu.SetActive(true);
            PauseState();
        }
    }
    public void playerLost()
    {
        PauseState();
        activeMenu = loseMenu;
        activeMenu.SetActive(true);

    }
}
