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

    private void Start() {
        _socketManager = new SocketManager(new Uri(URI + ":" + PORT + "/socket.io/"));

        _socketManager.Socket.On("connecting", OnConnecting);
        _socketManager.Socket.On("connect", OnConnected);
        _socketManager.Socket.On("welcome_message", OnWelcomeMessageReceived);
        _socketManager.Socket.On("disconnect", OnDisconnected);

        _socketManager.Socket.On("init_players", OnInitPlayersReceived);
        _socketManager.Socket.On("broadcast_positions", OnBroadcastPositionsReceived);
    }

    private void Update() {
        if (_socketManager.Socket.IsOpen) {
            float posX = transform.position.x;
            _socketManager.Socket.Emit("get_position", posX);
        }
    }

    #region DEFAULT CONNECTIONS/DISCONNECTION

    private void OnConnecting(Socket socket, Packet packet, object[] args) {
        Debug.Log("OnConnecting... to " + _socketManager.Uri);
    }

    private void OnConnected(Socket socket, Packet packet, object[] args) {
        Debug.Log("OnConnect!");
    }

    private void OnWelcomeMessageReceived(Socket socket, Packet packet, object[] args) {
        Debug.Log("Receive OnWelcomeMessage " + packet.Payload);
    }

    private void OnDisconnected(Socket socket, Packet packet, object[] args) {
        Debug.Log("OnDisconnect");
    }

    #endregion

    #region CUSTOM EVENTS

    private void OnInitPlayersReceived(Socket socket, Packet packet, object[] args) {
        Debug.Log("OnInitPlayersReceived " + packet.Payload);
    }

    private void OnBroadcastPositionsReceived(Socket socket, Packet packet, object[] args) {
        Debug.Log("OnBroadcastPositionsReceived " + packet.Payload);
    }

    #endregion

}
