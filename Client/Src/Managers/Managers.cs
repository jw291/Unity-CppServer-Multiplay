using Unity.VisualScripting;
using UnityEngine;

public class Managers : MonoBehaviour
{
    static Managers instance;
    public static Managers Instance => instance;

    [SerializeField] private NetworkManager networkManager;
    [SerializeField] private FlockManagerBase flockManager;
    [SerializeField] private PoolManager poolManager;
    [SerializeField] private SpawnManager spawnManager;
    [SerializeField] private GameManager gameManager;
    [SerializeField] private SkillManager skillManager;
    [SerializeField] private PlayerManager playerManager;
    [SerializeField] private DataManager dataManager;
    [SerializeField] private UIManager uiManager;
    [SerializeField] private ShopManager shopManager;
    [SerializeField] private InventoryManager inventoryManager;
    public NetworkManager Network => networkManager;
    public FlockManagerBase Flock => flockManager;
    public GameManager Game => gameManager;
    public SpawnManager Spawn => spawnManager;
    public PoolManager Pool => poolManager;
    public SkillManager Skill => skillManager;
    public PlayerManager Player => playerManager;
    public DataManager Data => dataManager;
    public UIManager UI => uiManager;
    public ShopManager Shop => shopManager;
    public InventoryManager Inventory => inventoryManager;
    private void Awake()
    {
        instance = this;
        Data.Init();
        UI.Init();
        Player.Init();
        Network.Init();
        Flock.Init();
        Game.Init();
        Spawn.Init();
        Pool.Init();
        Skill.Init();
        Shop.Init();
        Inventory.Init();
    }

    private void Start()
    {
        
    }
}
