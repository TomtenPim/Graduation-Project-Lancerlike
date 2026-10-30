using Game;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public struct Face
{
    public List<Vector3>    vertecies { get; private set; }
    public List<int>        triangels { get; private set; }
    public List<Vector2>    uvs { get; private set; }


    public Face(List<Vector3> vertecies, List<int> triangels, List<Vector2> uvs)
    {
        this.vertecies = vertecies;
        this.triangels = triangels;
        this.uvs = uvs;
    }
}


[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class HexRenderer : MonoBehaviour
{
    private Mesh thisMesh;
    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;

    List<Face> faces = new List<Face>();

    public Material material;

    public float innerSize = 0.495f;
    public float outerSize = 0.55f;
    public float height = 0;
    public bool isFlatTopped;

    private void Awake()
    {
        meshFilter = GetComponent<MeshFilter>();
        meshRenderer = GetComponent<MeshRenderer>();

        thisMesh = new Mesh();
        thisMesh.name = "Hex";

        meshFilter.mesh = thisMesh;
        meshRenderer.material = material;
    }

    private void OnEnable()
    {
        DrawMesh();
    }

    public void OnValidate()
    {
        if (Application.isPlaying)
        {
            DrawMesh();
        }
    }

    public void DrawMesh()
    {
        if (meshFilter == null || meshRenderer == null)
        {
            return;
        }

        DrawFaces();
        CombineFaces();
    }

    private void DrawFaces()
    {
        faces = new List<Face>();

        for (int point = 0; point < 6; point++)
        {
            faces.Add(CreateFace(innerSize, outerSize, height / 2f, height / 2f, point));
        }
    }

    private Face CreateFace(float innerRad, float outerRad, float heightA, float heightB, int point, bool reverse = false)
    {
        Vector3 pointA = GetPoint(innerRad, heightB, point);
        Vector3 pointB = GetPoint(innerRad, heightB, (point < 5) ? point + 1 : 0);
        Vector3 pointC = GetPoint(outerRad, heightA, (point < 5) ? point + 1 : 0);
        Vector3 pointD = GetPoint(outerRad, heightA, point);

        List<Vector3> vertices = new List<Vector3>() { pointA, pointB, pointC, pointD };
        List<int> triangles = new List<int>() { 0, 1, 2, 2, 3, 0 };
        List<Vector2> uvs = new List<Vector2>() { new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, 1), new Vector2(0, 1) };

        if (reverse)
        {
            vertices.Reverse();
        }

        return new Face(vertices, triangles, uvs);
    }

    protected Vector3 GetPoint(float size, float height, int index)
    {
        float angleDegrees = isFlatTopped ? 60 * index : 60 * index - 30;
        float angleRadians = Mathf.PI / 180f * angleDegrees;
        return new Vector3(size * Mathf.Cos(angleRadians), height, size * Mathf.Sin(angleRadians));
    }

    private void CombineFaces()
    {
        List<Vector3> vertecies = new List<Vector3>();
        List<int> tris = new List<int>();
        List<Vector2> uvs = new List<Vector2>();

        for (int i = 0; i < faces.Count; i++)
        {
            vertecies.AddRange(faces[i].vertecies);
            uvs.AddRange(faces[i].uvs);

            int offset = (4 * i);
            foreach (int triangle in faces[i].triangels)
            {
                tris.Add(triangle + offset);
            }
        }

        thisMesh.vertices = vertecies.ToArray();
        thisMesh.triangles = tris.ToArray();
        thisMesh.uv = uvs.ToArray();
        thisMesh.RecalculateNormals();

    }

    public void SetMaterial(Material material)
    {
        this.material = material;
    }
}
