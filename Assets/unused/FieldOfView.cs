using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HelperFunctions;
public class FieldOfView : MonoBehaviour
{
    [SerializeField] private LayerMask unwalkableMask;
    private Mesh mesh;
    private float fov;
    private Vector3 origin;
    private float viewDistance;
    private float startingAngle;

    // Start is called before the first frame update
    void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        fov = 90.0f;
        origin = Vector3.zero;
        viewDistance = 50f;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        int rayCount = 20;
        float angle = startingAngle;
        float angleIncrease = fov / 20;
        Vector3[] vertices = new Vector3[rayCount + 2];
        Vector2[] uv = new Vector2[rayCount + 2];
        int[] triangles = new int[rayCount * 3];

        vertices[0] = origin;

        int vertexIndex = 1;
        int triangleIndex = 0;

        for (int i = 0;i <= rayCount; i++)
        {
            Vector3 vertex;
            RaycastHit2D hit = Physics2D.Raycast(origin, MyMathUtils.GetVectorFromAngle(angle), viewDistance, unwalkableMask);
            if (hit.collider == null)
            {
                vertex = origin + MyMathUtils.GetVectorFromAngle(angle) * viewDistance;
            }
            else
            {
                vertex = hit.point;
            }
            vertices[vertexIndex] = vertex;

            if  (i > 0)
            {
                triangles[triangleIndex + 0] = 0;
                triangles[triangleIndex + 1] = vertexIndex - 1;
                triangles[triangleIndex + 2] = vertexIndex;

                triangleIndex += 3;
            }

            vertexIndex++;
            angle -= angleIncrease;
        }

        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.triangles = triangles;
        mesh.bounds = new Bounds(origin, Vector3.one * 1000.0f);

    }

    public void SetOrigin(Vector3 origin)
    {
        this.origin = origin;
    }

    public void SetDirection(Vector3 direction)
    {
        startingAngle = MyMathUtils.GetAngleFromVectorFloat(direction);
    }

    public void SetViewDistance(float distance)
    {
        viewDistance = distance;
    }

    public void SetFOV(float fov)
    {
        this.fov = fov;
    }
}
