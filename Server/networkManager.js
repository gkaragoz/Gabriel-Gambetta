const { v4: uuidv4 } = require("uuid");
const events = require("./events");

const TICK_RATE = 2;

let io;
let playerSockets = [];

const logOnlinePeopleCount = () => {
  console.log("ONLINE PLAYERS: " + playerSockets.length);
  console.log("List of IDs/SocketIDs ");
  playerSockets.map((p, index) => {
    console.log(`${index}: \t ${p.id} / ${p.socket.id}`);
  });
};

const getPlayerBySocketId = (socket) => {
  return playerSockets.find((p) => p.socket === socket);
};

module.exports = {
  io: (socketio) => {
    io = socketio;

    io.on(events.CONNECT, (socket) => {
      handleNewConnection(socket);
      handleDisconnection(socket);
      handleMovement(socket);
    });

    const handleNewConnection = (socket) => {
      const player = {
        id: uuidv4(),
        socket,
        position: 0,
      };

      console.log(
        `New player connected: ${socket.handshake.address} [${player.id}]`
      );

      playerSockets.push(player);

      socket.emit("welcome_message", player.id);

      if (playerSockets.length > 1) {
        socket.emit(
          "INIT_PLAYERS",
          playerSockets.map((p) => p.id).filter((id) => id !== player.id)
        );
      }

      socket.broadcast.emit("PLAYER_CONNECTED", player.id);

      logOnlinePeopleCount();
    };

    const handleDisconnection = (socket) => {
      socket.on(events.DISCONNECT, () => {
        const disconnectedPlayer = getPlayerBySocketId(socket);

        if (disconnectedPlayer) {
          console.log("PLAYER DISCONNECTED: " + disconnectedPlayer.id);

          io.emit("PLAYER_DISCONNECTED", disconnectedPlayer.id);

          playerSockets = playerSockets.filter((p) => p.socket !== socket);
        }

        logOnlinePeopleCount();
      });
    };

    const handleMovement = (socket) => {
      socket.on("MOVE", (data) => {
        const player = getPlayerBySocketId(socket);

        if (!player) {
          console.log("Player not found: " + socket.id);
          return;
        }

        player.position = data;
      });
    };

    // sends each client its current sequence number
    setInterval(() => {
      playerSockets.forEach((player) => {
        player.socket.emit("MOVE", {
          id: player.id,
          posX: player.position,
        });
      });
    }, 1000 / TICK_RATE);
  },
};
