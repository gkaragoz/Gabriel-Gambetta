﻿using UnityEngine;
using BestHTTP.SocketIO;
using System;
using MessagePack;

public class NetworkManager : MonoBehaviour {

    public static NetworkManager instance;
    public static Action<string> onConnected;
    public static Action onDisconnected;

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
        MessagePackSerializer.DefaultOptions = MessagePack.Resolvers.ContractlessStandardResolver.Options;

        SocketOptions options = new SocketOptions();
        options.ConnectWith = BestHTTP.SocketIO.Transports.TransportTypes.WebSocket;
        _socketManager = new SocketManager(new Uri(URI + ":" + PORT + "/socket.io/"), options);

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

        if (Input.GetKey(KeyCode.UpArrow)) {
            SendPosition(Direction.UP);
        }
        if (Input.GetKey(KeyCode.RightArrow)) {
            SendPosition(Direction.RIGHT);
        }
        if (Input.GetKey(KeyCode.DownArrow)) {
            SendPosition(Direction.DOWN);
        }
        if (Input.GetKey(KeyCode.LeftArrow)) {
            SendPosition(Direction.LEFT);
        }
    }

    private void SendPosition(Direction direction) {
        Vector3 inputVector = Vector3.zero;
        switch (direction) {
            case Direction.RIGHT:
                inputVector = Vector3.right;
                break;
            case Direction.LEFT:
                inputVector = Vector3.left;
                break;
            case Direction.UP:
                inputVector = Vector3.up;
                break;
            case Direction.DOWN:
                inputVector = Vector3.down;
                break;
        }

        NetworkInput networkInput = new NetworkInput() {
            inputX = inputVector.x,
            inputY = inputVector.y
        };

        byte[] bytes = MessagePackSerializer.Serialize(networkInput);
        _socketManager.Socket.Emit("MOVE", bytes);
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

        onConnected?.Invoke(packet.Payload);
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
        Debug.Log("OnInitPlayersReceived " + packet.Payload);
    }

    private void OnPlayerConnected(Socket socket, Packet packet, object[] args) {
        Debug.Log("OnPlayerConnected " + packet.Payload);
    }

    private void OnPlayerDisconnected(Socket socket, Packet packet, object[] args) {
        Debug.Log("OnPlayerDisconnected" + packet.Payload);
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
