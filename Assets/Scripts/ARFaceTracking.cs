using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

[RequireComponent(typeof(ARFaceManager))]
public class ARFaceTracking : MonoBehaviour
{
    [SerializeField]TMPro.TMP_Text tMP_Text;
    ARFaceManager aRFaceManager;

    ARFace aRFace;

    private void Awake()
    {
        // This is inefficient. You should re-use a saved reference instead.
        aRFaceManager = GetComponent<ARFaceManager>();
    }

    // Start is called before the first frame update
    void Start()
    {
        // SubscribeToFacesChanged
        aRFaceManager.facesChanged += OnFacesChanged;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnFacesChanged(ARFacesChangedEventArgs changes)
    {
        foreach (var face in changes.added)
        {
            // handle added faces
            tMP_Text.text = $"Face Added: {face.name}";
            Debug.Log($"Face Added: {face.name}");
            aRFace = face;
        }

        foreach (var face in changes.updated)
        {
            // handle updated faces
            tMP_Text.text = $"Face Updated: {face.name}";
            Debug.Log($"Face Updated: {face.name}");
            aRFace = face;
        }

        foreach (var face in changes.removed)
        {
            // handle removed faces
            tMP_Text.text = $"Face Removed: {face.name}";
            Debug.Log($"Face Removed: {face.name}");
            aRFace = null;
        }
    }
}
