using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

[RequireComponent(typeof(MeshCollider))]
public class SplineMeshCollider : MonoBehaviour
{
    [Header("Spline settings")]
    [Tooltip("The spline to generate the road from.")]
    public SplineContainer splineContainer;

    [Header("Road settings")]
    [Tooltip("The width of the road.")]
    public float roadWidth = 1f;

    private void Start()
    {
        if (splineContainer == null)
        {
            Debug.LogError("SplineContainer reference is missing");
            return;
        }

        CreateMeshCollider();
    }
    private void OnValidate()
    {
        SplineMeshCollider collider = GetComponent<SplineMeshCollider>();

        if (collider != null)
        {
            collider.CreateMeshCollider();
        }
    

    CreateMeshCollider();
    }

    public void CreateMeshCollider()
    {
        MeshCollider meshCollider = GetComponent<MeshCollider>();

        Mesh mesh = GenerateMeshFromSpline(splineContainer, roadWidth);

        if (mesh == null)
        {
            Debug.LogError("Mesh generation failed");
            return;
        }

        try
        {
            meshCollider.sharedMesh = mesh;
        }
        catch (System.Exception e)
        {
            Debug.LogError("Failed to set MeshCollider.sharedMesh: " + e.Message);
        }
    }


    private Mesh GenerateMeshFromSpline(SplineContainer splineContainer, float roadWidth = 1f)
    {
        // Check if splineContainer is null
        if (splineContainer == null)
        {
            Debug.LogError("splineContainer is null");
            return null;
        }

        Spline spline = splineContainer.Spline;

        // Check if spline is null
        if (spline == null)
        {
            Debug.LogError("spline is null");
            return null;
        }

        Mesh mesh = new Mesh();

        int resolution = 100;
        Vector3[] vertices = new Vector3[resolution * 2 + 2];
        int[] triangles = new int[resolution * 2 * 6];

        for (int i = 0; i <= resolution; i++)
        {
            float t = i / (float)resolution;

            float3 position, tangent, upVector;
            bool success = spline.Evaluate(t, out position, out tangent, out upVector);

            if (!success)
            {
                Debug.LogError($"Failed to evaluate spline at t = {t}");
                return null;
            }

            float3 sideVector = math.cross(tangent, upVector);
            sideVector = math.normalize(sideVector) * roadWidth / 2f;

            // Generate vertices for the sides of the road
            vertices[2 * i] = position - sideVector;
            vertices[2 * i + 1] = position + sideVector;

            // Skip the first iteration because there's not enough vertices yet to form a quad
            if (i == 0) continue;

            // Generate two triangles for the road segment
            int j = (i - 1) * 12;
            triangles[j] = 2 * (i - 1);
            triangles[j + 1] = 2 * i + 1;
            triangles[j + 2] = 2 * i;
            triangles[j + 3] = 2 * (i - 1);
            triangles[j + 4] = 2 * (i - 1) + 1;
            triangles[j + 5] = 2 * i + 1;
            triangles[j + 6] = 2 * (i - 1) + 1;
            triangles[j + 7] = 2 * (i - 1);
            triangles[j + 8] = 2 * i;
            triangles[j + 9] = 2 * (i - 1) + 1;
            triangles[j + 10] = 2 * i;
            triangles[j + 11] = 2 * i + 1;
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;

        return mesh;
    }

}
