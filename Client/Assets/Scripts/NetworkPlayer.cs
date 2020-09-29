using UnityEngine;
using MessagePack;

public class NetworkPlayer : MonoBehaviour {

    private ulong _updateRate = 50;
    private ulong _nextUpdate;

    private NetworkManager _networkManager = null;
    public string SocketId { get; set; }

    [Header("Debug")]
    [SerializeField]
    private bool _isMe = false;

    public bool IsMe {
        get { return _isMe; }
        set { 
            _isMe = value;
            SetColor();
        }
    }

    private void Start() {
        _networkManager = NetworkManager.instance;
    }

    private void Update() {
        if (_networkManager.SocketManager.Socket.IsOpen) {
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
        _networkManager.SocketManager.Socket.Emit("MOVE", bytes);
    }

    private void SetColor() {
        if (IsMe) {
            GetComponent<SpriteRenderer>().color = Color.red;
        } else {
            GetComponent<SpriteRenderer>().color = Color.blue;
        }
    }

}
