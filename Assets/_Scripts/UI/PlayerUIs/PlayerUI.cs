using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
namespace _Scripts.UI.PlayerUIs
{
    public class PlayerUI : MonoBehaviour
    {
        [FormerlySerializedAs("healthSlider")]
        public LifeUIController lifeUIController;
        
        [FormerlySerializedAs("globalItemUI")]
        public ItemsUI itemsUI;
    }
}
