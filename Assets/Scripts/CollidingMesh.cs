using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class CollidingMesh : MonoBehaviour {

    SkinnedMeshRenderer skinnedMeshRenderer;

    Mesh mesh;

    public void Start() {

        skinnedMeshRenderer = this.GetComponent<SkinnedMeshRenderer>();
        mesh = new Mesh();

        this.gameObject.AddComponent<MeshCollider>().convex = true;
        this.gameObject.AddComponent<Rigidbody>().useGravity = false;
        this.gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;

    }

    public void Update() {

        skinnedMeshRenderer.BakeMesh(mesh);
        this.GetComponent<MeshCollider>().sharedMesh = mesh;

    }

    public void OnCollisionEnter(Collision collision) {

        bool isValidCollision = (!(collision.gameObject.transform.parent != null
            && collision.gameObject.transform.parent.name.Equals("Player")) && !collision.gameObject.name.Contains("Piece"));

        if (!Controller.isGameOver) {
            if (isValidCollision) {
                Controller.GameOver();
            }
        }

    }

}