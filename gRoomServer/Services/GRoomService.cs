using Grpc.Core;
using gRoom.gRPC.Messages;
using Google.Protobuf.WellKnownTypes;
using gRoom.gRPC.Utils;

namespace gRoom.gRPC.Services;

public class GRoomService : GRoom.GRoomBase
{
    private readonly ILogger<GRoomService> _logger;
    public GRoomService(ILogger<GRoomService> logger)
    {
        _logger = logger;
    }

    public override Task<RoomRegistrationResponse> RegisterToRoom(RoomRegistrationRequest request, ServerCallContext context)
    {
        _logger.LogInformation("Service called...");
        _logger.LogInformation($"Room Name: {request.RoomName}");
        var rnd = new Random();
        var roomNum = rnd.Next(1, 100);
        _logger.LogInformation($"Room no. {roomNum}");
        var resp = new RoomRegistrationResponse { RoomId = roomNum };
        return Task.FromResult(resp);
    }

    public override async Task<NewsStreamStatus> SendNewsFlash(IAsyncStreamReader<NewsFlash> newsStream, ServerCallContext context){
        while (await newsStream.MoveNext()){
            var news = newsStream.Current;
            MessagesQueue.AddNewsToQueue(news);
            Console.WriteLine($"News flash: {news.NewsItem}");
        }
        return new NewsStreamStatus{Success=true};
    }

    public override async Task StartMonitoring(Empty request, IServerStreamWriter<ReceivedMessage> streamWriter, ServerCallContext context){
        while(true){
            //await streamWriter.WriteAsync(new ReceivedMessage{MsgTime=Timestamp.FromDateTime(DateTime.UtcNow), User="1", Contents="Test Message"});
            if(MessagesQueue.GetMessagesCount()>0){
                await streamWriter.WriteAsync(MessagesQueue.GetNextMessage());
            }
            await Task.Delay(500);
        }
    }
}

