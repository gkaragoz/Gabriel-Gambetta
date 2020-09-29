using UnityEngine;
using BestHTTP.SocketIO;
using System;
using MessagePack;

public class NetworkManager : MonoBehaviour {

    public static NetworkManager instance;
    public static Action<string> onConnected;
    public static Action onDisconnected;
    public static Action<string[]> onInitPlayersReceived;
    public static Action<string> onPlayerConnected;
    public static Action<string> onPlayerDisconnected;

    private void Awake() {
        if (instance == null) {
            instance = this;
        } else if (instance != this) {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(instance);
    }

    [SerializeField]
    private string URI = "http://localhost";
    [SerializeField]
    private string PORT = "3000";

    private SocketManager _socketManager;
    public SocketManager SocketManager { get { return _socketManager; } }

    private void Start() {
        MessagePackSerializer.DefaultOptions = MessagePack.Resolvers.ContractlessStandardResolver.Options;

        SocketOptions options = new SocketOptions();
        options.ConnectWith = BestHTTP.SocketIO.Transports.TransportTypes.WebSocket;
        _socketManager = new SocketManager(new Uri(URI + ":" + PORT + "/socket.io/"), options);

        _socketManager.Socket.On("connecting", OnConnecting);
        _socketManager.Socket.On("connect", OnConnected);
        _socketManager.Socket.On("welcome_message", OnWelcomeMessageReceived);
        _socketManager.Socket.On("disconnect", OnDisconnected);
    }

    #region DEFAULT CONNECTIONS/DISCONNECTION

    private void OnConnecting(Socket socket, Packet packet, object[] args) {
        Debug.Log("OnConnecting... to " + _socketManager.Uri);
    }

    private void OnConnected(Socket socket, Packet packet, object[] args) {
        Debug.Log("OnConnect!");

        _socketManager.Socket.On("INIT_PLAYERS", OnInitPlayersReceived);
        _socketManager.Socket.On("PLAYER_CONNECTED", OnPlayerConnected);
        _socketManager.Socket.On("PLAYER_DISCONNECTED", OnPlayerDisconnected);
        _socketManager.Socket.On("MOVE", OnBroadcastPositionsReceived);
    }

    private void OnWelcomeMessageReceived(Socket socket, Packet packet, object[] args) {
        NetworkWelcomeMessageResponse receivedData = MessagePackSerializer.Deserialize<NetworkWelcomeMessageResponse>(packet.Attachments[0]);

        onConnected?.Invoke(receivedData.id);
    }

    private void OnDisconnected(Socket socket, Packet packet, object[] args) {
        Debug.Log("OnDisconnect");

        _socketManager.Socket.Off("INIT_PLAYERS");
        _socketManager.Socket.Off("PLAYER_CONNECTED");
        _socketManager.Socket.Off("PLAYER_DISCONNECTED");
        _socketManager.Socket.Off("MOVE");

        onDisconnected?.Invoke();
    }

    #endregion

    #region CUSTOM EVENTS

    private void OnInitPlayersReceived(Socket socket, Packet packet, object[] args) {
        NetworkInitPlayersResponse receivedData = MessagePackSerializer.Deserialize<NetworkInitPlayersResponse>(packet.Attachments[0]);

        onInitPlayersReceived?.Invoke(receivedData.ids);
    }

    private void OnPlayerConnected(Socket socket, Packet packet, object[] args) {
        NetworkPlayerConnectedResponse receivedData = MessagePackSerializer.Deserialize<NetworkPlayerConnectedResponse>(packet.Attachments[0]);

        onPlayerConnected?.Invoke(receivedData.id);
    }

    private void OnPlayerDisconnected(Socket socket, Packet packet, object[] args) {
        NetworkDisconnectedPlayerResponse receivedData = MessagePackSerializer.Deserialize<NetworkDisconnectedPlayerResponse>(packet.Attachments[0]);

        onPlayerDisconnected?.Invoke(receivedData.id);
    }

    private void OnBroadcastPositionsReceived(Socket socket, Packet packet, object[] args) {
        //Debug.Log(MessagePackSerializer.ConvertToJson(packet.Attachments[0]));

        NetworkInputResponse[] receivedDatas = MessagePackSerializer.Deserialize<NetworkInputResponse[]>(packet.Attachments[0]);
        Debug.Log("OnBroadcastPositionsReceived:");
        foreach (var receivedData in receivedDatas) {
            Debug.Log("id: " + receivedData.id + " posX: " + receivedData.posX + " posY: " + receivedData.posY);
        }
    }

    #endregion

}
