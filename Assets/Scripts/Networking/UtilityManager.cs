using System.Text;
using System.Threading.Tasks;
using Unity.Collections;
using Unity.Netcode;
using Unity.Services.Lobbies.Models;
using UnityEngine;

namespace Multi
{
    public class UtilityManager : MonoBehaviour
    {
        // Length of the random key
        public int keyLength = 16;
        // Include special characters in the random key
        public bool includeSpecialCharacters = false;
        // Character sets
        private const string alphanumericChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        private const string specialCharsComplete = "!@#$%^&*()-_=+[]{}|;:'\",.<>?/";
        private const string specialChars = "!@#$%^&";

        // Method to generate the random key
        public static string GenerateRandomKey(int length, bool includeSpecials)
        {
            StringBuilder keyBuilder = new StringBuilder();
            string charSet = alphanumericChars;

            if (includeSpecials)
            {
                charSet += specialChars;
            }

            for (int i = 0; i < length; i++)
            {
                char randomChar = charSet[Random.Range(0, charSet.Length)];
                keyBuilder.Append(randomChar);
            }

            return keyBuilder.ToString();
        }

        public static Task WaitForSeconds(float _sec)
        {
            return Task.Delay((int)(_sec * 1000));
        }
    }

    //Functional Enums
    public enum UserType { Server, Player }
    public enum PlayerNetworkType { Host, Client }
    public enum PlayerType { Local, Remote }
    public enum GamePlayType { FriendPlay, TeamPlay }
    public enum ARType { AR, Non_AR }

    //Functional Structs
    public struct PlayerDataNetPack : INetworkSerializable
    {
        public FixedString32Bytes id;
        public FixedString128Bytes name;
        public bool isHost;

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref id);
            serializer.SerializeValue(ref name);
            serializer.SerializeValue(ref isHost);
        }
    }
    [System.Serializable]
    public struct PlayerData
    {
        public string id;
        public string name;
        public bool isHost;

        public PlayerData(string _id, string _name, bool _isHost)
        {
            id = _id;
            name = _name;
            isHost = _isHost;
        }
        public PlayerData(PlayerDataNetPack _playerData)
        {
            id = _playerData.id.ConvertToString();
            name = _playerData.name.ConvertToString();
            isHost = _playerData.isHost;
        }
    }
    public struct ChatMessageNetPack : INetworkSerializable
    {
        public FixedString32Bytes senderId;
        public FixedString32Bytes receiverId;
        public FixedString128Bytes message;

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref senderId);
            serializer.SerializeValue(ref receiverId);
            serializer.SerializeValue(ref message);
        }
    }
    [System.Serializable]
    public struct ChatMessage
    {
        public string senderId;
        public string receiverId;
        public string message;

        public ChatMessage(string _senderId, string _receiverId, string _message)
        {
            senderId = _senderId;
            receiverId = _receiverId;
            message = _message;
        }
        public ChatMessage(ChatMessageNetPack _chatMessage)
        {
            senderId = _chatMessage.senderId.ConvertToString();
            receiverId = _chatMessage.receiverId.ConvertToString();
            message = _chatMessage.message.ConvertToString();
        }
    }

    public struct LobbyDataNetPack : INetworkSerializable
    {
        public FixedString32Bytes id;
        public FixedString128Bytes name;
        public FixedString32Bytes gameMode;
        public int maxPlayers;
        public int maxTeam;
        public FixedString32Bytes mapName;

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref id);
            serializer.SerializeValue(ref name);
            serializer.SerializeValue(ref gameMode);
            serializer.SerializeValue(ref maxPlayers);
            serializer.SerializeValue(ref maxTeam);
            serializer.SerializeValue(ref mapName);
        }
    }
    [System.Serializable]
    public struct LobbyData
    {
        public string id;
        public string name;
        public string gameMode;
        public int maxPlayers;
        public int maxTeam;
        public string mapName;

        public LobbyData(string _id, string _name, string _gameMode, int _maxPlayers, int _maxTeam, string _mapName)
        {
            id = _id;
            name = _name;
            gameMode = _gameMode;
            maxPlayers = _maxPlayers;
            maxTeam = _maxTeam;
            mapName = _mapName;
        }
        public LobbyData(LobbyDataNetPack _lobbyData)
        {
            id = _lobbyData.id.ConvertToString();
            name = _lobbyData.name.ConvertToString();
            gameMode = _lobbyData.gameMode.ConvertToString();
            maxPlayers = _lobbyData.maxPlayers;
            maxTeam = _lobbyData.maxTeam;
            mapName = _lobbyData.mapName.ConvertToString();
        }
    }
}
