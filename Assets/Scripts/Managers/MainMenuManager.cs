using UnityEngine;

public class MainMenuManager : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject howToPlayPanel;
    [SerializeField] private GameObject creditsPanel;
    [SerializeField] private GameObject mainMenuPanel;

    private void Awake()
    {
        howToPlayPanel.SetActive(false);
        creditsPanel.SetActive(false);
    }

    public void OnPlayClicked()
    {
        mainMenuPanel.SetActive(false);
        UnityEngine.SceneManagement.SceneManager.LoadScene("Gameplay");
    }

    public void OnHowToPlayClicked()
    {
        mainMenuPanel.SetActive(false);
        howToPlayPanel.SetActive(true);
    }

    public void OnCreditsClicked()
    {
        mainMenuPanel.SetActive(false);
        creditsPanel.SetActive(true);
    }

    public void OnQuitClicked()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    public void OnBackFromHowToPlay()
    {
        howToPlayPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
    }

    public void OnBackFromCredits()
    {
        creditsPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
    }
}
