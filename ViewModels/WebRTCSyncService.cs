using SIPSorcery.Net;
using System;
using System.Diagnostics;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PerpetuaNet;

public class WebRTCSyncService
{
    private RTCPeerConnection _pc;
    private ClientWebSocket _ws;

    public async Task InitializeAndSync()
    {
        try
        {
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
            await _ws.ConnectAsync(new Uri("ws://localhost:5000/ws"), CancellationToken.None);

            _pc.onicecandidate += async (candidate) =>
            {
                var json = System.Text.Json.JsonSerializer.Serialize(candidate);
                await _ws.SendAsync(Encoding.UTF8.GetBytes(json), WebSocketMessageType.Text, true, CancellationToken.None);
                Debug.WriteLine($"WebRTC: Candidato ICE enviado: {json}");
            };

            var offer = _pc.createOffer();
            await _pc.setLocalDescription(offer);

            // Enviar oferta ao servidor de sinalização
            var offerJson = System.Text.Json.JsonSerializer.Serialize(offer);
            await _ws.SendAsync(Encoding.UTF8.GetBytes(offerJson), WebSocketMessageType.Text, true, CancellationToken.None);
            Debug.WriteLine("WebRTC: Oferta enviada ao servidor de sinalização");

            // Receber resposta (simulação básica)
            var buffer = new byte[1024];
            var result = await _ws.ReceiveAsync(buffer, CancellationToken.None);
            var answerJson = Encoding.UTF8.GetString(buffer, 0, result.Count);
            var answer = System.Text.Json.JsonSerializer.Deserialize<RTCSessionDescriptionInit>(answerJson);
            await _pc.setRemoteDescription(answer);
            Debug.WriteLine("WebRTC: Resposta recebida e configurada");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Erro na inicialização do WebRTC: {ex.Message}");
            throw;
        }
    }
}