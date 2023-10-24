using UnityEngine;
using Zenject;

[RequireComponent(typeof(PlayerUnitMovementComponent))]
public class PlayerUnit : MonoBehaviour
{
    [Inject] private IInitialize<PlayerUnitMovementComponent> _playerUnitMovementComponentInitialize;
    [Inject] private IActivate<PlayerUnitMovementComponent> _playerUnitMovementComponentActivate;
    private void Start()
    {
        _playerUnitMovementComponentInitialize.Initialize();
        _playerUnitMovementComponentActivate.Activate();
    }
}