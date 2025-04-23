using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.Collections;
using UnityEngine.UI;
using System;

namespace Multi
{
    public class PlayerNetwork : NetworkBehaviour
    {
        //Debug
        public PlayerData playerData;

        private NetworkVariable<PlayerDataNetPack> playerDataNetPack = new NetworkVariable<PlayerDataNetPack>(
            new PlayerDataNetPack { id = 0.ToString(), name = "", isHost = false },
            NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

        public Dictionary<string, List<ChatMessage>> chats;

        [SerializeField]
        private MainManager main;

        // User Mechanincs
        [SerializeField]
        private float moveSpeed;

        // Start is called before the first frame update
        void Start()
        {
            main = MainManager.Instance;
            if (IsLocalPlayer)
            {
                ///// Initialize and Sync Local Player /////
                /// Initialize Variables
                chats = new Dictionary<string, List<ChatMessage>>();
                /// Sync Player Data at start
                setPlayerData(new FixedString128Bytes(main.sceMag.userName));
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (!IsLocalPlayer) return;

            if (Input.GetKeyDown(KeyCode.G))
            {
                //playerData.Value.id = (int)Random.Range(1, 100);
            }

            Vector3 movDir = new Vector3(0, 0, 0);
            if (Input.GetKey(KeyCode.W)) movDir.z += 1;
            if (Input.GetKey(KeyCode.S)) movDir.z -= 1;
            if (Input.GetKey(KeyCode.D)) movDir.x += 1;
            if (Input.GetKey(KeyCode.A)) movDir.x -= 1;

            transform.position += moveSpeed * movDir * Time.deltaTime;
        }

        /// <summary>
        /// Flow Functions
        /// </summary>
        ///

        // Called at local instance to sync it's value to all the remote instance of this local player
        public void setPlayerData(FixedString128Bytes name)
        {
            //playerData.Value = new PlayerData { id = new FixedString32Bytes(UtilityManager.GenerateRandomKey(5, false)), name = name, isHost = IsServer };
            playerDataNetPack.Value = new PlayerDataNetPack { id = UtilityManager.GenerateRandomKey(5, false), name = name.ConvertToString(), isHost = IsServer };
            playerData = new PlayerData(playerDataNetPack.Value);
        }

        // Called when new player spawn to sync local player data to remote instances of this local player
        public void syncPlayerData()
        {
            try
            {
                PlayerDataNetPack playerDataNetPack = new PlayerDataNetPack { id = playerData.id, name = playerData.name, isHost = playerData.isHost };
                syncPlayerDataServerRpc(playerDataNetPack);
            }catch(Exception e)
            {
                Debug.Log(e.Message);
            }
        }

        public void setupLocalUser()
        {
            //Assign Player Name
            gameObject.name = $"{playerData.name}_Local";

            //Assign Player Instance
            main.sceMag.localPlayer = gameObject;
            main.sceMag.localPlayerNet = gameObject.GetComponent<PlayerNetwork>();

            /// UI
            main.sceMag.setupLocalPlayerUI();
        }

        public void setupRemoteUser()
        {
            //Apply Player Name
            gameObject.name = $"{playerData.name}_Remote";

            //Assign Player Instance
            main.sceMag.remotePlayers.Add(gameObject);

            //Instantiate PlayerListPrefab for all remote users
            GameObject connetedPlayerListInst = Instantiate(main.sceMag.playerListPrefab);
            connetedPlayerListInst.transform.SetParent(main.sceMag.playerListContent.transform);
            connetedPlayerListInst.transform.GetChild(0).GetComponent<TMPro.TMP_Text>().text = name;
            connetedPlayerListInst.transform.GetChild(1).GetComponent<Button>().onClick.AddListener(() => ChatManager.Instance.enableChat(playerData));

            //Create chat variable instance
            main.sceMag.localPlayerNet.chats.Add(playerData.id, new List<ChatMessage>());
            // Add Extra msg for debug
            //for (int i = 0; i < 5; i++)
            //{
            //    main.sceMag.localPlayerNet.chats[playerData.id].Add(
            //        new ChatMessage()
            //        {
            //            senderId = playerData.id,
            //            receiverId = main.sceMag.localPlayerNet.playerData.id,
            //            message = UtilityManager.GenerateRandomKey(16, false)
            //        }
            //    );
            //}
            //for (int i = 0; i < 5; i++)
            //{
            //    main.sceMag.localPlayerNet.chats[playerData.id].Add(
            //        new ChatMessage()
            //        {
            //            senderId = main.sceMag.localPlayerNet.playerData.id,
            //            receiverId = playerData.id,
            //            message = UtilityManager.GenerateRandomKey(16, false)
            //        }
            //    );
            //}
        }

        /// <summary>
        /// Network Functions
        /// </summary>
        ///

        public override void OnNetworkSpawn()
        {
            playerDataNetPack.OnValueChanged = (PlayerDataNetPack prevVal, PlayerDataNetPack newVal) =>
            {
                //Debug
                playerData = new PlayerData(newVal);
                // All User Specific
                Debug.Log($"Value Synced for client ID : {OwnerClientId} ::: Values :: id : {playerData.id}, name : {playerData.name}, isHost : {playerData.isHost}");

                // Local User Specific
                if (IsLocalPlayer)
                {
                    setupLocalUser();
                }

                // Remote User Specific
                if (!IsLocalPlayer)
                {
                    // Send Local Player Data
                    main.sceMag.localPlayer.GetComponent<PlayerNetwork>().syncPlayerData();
                    // Setup local player's object on remote users
                    setupRemoteUser();
                }
            };
        }

        /// <summary>
        /// RPCs Functions
        /// </summary>
        ///

        [ServerRpc]
        void syncPlayerDataServerRpc(PlayerDataNetPack _playerDataNetPack)
        {
            syncPlayerDataClientRpc(_playerDataNetPack);
        }
        [ClientRpc]
        void syncPlayerDataClientRpc(PlayerDataNetPack _playerDataNetPack)
        {
            playerData = new PlayerData(_playerDataNetPack);
            if (!IsLocalPlayer) {
                // All User Specific
                Debug.Log($"Value Synced for client ID : {OwnerClientId} ::: Value :: id : {playerData.id}, name : {playerData.name}, isHost : {playerData.isHost}");
                // Setup remote player's object on local user
                setupRemoteUser();
            }
        }


        [ServerRpc]
        public void sendMsgServerRPC(ChatMessageNetPack _msg)
        {
            sendMsgClientRPC(_msg);
        }
        [ClientRpc]
        public void sendMsgClientRPC(ChatMessageNetPack _msg)
        {
            ChatMessage msg = new ChatMessage(_msg);
            if(IsLocalPlayer || _msg.receiverId == main.sceMag.localPlayer.GetComponent<PlayerNetwork>().playerData.id)
            {
                ChatManager.Instance.addMsg(msg);
                Debug.Log($"ReceiverID : {msg.receiverId} :: SenderID : {msg.senderId} :: Message : {msg.message}");
            }
        }

        [ServerRpc]
        public void spawnObjServerRPC(int index, ServerRpcParams rpcParams = default)
        {
            GameObject instance = Instantiate(main.arColab.spawnObjectList[index]);
            NetworkObject instanceNetworkObject = instance.GetComponent<NetworkObject>();
            instanceNetworkObject.SpawnWithOwnership(rpcParams.Receive.SenderClientId);
            //instance.transform.SetParent(main.arColab.marker.transform);
            //instance.transform.position = Vector3.zero;
            instance.transform.position = main.arColab.marker.transform.position + new Vector3(0, 0.25f, 0);
            main.arColab.spawnedObjectList.Add(instance);
        }
    }
}