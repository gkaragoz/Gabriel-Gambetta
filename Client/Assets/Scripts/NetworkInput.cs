using MessagePack;
using UnityEngine;

[MessagePackObject]
public class NetworkInput {
    [Key(0)]
    public Vector3 Input { get; set; }
}

public class NetworkInputResponse {
    public string id { get; set; }
    public float posX { get; set; }
    public float posY { get; set; }
}