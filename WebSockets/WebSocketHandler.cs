using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;

namespace WebApiTestHarness.WebSockets
{
    public static class WebSocketHandler
    {
        private static readonly ConcurrentDictionary<string, WebSocketClient> _clients = new();

        public static async Task HandleAsync(HttpContext context)
        {
            if (!context.WebSockets.IsWebSocketRequest)
            {
                context.Response.StatusCode = 400;
                await context.Response.WriteAsync("WebSocket upgrade required");
                return;
            }

            var clientId = context.Request.Query["clientId"].FirstOrDefault() ?? Guid.NewGuid().ToString();
            var subscriptionsParam = context.Request.Query["subscriptions"].FirstOrDefault() ?? "";
            var initialSubscriptions = subscriptionsParam
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(s => s.Trim())
                .ToHashSet();

            using var ws = await context.WebSockets.AcceptWebSocketAsync();
            var client = new WebSocketClient(clientId, ws, initialSubscriptions);
            _clients[clientId] = client;

            await SendAsync(ws, new
            {
                type = "connected",
                clientId,
                timestamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                subscriptions = initialSubscriptions.ToArray()
            });

            // Start background broadcaster for this client
            using var cts = new CancellationTokenSource();
            var broadcastTask = BroadcastLoopAsync(client, cts.Token);

            try
            {
                await ReceiveLoopAsync(ws, client, cts.Token);
            }
            finally
            {
                cts.Cancel();
                _clients.TryRemove(clientId, out _);
                try { await broadcastTask; } catch { }

                if (ws.State == WebSocketState.Open || ws.State == WebSocketState.CloseReceived)
                {
                    await ws.CloseAsync(WebSocketCloseStatus.NormalClosure, "Client closed connection", CancellationToken.None);
                }
            }
        }

        private static async Task ReceiveLoopAsync(WebSocket ws, WebSocketClient client, CancellationToken ct)
        {
            var buffer = new byte[4096];
            while (ws.State == WebSocketState.Open && !ct.IsCancellationRequested)
            {
                WebSocketReceiveResult result;
                try
                {
                    result = await ws.ReceiveAsync(buffer, ct);
                }
                catch (OperationCanceledException) { break; }
                catch (WebSocketException) { break; }

                if (result.MessageType == WebSocketMessageType.Close)
                {
                    await SendAsync(ws, new
                    {
                        type = "disconnected",
                        clientId = client.ClientId,
                        reason = "Client closed connection"
                    });
                    break;
                }

                var raw = Encoding.UTF8.GetString(buffer, 0, result.Count);
                try
                {
                    using var doc = JsonDocument.Parse(raw);
                    var msgType = doc.RootElement.TryGetProperty("type", out var t) ? t.GetString() : null;
                    var channel = doc.RootElement.TryGetProperty("channel", out var ch) ? ch.GetString() : null;

                    switch (msgType)
                    {
                        case "subscribe" when channel != null:
                            client.Subscriptions.Add(channel);
                            await SendAsync(ws, new { type = "ack", channel, status = "subscribed", timestamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ") });
                            break;

                        case "unsubscribe" when channel != null:
                            client.Subscriptions.Remove(channel);
                            await SendAsync(ws, new { type = "unsubscribed", channel, timestamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ") });
                            break;

                        case "pong":
                            client.LastPong = DateTime.UtcNow;
                            break;

                        default:
                            await SendAsync(ws, new { type = "error", message = "Unknown message type", received = msgType });
                            break;
                    }
                }
                catch (JsonException)
                {
                    await SendAsync(ws, new { type = "error", message = "Invalid JSON" });
                }
            }
        }

        private static async Task BroadcastLoopAsync(WebSocketClient client, CancellationToken ct)
        {
            var pingInterval = TimeSpan.FromSeconds(30);
            var messageInterval = TimeSpan.FromSeconds(5);
            var lastPing = DateTime.UtcNow;
            var lastMessage = DateTime.UtcNow;
            var counter = 0;

            while (!ct.IsCancellationRequested && client.WebSocket.State == WebSocketState.Open)
            {
                try
                {
                    await Task.Delay(1000, ct);
                }
                catch (OperationCanceledException) { break; }

                var now = DateTime.UtcNow;
                counter++;

                // Ping
                if (now - lastPing >= pingInterval)
                {
                    await SendAsync(client.WebSocket, new { type = "ping", timestamp = now.ToString("yyyy-MM-ddTHH:mm:ssZ") });
                    lastPing = now;
                }

                // Channel messages
                if (now - lastMessage >= messageInterval)
                {
                    lastMessage = now;

                    if (client.Subscriptions.Contains("events"))
                        await SendAsync(client.WebSocket, new
                        {
                            type = "message",
                            channel = "events",
                            data = new { id = $"event-{counter}", type = "api.request", timestamp = now.ToString("yyyy-MM-ddTHH:mm:ssZ"), metadata = new { path = "/test/simple", method = "GET" } }
                        });

                    if (client.Subscriptions.Contains("logs"))
                        await SendAsync(client.WebSocket, new
                        {
                            type = "message",
                            channel = "logs",
                            data = new { timestamp = now.ToString("yyyy-MM-ddTHH:mm:ssZ"), level = "INFO", message = "Request received" }
                        });

                    if (client.Subscriptions.Contains("metrics"))
                        await SendAsync(client.WebSocket, new
                        {
                            type = "message",
                            channel = "metrics",
                            data = new { timestamp = now.ToString("yyyy-MM-ddTHH:mm:ssZ"), throughput = 1500 + counter, avgLatency = 45, errorRate = 0.01 }
                        });

                    if (client.Subscriptions.Contains("notifications"))
                        await SendAsync(client.WebSocket, new
                        {
                            type = "message",
                            channel = "notifications",
                            data = new { id = $"notif-{counter}", title = "New message", message = "You have 1 new notification" }
                        });

                    if (client.Subscriptions.Contains("broadcast"))
                        await SendAsync(client.WebSocket, new
                        {
                            type = "message",
                            channel = "broadcast",
                            data = new { id = $"broadcast-{counter}", title = "System Announcement", message = "All systems operational" }
                        });
                }
            }
        }

        private static async Task SendAsync(WebSocket ws, object payload)
        {
            if (ws.State != WebSocketState.Open) return;
            var json = JsonSerializer.Serialize(payload, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
            var bytes = Encoding.UTF8.GetBytes(json);
            try
            {
                await ws.SendAsync(bytes, WebSocketMessageType.Text, true, CancellationToken.None);
            }
            catch { }
        }
    }

    public class WebSocketClient
    {
        public string ClientId { get; }
        public WebSocket WebSocket { get; }
        public HashSet<string> Subscriptions { get; }
        public DateTime LastPong { get; set; } = DateTime.UtcNow;

        public WebSocketClient(string clientId, WebSocket ws, HashSet<string> initialSubscriptions)
        {
            ClientId = clientId;
            WebSocket = ws;
            Subscriptions = initialSubscriptions;
        }
    }
}
