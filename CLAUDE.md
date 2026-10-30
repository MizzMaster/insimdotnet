# InSimDotNet

A light-weight .NET library for communicating with Live for Speed (LFS) using the InSim protocol.

## Build and Development
- **Framework**: .NET Framework 4.8 (based on `InSimDotNet.csproj`)
- **Build**: `dotnet build`

## Architecture Overview
`InSimDotNet` provides a low-level managed wrapper around the InSim protocol. It handles the underlying socket communication (TCP and UDP) and provides a strongly-typed system for sending and receiving InSim packets.

### Core Components
- **InSim**: The main entry point. Manages the connection lifecycle (`Initialize`, `Disconnect`), handles the send/receive loops, and provides the `Bind<T>` mechanism for packet handling.
- **Packets** (`/Packets`):
    - **IPacket**: Base interface for all InSim packets.
    - **ISendable**: Interface for packets that can be sent to LFS.
    - **PacketFactory**: Responsible for identifying packet types from raw bytes and instantiating the corresponding packet class.
- **Sockets**:
    - **TcpSocket**: Handles the primary TCP connection for commands and most data.
    - **UdpSocket**: Handles optional UDP communication, typically used for high-frequency car position data (NLP/MCI).
- **BindingManager**: Internal mechanism that maps `PacketType` to specific callback delegates.

## Development Patterns

### Connecting to LFS
The library uses an `InSimSettings` object to configure the connection:
```csharp
var settings = new InSimSettings {
    Host = "127.0.0.1",
    Port = 20000,
    IName = "MyInSimApp"
};
insim.Initialize(settings);
```

### Handling Packets (Binding)
The library uses a callback-based system to react to incoming packets:
```csharp
insim.Bind<IS_NPL>(packet => {
    Console.WriteLine($"Player joined: {packet.PName}");
});
```

### Sending Data
Packets can be sent individually or in batches:
```csharp
// Send a typed packet
insim.Send(new IS_MST { Msg = "/say Hello!" });

// Send a formatted message/command
insim.Send("/say Hello World!");
```

## Protocol Implementation
The library strictly follows the InSim specification (detailed in `../docs/InSim.txt`). It implements:
- **Initialisation**: Sending the `IS_ISI` packet to establish communication.
- **Keep-Alive**: Automatically responding to `TINY_NONE` packets to prevent timeout (70s limit).
- **Data Marshalling**: Handling LFS-specific encoding via `LfsEncoding`.

## Project Structure
- `InSim.cs`: Core connection and event management.
- `Packets/`: Definitions for all `ISP_` packet types.
- `TcpSocket.cs` / `UdpSocket.cs`: Low-level networking.
- `BindingManager.cs`: Callback registration and execution.
