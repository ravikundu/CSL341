using System.Linq;
using UnityEngine;

namespace Multi
{
    public class Startup : MonoBehaviour
    {
        public string Client;
        public string Server;

        void Start()
        {
            if (System.Environment.GetCommandLineArgs().Any(arg => arg == "-port"))
            {
                Debug.Log("Starting server");
                UnityEngine.SceneManagement.SceneManager.LoadScene(Server);
            }
            else
            {
                Debug.Log("Starting client");
                UnityEngine.SceneManagement.SceneManager.LoadScene(Client);
            }
        }
    }
}
