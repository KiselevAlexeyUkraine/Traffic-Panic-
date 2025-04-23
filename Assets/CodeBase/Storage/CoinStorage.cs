using UnityEngine;

namespace Codebase.Storage
{
    public static class CoinStorage
    {
        private const string CoinsKey = "Coins";

        public static int GetCoins() => PlayerPrefs.GetInt(CoinsKey, 0);

        public static void AddCoins(int amount)
        {
            int current = GetCoins();
            PlayerPrefs.SetInt(CoinsKey, current + amount);
            PlayerPrefs.Save();
        }

        public static void ResetCoins()
        {
            PlayerPrefs.SetInt(CoinsKey, 0);
            PlayerPrefs.Save();
        }
    }
}