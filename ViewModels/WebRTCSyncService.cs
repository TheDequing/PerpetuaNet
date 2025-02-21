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
    private readonly string _logFile = "C:\\PerpetuaNet\\logs.txt"; // Caminho do arquivo de log

    public async Task InitializeAndSync()
    {
        try
        {
            Log("Iniciando sincronização WebRTC...");
            var config = new RTCConfiguration
            {
                iceServers = new List<RTCIceServer>
                {
                    new RTCIceServer { urls = "stun:stun.l.google.com:19302" } // STUN do Google
                }
            };
            _pc = new RTCPeerConnection(config);
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

            _ws = new ClientWebSocket();
            await _ws.ConnectAsync(new Uri("wss://perpetuanetserver.onrender.com/ws"), CancellationToken.None);
            Log("WebRTC: Conectado ao WebSocket");

            _pc.onicecandidate += async (candidate) =>
            {
                await SendIceCandidateAsync(candidate);
            };

            var offer = _pc.createOffer();
            _pc.setLocalDescription(offer);
            Log("WebRTC: Oferta criada e configurada localmente");

            var offerJson = System.Text.Json.JsonSerializer.Serialize(offer);
            await _ws.SendAsync(Encoding.UTF8.GetBytes(offerJson), WebSocketMessageType.Text, true, CancellationToken.None);
            Log("WebRTC: Oferta enviada ao servidor de sinalização");

            var buffer = new byte[1024];
            var result = await _ws.ReceiveAsync(buffer, CancellationToken.None);
            var answerJson = Encoding.UTF8.GetString(buffer, 0, result.Count);
            Log($"WebRTC: Resposta recebida do servidor: {answerJson}");
            if (string.IsNullOrEmpty(answerJson))
            {
                Log("Erro: Resposta do servidor é nula ou vazia");
                return;
            }
            var answer = System.Text.Json.JsonSerializer.Deserialize<RTCSessionDescriptionInit>(answerJson);
            if (answer == null)
            {
                Log("Erro: Não foi possível desserializar a resposta");
                return;
            }
            _pc.setRemoteDescription(answer);
            Log("WebRTC: Resposta recebida e configurada");
        }
        catch (Exception ex)
        {
            Log($"Erro na inicialização do WebRTC: {ex.Message}");
            throw;
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