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
        Log("Aplicativo iniciado, preparando sincroniza��o WebRTC...");
        try
        {
            Log("Iniciando sincroniza��o WebRTC...");
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
                channel.send("Sincroniza��o iniciada!");
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

            Log("Criando oferta...");
            var offer = _pc.createOffer();
            Log("Configurando descri��o local...");
            await _pc.setLocalDescription(offer); // Este m�todo ainda retorna Task
            Log("WebRTC: Oferta criada e configurada localmente");

            var offerJson = System.Text.Json.JsonSerializer.Serialize(offer);
            Log("Enviando oferta ao servidor...");
            await _ws.SendAsync(Encoding.UTF8.GetBytes(offerJson), WebSocketMessageType.Text, true, CancellationToken.None);
            Log("WebRTC: Oferta enviada ao servidor de sinaliza��o");

            Log("Aguardando resposta do servidor...");
            while (_ws.State == WebSocketState.Open)
            {
                var buffer = new byte[1024];
                var result = await _ws.ReceiveAsync(buffer, CancellationToken.None);
                var answerJson = Encoding.UTF8.GetString(buffer, 0, result.Count);
                Log($"WebRTC: Resposta recebida do servidor: {answerJson}");

                // Filtrar apenas mensagens SDP v�lidas (com "type" e "sdp")
                if (answerJson.Contains("\"type\":") && answerJson.Contains("\"sdp\":"))
                {
                    var answer = System.Text.Json.JsonSerializer.Deserialize<RTCSessionDescriptionInit>(answerJson);
                    if (answer?.sdp != null)
                    {
                        Log("Configurando descri��o remota...");
                        var setResult = _pc.setRemoteDescription(answer); // M�todo s�ncrono, retorna SetDescriptionResultEnum
                        if (setResult == SetDescriptionResultEnum.OK)
                        {
                            Log("WebRTC: Resposta recebida e configurada");
                            break; // Sai do loop ap�s configurar a resposta v�lida
                        }
                        else
                        {
                            Log($"Erro ao configurar descri��o remota: {setResult}");
                        }
                    }
                    else
                    {
                        Log("Erro: Resposta inv�lida ou nula ap�s desserializa��o");
                    }
                }
                else
                {
                    Log("Mensagem ignorada: n�o � uma descri��o SDP v�lida");
                }
            }
        }
        catch (Exception ex)
        {
            Log($"Erro na inicializa��o do WebRTC: {ex.Message}\nStackTrace: {ex.StackTrace}");
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
            Log("WebRTC: WebSocket n�o est� aberto para enviar candidato ICE");
        }
    }

    private void Log(string message)
    {
        Debug.WriteLine(message);
        File.AppendAllText(_logFile, $"{DateTime.Now}: {message}\n");
    }
}