using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

[RequireComponent(typeof(ARTrackedImageManager))]
public class ARImageTracking : MonoBehaviour
{
    [SerializeField] TMPro.TMP_Text tMP_Text;
    [SerializeField] ARTrackedImageManager m_TrackedImageManager;
    [SerializeField] GameObject cube;
    [SerializeField] GameObject sphere;
    [SerializeField] GameObject cylinder;
    [SerializeField] GameObject capsule;
    Dictionary<string,ARTrackedImage> imageCurrentlyTracking = new Dictionary<string, ARTrackedImage>();

    // Start is called before the first frame update
    void Awake()
    {
        m_TrackedImageManager = GetComponent<ARTrackedImageManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnEnable() => m_TrackedImageManager.trackedImagesChanged += OnChanged;

    void OnDisable() => m_TrackedImageManager.trackedImagesChanged -= OnChanged;

    void OnChanged(ARTrackedImagesChangedEventArgs eventArgs)
    {
        foreach (var newImage in eventArgs.added)
        {
            // Handle added event
            tMP_Text.text = "Added";
        }

        foreach (var updatedImage in eventArgs.updated)
        {
            // Handle updated event
            tMP_Text.text = "Updated";
        }

        foreach (var removedImage in eventArgs.removed)
        {
            // Handle removed event
            tMP_Text.text = "removed";
        }

        ListAllImages();
    }

    void ListAllImages()
    {
        Debug.Log($"There are {m_TrackedImageManager.trackables.count} images being tracked.");
        tMP_Text.text = $"There are {m_TrackedImageManager.trackables.count} images being tracked.";

        imageCurrentlyTracking.Clear();

        foreach (var trackedImage in m_TrackedImageManager.trackables)
        {
            Debug.Log($"Image: {trackedImage.referenceImage.name} is at " + $"{trackedImage.transform.position}");
            tMP_Text.text += $"Image: {trackedImage.referenceImage.name} is at " + $"{trackedImage.transform.position} :: ";

            imageCurrentlyTracking.Add(trackedImage.referenceImage.name, trackedImage);
        }

        setImageRef(cube, "one");
        setImageRef(sphere, "two");
        setImageRef(cylinder, "marker");
        setImageRef(capsule, "earth");
    }

    void setImageRef(GameObject obj, string markerName)
    {
        if (imageCurrentlyTracking.Keys.ToList().Contains(markerName))
        {
            obj.transform.SetParent(imageCurrentlyTracking[markerName].transform);
            obj.transform.localPosition = Vector3.zero;
            obj.SetActive(true);
        }
        else
        {
            obj.SetActive(false);
        }
    }
}
