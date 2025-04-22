using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Matchmaker.Models;
using Unity.Services.Matchmaker;
using UnityEngine;
using TMPro;
using System;

namespace Multi
{
    public class MatchmakingManager : MonoBehaviour
    {
        public static MatchmakingManager Instance { get; set; }

        [Header("Functional")]
        public float findMatchRetryAfterMax;
        public float findMatchPollingMax;

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
        void Start()
        {

        }

        private void Update()
        {
            
        }

        public async void findGameServerMatch(List<Unity.Services.Lobbies.Models.Player> lobbyPlayers)
        {
            List<Player> matchMakingPlayers = lobbyPlayersToMatchmakerPlayers(lobbyPlayers);

            var attributes = new Dictionary<string, object>();
            string queueName = "TestQueue";
            var options = new CreateTicketOptions(queueName, attributes);

            // if we dont find a match, wait a second and try again
            while (!await FindMatch(matchMakingPlayers, options))
            {
                await UtilityManager.WaitForSeconds(findMatchRetryAfterMax);
                Debug.Log("Unable to find match. Retrying...");
            }                
        }

        async Task<bool> FindMatch(List<Player> players, CreateTicketOptions options)
        {
            var transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
            var ticketResponse = await MatchmakerService.Instance.CreateTicketAsync(players, options);

            while (true)
            {
                await UtilityManager.WaitForSeconds(findMatchPollingMax);
                Debug.Log("Polling...");
                var ticketStatusResponse = await MatchmakerService.Instance.GetTicketAsync(ticketResponse.Id);
                if (ticketStatusResponse?.Value is MultiplayAssignment assignment)
                {
                    Debug.Log("Response " + assignment.Status);
                    FindFirstObjectByType<TMP_Text>()?.SetText("Response " + assignment.Status);
                    switch (assignment.Status)
                    {
                        case MultiplayAssignment.StatusOptions.Found:
                            {
                                if (assignment.Port.HasValue)
                                {
                                    Debug.Log("Start Client");

                                    //transport.SetConnectionData(assignment.Ip, (ushort)assignment.Port);
                                    //bool result = NetworkManager.Singleton.StartClient();

                                    //// Logging and showing on UI
                                    //Debug.Log("StartClient " + result);
                                    //FindFirstObjectByType<TMP_Text>().SetText("StartClient " + result);
                                    //NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnectedCallback;

                                    //return result; // if we fail to connect try again w/ a false result
                                }

                                Debug.LogError("No port found");
                                return false;
                            }
                        case MultiplayAssignment.StatusOptions.Timeout:
                            {
                                Debug.Log("FindMatch :: Timeout.");
                                Debug.LogError(assignment.ToString());
                                return false;
                            }
                        case MultiplayAssignment.StatusOptions.Failed:
                            {
                                Debug.LogError(assignment.ToString());
                                return false;
                            }
                    }
                }
            }
        }

        private void OnClientConnectedCallback(ulong obj)
        {
            Debug.Log("OnClientConnectedCallback");
        }

        List<Player> lobbyPlayersToMatchmakerPlayers(List<Unity.Services.Lobbies.Models.Player> _players)
        {
            List<Player> _playersN = new List<Player>();

            for (int i = 0; i < _players.Count; i++)
            {
                Player _player = new Player(
                    id: _players[i].Id,
                    customData: _players[i].Data
                );
                _playersN.Add(_player);
            }

            return _playersN;
        }

        //void LogConnectionEvent(NetworkManager manager, ConnectionEventData data)
        //{
        //    switch (data.EventType)
        //    {
        //        case ConnectionEvent.ClientConnected:
        //            FindFirstObjectByType<TMP_Text>().SetText("Client connected " + data.ClientId +
        //                                                      " Count:" +
        //                                                      NetworkManager.Singleton.ConnectedClientsIds.Count + " Port:" +
        //                                                      (manager.NetworkConfig.NetworkTransport as UnityTransport)?.ConnectionData.Port);
        //            break;
        //        case ConnectionEvent.ClientDisconnected:
        //            FindFirstObjectByType<TMP_Text>()
        //                .SetText("Client disconnected " + data.ClientId + " Count:" +
        //                         NetworkManager.Singleton.ConnectedClientsIds.Count + " Port:" +
        //                         (manager.NetworkConfig.NetworkTransport as UnityTransport)?.ConnectionData.Port);
        //            break;
        //    }
        //}
    }
}
