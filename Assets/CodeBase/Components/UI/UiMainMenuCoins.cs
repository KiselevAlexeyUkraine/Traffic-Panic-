using TMPro;
using UnityEngine;
using Codebase.Storage;

namespace Codebase.Components.Ui
{
    public class UiMainMenuCoins : MonoBehaviour
    {
        [SerializeField] private TMP_Text _coinText;

        private void Start()
        {
            _coinText.text = "Coins: " + CoinStorage.GetCoins();
        }
    }
}