using Grpc.Net.Client;
using gRoom.gRPC.Messages;
using Google.Protobuf.WellKnownTypes;

using var channel = GrpcChannel.ForAddress("http://localhost:5281");
var client = new GRoom.GRoomClient(channel);

Console.WriteLine("*** Admin Console started ***");
Console.WriteLine("Listening...");

// ADD THE MONITORING CODE BELOW THIS LINE
using var call = client.StartMonitoring(new Empty());
var cts = new CancellationTokenSource();

while(await call.ResponseStream.MoveNext(cts.Token)){
    var msg = call.ResponseStream.Current;
    Console.WriteLine($"New message: {msg.Contents}, user: {msg.User}, at: {msg.MsgTime}");
}
