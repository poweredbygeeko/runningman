using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class Road : MonoBehaviour {

    public static int pieceIndex = 0;
    public static int drawDistance = 20;

    public static float pieceSize;
    public static float zPos = 0;

    public static Dictionary<int, GameObject> pieces = new Dictionary<int, GameObject>();

    public void Awake() {

        new GameObject("Pieces").transform.parent = this.transform;
        new GameObject("Obstacles").transform.parent = this.transform;
        new GameObject("Consumables").transform.parent = this.transform;

    }

    public void Start() {

        for (int i = 0; i < drawDistance * 2; i++) {
            InitPiece();
        }

    }

    public void Update() {
        CheckInitPiece();
    }

    public static void InitPiece() {

        GameObject piece = new GameObject("Piece_" + pieceIndex);
        piece.AddComponent<MeshRenderer>();
        piece.AddComponent<MeshFilter>();

        Vector3 vertex1 = new Vector3(-pieceSize / 2, 0, -pieceSize / 2);
        Vector3 vertex2 = new Vector3(-pieceSize / 2, 0, pieceSize / 2);
        Vector3 vertex3 = new Vector3(pieceSize / 2, 0, pieceSize / 2);
        Vector3 vertex4 = new Vector3(pieceSize / 2, 0, -pieceSize / 2);

        Vector3[] vertices = { vertex1, vertex2, vertex3, vertex4 };

        Vector2 uv1 = new Vector2(0, 0);
        Vector2 uv2 = new Vector2(0, 1);
        Vector2 uv3 = new Vector2(1, 1);
        Vector2 uv4 = new Vector2(1, 0);

        Vector2[] uvs = { uv1, uv2, uv3, uv4 };

        int[] triangles = { 0, 1, 2, 0, 2, 3 };

        Mesh mesh = new Mesh();

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;
        mesh.RecalculateNormals();

        piece.GetComponent<MeshFilter>().mesh = mesh;
        piece.GetComponent<MeshRenderer>().material = Resources.Load<Material>("Materials/Road/Normal/Road");

        if (pieceIndex > 0) {
            if (Random.Range(0, 10) >= 6) {
                if (Random.Range(0f, 1f) >= .825f) {
                    piece.transform.localScale = new Vector3(1, 1, .5f);
                    piece.GetComponent<Renderer>().material = Resources.Load<Material>("Materials/Road/Crossing/Crossing");
                    Obstacle.obstacles.Add(pieceIndex, Obstacle.InitTrafficLights(zPos + piece.GetComponent<Renderer>().bounds.size.z / 2));
                } else {
                    Obstacle.obstacles.Add(pieceIndex, Obstacle.InitObstacle(zPos + piece.GetComponent<Renderer>().bounds.size.z / 2));
                }
            }
        }

        piece.transform.position = new Vector3(0, 0, zPos + piece.GetComponent<Renderer>().bounds.size.z / 2);
        piece.AddComponent<MeshCollider>();
        piece.transform.parent = GameObject.Find("Road").transform.Find("Pieces");

        zPos += piece.transform.GetComponent<Renderer>().bounds.size.z;

        pieces.Add(pieceIndex, piece);
        pieceIndex++;

        if(Random.Range(0, 100) > 50) {
            Consumable.InitConsumable(piece);
        }

    }

    private void CheckInitPiece() {

        GameObject nearestPiece = pieces[pieceIndex - (drawDistance * 2) + (drawDistance / 2)];
        float distance = nearestPiece.transform.position.z - GameObject.Find("Player").transform.position.z;

        if(distance < pieceSize) {

            InitPiece();

            int index = pieceIndex - drawDistance * 2 - 1;

            Destroy(pieces[index]);
            pieces.Remove(index);

            if(Obstacle.obstacles.ContainsKey(index)) {
                Destroy(Obstacle.obstacles[index]);
                Obstacle.obstacles.Remove(index);
            }

        }

    }

    
}