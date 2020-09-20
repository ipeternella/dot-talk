# 💬 Dottalk  💬

A simple chatting system written with `C#`, `ASP.NET Core` and `SignalR` that can actually be scaled up horizontally (thanks to Redis storage).

![Alt text](Docs/Gif/ChatDemo.gif)

## Dottalk and Dotcli

This solution is composed of three projects:

- `Dottalk`: chatting system backend written with C#, ASP.NET Core and SignalR;
- `Dotcli`: chatting client written with Razor pages (minimal chat client demonstration);
- `Tests`: tests for the `Dottalk` application

## Dotcli

Dotcli is a simple chatting client demonstration which was built with `Razor` pages and `@microsoft/signalr` library.

### Joining and leaving chat rooms demonstration

![Alt text](Docs/Gif/ChatRoomJoinRoom.gif)

### Client technologies: Razor web pages and @microsoft/signalr library

The client application is a simple demonstration of the way `SignalR` uses `RPC` for communication between the client and the backend of the chatting system. The client application is served by a `Kestrel` server using `Razor` web pages and the HTML client uses a js library supplied my `Microsoft` named `@microsoft/signalr`.

The JS library `@microsoft/signalr` does all the heavy lifting in terms of the real-time communication protocols:

- implements the `websocket` protocol, reconnections and keep-alive mechanisms;
- implements transport fallbacks such as `HTTP long-polling` when `websockets` are not available (good for old browsers)
- implements the `RPC` communication in order to trigger remote procedures on the server.

### Beware: no real authentication

This application doesn't offer (not yet) a production-ready mean of authentication! The client barely uses the field `user name` which obviously is the same as not having any authentication at all as it easily leads to impersonification of another user by using his `user name`. Still, for demonstration purposes of the chat engine, a simple `user name` is more than enough and is used by the client/backend to identify users.

## Dottalk

Dottalk is the backend of the chatting application. It was built using `ASP.NET Core` and `SignalR` as the main library for dealing with real-time communication, mainly via websockets, and the `RPC` mechanism that is used by the `Dotcli` application.

### Redis and PostgreSQL

A real chatting application cannot sustain all its data in-memory as that makes horizontal scalling impossible. In order to solve this, `Redis` was used to persist a data structure named `ChatRoomConnectionPool` which basically stores all the `server instances` that are hosting a current `chat room`.

Besides being super-fast, using `Redis` to store such data structure allows many server instances (horizontally scalled) to have access to the amount of users currently connected to the chat room to see, for example, if the room is full or not.

Moreover, by knowing which server instances are currently hosting a given chat room, `Redis` can also be used as a `message broker` in order to send messages from one server instance to another to be able to broadcast messages sent to a given chat room across the many server instances that are hosting the chat room.

But... this also brings challenges: as `Redis` state can be "polluted" with server instance crashes and other bugs. Some techniques must still be implemented to deal with these problems. But, again, an external data store is necessary to be able to scale the chatting application.

PostgreSQL, on the other hand, is used to definitely persist the messages sent by the users (still a TODO).