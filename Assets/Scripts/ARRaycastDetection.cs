using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

[RequireComponent(typeof(ARRaycastManager))]
public class ARRaycastDetection : MonoBehaviour
{
    [SerializeField] TMPro.TMP_Text tMP_Text;
    [SerializeField] GameObject objectToInst;
    GameObject objectInst;
    ARRaycastManager m_RaycastManager;
    Vector2 screenPoint;
    List<ARRaycastHit> m_Hits = new List<ARRaycastHit>();


    private void Awake()
    {
        m_RaycastManager = GetComponent<ARRaycastManager>();
    }

    void Update()
    {
        tMP_Text.text = "";
        if (Input.touchCount == 0)
            return;

        screenPoint = Input.GetTouch(0).position;

        if (m_RaycastManager.Raycast(screenPoint, m_Hits, TrackableType.AllTypes))
        {
            tMP_Text.text = "Raycast :: ";
            if (m_Hits.Count > 0)
            {
                tMP_Text.text += "Count :: ";
                if (objectInst == null)
                {
                    tMP_Text.text += "Inst :: ";
                    objectInst = Instantiate(objectToInst, m_Hits[0].pose.position, m_Hits[0].pose.rotation);
                }
                else
                {
                    tMP_Text.text += $"Pos : {m_Hits[0].pose.position} :: ";
                    objectInst.transform.position = m_Hits[0].pose.position;
                }
            }
        }
    }
}
