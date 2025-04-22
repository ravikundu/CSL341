using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

[RequireComponent(typeof(ARRaycastManager), typeof(ARAnchorManager))]
public class ARAnchorHandler : MonoBehaviour
{
    ARRaycastManager aRRaycastManager;
    ARAnchorManager aRAnchorManager;

    [SerializeField] GameObject arAnchorPrefab;
    GameObject arAnchor;

    Vector2 screenPoint;
    List<ARRaycastHit> m_Hits = new List<ARRaycastHit>();

    TMPro.TMP_Text tMP_Text;


    private void Awake()
    {
        aRRaycastManager = GetComponent<ARRaycastManager>();
        aRAnchorManager = GetComponent<ARAnchorManager>();
    }

    void Update()
    {
        raycastAnchorHandler1();
    }

    void raycastAnchorHandler1()
    {
        tMP_Text.text = "";
        if (Input.touchCount == 0)
            return;

        screenPoint = Input.GetTouch(0).position;

        if (aRRaycastManager.Raycast(screenPoint, m_Hits, TrackableType.AllTypes))
        {
            tMP_Text.text = "Raycast :: ";
            if (m_Hits.Count > 0)
            {
                tMP_Text.text += "Count :: ";
                if (arAnchor == null)
                {
                    tMP_Text.text += "Inst :: ";
                    arAnchor = Instantiate(arAnchorPrefab, m_Hits[0].pose.position, m_Hits[0].pose.rotation);
                    arAnchor.AddComponent<ARAnchor>();
                }
                else
                {
                    tMP_Text.text += $"Pos : {m_Hits[0].pose.position} :: ";
                    arAnchor.transform.position = m_Hits[0].pose.position;
                }
            }
        }
    }
}
