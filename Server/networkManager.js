const { encode, decode } = require("@msgpack/msgpack");
const { v4: uuidv4 } = require("uuid");
const events = require("./events");
const Player = require("./player");

const TICK_RATE = 20;

let io;
let players = [];

const logOnlinePeopleCount = () => {
  console.log("ONLINE PLAYERS: " + players.length);
  console.log("List of IDs/SocketIDs ");
  players.map((p, index) => {
    console.log(`${index}: \t ${p.id} / ${p.socket.id}`);
  });
};

const getPlayerBySocketId = (socket) => {
  return players.find((p) => p.socket === socket);
};

module.exports = {
  io: (socketio) => {
    io = socketio;

    io.on(events.CONNECT, (socket) => {
      handleNewConnection(socket);
      handleDisconnection(socket);

      const player = getPlayerBySocketId(socket);
      player.handleEvents();
    });

    const handleNewConnection = (socket) => {
      const player = new Player(uuidv4(), socket, { x: 0, y: 0 }, 0.05);

      console.log(
        `New player connected: ${socket.handshake.address} [${player.id}]`
      );

      players.push(player);

      const welcomeMessageResponse = { id: player.id };
      socket
        .binary(true)
        .emit("welcome_message", encode(welcomeMessageResponse));

      if (players.length > 1) {
        const onlinePlayersResponse = {
          ids: players.map((p) => p.id).filter((id) => id !== player.id),
        };

        socket.binary(true).emit("INIT_PLAYERS", encode(onlinePlayersResponse));
      }

      const playerConnectedResponse = { id: player.id };
      socket
        .binary(true)
        .broadcast.emit("PLAYER_CONNECTED", encode(playerConnectedResponse));

      logOnlinePeopleCount();
    };

    const handleDisconnection = (socket) => {
      socket.on(events.DISCONNECT, () => {
        const disconnectedPlayer = getPlayerBySocketId(socket);

        if (disconnectedPlayer) {
          console.log("PLAYER DISCONNECTED: " + disconnectedPlayer.id);

          const disconnectedPlayerId = { id: disconnectedPlayer.id };
          io.binary(true).emit(
            "PLAYER_DISCONNECTED",
            encode(disconnectedPlayerId)
          );

          players = players.filter((p) => p.socket !== socket);
        }

        logOnlinePeopleCount();
      });
    };

    // sends each client its current sequence number
    setInterval(() => {
      let data = [];

      players.forEach((player) => {
        const playerData = {
          id: player.id,
          posX: player.position.x,
          posY: player.position.y,
        };

        data.push(playerData);
      });

      io.binary(true).emit("MOVE", encode(data));
    }, 1000 / TICK_RATE);
  },
};
