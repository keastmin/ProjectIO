using Fusion;

public class NetworkSystemBase : NetworkBehaviour
{
    public static NetworkSystemBase Instance { get; private set; }

    public virtual void SetUp() { }
    public virtual void TearDown() { }
}