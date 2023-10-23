using System.Collections.Generic;
using UnityEngine;

public class MonoCache : MonoBehaviour
{
    public static List<MonoCache> AllUpdate = new(10001);
    public static List<MonoCache> AllFixedUpdate = new(10001);
    public static List<MonoCache> AllLateUpdate = new(10001);

    private void OnEnable() => AllUpdate.Add(this);

    private void OnDisable() => AllUpdate.Remove(this);

    private void OnDestroy() => AllUpdate.Remove(this);

    protected void AddFixedUpdate() => AllFixedUpdate.Add(this);
    
    protected void RemoveFixedUpdate() => AllFixedUpdate.Remove(this);
    
    protected void AddLateUpdate() => AllLateUpdate.Add(this);
    
    protected void RemoveLateUpdate() => AllLateUpdate.Remove(this);

    public void Tick() => Run();
    
    public virtual void Run() { }
    
    public void FixedTick() => FixedRun();
    
    public virtual void FixedRun() { }
    
    public void LateTick() => LateRun();
    
    public virtual void LateRun() { }
}
