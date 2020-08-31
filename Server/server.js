const socketio = require("socket.io");
const io = socketio.listen(3000);

console.log("server is up at port 3000");

const events = require("./events");

io.on(events.CONNECT, function (socket) {
  console.log("user has been connected");

  socket.emit(events.DISCONNECT, "sample data");

  socket.on(events.DISCONNECT, () => {
    console.log("user disconnected");
  });
});
