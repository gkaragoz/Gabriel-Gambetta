using UnityEngine;
using BestHTTP.SocketIO;
using System;

public class NetworkManager : MonoBehaviour {

    public static NetworkManager instance;

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

    private ulong _updateRate = 50;
    private ulong _nextUpdate;

    private void Start() {
        _socketManager = new SocketManager(new Uri(URI + ":" + PORT + "/socket.io/"));

        _socketManager.Socket.On("connecting", OnConnecting);
        _socketManager.Socket.On("connect", OnConnected);
        _socketManager.Socket.On("welcome_message", OnWelcomeMessageReceived);
        _socketManager.Socket.On("disconnect", OnDisconnected);
    }

    private void Update() {
        if (_socketManager.Socket.IsOpen) {
            Tick();
        }
    }

    private void Tick() {
        var now = new Date();
        if (now < _nextUpdate) {
            return;
        }

        _nextUpdate = now + this._updateRate;
        SendPosition();
    }

    private void SendPosition() {
        _socketManager.Socket.Emit("MOVE", transform.position.x);
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
        Debug.Log("Receive OnWelcomeMessage " + packet.Payload);
    }

    private void OnDisconnected(Socket socket, Packet packet, object[] args) {
        Debug.Log("OnDisconnect");

        _socketManager.Socket.Off("INIT_PLAYERS");
        _socketManager.Socket.Off("PLAYER_CONNECTED");
        _socketManager.Socket.Off("PLAYER_DISCONNECTED");
        _socketManager.Socket.Off("MOVE");
    }

    #endregion

    #region CUSTOM EVENTS

    private void OnInitPlayersReceived(Socket socket, Packet packet, object[] args) {
        Debug.Log("OnInitPlayersReceived " + packet.Payload);
    }

    private void OnPlayerConnected(Socket socket, Packet packet, object[] args) {
        Debug.Log("OnPlayerConnected " + packet.Payload);
    }

    private void OnPlayerDisconnected(Socket socket, Packet packet, object[] args) {
        Debug.Log("OnPlayerDisconnected" + packet.Payload);
    }

    private void OnBroadcastPositionsReceived(Socket socket, Packet packet, object[] args) {
        Debug.Log("OnBroadcastPositionsReceived " + packet.Payload);
    }

    #endregion

}
