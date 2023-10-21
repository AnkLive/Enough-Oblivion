using UnityEngine;

public class GlobalUpdate : MonoBehaviour
{
    private void Update()
    {
        for (int i = 0; i < MonoCache.AllUpdate.Count; i++) MonoCache.AllUpdate[i].Tick();
    }
    
    private void FixedUpdate()
    {
        for (int i = 0; i < MonoCache.AllFixedUpdate.Count; i++) MonoCache.AllFixedUpdate[i].FixedTick();
    }
    
    private void LateUpdate()
    {
        for (int i = 0; i < MonoCache.AllLateUpdate.Count; i++) MonoCache.AllLateUpdate[i].LateTick();
    }
}