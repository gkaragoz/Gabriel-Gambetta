using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BestHTTP;
using BestHTTP.SocketIO;
using System;

public class NetworkManager : MonoBehaviour {

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
    }

    private void OnConnecting(Socket socket, Packet packet, object[] args) {
        Debug.Log("OnConnecting... to " + _socketManager.Uri);
    }

    private void OnConnected(Socket socket, Packet packet, object[] args) {
        Debug.Log("OnConnect!");
    }

    private void OnWelcomeMessageReceived(Socket socket, Packet packet, object[] args) {
        Debug.Log("Receive OnWelcomeMessage");
    }

    private void OnDisconnected(Socket socket, Packet packet, object[] args) {
        Debug.Log("OnDisconnect");
    }
}
