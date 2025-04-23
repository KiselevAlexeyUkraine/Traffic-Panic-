using TMPro;
using UnityEngine;
using Codebase.Storage;
using Codebase.Components.Player;

namespace Codebase.Components.Ui
{
    public class UiCoins : MonoBehaviour
    {
        [SerializeField] private TMP_Text _coinText;
        [SerializeField] private PlayerCollisionHandler _collisionHandler;

        private int _coinCount;

        private void Awake()
        {
            UpdateUI();
            _collisionHandler.OnCoinCollected += HandleCoinCollected;
        }

        private void OnDestroy()
        {
            _collisionHandler.OnCoinCollected -= HandleCoinCollected;
        }

        private void HandleCoinCollected()
        {
            _coinCount++;
            CoinStorage.AddCoins(1);
            UpdateUI();
        }

        private void UpdateUI()
        {
            _coinText.text = "Coins: " + _coinCount;
        }
    }
}