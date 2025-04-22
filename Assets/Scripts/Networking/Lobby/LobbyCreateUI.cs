using System.Collections;
using System.Collections.Generic;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

namespace Multi
{
    public class LobbyCreateUI : MonoBehaviour
    {
        public static LobbyCreateUI Instance { get; set; }

        [Header("UI - General")]
        public GameObject createUI;
        public TMPro.TMP_InputField lobbyName;
        public TMPro.TMP_InputField lobbyMaxPlayers;
        public Toggle lobbyIsPrivate;
        public Toggle lobbyIsLocked;

        [Header("UI - Buttons")]
        public Button createLobbyBtn;
        public Button closeCreateLobbyBtn;

        //[Header("Functional")]

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
            ///// Add Button Listeners /////
            createLobbyBtn.onClick.AddListener(createLobby);
            closeCreateLobbyBtn.onClick.AddListener(closeCreateLobbyUI);
        }

        public async void createLobby()
        {
            try
            {
                CreateLobbyOptions _lobbyOptions = new CreateLobbyOptions
                {
                    Player = MainManager.Instance.currentPlayer,
                    IsPrivate = lobbyIsPrivate.isOn,
                    IsLocked = lobbyIsLocked.isOn,
                    //Password = null,
                    Data = new Dictionary<string, DataObject>() {
                        {StringManager.LobbyGameMode, new DataObject(DataObject.VisibilityOptions.Public,"Trial")},
                        {StringManager.LobbyStartGameKey, new DataObject(DataObject.VisibilityOptions.Member,"0")},
                    }
                };

                Lobby lobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName.text, (int)float.Parse(lobbyMaxPlayers.text), _lobbyOptions);

                LobbyManager.Instance.hostedLobbies.Add(lobby);

                Debug.Log($"Created Lobby :: ID : {lobby.Id}, Name :{lobby.Name}, MaxPlayers :{lobby.MaxPlayers}");

                closeCreateLobbyUI();
                LobbyListUI.Instance.joinLobbyById(lobby.Id);

                //LobbyListUI.Instance.refreshLobbiesList();
            }
            catch (LobbyServiceException e)
            {
                Debug.Log(e);
            }
        }

        public void closeCreateLobbyUI()
        {
            LobbyListUI.Instance.listUI.SetActive(true);
            createUI.SetActive(false);
            lobbyName.text = "";
            lobbyMaxPlayers.text = "";
            lobbyIsPrivate.isOn = true;
            lobbyIsLocked.isOn = true;
        }

    }
}

