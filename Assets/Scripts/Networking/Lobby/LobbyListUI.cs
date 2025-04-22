using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

namespace Multi
{
    public class LobbyListUI : MonoBehaviour
    {
        public static LobbyListUI Instance { get; set; }

        [Header("UI - General")]
        public GameObject listUI;
        public GameObject listScrollView;
        public GameObject listContent;
        public GameObject listPrefab;
        public GameObject listNoLobbies;
        public GameObject joinLobbyByIdUI;
        public TMPro.TMP_InputField joinLobbyByIdInputfield;

        [Header("UI - Buttons")]
        public Button refreshLobbyListBtn;
        public Button openCreateLobbyBtn;
        public Button openJoinLobbyByIdUIBtn;
        public Button closeJoinLobbyByIdUIBtn;
        public Button joinLobbyByIdBtn;
        public Button closeLobbyListBtn;

        [Header("Functional")]
        public QueryLobbiesOptions currentQueryLobbiesOptions;
        public List<QueryFilter> currentQueryFilters = new List<QueryFilter>();
        public List<QueryOrder> currentQueryOrders = new List<QueryOrder>();
        public List<Lobby> lobbies = new List<Lobby>();
        public float refreshLobbyListTimerMax = 15f;
        private float refreshLobbyListTimer;

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
            refreshLobbyListTimer = refreshLobbyListTimerMax;

            ///// Add Button Listeners /////
            refreshLobbyListBtn.onClick.AddListener(refreshLobbiesList);
            openCreateLobbyBtn.onClick.AddListener(openCreateLobbyUI);
            //openJoinLobbyByIdUIBtn.onClick.AddListener(openJoinLobbyByIdUI);
            //closeJoinLobbyByIdUIBtn.onClick.AddListener(closeJoinLobbyByIdUI);
            //joinLobbyByIdBtn.onClick.AddListener(() => joinLobbyById(joinLobbyByIdInputfield.text));
            closeLobbyListBtn.onClick.AddListener(() => { Debug.Log("closeLobbyListBtn : CLicked"); });
        }

        private void Update()
        {
            //HandleRefreshLobbyList(); // Disabled Auto Refresh for testing with multiple builds
        }

        private void HandleRefreshLobbyList()
        {
            if (UnityServices.State == ServicesInitializationState.Initialized && AuthenticationService.Instance.IsSignedIn)
            {
                refreshLobbyListTimer -= Time.deltaTime;
                if (refreshLobbyListTimer < 0f)
                {
                    refreshLobbyListTimer = refreshLobbyListTimerMax;
                    refreshLobbiesList();
                }
            }
        }

        public void refreshLobbiesList()
        {
            //LobbyManager.Instance.Log("Refreshing lobby list.");
            currentQueryLobbiesOptions = new QueryLobbiesOptions
            {
                Filters = currentQueryFilters,
                Order = currentQueryOrders,
                //SampleResults = true,
                //Count = 10,
                //Skip = 0,
                //ContinuationToken = null
            };

            clearLobbiesList();
            listLobbies();
        }

        void clearLobbiesList()
        {
            //Clear Lobby List
            foreach (Transform lobby in listContent.transform)
            {
                Destroy(lobby.gameObject);
            }
        }

        public async void listLobbies()
        {
            ////////  Reset Lobby List  ////////
            listNoLobbies.SetActive(false);

            try
            {
                QueryResponse queryResponse = await Lobbies.Instance.QueryLobbiesAsync(currentQueryLobbiesOptions);
                Debug.Log($"Lobbies found : {queryResponse.Results.Count}");

                lobbies = queryResponse.Results;

                foreach (Lobby lobby in queryResponse.Results)
                {
                    Transform listPrefabInst = Instantiate(listPrefab, listContent.transform).transform;
                    listPrefabInst.GetChild(0).GetComponent<TMPro.TMP_Text>().text = lobby.Name;
                    listPrefabInst.GetComponent<Button>().onClick.AddListener(() => joinLobbyById(lobby.Id));
                }

                if (lobbies.Count == 0)
                {
                    listNoLobbies.SetActive(true);
                }

            }
            catch (LobbyServiceException e)
            {
                Debug.Log(e);
            }
        }

        public async void joinLobbyById(string _lobbyId)
        {
            JoinLobbyByIdOptions joinLobbyByIdOptions = new JoinLobbyByIdOptions {
                Player = MainManager.Instance.currentPlayer,
                //Password = ""
            };
            Lobby lobby = await LobbyService.Instance.JoinLobbyByIdAsync(_lobbyId, joinLobbyByIdOptions);

            listUI.SetActive(false);
            LobbyUI.Instance.startLobby(lobby);
        }

        public void openCreateLobbyUI()
        {
            listUI.SetActive(false);
            LobbyCreateUI.Instance.createUI.SetActive(true);
        }

        public void openJoinLobbyByIdUI()
        {
            listUI.SetActive(false);
            joinLobbyByIdUI.SetActive(true);
        }

        public void closeJoinLobbyByIdUI()
        {
            listUI.SetActive(true);
            joinLobbyByIdUI.SetActive(false);
            joinLobbyByIdInputfield.text = "";
        }
    }
}

