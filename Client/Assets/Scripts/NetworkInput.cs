public class NetworkInput {
    public float inputX { get; set; }
    public float inputY { get; set; }
}

public class NetworkInputResponse {
    public string id { get; set; }
    public float posX { get; set; }
    public float posY { get; set; }
}

public class NetworkInitPlayersResponse {
    public string[] ids { get; set; }
}

public class NetworkDisconnectedPlayerResponse {
    public string id { get; set; }
}

public class NetworkWelcomeMessageResponse {
    public string id { get; set; }
}

public class NetworkPlayerConnectedResponse {
    public string id { get; set; }
}