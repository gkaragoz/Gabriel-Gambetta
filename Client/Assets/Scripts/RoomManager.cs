using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RoomManager : MonoBehaviour {

    [SerializeField]
    private NetworkPlayer _networkPlayerPrefab = null;

    [Header("Debug")]
    [SerializeField]
    private List<NetworkPlayer> _networkPlayers = new List<NetworkPlayer>();

    private void Awake() {
        NetworkManager.onConnected += OnConnected;
        NetworkManager.onDisconnected += OnDisconnected;
        NetworkManager.onInitPlayersReceived += OnInitPlayersReceived;
        NetworkManager.onPlayerConnected += OnPlayerConnected;
        NetworkManager.onPlayerDisconnected += OnPlayerDisconneced;
    }

    private void OnConnected(string socketId) {
        SpawnPlayer(socketId).IsMe = true;
    }

    private void OnDisconnected() {
        NetworkPlayer disconnectedPlayer = GetMe();

        if (!disconnectedPlayer) {
            Debug.LogWarning("Disconnected player not found!");
        }

        DestroyPlayer(disconnectedPlayer);
    }

    private void OnInitPlayersReceived(string[] socketIds) {
        Debug.Log("OnInitPlayersReceived " + socketIds.Length + " players found!");

        foreach (string id in socketIds) {
            SpawnPlayer(id);
        }
    }

    private void OnPlayerConnected(string socketId) {
        SpawnPlayer(socketId);
    }

    private void OnPlayerDisconneced(string socketId) {
        NetworkPlayer disconnectedPlayer = GetPlayerBySocketId(socketId);

        Debug.Log("SOCKETIDDDD " + socketId);
        if (!disconnectedPlayer) {
            Debug.LogWarning("Disconnected player not found!");
        }

        DestroyPlayer(disconnectedPlayer);
    }

    private NetworkPlayer GetPlayerBySocketId(string socketId) {
        return _networkPlayers.Where(p => p.SocketId == socketId).SingleOrDefault();
    }

    private NetworkPlayer SpawnPlayer(string socketId) {
        NetworkPlayer spawnedPlayer = Instantiate(_networkPlayerPrefab, Vector3.zero, Quaternion.identity);
        spawnedPlayer.SocketId = socketId;

        _networkPlayers.Add(spawnedPlayer);

        return spawnedPlayer;
    }

    private void DestroyPlayer(NetworkPlayer networkPlayer) {
        this._networkPlayers.Remove(networkPlayer);

        Destroy(networkPlayer.gameObject);
    }
    
    private NetworkPlayer GetMe() {
        return this._networkPlayers.Where(p => p.IsMe == true).SingleOrDefault();
    }

}
