using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

[RequireComponent(typeof(ARPointCloudManager))]
public class ARPointCloudHandler : MonoBehaviour
{
    public ARPointCloudManager aRPointCloud;
    List<Vector3> pointsCloud = new List<Vector3>();

    public TMPro.TMP_Text tMP_Text;

    private void Awake()
    {
        aRPointCloud = GetComponent<ARPointCloudManager>();
    }

    void OnEnable()
    {
        if (aRPointCloud != null)
        {
            aRPointCloud.pointCloudsChanged += OnPointCloudsChanged;
        }
    }

    void OnDisable()
    {
        if (aRPointCloud != null)
        {
            aRPointCloud.pointCloudsChanged -= OnPointCloudsChanged;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        tMP_Text.text = $"Point Cloud Count: {pointsCloud.Count}";
    }

    void OnPointCloudsChanged(ARPointCloudChangedEventArgs eventArgs)
    {
        // Clear previous points
        foreach (var obj in pointsCloud)
        {
            // Distroy Point Objects if needed
        }

        pointsCloud.Clear();

        // Get updated points
        foreach (var pointCloud in eventArgs.updated)
        {
            if (pointCloud.positions.HasValue)
            {
                foreach (Vector3 point in pointCloud.positions.Value)
                {
                    // Do Something with Point
                    pointsCloud.Add(point);
                }
            }
        }
    }
}
