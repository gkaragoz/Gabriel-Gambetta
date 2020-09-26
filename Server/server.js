const server = require("http").createServer();
const socketio = require("socket.io");

const PORT = 3000;
const io = socketio(server);

require("./networkManager").io(io);

server.listen(PORT, () => {
  console.log(`Listening on port ${PORT}`);
});
