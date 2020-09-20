# 💬 Dottalk  💬

A simple chatting system written with `C#`, `ASP.NET Core` and `SignalR` that can actually be scaled up horizontally (thanks to Redis storage).

![Alt text](Docs/Gif/ChatDemo.gif)

## Dottalk and Dotcli

This solution is composed of three projects:

- `Dottalk`: chatting system backend written with C#, ASP.NET Core and SignalR;
- `Dotcli`: chatting client written with Razor pages (minimal chat client demonstration);
- `Tests`: tests for the `Dottalk` application