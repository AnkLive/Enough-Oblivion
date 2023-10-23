using UnityEngine;

[RequireComponent(typeof(PlayerUnitMovementComponent))]
public class PlayerUnit : MonoBehaviour
{
    private PlayerUnitMovementComponent _playerUnitMovementComponent;
    private void Start()
    {
        _playerUnitMovementComponent = gameObject.GetComponent<PlayerUnitMovementComponent>();
        _playerUnitMovementComponent.Initialize();
        _playerUnitMovementComponent.Activate();
    }
}