using UnityEngine;
using System.Collections;

public class Consumable : MonoBehaviour {

    private static float rotationSpeed = 90;

    void Start() {
        if(this.name.Contains("Boost")) {
            this.transform.Rotate(Vector3.left, 30);
        }
        float scale = Player.playerSize.x * .65f / this.GetComponent<Renderer>().bounds.size.x;
        this.transform.localScale = Vector3.one * scale;
        this.gameObject.AddComponent<MeshCollider>();
        GetComponent<MeshCollider>().convex = true;
        GetComponent<MeshCollider>().isTrigger = true;
    }

    void Update() {
        this.transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other) {
        if(other.transform.parent != null && other.transform.parent.name.Equals("Player")) {
            if(this.name.Contains("Boost")) {
                Time.timeScale = 2.5f;
                Controller.isCollisionsActive = false;
            }
            Destroy(this.gameObject);
        }
    }

    public static void InitConsumable(GameObject piece) {

        string[] consumables = { "Coin", "Boost" };
        string consumableName = consumables[Random.Range(0, 2)];

        Bounds bounds = piece.GetComponent<Renderer>().bounds;

        GameObject powerup = Instantiate(Resources.Load<GameObject>("Models/Powerups/" + consumableName));
        powerup.transform.position = new Vector3(Random.Range(bounds.min.x, bounds.max.x), Player.playerSize.y * .85f, Random.Range(bounds.min.z, bounds.max.z));
        powerup.AddComponent<Consumable>();
        powerup.transform.parent = GameObject.Find("Road").transform.Find("Consumables");

    }

}
