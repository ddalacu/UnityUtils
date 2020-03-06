using System.Threading.Tasks;

public interface ISaveSystem
{
    void SaveState(ISaveState obj);

    void LoadState(ISaveState obj, out bool isDefault);

    void LoadState(ISaveState obj);
}