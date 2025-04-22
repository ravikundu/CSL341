using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Collections;
using Unity.Netcode;

namespace Multi
{
    public class SceneManager : MonoBehaviour
    {
        public static SceneManager Instance { get; set; }

        // Player Setup
        [Header("Player Setup")]
        public GameObject playerSetupUI;
        public GameObject userNameUI;
        public GameObject userTypeUI;
        public TMPro.TMP_Text userName_tMP_Text;
        public string userName;
        public PlayerNetworkType pNetType;

        // Player Play
        [Header("Player Play - UI")]
        public GameObject playerPlayUI;
        public GameObject playerList;
        public GameObject playerListContent;
        public GameObject playerListPrefab;

        [Header("Player Play - Functional")]
        public GameObject localPlayer;
        public PlayerNetwork localPlayerNet;
        public List<GameObject> remotePlayers = new List<GameObject>();

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

        public void setUserName()
        {
            userNameUI.SetActive(false);
            userName = userName_tMP_Text.text;
            userTypeUI.SetActive(true);
        }

        public void setupLocalPlayerUI()
        {
            // Setup Button Clicks for Local Player Network
            playerPlayUI.SetActive(true);
        }

        /// <summary>
        /// User Chat Functions
        /// </summary>
        ///

       

        /// <summary>
        /// User Type Start Functions
        /// </summary>

        public void StartServer()
        {
            NetworkManager.Singleton.StartServer();
            playerSetupUI.SetActive(false);
        }

        public void StartHost()
        {
            pNetType = PlayerNetworkType.Host;
            NetworkManager.Singleton.StartHost();
            playerSetupUI.SetActive(false);
            //StartCoroutine(loadScene());
        }

        IEnumerator loadScene()
        {
            yield return new WaitForSeconds(2);
            NetworkManager.Singleton.SceneManager.LoadScene("RummyMulti", UnityEngine.SceneManagement.LoadSceneMode.Single);
        }

        public void StartClient()
        {
            pNetType = PlayerNetworkType.Client;
            NetworkManager.Singleton.StartClient();
            playerSetupUI.SetActive(false);
        }

        public void StartHostByName(string _userName)
        {
            MainManager.Instance.joinedGame = true;
            LobbyUI.Instance.startGameBtn.interactable = true;
            LobbyManager.Instance.lobby.SetActive(false);
            ArColab.Instance.ArColabUI.SetActive(true);

            userName = _userName;
            StartHost();

            Debug.Log("Starting Host...");
        }

        public void StartClientByName(string _userName)
        {
            MainManager.Instance.joinedGame = true;
            LobbyUI.Instance.startGameBtn.interactable = true;
            LobbyManager.Instance.lobby.SetActive(false);
            ArColab.Instance.ArColabUI.SetActive(true);

            userName = _userName;
            StartClient();

            Debug.Log("Starting Client...");
        }
    }
}

