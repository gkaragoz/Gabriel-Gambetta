import { Client, Room } from "colyseus";

export class ChatRoom extends Room {
  // this room supports only 4 clients connected
  maxClients = 4;

  onCreate (options: any) {
    console.log("ChatRoom created!", options);

    this.onMessage("message", (client, message) => {
        console.log("ChatRoom received message from", client.sessionId, ":", message);
        this.broadcast("messages", `(${client.sessionId}) ${message}`);
    });
  }

  onJoin (client: Client, options: any) {
    this.broadcast("messages", `${ client.sessionId } joined.`);
  }

  onLeave (client: Client, consented: boolean) {
    this.broadcast("messages", `${ client.sessionId } left.`);
  }

  onDispose() {
    console.log("Dispose ChatRoom");
  }

}
