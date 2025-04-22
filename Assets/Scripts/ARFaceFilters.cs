using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

[RequireComponent(typeof(ARFaceManager))]
public class ARFaceFilters : MonoBehaviour
{
    ARFaceManager aRFaceManager;
    ARFace aRFace;

    public Color colorToApply;

    public Material defaultMaterial;
    public Material materialToApply;

    public GameObject infoUI;

    [SerializeField] TMPro.TMP_Text tMP_Text;

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

    public void getDefault()
    {
        defaultMaterial = aRFace.GetComponent<Renderer>().material;
    }

    public void resetFace()
    {
        aRFace.GetComponent<Renderer>().material = defaultMaterial;
        aRFace.transform.GetChild(1).gameObject.SetActive(false);
        aRFace.transform.GetChild(2).gameObject.SetActive(false);
    }

    public void applyColor()
    {
        if (aRFace == null)
        {
            Debug.Log("No face found");
            return;
        }
        resetFace();
        aRFace.GetComponent<Material>().color = colorToApply;
        toggleInfoUI();
    }

    public void applyTexture()
    {
        if (aRFace == null)
        {
            Debug.Log("No face found");
            return;
        }
        resetFace();
        aRFace.GetComponent<Renderer>().material = materialToApply;
        toggleInfoUI();
    }

    public void applyAddOns()
    {
        if (aRFace == null)
        {
            Debug.Log("No face found");
            return;
        }
        aRFace.transform.GetChild(1).gameObject.SetActive(true);
        aRFace.transform.GetChild(2).gameObject.SetActive(true);
        toggleInfoUI();
    }

    public void toggleInfoUI()
    {
        if(infoUI.activeInHierarchy) infoUI.SetActive(false);
        else infoUI.SetActive(true);
    }
}



