Quick.Protocol
=====

A simple protocol for TCP,Pipeline,SerialPort,WebSocket.
The easiest way how to use Quick.Protocol is via the Quick.Protocol NuGet package. 

+ QP/TCP
+ QP/SerialPort
+ QP/Pipeline
+ QP/WebSocket

Reserved Package Type
----
+ 0: CommandRequestPackage
+ 1: HeartBeatPackage
+ 2: SplitPackage
+ 3: TcpGuard->TcpPackage
+ 255: CommandResponsePackage
