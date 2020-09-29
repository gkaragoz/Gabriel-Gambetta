using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UINetworkInfo : MonoBehaviour {

    [SerializeField]
    private TextMeshProUGUI _txtSocketID = null;

    private void Awake() {
        NetworkManager.onConnected += OnConnected;
        NetworkManager.onDisconnected += OnDisconnected;
    }

    private void OnConnected(string socketId) {
        _txtSocketID.text = socketId;
    }

    private void OnDisconnected() {
        _txtSocketID.text = "Disconnected.";
    }

}
