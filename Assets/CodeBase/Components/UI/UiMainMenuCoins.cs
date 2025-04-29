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
            UpdateCoinsTotall();
        }

        public void UpdateCoinsTotall()
        {
            _coinText.text = "Coins: " + CoinStorage.GetCoins();
        }
    }
}