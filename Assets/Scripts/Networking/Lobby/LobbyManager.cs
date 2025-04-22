using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Authentication;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;
using System.Threading.Tasks;

namespace Multi
{
    [RequireComponent(typeof(LobbyListUI), typeof(LobbyCreateUI), typeof(LobbyUI))]
    public class LobbyManager : MonoBehaviour
    {
        public static LobbyManager Instance { get; set; }

        [Header("Lobby - Main")]
        public GameObject lobby;
        public GameObject logs;
        public GameObject main;

        [Header("Lobby - Functional Flow")]
        public List<Lobby> hostedLobbies = new List<Lobby>();

        ///// <summary>
        ///// Lobby Events
        ///// </summary>
        //public event EventHandler<LobbyEventArgs> OnJoinedLobby;                    
        //public event EventHandler<LobbyEventArgs> OnJoinedLobbyUpdate;
        //public event EventHandler OnLeftLobby;
        //public event EventHandler<LobbyEventArgs> OnKickedFromLobby;
        //public event EventHandler<LobbyEventArgs> OnLobbyGameModeChanged;
        //public event EventHandler<OnLobbyListChangedEventArgs> OnLobbyListChanged;
        ///// <summary>
        ///// Lobby Args
        ///// </summary>
        //public class LobbyEventArgs : EventArgs
        //{
        //    public Lobby lobby;
        //}
        //public class OnLobbyListChangedEventArgs : EventArgs
        //{
        //    public List<Lobby> lobbyList;
        //}

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

        // Update is called once per frame
        private void Update()
        {
            //HandleRefreshLobbyList(); // Disabled Auto Refresh for testing with multiple builds
        }

        public void Log(string message)
        {
            Debug.Log(message);
        }

        //// Lobby Update
        //public async void UpdateLobbyGameMode(GameMode gameMode)
        //{
        //    try
        //    {
        //        Debug.Log("UpdateLobbyGameMode " + gameMode);

        //        Lobby lobby = await Lobbies.Instance.UpdateLobbyAsync(joinedLobby.Id, new UpdateLobbyOptions
        //        {
        //            Data = new Dictionary<string, DataObject> {
        //            { KEY_GAME_MODE, new DataObject(DataObject.VisibilityOptions.Public, gameMode.ToString()) }
        //        }
        //        });

        //        joinedLobby = lobby;

        //        OnLobbyGameModeChanged?.Invoke(this, new LobbyEventArgs { lobby = joinedLobby });
        //    }
        //    catch (LobbyServiceException e)
        //    {
        //        Debug.Log(e);
        //    }
        //}

        //// Player Update
        //public async void UpdatePlayerName(string playerName)
        //{
        //    MainManager.Instance.playerName = playerName;

        //    if (joinedLobby != null)
        //    {
        //        try
        //        {
        //            UpdatePlayerOptions options = new UpdatePlayerOptions();

        //            options.Data = new Dictionary<string, PlayerDataObject>() {
        //            {
        //                KEY_PLAYER_NAME, new PlayerDataObject(
        //                    visibility: PlayerDataObject.VisibilityOptions.Public,
        //                    value: MainManager.Instance.playerName)
        //            }
        //        };

        //            string playerId = AuthenticationService.Instance.PlayerId;

        //            Lobby lobby = await LobbyService.Instance.UpdatePlayerAsync(joinedLobby.Id, playerId, options);
        //            joinedLobby = lobby;

        //            OnJoinedLobbyUpdate?.Invoke(this, new LobbyEventArgs { lobby = joinedLobby });
        //        }
        //        catch (LobbyServiceException e)
        //        {
        //            Debug.Log(e);
        //        }
        //    }
        //}
    }
}
