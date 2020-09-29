using MessagePack;
using UnityEngine;

[MessagePackObject]
public class NetworkInput {
    [Key(0)]
    public Vector3 Input { get; set; }
}