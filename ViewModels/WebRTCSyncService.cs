using SIPSorcery.Net;
using System.Diagnostics;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.Json;
using System;
using Serilog;

namespace PerpetuaNet;

public class SignalingMessage
{
    public int Type { get; set; } // 1 = offer, 2 = answer
    public string? Sdp { get; set; }
}

public class WebRTCSyncService : IDisposable
{
    private RTCPeerConnection? _pc;
    private ClientWebSocket? _ws;
    private readonly string _logFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs.txt");
    private bool _disposed = false;

    public WebRTCSyncService()
    {
        // Configuração inicial do Serilog
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .WriteTo.File(_logFile, rollingInterval: RollingInterval.Day)
            .CreateLogger();
    }

    public async Task InitializeAndSync()
    {
        Log.Information("Aplicativo iniciado, preparando sincronização WebRTC...");
        try
        {
            Log.Information("Iniciando sincronização WebRTC...");
            Log.Information("Configurando RTCConfiguration com STUN...");
            var config = new RTCConfiguration
            {
                iceServers = new List<RTCIceServer>
                {
                    new RTCIceServer { urls = "stun:stun.l.google.com:19302" }
                }
            };

            Log.Information("Criando RTCPeerConnection...");
            _pc = new RTCPeerConnection(config);

            Log.Information("Criando canal de dados...");
            var channel = await _pc.createDataChannel("syncChannel");

            channel.onopen += () =>
            {
                channel.send("Sincronização iniciada!");
                Log.Information("WebRTC: Canal de dados aberto");
            };
            channel.onmessage += (ch, protocol, data) =>
            {
                Log.Information("WebRTC: Recebido: {Data}", Encoding.UTF8.GetString(data));
            };

            Log.Information("Inicializando ClientWebSocket...");
            _ws = new ClientWebSocket();

            Log.Information("Conectando ao WebSocket wss://perpetuanetserver.onrender.com/ws...");
            using (var cts = new CancellationTokenSource(TimeSpan.FromSeconds(10)))
            {
                try
                {
                    await _ws.ConnectAsync(new Uri("wss://perpetuanetserver.onrender.com/ws"), cts.Token);
                    Log.Information("WebRTC: Conectado ao WebSocket");
                }
                catch (OperationCanceledException)
                {
                    Log.Error("Erro: Timeout ao conectar ao WebSocket");
                    return;
                }
                catch (WebSocketException wex)
                {
                    Log.Error("Erro ao conectar ao WebSocket: {Message}", wex.Message);
                    return;
                }
            }

            _pc.onicecandidate += async (candidate) =>
            {
                await SendIceCandidateAsync(candidate);
            };

            // Tenta receber uma oferta primeiro
            RTCSessionDescriptionInit? remoteOffer = null;
            Log.Information("Verificando se há oferta remota...");
            using (var offerCts = new CancellationTokenSource(TimeSpan.FromSeconds(30)))
            {
                while (_ws.State == WebSocketState.Open && remoteOffer == null && !offerCts.Token.IsCancellationRequested)
                {
                    var buffer = new ArraySegment<byte>(new byte[1024]);
                    var result = await _ws.ReceiveAsync(buffer, offerCts.Token);
                    var messageJson = Encoding.UTF8.GetString(buffer.Array!, 0, result.Count);
                    Log.Information("WebRTC: Mensagem recebida do servidor: {Message}", messageJson);

                    var msg = JsonSerializer.Deserialize<SignalingMessage>(messageJson);
                    if (msg?.Type == 1 && !string.IsNullOrEmpty(msg.Sdp))
                    {
                        remoteOffer = new RTCSessionDescriptionInit { type = RTCSdpType.offer, sdp = msg.Sdp };
                        Log.Information("Oferta remota recebida, configurando descrição remota...");
                        await _pc.SetRemoteDescription(remoteOffer);
                        Log.Information("WebRTC: Oferta remota configurada");
                        break;
                    }
                    else
                    {
                        Log.Warning("Mensagem ignorada: não é uma oferta válida");
                    }
                }
            }

            if (remoteOffer == null)
            {
                // Se não recebeu oferta, cria e envia uma
                Log.Information("Nenhuma oferta recebida, criando oferta local...");
                var offer = await _pc.createOffer();
                Log.Information("Configurando descrição local...");
                await _pc.SetLocalDescription(offer);
                Log.Information("WebRTC: Oferta criada e configurada localmente");

                var offerJson = JsonSerializer.Serialize(new SignalingMessage { Type = 1, Sdp = offer.sdp });
                Log.Information("Enviando oferta ao servidor...");
                await _ws.SendAsync(Encoding.UTF8.GetBytes(offerJson), WebSocketMessageType.Text, true, CancellationToken.None);
                Log.Information("WebRTC: Oferta enviada ao servidor de sinalização");
            }
            else
            {
                // Se recebeu oferta, cria e envia uma resposta
                Log.Information("Criando resposta para a oferta remota...");
                var answer = await _pc.createAnswer();
                Log.Information("Configurando descrição local com resposta...");
                await _pc.SetLocalDescription(answer);
                Log.Information("WebRTC: Resposta criada e configurada localmente");

                var answerJson = JsonSerializer.Serialize(new SignalingMessage { Type = 2, Sdp = answer.sdp });
                Log.Information("Enviando resposta ao servidor...");
                await _ws.SendAsync(Encoding.UTF8.GetBytes(answerJson), WebSocketMessageType.Text, true, CancellationToken.None);
                Log.Information("WebRTC: Resposta enviada ao servidor de sinalização");
            }

            Log.Information("Aguardando conexão ou resposta adicional...");
            using (var answerCts = new CancellationTokenSource(TimeSpan.FromSeconds(30)))
            {
                while (_ws.State == WebSocketState.Open && !answerCts.Token.IsCancellationRequested)
                {
                    var buffer = new ArraySegment<byte>(new byte[1024]);
                    var result = await _ws.ReceiveAsync(buffer, answerCts.Token);
                    var answerJson = Encoding.UTF8.GetString(buffer.Array!, 0, result.Count);
                    Log.Information("WebRTC: Resposta recebida do servidor: {Message}", answerJson);

                    var msg = JsonSerializer.Deserialize<SignalingMessage>(answerJson);
                    if (msg?.Type == 2 && !string.IsNullOrEmpty(msg.Sdp))
                    {
                        var answer = new RTCSessionDescriptionInit { type = RTCSdpType.answer, sdp = msg.Sdp };
                        Log.Information("Configurando descrição remota com resposta...");
                        await _pc.SetRemoteDescription(answer);
                        Log.Information("WebRTC: Resposta recebida e configurada");
                        break;
                    }
                    else
                    {
                        Log.Warning("Mensagem ignorada: não é uma resposta válida");
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Erro na inicialização do WebRTC: {Message}", ex.Message);
        }
    }

    private async Task SendIceCandidateAsync(RTCIceCandidate candidate)
    {
        if (_ws != null && _ws.State == WebSocketState.Open)
        {
            var json = JsonSerializer.Serialize(candidate);
            await _ws.SendAsync(Encoding.UTF8.GetBytes(json), WebSocketMessageType.Text, true, CancellationToken.None);
            Log.Information("WebRTC: Candidato ICE enviado: {Candidate}", json);
        }
        else
        {
            Log.Warning("WebRTC: WebSocket não está aberto para enviar candidato ICE");
        }
    }

    public void Dispose()
    {
        if (_disposed) return;

        try
        {
            if (_ws != null && _ws.State == WebSocketState.Open)
            {
                _ws.CloseAsync(WebSocketCloseStatus.NormalClosure, "Fechando", CancellationToken.None).GetAwaiter().GetResult();
                Log.Information("WebSocket fechado");
            }
            _ws?.Dispose();

            if (_pc != null)
            {
                _pc.Close();
                _pc.Dispose();
                Log.Information("RTCPeerConnection fechado");
            }

            Log.CloseAndFlush(); // Fecha o Serilog
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Erro ao liberar recursos: {Message}", ex.Message);
        }

        _disposed = true;
    }
}