using Zenject;

public interface ISubscriber<T>
{
    public void Subscribe();
    public void Unsubscribe();
}

public interface IActivate<T>
{
    public void Activate();
    public void Deactivate();
}

public interface IInitialize<T>
{
    public void Initialize();
}

public class InitializeInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        
    }
}