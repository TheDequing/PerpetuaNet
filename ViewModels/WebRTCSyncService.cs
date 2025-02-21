using SIPSorcery.Net;
using System.Diagnostics;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;

namespace PerpetuaNet;

public class WebRTCSyncService
{
    private RTCPeerConnection? _pc;
    private ClientWebSocket? _ws;

    public async Task InitializeAndSync()
    {
        try
        {
            Debug.WriteLine("Iniciando sincronização WebRTC...");
            _pc = new RTCPeerConnection();
            var channel = await _pc.createDataChannel("syncChannel");

            channel.onopen += () =>
            {
                channel.send("Sincronização iniciada!");
                Debug.WriteLine("WebRTC: Canal de dados aberto");
            };
            channel.onmessage += (ch, protocol, data) =>
            {
                Debug.WriteLine($"WebRTC: Recebido: {Encoding.UTF8.GetString(data)}");
            };

            _ws = new ClientWebSocket();
            await _ws.ConnectAsync(new Uri("wss://perpetuanetserver.onrender.com/ws"), CancellationToken.None); // Use wss://
            Debug.WriteLine("WebRTC: Conectado ao WebSocket");

            _pc.onicecandidate += async (candidate) =>
            {
                await SendIceCandidateAsync(candidate);
            };

            var offer = _pc.createOffer();
            _pc.setLocalDescription(offer);
            Debug.WriteLine("WebRTC: Oferta criada e configurada localmente");

            var offerJson = System.Text.Json.JsonSerializer.Serialize(offer);
#pragma warning disable CS4014 // Suprimir aviso
            await _ws.SendAsync(Encoding.UTF8.GetBytes(offerJson), WebSocketMessageType.Text, true, CancellationToken.None);
#pragma warning restore CS4014
            Debug.WriteLine("WebRTC: Oferta enviada ao servidor de sinalização");

            var buffer = new byte[1024];
            var result = await _ws.ReceiveAsync(buffer, CancellationToken.None);
            var answerJson = Encoding.UTF8.GetString(buffer, 0, result.Count);
            var answer = System.Text.Json.JsonSerializer.Deserialize<RTCSessionDescriptionInit>(answerJson);
            _pc.setRemoteDescription(answer);
            Debug.WriteLine("WebRTC: Resposta recebida e configurada");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Erro na inicialização do WebRTC: {ex.Message}");
            throw;
        }
    }

    private async Task SendIceCandidateAsync(RTCIceCandidate candidate)
    {
        if (_ws != null && _ws.State == WebSocketState.Open)
        {
            var json = System.Text.Json.JsonSerializer.Serialize(candidate);
            await _ws.SendAsync(Encoding.UTF8.GetBytes(json), WebSocketMessageType.Text, true, CancellationToken.None);
            Debug.WriteLine($"WebRTC: Candidato ICE enviado: {json}");
        }
        else
        {
            Debug.WriteLine("WebRTC: WebSocket não está aberto para enviar candidato ICE");
        }
    }
}