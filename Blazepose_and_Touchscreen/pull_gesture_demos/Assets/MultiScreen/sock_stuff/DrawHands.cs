using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawHands : MonoBehaviour
{
    public SocketClient_Cam sockData;
    public GameObject[] joints = new GameObject[21];
    private Vector3 outScreenPos = new Vector3(0.0f, 1.0f, 0.0f);

    // appearance variables
    public int _cylinderResolution = 12;
    private const float CYLINDER_MESH_RESOLUTION = 0.1f; //in centimeters, meshes within this resolution will be re-used
    public float _cylinderRadius = 0.05f;
    public bool _castShadows = true;
    [SerializeField]
    private Material _material;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (sockData.handpoints[0,0] == 0) {
            for (var i = 0; i < 21; i++) { joints[i].transform.position = outScreenPos; }
            return;
        }

        for (var i = 0; i < 21; i++) {
            joints[i].transform.position = new Vector3(-sockData.handpoints[i, 0], sockData.handpoints[i, 2], -sockData.handpoints[i, 1]);
        }
        drawAllCylinders(joints);
    }

    private void drawAllCylinders(GameObject[] joints)
    {
        drawCylinder(joints[0].transform.position, joints[1].transform.position);
        drawCylinder(joints[1].transform.position, joints[2].transform.position);
        drawCylinder(joints[2].transform.position, joints[3].transform.position);
        drawCylinder(joints[3].transform.position, joints[4].transform.position);
        drawCylinder(joints[0].transform.position, joints[5].transform.position);
        drawCylinder(joints[5].transform.position, joints[6].transform.position);
        drawCylinder(joints[6].transform.position, joints[7].transform.position);
        drawCylinder(joints[7].transform.position, joints[8].transform.position);
        drawCylinder(joints[5].transform.position, joints[9].transform.position);
        drawCylinder(joints[9].transform.position, joints[10].transform.position);
        drawCylinder(joints[10].transform.position, joints[11].transform.position);
        drawCylinder(joints[11].transform.position, joints[12].transform.position);
        drawCylinder(joints[9].transform.position, joints[13].transform.position);
        drawCylinder(joints[13].transform.position, joints[14].transform.position);
        drawCylinder(joints[14].transform.position, joints[15].transform.position);
        drawCylinder(joints[15].transform.position, joints[16].transform.position);
        drawCylinder(joints[13].transform.position, joints[17].transform.position);
        drawCylinder(joints[17].transform.position, joints[18].transform.position);
        drawCylinder(joints[18].transform.position, joints[19].transform.position);
        drawCylinder(joints[19].transform.position, joints[20].transform.position);
        drawCylinder(joints[0].transform.position, joints[17].transform.position);
    }

    private void drawCylinder(Vector3 a, Vector3 b)
    {
        if (isNaN(a) || isNaN(b)) { return; }

        float length = (a - b).magnitude;

        if ((a - b).magnitude > 0.001f)
        {
            Graphics.DrawMesh(getCylinderMesh(length),
                              Matrix4x4.TRS(a,
                                            Quaternion.LookRotation(b - a),
                                            new Vector3(transform.lossyScale.x, transform.lossyScale.x, 1)),
                              _material,
                              gameObject.layer,
                              null, 0, null, _castShadows);
        }
    }

    private bool isNaN(Vector3 v)
    {
        return float.IsNaN(v.x) || float.IsNaN(v.y) || float.IsNaN(v.z);
    }

    private Dictionary<int, Mesh> _meshMap = new Dictionary<int, Mesh>();
    private Mesh getCylinderMesh(float length)
    {
        int lengthKey = Mathf.RoundToInt(length * 100 / CYLINDER_MESH_RESOLUTION);

        Mesh mesh;
        if (_meshMap.TryGetValue(lengthKey, out mesh))
        {
            return mesh;
        }

        mesh = new Mesh();
        mesh.name = "GeneratedCylinder";
        mesh.hideFlags = HideFlags.DontSave;

        List<Vector3> verts = new List<Vector3>();
        List<Color> colors = new List<Color>();
        List<int> tris = new List<int>();

        Vector3 p0 = Vector3.zero;
        Vector3 p1 = Vector3.forward * length;
        for (int i = 0; i < _cylinderResolution; i++)
        {
            float angle = (Mathf.PI * 2.0f * i) / _cylinderResolution;
            float dx = _cylinderRadius * Mathf.Cos(angle);
            float dy = _cylinderRadius * Mathf.Sin(angle);

            Vector3 spoke = new Vector3(dx, dy, 0);

            verts.Add(p0 + spoke);
            verts.Add(p1 + spoke);

            colors.Add(Color.white);
            colors.Add(Color.white);

            int triStart = verts.Count;
            int triCap = _cylinderResolution * 2;

            tris.Add((triStart + 0) % triCap);
            tris.Add((triStart + 2) % triCap);
            tris.Add((triStart + 1) % triCap);

            tris.Add((triStart + 2) % triCap);
            tris.Add((triStart + 3) % triCap);
            tris.Add((triStart + 1) % triCap);
        }

        mesh.SetVertices(verts);
        mesh.SetIndices(tris.ToArray(), MeshTopology.Triangles, 0);
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
        mesh.UploadMeshData(true);

        _meshMap[lengthKey] = mesh;

        return mesh;
    }
}
