using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverController : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject gameOverPanel;

    [Header("Timing")]
    [SerializeField] private float gameOverDelay = 2f;

    [Header("Player Components To Disable")]
    [SerializeField] private List<MonoBehaviour> componentsToDisable = new List<MonoBehaviour>();

    private bool hasGameOver;

    private void OnEnable()
    {
        if (Game.Instance != null && Game.Instance.PlayerOne != null)
        {
            Game.Instance.PlayerOne.OnDeath += HandleGameOver;
        }
    }

    private void OnDisable()
    {
        if (Game.Instance != null && Game.Instance.PlayerOne != null)
        {
            Game.Instance.PlayerOne.OnDeath -= HandleGameOver;
        }
    }

    private void HandleGameOver()
    {
        if (hasGameOver)
        {
            return;
        }

        hasGameOver = true;
        StartCoroutine(ShowGameOverAfterDelay());
    }

    private System.Collections.IEnumerator ShowGameOverAfterDelay()
    {
        if (gameOverDelay > 0f)
        {
            yield return new WaitForSeconds(gameOverDelay);
        }

        Time.timeScale = 0f;

        foreach (var component in componentsToDisable)
        {
            if (component != null)
            {
                component.enabled = false;
            }
        }

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }
    }

    public void RestartScene()
    {
        Time.timeScale = 1f;
        if (Game.Instance != null && Game.Instance.PlayerOne != null)
        {
            Game.Instance.PlayerOne.ResetStats();
        }
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
