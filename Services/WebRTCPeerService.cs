using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SIPSorcery.Net;

namespace PerpetuaNet.Services
{
    public class WebRTCPeerService
    {
        private RTCPeerConnection _peerConnection;
        private ClientWebSocket? _webSocket; // Campo nullable para evitar warning
        private readonly DatabaseHelper _databaseHelper = new DatabaseHelper();
        // Lista de peers conectados (em uma implementação real, essa lista seria gerenciada via sinalização)
        public List<string> ConnectedPeers { get; } = new List<string>();

        public WebRTCPeerService()
        {
            // Configuração ICE utilizando um servidor STUN público
            var iceServers = new List<RTCIceServer>
            {
                new RTCIceServer { urls = "stun:stun.l.google.com:19302" }
                // Adicione servidores TURN aqui se necessário.
            };

            var config = new RTCConfiguration { iceServers = iceServers };
            _peerConnection = new RTCPeerConnection(config);

            // Quando um candidato ICE for gerado, envie-o para o servidor de sinalização.
            _peerConnection.onicecandidate += candidate =>
            {
                if (candidate != null)
                {
                    // Envia apenas a string do candidato.
                    SendMessageAsync($"candidate:{candidate.candidate}").Wait();
                }
            };

            // Registra mudanças no estado da conexão.
            _peerConnection.onconnectionstatechange += state =>
            {
                Console.WriteLine($"[P2P] Estado da conexão: {state}");
                // Em uma implementação real, a lista ConnectedPeers seria atualizada aqui.
            };
        }

        /// <summary>
        /// Conecta ao servidor de sinalização utilizando o URL fornecido.
        /// </summary>
        public async Task ConnectToSignalingServer(string signalingServerUrl)
        {
            _webSocket = new ClientWebSocket();
            await _webSocket.ConnectAsync(new Uri(signalingServerUrl), CancellationToken.None);
            Console.WriteLine($"[SIGNAL] Conectado ao servidor de sinalização em {signalingServerUrl}");
            _ = Task.Run(async () => await ReceiveMessagesAsync());
        }

        private async Task ReceiveMessagesAsync()
        {
            var buffer = new byte[4096];
            while (_webSocket != null && _webSocket.State == WebSocketState.Open)
            {
                var result = await _webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                if (result.MessageType == WebSocketMessageType.Close)
                {
                    await _webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None);
                }
                else
                {
                    var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                    ProcessSignalingMessage(message);
                }
            }
        }

        private void ProcessSignalingMessage(string message)
        {
            Console.WriteLine($"[SIGNAL] Mensagem recebida: {message}");
            if (message.StartsWith("answer:"))
            {
                var sdp = message.Substring("answer:".Length);
                // Cria uma resposta SDP e define como descrição remota.
                var answerInit = new RTCSessionDescriptionInit { type = RTCSdpType.answer, sdp = sdp };
                _peerConnection.setRemoteDescription(answerInit);
            }
            else if (message.StartsWith("candidate:"))
            {
                var candidateStr = message.Substring("candidate:".Length);
                // Cria um candidato ICE usando RTCIceCandidateInit.
                var candidateInit = new RTCIceCandidateInit { candidate = candidateStr };
                _peerConnection.addIceCandidate(candidateInit);
            }
            else if (message.StartsWith("offer:"))
            {
                // Aqui você pode implementar a lógica de resposta a uma oferta.
                Console.WriteLine("[SIGNAL] Oferta recebida (implementação de resposta necessária).");
            }
            else if (message.StartsWith("data:"))
            {
                Console.WriteLine($"[SIGNAL] Dados recebidos: {message.Substring("data:".Length)}");
            }
        }

        private async Task SendMessageAsync(string message)
        {
            if (_webSocket != null && _webSocket.State == WebSocketState.Open)
            {
                var bytes = Encoding.UTF8.GetBytes(message);
                await _webSocket.SendAsync(new ArraySegment<byte>(bytes), WebSocketMessageType.Text, true, CancellationToken.None);
            }
        }

        /// <summary>
        /// Inicia a conexão P2P: conecta ao servidor de sinalização, cria uma oferta SDP e a envia.
        /// </summary>
        public async Task StartPeerConnection()
        {
            // Conecta ao servidor de sinalização usando um servidor público (Glitch)
            string signalingServerUrl = "wss://chlorinated-workable-cupboard.glitch.me";
            await ConnectToSignalingServer(signalingServerUrl);

            Console.WriteLine("[P2P] Inicializando conexão WebRTC...");
            var offer = _peerConnection.createOffer(null);
            await _peerConnection.setLocalDescription(offer);
            Console.WriteLine($"[P2P] Oferta SDP criada: {offer.sdp}");
            await SendMessageAsync($"offer:{offer.sdp}");
        }

        /// <summary>
        /// Sincroniza os dados do banco local com os peers conectados.
        /// </summary>
        public async Task SyncDatabaseWithPeers()
        {
            Console.WriteLine("[P2P] Iniciando sincronização de dados...");
            string data = _databaseHelper.RetrieveData();
            await SendMessageAsync($"data:{data}");
        }
    }
}
