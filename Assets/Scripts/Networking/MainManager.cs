using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.Networking;

namespace Multi
{
    public class MainManager : MonoBehaviour
    {
        public static MainManager Instance { get; set; }

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

        [Header("Managers")]
        public RelayManager relayManager;
        public LobbyManager lobbyManager;
        public NetworkManager netMag;
        public SceneManager sceMag;
        public ChatManager chatMag;
        public StringManager strMag;
        public ArColab arColab;

        [Header("User Properties")]
        public UserType userType;

        [Header("Server Properties")]
        public int maxConnectionsAllowed;

        [Header("Player Properties")]
        public string playerName;
        public Player currentPlayer;
        public GamePlayType gamePlayType;
        public bool joinedGame;

        [Header("Functional")]
        public float checkInternetTimerMax = 5;
        public float checkInternetTimer;

        async void Start()
        {
            StartCoroutine(RequestPermissions());

            if(userType == UserType.Player)
            {
                await startUnityServices();
                await startAuthentication();
            }

            if (userType == UserType.Server) sceMag.StartServer();
        }

        private void Update()
        {
            checkInternetTimer += Time.deltaTime;
            if (checkInternetTimer > checkInternetTimerMax)
            {
                checkInternetTimer = 0;
                StartCoroutine(CheckInternet());
            }
            
        }

        public async Task startUnityServices()
        {
            InitializationOptions initializationOptions = new InitializationOptions();
            playerName = UtilityManager.GenerateRandomKey(10, false);
            Debug.Log($"PlayerName: {playerName}");
            initializationOptions.SetProfile(profile: playerName);

            // Initialize Unity Services
            await UnityServices.InitializeAsync(initializationOptions);
        }

        public async Task startAuthentication()
        {
            // SignIn User
            AuthenticationService.Instance.SignedIn += () =>
            {
                Debug.Log($"Signed In. Player :: " +
                    $"ID : {AuthenticationService.Instance.PlayerId}, " +
                    $"Name : {AuthenticationService.Instance.PlayerName}, " +
                    $"Info : {AuthenticationService.Instance.PlayerInfo.Username}");

                currentPlayer = createPlayerInstance();

                LobbyListUI.Instance.refreshLobbiesList();
            };
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }

        public Player createPlayerInstance()
        {
            return new Player(
                id: AuthenticationService.Instance.PlayerId,
                profile: new PlayerProfile(AuthenticationService.Instance.PlayerName),
                data: new Dictionary<string, PlayerDataObject>() {
                    { StringManager.PlayerName, new PlayerDataObject(PlayerDataObject.VisibilityOptions.Public, MainManager.Instance.playerName) },
                },
                joined: DateTime.Now,
                lastUpdated: DateTime.Now
            //connectionInfo: null,
            //allocationId: null
            );
        }

        IEnumerator RequestPermissions()
        {
            if (!Permission.HasUserAuthorizedPermission(Permission.Camera))
            {
                Permission.RequestUserPermission(Permission.Camera);
            }

            if (!Permission.HasUserAuthorizedPermission(Permission.ExternalStorageRead))
            {
                Permission.RequestUserPermission(Permission.ExternalStorageRead);
            }

            if (!Permission.HasUserAuthorizedPermission(Permission.ExternalStorageWrite))
            {
                Permission.RequestUserPermission(Permission.ExternalStorageWrite);
            }

            yield return null;
        }

        IEnumerator CheckInternet()
        {
            using (UnityWebRequest www = UnityWebRequest.Get("https://google.com"))
            {
                yield return www.SendWebRequest();

                if (www.result != UnityWebRequest.Result.Success)
                {
                    Debug.LogError("No internet: " + www.error);
                }
                else
                {
                    Debug.Log("Internet OK!");
                }
            }
        }
    }
}
