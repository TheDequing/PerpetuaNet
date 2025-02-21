using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SIPSorcery.Net;
using System.Diagnostics;

namespace PerpetuaNet.ViewModels;

public partial class SyncViewModel : ObservableObject
{
    [ObservableProperty]
    private string _syncStatus = "Sincronizando com 0 peers...";

    [RelayCommand]
    private async Task ForceSync()
    {
        try
        {
            var pc = new RTCPeerConnection();
            var channelTask = pc.createDataChannel("syncChannel");
            var dataChannel = await channelTask;

            dataChannel.onopen += () =>
            {
                dataChannel.send("Sincronização iniciada!");
                SyncStatus = "Canal P2P aberto!";
                Debug.WriteLine("DataChannel aberto");
            };
            dataChannel.onmessage += (channel, protocol, data) =>
            {
                SyncStatus = $"Recebido: {System.Text.Encoding.UTF8.GetString(data)}";
                Debug.WriteLine($"Mensagem recebida: {data}");
            };

            var offer = pc.createOffer();
            await pc.setLocalDescription(offer);
            Debug.WriteLine("Oferta configurada");
        }
        catch (Exception ex)
        {
            SyncStatus = $"Erro: {ex.Message}";
            Debug.WriteLine($"Erro no ForceSync: {ex}");
        }
    }
}