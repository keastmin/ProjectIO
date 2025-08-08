using Fusion;

public class PlayerRole : NetworkBehaviour
{
    public static PlayerRole Instance { get; private set; }

    [Networked] public NetworkDictionary<PlayerPosition, PlayerRef> Role { get; }

    bool isInitialized;

    public bool IsRunner => Role[PlayerPosition.Runner] == Runner.LocalPlayer;
    public bool IsBuilder => Role[PlayerPosition.Builder] == Runner.LocalPlayer;

    void Awake()
    {
        Instance = this;
    }

    public override void Spawned()
    {
        Initialize();
    }

    public void Initialize()
    {
        if (isInitialized) { return; }
        isInitialized = true;

        if (Object.HasStateAuthority)
        {
            foreach (var player in Runner.ActivePlayers)
            {
                var isHostPlayer = player == Runner.LocalPlayer;
                if (isHostPlayer)
                {
                    var hostPosition = Runner.SessionInfo.Properties["IsHostBuilder"] ? PlayerPosition.Builder : PlayerPosition.Runner;
                    Role.Add(hostPosition, player);
                }
                else
                {
                    var clientPosition = Runner.SessionInfo.Properties["IsHostBuilder"] ? PlayerPosition.Runner : PlayerPosition.Builder;
                    Role.Add(clientPosition, player);
                }
            }
        }
    }
}