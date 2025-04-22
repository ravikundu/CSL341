using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using Multi;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ArColab : MonoBehaviour
{
    public static ArColab Instance { get; set; }

    public MainManager mm;
    public GameObject markerPrefab;
    public GameObject marker;
    public ARRaycastManager aRRaycastManager;
    Vector2 screenPoint;
    List<ARRaycastHit> m_Hits = new List<ARRaycastHit>();

    public GameObject ArColabUI;
    public GameObject mainUI;
    public GameObject playerList;
    public GameObject objectList;
    public Button playerListBtn;
    public Button objectListBtn;
    public Button playerListCloseBtn;
    public Button objectListCloseBtn;
    public List<GameObject> spawnObjectList = new();
    public List<GameObject> spawnedObjectList = new();
    public ARType arType;

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
        playerListBtn.onClick.AddListener(() => activateUI(2));
        objectListBtn.onClick.AddListener(() => activateUI(3));
        playerListCloseBtn.onClick.AddListener(() => activateUI(1));
        objectListCloseBtn.onClick.AddListener(() => activateUI(1));
    }

    private void Update()
    {
        if (mm.joinedGame)
        {
            placeMarker();
        }
    }

    void activateUI(int uiIndex)
    {
        mainUI.SetActive(uiIndex == 1);
        playerList.SetActive(uiIndex == 2);
        objectList.SetActive(uiIndex == 3);
    }

    public void spawnObject(int index)
    {
        mm.sceMag.localPlayerNet.spawnObjServerRPC(index);
    }

    void placeMarkerN()
    {
        if (Input.touchCount == 0)
            return;

        screenPoint = Input.GetTouch(0).position;
    }

    void placeMarker()
    {
        if (Input.touchCount == 0)
            return;

        screenPoint = Input.GetTouch(0).position;

        if (arType == ARType.AR)
        {
            if (aRRaycastManager.Raycast(screenPoint, m_Hits, TrackableType.AllTypes))
            {
                Debug.Log("AR Hit object: " + m_Hits[0].trackableId);
                if (m_Hits.Count > 0)
                {
                    if (marker == null)
                    {
                        marker = Instantiate(markerPrefab, m_Hits[0].pose.position, m_Hits[0].pose.rotation);
                        marker.AddComponent<ARAnchor>();
                    }
                    else
                    {
                        marker.transform.position = m_Hits[0].pose.position;
                    }
                }
            }
        }
        else
        {
            if (Input.GetTouch(0).phase == TouchPhase.Began)
            {
                Ray ray = Camera.main.ScreenPointToRay(screenPoint);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit))
                {
                    Debug.Log("Non AR Hit object: " + hit.collider.name);

                    if (hit.collider != null)
                    {
                        if (marker == null)
                        {
                            marker = Instantiate(markerPrefab, hit.transform.position, hit.transform.rotation);
                            marker.AddComponent<ARAnchor>();
                        }
                        else
                        {
                            marker.transform.position = hit.transform.position;
                        }
                    }
                }
            }
        }
    }
}
