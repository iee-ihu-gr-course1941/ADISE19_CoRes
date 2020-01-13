using Schema;
using TMPro;
using UnityEngine;

public class CurrentGame : MonoBehaviour
{
    public SocketIo socketIo;
    public GameObject bottomBar;
    public GameObject[] playerPrefabs = new GameObject [4];
    public GameObject GoNode;
    
    private Vector3[] offsets = {
        new Vector3(0.15f, 0, 0.15f),
        new Vector3(-0.15f, 0, 0.15f), 
        new Vector3(0.15f, 0, -0.15f),
        new Vector3(-0.15f, 0, -0.15f)};
    
    // Start is called before the first frame update
    void Start()
    {
        socketIo.PlayerJoined += SocketIoOnPlayerJoined;
        socketIo.PlayerLeft += SocketIoOnPlayerLeft;

        var userId = AuthenticationManager.Instance.user.Id;

        UpdateBottomBar();
        SetupPlayers();
    }

    private void SocketIoOnPlayerJoined(Player player)
    {
        UpdateBottomBar();
        AddPlayer(player);
    }
    
    private void SocketIoOnPlayerLeft(Player player)
    {
        UpdateBottomBar();
    }

    void SetupPlayers()
    {
        var game = GameManager.Instance.Game;

        foreach (var player in game.Players)
        {
            Vector3 playerPos = GoNode.transform.position + offsets[player.Index];
            GameObject newPlayer = Instantiate(playerPrefabs[player.Index], playerPos, Quaternion.identity);
            newPlayer.GetComponent<PlayerMovement>().offset = offsets[player.Index];
        }
    }

    void AddPlayer(Player player)
    {
        Vector3 playerPos = GoNode.transform.position + offsets[player.Index];
        GameObject newPlayer = Instantiate(playerPrefabs[player.Index], playerPos, Quaternion.identity);
        newPlayer.GetComponent<PlayerMovement>().offset = offsets[player.Index];
    }
    
    private void UpdateBottomBar()
    {
        var game = GameManager.Instance.Game;
        var bottomBarTransform = bottomBar.transform;

        if (game == null) return;

        for (var i = 0; i < 4; i++)
        {
            var nameTextTransform = bottomBarTransform.GetChild(i).GetChild(0);
            var balanceTextTransform = bottomBarTransform.GetChild(i).GetChild(1);
            var nameTextMeshPro = nameTextTransform.GetComponent<TextMeshProUGUI>();
            var balanceTextMeshPro = balanceTextTransform.GetComponent<TextMeshProUGUI>();

            nameTextMeshPro.text = "";
            balanceTextMeshPro.text = "";
        }

        foreach (var player in game.Players)
        {
            var index = player.Index;
        
            var nameTextTransform = bottomBarTransform.GetChild(index).GetChild(0);
            var balanceTextTransform = bottomBarTransform.GetChild(index).GetChild(1);
            var nameTextMeshPro = nameTextTransform.GetComponent<TextMeshProUGUI>();
            var balanceTextMeshPro = balanceTextTransform.GetComponent<TextMeshProUGUI>();

            nameTextMeshPro.text = player.UserId;
            balanceTextMeshPro.text = player.Balance + "ΔΜ";
        }
    }
}
