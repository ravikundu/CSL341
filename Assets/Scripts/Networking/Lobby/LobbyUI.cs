using System.Collections;
using System.Collections.Generic;
using Unity.Services.Authentication;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

namespace Multi
{
    public class LobbyUI : MonoBehaviour
    {
        public static LobbyUI Instance { get; set; }

        [Header("UI - General")]
        public GameObject listUI;
        public GameObject listScrollView;
        public GameObject listContent;
        public GameObject listPrefab;
        public GameObject listNoPlayers;
        public TMPro.TMP_Text lobbyName;
        private List<GameObject> playerListInst = new List<GameObject>();

        [Header("UI - Buttons")]
        public Button exitLobbyBtn;
        public Button startGameBtn;

        [Header("Functional")]
        public float heartbeatTimerMax = 15;
        public float lobbyPollTimerMax = 1.1f;
        public Lobby joinedLobby;
        public float heartbeatTimer;
        public float lobbyPollTimer;

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

        private void Start()
        {
            heartbeatTimer = heartbeatTimerMax;
            lobbyPollTimer = lobbyPollTimerMax;

            ///// Add Button Listeners /////
            exitLobbyBtn.onClick.AddListener(exitLobbyNet);
            startGameBtn.onClick.AddListener(startGame);
        }

        public void startLobby(Lobby lobby)
        {
            joinedLobby = lobby;
            lobbyName.text = $"Joined Lobby: {lobby.Name}";
            listUI.SetActive(true);
            refreshPlayersList();
        }

        private void Update()
        {
            HandleLobbyHeartbeat();
            // To Stop lobby polling update sync after joinedGame
            if (!MainManager.Instance.joinedGame) HandleLobbyPolling();
        }

        private async void HandleLobbyHeartbeat()
        {
            if (IsLobbyHost())
            {
                heartbeatTimer -= Time.deltaTime;
                if (heartbeatTimer < 0f)
                {
                    heartbeatTimer = heartbeatTimerMax;

                    await LobbyService.Instance.SendHeartbeatPingAsync(joinedLobby.Id);

                    LobbyManager.Instance.Log("Sent HeartbeatPingAsync.");

                }
            }
        }

        private async void HandleLobbyPolling()
        {
            if (joinedLobby != null)
            {
                lobbyPollTimer -= Time.deltaTime;
                if (lobbyPollTimer < 0f)
                {
                    lobbyPollTimer = lobbyPollTimerMax;

                    joinedLobby = await LobbyService.Instance.GetLobbyAsync(joinedLobby.Id);

                    if (!IsPlayerInLobby())
                    {
                        // Player was kicked out of this lobby
                        Debug.Log("Kicked from Lobby!");

                        exitLobby();

                        joinedLobby = null;
                    }
                    else
                    {
                        LobbyManager.Instance.Log($"LobbyAsync :: PlayerCount:{joinedLobby.Players.Count}, Data['RelayKey']: {joinedLobby.Data[StringManager.LobbyStartGameKey].Value}");
                    }

                    // Join Relay network for lobby non-host
                    if (!MainManager.Instance.joinedGame && joinedLobby != null && joinedLobby.Data[StringManager.LobbyStartGameKey] != null &&
                        joinedLobby.Data[StringManager.LobbyStartGameKey].Value != "0")
                    {
                        if (!IsLobbyHost())
                        {
                            RelayManager.Instance.joinRelayServer(joinedLobby.Data[StringManager.LobbyStartGameKey].Value);
                            listUI.SetActive(false);
                        }
                    }

                    //LobbyManager.Instance.OnJoinedLobbyUpdate?.Invoke(this, new LobbyEventArgs { lobby = joinedLobby });
                    refreshPlayersList();
                }
            }
        }

        public bool IsLobbyHost()
        {
            return joinedLobby != null && joinedLobby.HostId == AuthenticationService.Instance.PlayerId;
        }

        private bool IsPlayerInLobby()
        {
            if (joinedLobby != null && joinedLobby.Players != null)
            {
                foreach (Player player in joinedLobby.Players)
                {
                    if (player.Id == AuthenticationService.Instance.PlayerId)
                    {
                        // This player is in this lobby
                        return true;
                    }
                }
            }
            return false;
        }

        void refreshPlayersList()
        {
            clearPlayersList();

            if (joinedLobby != null)
            {
                // Add Players
                foreach (Player player in joinedLobby.Players)
                {
                    Transform listPrefabInst = Instantiate(listPrefab, listContent.transform).transform;
                    listPrefabInst.GetChild(0).GetComponent<TMPro.TMP_Text>().text = player.Data[StringManager.PlayerName].Value;
                    if (!IsLobbyHost()) listPrefabInst.GetChild(1).gameObject.SetActive(false);
                    if (IsLobbyHost() && player.Id != AuthenticationService.Instance.PlayerId)
                    {
                        listPrefabInst.GetChild(1).GetComponent<Button>().onClick.AddListener(() => kickOutPlayerNet(player));
                    }
                    else
                    {
                        listPrefabInst.GetChild(1).gameObject.SetActive(false);
                    }

                    playerListInst.Add(listPrefabInst.gameObject);
                }

                // Deactivate Start Game Btn
                if (!IsLobbyHost()) startGameBtn.gameObject.SetActive(false);
            }
        }

        void clearPlayersList()
        {
            //Clear Players List
            for (int i = 0; i < playerListInst.Count; i++)
            {
                DestroyImmediate(playerListInst[i]);
            }
            playerListInst.Clear();
        }

        async void kickOutPlayerNet(Player player)
        {
            if (IsLobbyHost())
            {
                try
                {
                    await LobbyService.Instance.RemovePlayerAsync(joinedLobby.Id, player.Id);
                    // Player will be removed from the list using HandleLobbyPolling 
                }
                catch (LobbyServiceException e)
                {
                    Debug.Log(e);
                }
            }
        }

        async void exitLobbyNet()
        {
            if (joinedLobby != null)
            {
                try
                {
                    await LobbyService.Instance.RemovePlayerAsync(joinedLobby.Id, AuthenticationService.Instance.PlayerId);
                    exitLobby();
                }
                catch (LobbyServiceException e)
                {
                    Debug.Log(e);
                }
            }
        }

        void exitLobby()
        {
            LobbyManager.Instance.Log("Exited lobby");
            listUI.SetActive(false);
            clearPlayersList();
            joinedLobby = null;
            LobbyListUI.Instance.refreshLobbiesList();
            LobbyListUI.Instance.listUI.SetActive(true);
        }

        async void startGame()
        {
            startGameBtn.interactable = false;
            if (IsLobbyHost())
            {
                if(MainManager.Instance.gamePlayType == GamePlayType.FriendPlay)
                {
                    // Create Relay and join network for lobby host
                    string relayCode = await RelayManager.Instance.createRelayServer();

                    try
                    {
                        Lobby lobby = await Lobbies.Instance.UpdateLobbyAsync(joinedLobby.Id, new UpdateLobbyOptions
                        {
                            Data = new Dictionary<string, DataObject>
                            {
                                {StringManager.LobbyStartGameKey, new DataObject(DataObject.VisibilityOptions.Member,relayCode) }
                            }
                        });

                        joinedLobby = lobby;

                        if(listUI != null) listUI.SetActive(false);
                    }
                    catch (LobbyServiceException e)
                    {
                        startGameBtn.interactable = true;
                        Debug.Log(e);
                    }
                }
            }
        }
    }
}

