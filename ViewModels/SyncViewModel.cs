using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using WebRTCNET; // Note o namespace correto: WebRTCNET, não WebRtc

namespace PerpetuaNet.ViewModels;

public partial class SyncViewModel : ObservableObject
{
    [ObservableProperty]
    private string _syncStatus = "Sincronizando com 0 peers...";

    [RelayCommand]
    private void ForceSync()
    {
        var localPeer = new PeerConnection();
        var remotePeer = new PeerConnection(); // Simulação
        var channel = localPeer.CreateDataChannel("syncChannel");

        localPeer.OnIceCandidate += (candidate) => SyncStatus = $"Candidato: {candidate}";
        channel.OnOpen += () =>
        {
            channel.Send("Sincronização iniciada!");
            SyncStatus = "Canal P2P aberto!";
        };
        channel.OnMessage += (data) => SyncStatus = $"Recebido: {data}";
        localPeer.CreateOffer();
    }
}