using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointCloudVisualizer : MonoBehaviour
{
    public int maxStars = 1000;
    public int universeSize = 10;
    public ParticleSystem theParticleSystem;
    private ParticleSystem.Particle[] points;

    void Start()
    {
        GeneratePoints();
    }

    void Update()
    {
        if (points != null)
        {
            theParticleSystem.SetParticles(points, points.Length);
        }
    }

    private void GeneratePoints()
    {
        points = new ParticleSystem.Particle[maxStars];

        for (int i = 0; i < maxStars; i++)
        {
            points[i].position = Random.insideUnitSphere * universeSize;
            points[i].startSize = Random.Range(0.05f, 0.05f);
            points[i].startColor = new Color(1, 1, 1, 1);
        }
    }
}
