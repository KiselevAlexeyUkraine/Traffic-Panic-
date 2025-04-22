using UnityEngine;
using Codebase.Components.Ui.Pages;
using Codebase.Components.Player;
using Zenject;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private PageSwitcher _pageSwitcher;
    [SerializeField] private PlayerMovement _playerMovement;
    [SerializeField] private CursorToggle _cursorToggle;

    [Inject]
    private void Construct(CursorToggle cursorToggle)
    {
        _cursorToggle = cursorToggle;
    }

    private void EndLevelFailure()
    {
        Debug.Log("Уровень завершён. Игрок погиб.");
        _cursorToggle.Enable();
        _playerMovement.enabled = false;
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
