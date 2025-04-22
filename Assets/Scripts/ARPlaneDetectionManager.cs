using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

[RequireComponent(typeof(ARPlaneManager))]
public class ARPlaneDetectionManager : MonoBehaviour
{
    public TMPro.TMP_Text tMP_Text;

    private void Awake()
    {
        GetComponent<ARPlaneManager>().planesChanged += ARFeatures_planesChanged;
    }

    private void Start()
    {
        if (LoaderUtility.GetActiveLoader()?.GetLoadedSubsystem<XRPlaneSubsystem>() != null)
        {
            // XRPlaneSubsystem was loaded. The platform supports plane detection.
            tMP_Text.text = "Supported";
        }
        else
        {
            tMP_Text.text = "Not Supported";
        }
    }

    private void ARFeatures_planesChanged(ARPlanesChangedEventArgs changes)
    {
        foreach (var plane in changes.added)
        {
            tMP_Text.text = "";
        }

        foreach (var plane in changes.updated)
        {
            // handle updated planes
        }

        foreach (var plane in changes.removed)
        {
            // handle removed planes
        }
    }

    private void OnDestroy()
    {
        GetComponent<ARPlaneManager>().planesChanged -= ARFeatures_planesChanged;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
