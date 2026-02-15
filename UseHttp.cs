using System.Net;
using System.Text;
using System.Threading.Channels;
using Microsoft.Extensions.Logging;

namespace PutIsland;

public static class UseHttp {
    public static void StartServer(
        ChannelWriter<(string, string)> writer,
        ILogger logger,
        CancellationToken token,
        string host = "localhost",
        ushort port = 36000
    ) {
        Task.Factory.StartNew(
            () => { Starter(writer, logger, token, host, port); },
            token);
    }

    private static void Starter(ChannelWriter<(string, string)> writer,
        ILogger logger,
        CancellationToken token,
        string host, ushort port) {
        if (!HttpListener.IsSupported)
            throw new NotSupportedException(
                "HttpListener is not supported");
        logger.LogInformation($"starting HTTP listener at {host}:{port}");
        var listener = new HttpListener();
        listener.Prefixes.Add($"http://{host}:{port}/");
        try {
            listener.Start();
        } catch (HttpListenerException e) {
            logger.LogError($"HTTP listener error: {e.Message}");
            return;
        }

        Task.Factory.StartNew(async () => {
            while (!token.IsCancellationRequested)
                try {
                    await HandleReq(writer, listener, logger, token);
                } catch (Exception ex) {
                    Console.WriteLine($"HTTP Error: {ex.Message}");
                }
        }, token, TaskCreationOptions.LongRunning, TaskScheduler.Default);
        token.Register(() => listener.Close());
    }

    private static async Task HandleReq(
        ChannelWriter<(string, string)> writer,
        HttpListener listener,
        ILogger logger,
        CancellationToken cancellationToken
    ) {
        var ctx = await listener.GetContextAsync();
        var req = ctx.Request;
        var token = req.Url?.AbsolutePath.TrimStart('/');
        var rsp = ctx.Response;
        if (string.IsNullOrWhiteSpace(token)) {
            logger.LogWarning(
                $"{req.UserHostAddress} -> {req.RawUrl}: 没有 token");
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
        logger.LogInformation(
            $"{req.UserHostAddress} -> {req.RawUrl}: {reqText}");
        await writer.WriteAsync((token, reqText), cancellationToken);
        rsp.StatusCode = (int)HttpStatusCode.NoContent;
        rsp.Close();
    }
}