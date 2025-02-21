using SIPSorcery.Net;
using System.Diagnostics;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace PerpetuaNet;

public class WebRTCSyncService
{
    private RTCPeerConnection? _pc;
    private ClientWebSocket? _ws;
    private readonly string _logFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs.txt");

    public async Task InitializeAndSync()
    {
        Log("Aplicativo iniciado, preparando sincronização WebRTC...");
        try
        {
            Log("Iniciando sincronização WebRTC...");
            Log("Configurando RTCConfiguration com STUN...");
            var config = new RTCConfiguration
            {
                iceServers = new List<RTCIceServer>
                {
                    new RTCIceServer { urls = "stun:stun.l.google.com:19302" }
                }
            };

            Log("Criando RTCPeerConnection...");
            _pc = new RTCPeerConnection(config);

            Log("Criando canal de dados...");
            var channel = await _pc.createDataChannel("syncChannel");

            channel.onopen += () =>
            {
                channel.send("Sincronização iniciada!");
                Log("WebRTC: Canal de dados aberto");
            };
            channel.onmessage += (ch, protocol, data) =>
            {
                Log($"WebRTC: Recebido: {Encoding.UTF8.GetString(data)}");
            };

            Log("Inicializando ClientWebSocket...");
            _ws = new ClientWebSocket();

            Log("Conectando ao WebSocket wss://perpetuanetserver.onrender.com/ws...");
            using (var cts = new CancellationTokenSource(TimeSpan.FromSeconds(10)))
            {
                try
                {
                    await _ws.ConnectAsync(new Uri("wss://perpetuanetserver.onrender.com/ws"), cts.Token);
                    Log("WebRTC: Conectado ao WebSocket");
                }
                catch (OperationCanceledException)
                {
                    Log("Erro: Timeout ao conectar ao WebSocket");
                    return;
                }
                catch (WebSocketException wex)
                {
                    Log($"Erro ao conectar ao WebSocket: {wex.Message}");
                    return;
                }
            }

            _pc.onicecandidate += async (candidate) =>
            {
                await SendIceCandidateAsync(candidate);
            };

            // Tenta receber uma oferta primeiro
            RTCSessionDescriptionInit? remoteOffer = null;
            Log("Verificando se há oferta remota...");
            while (_ws.State == WebSocketState.Open && remoteOffer == null)
            {
                var buffer = new byte[1024];
                var result = await _ws.ReceiveAsync(buffer, CancellationToken.None);
                var messageJson = Encoding.UTF8.GetString(buffer, 0, result.Count);
                Log($"WebRTC: Mensagem recebida do servidor: {messageJson}");

                if (messageJson.Contains("\"type\":1") && messageJson.Contains("\"sdp\":"))
                {
                    remoteOffer = System.Text.Json.JsonSerializer.Deserialize<RTCSessionDescriptionInit>(messageJson);
                    if (remoteOffer?.sdp != null)
                    {
                        Log("Oferta remota recebida, configurando descrição remota...");
                        var setResult = _pc.setRemoteDescription(remoteOffer);
                        if (setResult == SetDescriptionResultEnum.OK)
                        {
                            Log("WebRTC: Oferta remota configurada");
                            break;
                        }
                        else
                        {
                            Log($"Erro ao configurar oferta remota: {setResult}");
                            return;
                        }
                    }
                }
                else
                {
                    Log("Mensagem ignorada: não é uma oferta válida");
                }
            }

            if (remoteOffer == null)
            {
                // Se não recebeu oferta, cria e envia uma
                Log("Nenhuma oferta recebida, criando oferta local...");
                var offer = _pc.createOffer();
                Log("Configurando descrição local...");
                await _pc.setLocalDescription(offer);
                Log("WebRTC: Oferta criada e configurada localmente");

                var offerJson = System.Text.Json.JsonSerializer.Serialize(offer);
                Log("Enviando oferta ao servidor...");
                await _ws.SendAsync(Encoding.UTF8.GetBytes(offerJson), WebSocketMessageType.Text, true, CancellationToken.None);
                Log("WebRTC: Oferta enviada ao servidor de sinalização");
            }
            else
            {
                // Se recebeu oferta, cria e envia uma resposta
                Log("Criando resposta para a oferta remota...");
                var answer = _pc.createAnswer();
                Log("Configurando descrição local com resposta...");
                await _pc.setLocalDescription(answer);
                Log("WebRTC: Resposta criada e configurada localmente");

                var answerJson = System.Text.Json.JsonSerializer.Serialize(answer);
                Log("Enviando resposta ao servidor...");
                await _ws.SendAsync(Encoding.UTF8.GetBytes(answerJson), WebSocketMessageType.Text, true, CancellationToken.None);
                Log("WebRTC: Resposta enviada ao servidor de sinalização");
            }

            Log("Aguardando conexão ou resposta adicional...");
            while (_ws.State == WebSocketState.Open)
            {
                var buffer = new byte[1024];
                var result = await _ws.ReceiveAsync(buffer, CancellationToken.None);
                var answerJson = Encoding.UTF8.GetString(buffer, 0, result.Count);
                Log($"WebRTC: Resposta recebida do servidor: {answerJson}");

                if (answerJson.Contains("\"type\":2") && answerJson.Contains("\"sdp\":"))
                {
                    var answer = System.Text.Json.JsonSerializer.Deserialize<RTCSessionDescriptionInit>(answerJson);
                    if (answer?.sdp != null)
                    {
                        Log("Configurando descrição remota com resposta...");
                        var setResult = _pc.setRemoteDescription(answer);
                        if (setResult == SetDescriptionResultEnum.OK)
                        {
                            Log("WebRTC: Resposta recebida e configurada");
                            break;
                        }
                        else
                        {
                            Log($"Erro ao configurar resposta remota: {setResult}");
                        }
                    }
                    else
                    {
                        Log("Erro: Resposta inválida ou nula após desserialização");
                    }
                }
                else
                {
                    Log("Mensagem ignorada: não é uma resposta válida");
                }
            }
        }
        catch (Exception ex)
        {
            Log($"Erro na inicialização do WebRTC: {ex.Message}\nStackTrace: {ex.StackTrace}");
        }
    }

    private async Task SendIceCandidateAsync(RTCIceCandidate candidate)
    {
        if (_ws != null && _ws.State == WebSocketState.Open)
        {
            var json = System.Text.Json.JsonSerializer.Serialize(candidate);
            await _ws.SendAsync(Encoding.UTF8.GetBytes(json), WebSocketMessageType.Text, true, CancellationToken.None);
            Log($"WebRTC: Candidato ICE enviado: {json}");
        }
        else
        {
            Log("WebRTC: WebSocket não está aberto para enviar candidato ICE");
        }
    }

    private void Log(string message)
    {
        Debug.WriteLine(message);
        File.AppendAllText(_logFile, $"{DateTime.Now}: {message}\n");
    }
}