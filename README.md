# Hamsta's Webhook
A simple C# websocket server. (no really.. it's just a websocket server.)


# How to use in combination with TamperMonkey.
Ensure you install the modded TamperMonkey script from this github and then browse on V3rmillion.
The script is up above and should be a simple copy and paste or one click install.


# Synapse Execution Code
I would recommend putting this in your autoexec. It should be undetectable by itself as it's just opening a local websocket.

```lua
local socket = syn.websocket.connect("ws://localhost:6969")

socket.OnMessage:Connect(function(Msg)
    loadstring(Msg)() -- Execute script
end)
```

# Script-Ware/KRNL Execution Code
I would recommend putting this in your autoexec. It should be undetectable by itself as it's just opening a local websocket.
```lua
local socket = WebSocket.connect("ws://localhost:6969")

socket.OnMessage:Connect(function(Msg)
    loadstring(Msg)() -- Execute script
end)```
