using UnityEngine;
using UnityEngine.SceneManagement;

public class GameUI : MonoBehaviour
{
    [SerializeField] public GameManager GameManager;
   
    public void StartGame()
    {
        GameManager.StartGame(); 
    }

  
    public void QuitGame()
    {
        Application.Quit();
        
    }
    public void MainMenu()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    
}
