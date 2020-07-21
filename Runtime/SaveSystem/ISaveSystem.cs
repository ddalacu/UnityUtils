using System.Threading.Tasks;

public interface ISaveSystem
{
    void SaveState(ISaveState obj);

    void LoadState(ISaveState obj, out bool isDefault);

    void LoadState(ISaveState obj);
}

public static class SaveSystemExtensions
{
    public static void SaveAll(this ServiceContainer container)
    {
        var service = container.GetService<ISaveSystem>();
        foreach (var item in container.GetServices())
        {
            if (item is ISaveState state)
            {
                service.SaveState(state);
            }
        }
    }
}
