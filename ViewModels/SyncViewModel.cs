// ViewModels/SyncViewModel.cs
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using WebRTCNET;

namespace PerpetuaNet.ViewModels;

public partial class SyncViewModel : ObservableObject
{
    [ObservableProperty]
    private string _syncStatus = "Sincronizando com 0 peers...";

    [RelayCommand]
    private void ForceSync()
    {
        var peer = new PeerConnection();
        var channel = peer.CreateDataChannel("syncChannel");
        channel.OnOpen += () =>
        {
            channel.Send("Sincronização iniciada!");
            SyncStatus = "Canal P2P aberto!";
        };
        channel.OnMessage += (data) => SyncStatus = $"Recebido: {data}";
        peer.CreateOffer();
    }
}