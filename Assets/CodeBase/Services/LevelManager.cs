using UnityEngine;
using Codebase.Components.Ui.Pages;
using Codebase.Components.Player;
using Zenject;
using System.Collections;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private PageSwitcher _pageSwitcher;
    [SerializeField] private PlayerMovement _playerMovement;
    [SerializeField] private CursorToggle _cursorToggle;
    [SerializeField] private PlayerCollisionHandler _playerCollisionHandler;

    [Inject]
    private void Construct(CursorToggle cursorToggle)
    {
        _cursorToggle = cursorToggle;
    }

    private void OnEnable()
    {
        _playerCollisionHandler.OnPlayerDeath += HandlePlayerDeath;
    }

    private void OnDisable()
    {
        _playerCollisionHandler.OnPlayerDeath -= HandlePlayerDeath;
    }

    private void HandlePlayerDeath()
    {
        Debug.Log("Игрок погиб. Завершаем уровень...");
        _playerMovement.enabled = false;
        StartCoroutine(EndLevelFailureWithDelay());
    }

    private IEnumerator EndLevelFailureWithDelay()
    {
        yield return new WaitForSeconds(1f);
        EndLevelFailure();
    }

    private void EndLevelFailure()
    {
        Debug.Log("Уровень завершён. Игрок погиб.");
        _cursorToggle.Enable();
        Time.timeScale = 0f;
        _pageSwitcher.Open(PageName.Failed);
    }

    public void EndLevelVictory()
    {
        Debug.Log("Уровень завершён. Победа!");
        _cursorToggle.Enable();
        _playerMovement.enabled = false;
        _pageSwitcher.Open(PageName.Complete);
    }
}
