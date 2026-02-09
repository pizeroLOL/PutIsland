using System.Net;
using System.Text;
using System.Threading.Channels;

namespace PutIsland;

public class UseHttp {
    public UseHttp(
        ChannelWriter<(string, string)> writer,
        // ConcurrentDictionary<string, Channel<string>> listeners,
        CancellationToken token,
        ushort port = 36000,
        string host = "localhost"
    ) {
        if (!HttpListener.IsSupported)
            throw new NotSupportedException("HttpListener is not supported");

        var listener = new HttpListener();
        listener.Prefixes.Add($"http://{host}:{port}/");
        listener.Start();
        Task.Factory.StartNew(async () => {
            while (!token.IsCancellationRequested)
                try {
                    await HandleReq(token, writer, listener);
                } catch (Exception ex) {
                    Console.WriteLine($"HTTP Error: {ex.Message}");
                }
        }, token, TaskCreationOptions.LongRunning, TaskScheduler.Default);
        token.Register(() => listener.Close());
    }

    private async Task HandleReq(
        CancellationToken cancellationToken,
        ChannelWriter<(string, string)> writer,
        HttpListener listener
    ) {
        var ctx = await listener.GetContextAsync();
        var req = ctx.Request;
        var token = req.Url?.AbsolutePath.TrimStart('/');
        var rsp = ctx.Response;
        if (string.IsNullOrWhiteSpace(token)) {
            rsp.StatusCode = (int)HttpStatusCode.BadRequest;
            await rsp.OutputStream.WriteAsync(
                new ReadOnlyMemory<byte>("没有 Token"u8.ToArray()),
                cancellationToken);
            rsp.Close();
            return;
        }

        // TODO: 动态取值
        // var inputLength = req.InputStream.Length - req.InputStream.Position;
        // Console.WriteLine(req.InputStream.Length);
        using var ms = new MemoryStream();
        await req.InputStream.CopyToAsync(ms, cancellationToken);
        var reqText = Encoding.UTF8.GetString(ms.ToArray());
        await writer.WriteAsync((token, reqText), cancellationToken);
        rsp.StatusCode = (int)HttpStatusCode.NoContent;
        rsp.Close();
    }
}