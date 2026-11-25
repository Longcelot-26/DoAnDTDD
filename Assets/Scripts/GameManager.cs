using UnityEngine;
using UnityEngine.UI;




public class GameManager : MonoBehaviour
{
   [SerializeField] public GameObject Mainmenu;
   [SerializeField] public GameObject gameOverMenu;
   [SerializeField] public GameObject Pausemenu;
   [SerializeField] public GameObject GameUI;
   [SerializeField] private AudioManager audioManager;
    void Start()
    {
        MainMenu();
        audioManager.StopDefaultSound();
    }
    public void MainMenu()
    {
        Mainmenu.SetActive(true);
        gameOverMenu.SetActive(false);
        Pausemenu.SetActive(false);
        Time.timeScale = 0f;
    }
    public void GameOverMenu()
    {
            gameOverMenu.SetActive(true);
            Mainmenu.SetActive(false);
            Pausemenu.SetActive(false);
            Time.timeScale = 0f;
    }
    public void Pause()
    {
        Pausemenu.SetActive(true);
        Mainmenu.SetActive(false);
        gameOverMenu.SetActive(false);
        Time.timeScale = 0f;
    }
    public void StartGame()
    {
        Mainmenu.SetActive(false);
        gameOverMenu.SetActive(false);
        Pausemenu.SetActive(false);
        Time.timeScale = 1f;
        audioManager.PlayDefaultSound();
    }
    public void Resume()
    {
        Pausemenu.SetActive(false);
        Mainmenu.SetActive(false);
        gameOverMenu.SetActive(false);
        Time.timeScale = 1f;
    }
}
