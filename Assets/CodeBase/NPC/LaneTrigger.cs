using UnityEngine;

namespace Codebase.Components.Player
{
    public class LaneTrigger : MonoBehaviour
    {
        [SerializeField] private Lane selectedLane = Lane.Lane3;

        public Lane SelectedLane => selectedLane;
    }
}