public interface ISaveState
{
    string SaveIdentifier { get; }

    string GetState();

    void LoadState(string state);

    void LoadDefaultState();
}