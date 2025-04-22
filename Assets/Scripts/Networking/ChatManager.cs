using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Multi
{
    public class ChatManager : MonoBehaviour
    {
        public static ChatManager Instance { get; set; }

        // Player Play
        [Header("Player Play - UI")]
        public GameObject playerChatList;
        public GameObject playerChatListContent;
        public GameObject playerChatListPrefab;
        public TMPro.TMP_InputField playerChatMsgInputField;
        public Button sendChatMsg;
        public Button closeChatList;

        [Header("Player Play - Functional")]
        [SerializeField]
        private Color chatListLocalColor;
        [SerializeField]
        private Color chatListRemoteColor;
        public PlayerData chatActiveRemotePD;

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
            sendChatMsg.onClick.AddListener(() => sendMsg(SceneManager.Instance.localPlayer, playerChatMsgInputField.text));
            closeChatList.onClick.AddListener(() => disableChat());
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void sendMsg(GameObject sender, string message)
        {
            PlayerNetwork localPN = sender.GetComponent<PlayerNetwork>();
            ChatMessageNetPack chatMessage = new ChatMessageNetPack
            {
                senderId = localPN.playerData.id,
                receiverId = chatActiveRemotePD.id,
                message = message
            };
            localPN.sendMsgServerRPC(chatMessage);
            //addMsg(chatMessage);
        }

        public void addMsg(ChatMessage chatMessage)
        {
            // Add msg to variable list
            addMsgToList(chatMessage);
            // check if chat window is active
            if (chatActiveRemotePD.id != "")
            {
                // Add msg prefab to content list UI if you are the receiver or sender
                if (chatMessage.receiverId == chatActiveRemotePD.id || chatMessage.receiverId == SceneManager.Instance.localPlayerNet.playerData.id) addMsgPrefabToList(chatMessage);
            }
        }

        public void addMsgToList(ChatMessage chatMessage)
        {
            PlayerNetwork pN = SceneManager.Instance.localPlayer.GetComponent<PlayerNetwork>();
            if (chatMessage.senderId == pN.playerData.id)
                pN.chats[chatMessage.receiverId].Add(chatMessage);
            else
                pN.chats[chatMessage.senderId].Add(chatMessage);
        }

        public void addMsgPrefabToList(ChatMessage chatMessage)
        {
            Transform playerListInst = Instantiate(playerChatListPrefab).transform;
            playerListInst.SetParent(playerChatListContent.transform);
            if (chatMessage.senderId == SceneManager.Instance.localPlayer.GetComponent<PlayerNetwork>().playerData.id)
                playerListInst.GetChild(0).GetComponent<RawImage>().color = chatListLocalColor;
            else
                playerListInst.GetChild(0).GetComponent<RawImage>().color = chatListRemoteColor;
            playerListInst.GetChild(1).GetComponent<TMPro.TMP_Text>().text = chatMessage.message;
        }

        public void enableChat(PlayerData remotePD)
        {
            SceneManager.Instance.playerList.SetActive(false);

            chatActiveRemotePD = remotePD;
            playerChatList.transform.GetChild(0).GetChild(1).GetComponent<TMPro.TMP_Text>().text = remotePD.name;
            for (int i = 0; i < SceneManager.Instance.localPlayer.GetComponent<PlayerNetwork>().chats[remotePD.id].Count; i++)
            {
                GameObject playerListInst = Instantiate(playerChatListPrefab);
                playerListInst.transform.SetParent(playerChatListContent.transform);
                if (SceneManager.Instance.localPlayer.GetComponent<PlayerNetwork>().chats[remotePD.id][i].senderId != remotePD.id)
                    playerListInst.transform.GetChild(0).GetComponent<RawImage>().color = chatListLocalColor;
                else
                    playerListInst.transform.GetChild(0).GetComponent<RawImage>().color = chatListRemoteColor;
                playerListInst.transform.GetChild(1).GetComponent<TMPro.TMP_Text>().text = SceneManager.Instance.localPlayer.GetComponent<PlayerNetwork>().chats[remotePD.id][i].message;
            }

            playerChatList.SetActive(true);
        }

        public void disableChat()
        {
            //Clear Previous Chat
            foreach (Transform chat in playerChatListContent.transform)
            {
                Destroy(chat.gameObject);
            }
            playerChatList.SetActive(false);
            chatActiveRemotePD = new PlayerData("", "", false);
            SceneManager.Instance.playerList.SetActive(true);
        }
    }
}

