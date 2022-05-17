using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class TrafficLights : MonoBehaviour {

    private static (bool, bool) state = (false, true);
    private static float timeSinceChange = 0;
    private static float changeTime = 0;

    private static Material greenMaterial;
    private static Material redMaterial;

    private static Color greenColor;
    private static Color redColor;
    private static Color greenEmissionColor;
    private static Color redEmissionColor;
    private static Color offColor;

    private static GameObject currentPiece;

    public void Start() {
        greenMaterial = Resources.Load<Material>("Materials/Traffic Lights/Green");
        redMaterial = Resources.Load<Material>("Materials/Traffic Lights/Red");
        offColor = new Color(0, 0, 0, 1);
        greenColor = new Color(.28f, .41f, .07f, 1);
        redColor = new Color(1, .15f, 0, 1);
        greenEmissionColor = new Color(.22f, .33f, .03f, 1);
        redEmissionColor = new Color(1, 0, 0, 1);
    }

    public void Update() {
        checkPlayerPos();
        checkLights();
    }

    private static void checkLights() {
        timeSinceChange += Time.deltaTime;
        if (timeSinceChange >= changeTime) {
            state = (!state.Item1, !state.Item2);
            greenMaterial.color = state.Item1 ? greenColor : offColor;
            greenMaterial.SetColor("_EmissionColor", state.Item1 ? greenEmissionColor : offColor);
            redMaterial.color = state.Item2 ? redColor : offColor;
            redMaterial.SetColor("_EmissionColor", state.Item2 ? redEmissionColor : offColor);
            if(state.Item1) {
                changeTime = 3.75f;
            } else {
                changeTime = 1.5f;
            }
            timeSinceChange = 0;
        }
    }


    private static void checkPlayerPos() {

        if (!Controller.isGameOver) {

            Vector3 rayOrigin = GameObject.Find("Player").transform.position + new Vector3(0, 1, 0);

            Ray ray = new Ray(rayOrigin, - new Vector3(0, Player.playerSize.y * 2.5f, 0));
            RaycastHit info;

            if (Physics.Raycast(ray, out info, Mathf.Infinity)) {
                Debug.DrawLine(ray.origin, rayOrigin + ray.direction * 100, Color.green);
                if (info.collider.gameObject.name.Contains("Piece")) {
                    currentPiece = info.collider.gameObject;
                }
            } else {
                Debug.DrawLine(ray.origin, rayOrigin + ray.direction * 100, Color.red);
            }

            if (currentPiece != null) {
                if (currentPiece.transform.localScale.z < 1) {
                    if (state.Item1 == false) {
                        Controller.GameOver();
                    }
                }
            }
        }
    }
}
