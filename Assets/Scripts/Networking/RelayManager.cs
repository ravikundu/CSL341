using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;

namespace Multi
{
    public class RelayManager : MonoBehaviour
    {
        public static RelayManager Instance { get; set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
            }
            else
            {
                Instance = this;
            }
        }

        // Start is called before the first frame update
        async void Start()
        {
            //await MainManager.Instance.startUnityServices();

            //await MainManager.Instance.startAuthentication();
        }

        public async Task<string> createRelayServer()
        {
            try
            {
                Allocation allocation = await RelayService.Instance.CreateAllocationAsync(MainManager.Instance.maxConnectionsAllowed - 1);

                string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

                Log($"Created relay : {joinCode}");

                // Method - 1
                //NetworkManager.Singleton.GetComponent<UnityTransport>().SetHostRelayData(
                //    ipAddress: allocation.RelayServer.IpV4,
                //    port: (ushort)allocation.RelayServer.Port,
                //    allocationId: allocation.AllocationIdBytes,
                //    key: allocation.Key,
                //    connectionData: allocation.ConnectionData
                //);

                // Method - 2
                RelayServerData relayServerData = new RelayServerData(allocation, "dtls");
                NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);

                //NetworkManager.Singleton.StartHost();
                SceneManager.Instance.StartHostByName(MainManager.Instance.currentPlayer.Data[StringManager.PlayerName].Value);

                return joinCode;
            }
            catch (RelayServiceException e)
            {
                Debug.Log(e);
                return null;
            }

        }

        public async void joinRelayServer(string _joinCode)
        {
            try
            {
                Log($"Joining relay : {_joinCode}");
                JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(_joinCode);
                Log($"Joined relay : {_joinCode}");

                //// Method - 1
                //NetworkManager.Singleton.GetComponent<UnityTransport>().SetClientRelayData(
                //    ipAddress: joinAllocation.RelayServer.IpV4,
                //    port: (ushort)joinAllocation.RelayServer.Port,
                //    allocationId: joinAllocation.AllocationIdBytes,
                //    key: joinAllocation.Key,
                //    connectionData: joinAllocation.ConnectionData,
                //    hostConnectionData: joinAllocation.HostConnectionData
                //);

                // Method - 2
                RelayServerData relayServerData = new RelayServerData(joinAllocation, "dtls");
                NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);

                //NetworkManager.Singleton.StartClient();
                SceneManager.Instance.StartClientByName(MainManager.Instance.currentPlayer.Data[StringManager.PlayerName].Value);
            }
            catch (RelayServiceException e)
            {
                Debug.Log(e);
            }

        }

        public void Log(string message)
        {
            Debug.Log(message);
        }
    }
}

