using UnityEngine;
using UnityEngine.UI;

public class HealthUnit : MonoBehaviour
{
    [field: SerializeField] public Image Image { get; set; }
    public bool IsEmpty { get; set; } = true;
}