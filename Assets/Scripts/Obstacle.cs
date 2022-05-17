using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class Obstacle : MonoBehaviour {

    public static Dictionary<int, GameObject> obstacles = new Dictionary<int, GameObject>();

    public static GameObject InitObstacle(float zPos) {

        int random = Random.Range(0, 4);

        if(random == 0) {
            return InitPipe(zPos);
        } else if(random == 1) {
            return InitCones(zPos);
        } else if(random == 2) {
            return InitBarricade(zPos);
        } else if(random == 3) {
            return InitFence(zPos);
        }

        return null;

    }

    public static GameObject InitPipe(float zPos) {

        GameObject pipe = new GameObject("Pipe_" + Road.pieceIndex);

        GameObject leftBase = Instantiate(Resources.Load<GameObject>("Models/Obstacles/Pipe/Base"));
        float scale = (Road.pieceSize / 15) / leftBase.GetComponent<Renderer>().bounds.size.x;
        leftBase.transform.localScale = Vector3.one * scale;
        leftBase.transform.position = new Vector3((-Road.pieceSize / 2) * .7f, 0, zPos);

        GameObject rightBase = Instantiate(Resources.Load<GameObject>("Models/Obstacles/Pipe/Base"));
        rightBase.transform.localScale = Vector3.one * scale;
        rightBase.transform.position = new Vector3(-leftBase.transform.position.x, 0, zPos);

        GameObject leftRiser = Instantiate(Resources.Load<GameObject>("Models/Obstacles/Pipe/MidSection"));

        float riserHeight = (Random.Range(0, 2) == 0) ? Player.playerSize.y * .5f : Player.playerSize.y * 0f;
        riserHeight /= leftRiser.GetComponent<Renderer>().bounds.size.x;

        leftRiser.transform.localScale = new Vector3(riserHeight, scale, scale);
        leftRiser.transform.rotation = Quaternion.Euler(0, 0, 90);
        leftRiser.transform.position = new Vector3(leftBase.transform.position.x, leftRiser.GetComponent<Renderer>().bounds.size.y / 2, zPos);

        GameObject rightRiser = Instantiate(Resources.Load<GameObject>("Models/Obstacles/Pipe/MidSection"));
        rightRiser.transform.localScale = leftRiser.transform.localScale;
        rightRiser.transform.rotation = leftRiser.transform.rotation;
        rightRiser.transform.position = new Vector3(-leftRiser.transform.position.x, leftRiser.transform.position.y, zPos);

        GameObject leftBend = Instantiate(Resources.Load<GameObject>("Models/Obstacles/Pipe/Bend"));
        leftBend.transform.localScale = Vector3.one * scale;
        leftBend.transform.position = new Vector3(leftRiser.transform.position.x, leftRiser.GetComponent<Renderer>().bounds.max.y, zPos);
        leftBend.transform.rotation = Quaternion.Euler(0, 180, 0);

        GameObject rightBend = Instantiate(Resources.Load<GameObject>("Models/Obstacles/Pipe/Bend"));
        rightBend.transform.localScale = Vector3.one * scale;
        rightBend.transform.position = new Vector3(rightRiser.transform.position.x, rightRiser.GetComponent<Renderer>().bounds.max.y, zPos);

        GameObject midsection = Instantiate(Resources.Load<GameObject>("Models/Obstacles/Pipe/Midsection"));
        float width = rightBend.GetComponent<Renderer>().bounds.min.x - leftBend.GetComponent<Renderer>().bounds.max.x;
        width /= midsection.GetComponent<Renderer>().bounds.size.x;
        midsection.transform.localScale = new Vector3(width, scale, scale);
        midsection.transform.position = new Vector3(0, leftBend.GetComponent<Renderer>().bounds.max.y - midsection.GetComponent<Renderer>().bounds.size.y / 2, zPos);

        GameObject[] gameObjects = { leftBase, rightBase, leftRiser, rightRiser, leftBend, rightBend, midsection };
        foreach (GameObject gameObject in gameObjects) {
            gameObject.AddComponent<MeshCollider>().convex = true;
            gameObject.transform.parent = pipe.transform;
        }

        pipe.transform.parent = GetParent("Pipes");

        return pipe;

    }

    public static GameObject InitCones(float zPos) {

        GameObject cones = new GameObject("Cones_" + Road.pieceIndex);
        cones.transform.position = new Vector3(0, 0, zPos);

        int amount = 8;

        float totalWidth = Road.pieceSize * .75f;
        float distance = totalWidth / amount;
        float xPos = -totalWidth / 2 + distance / 2;

        float scale = totalWidth / amount * .75f;
        scale /= Resources.Load<GameObject>("Models/Obstacles/Cone").transform.Find("Cone").GetComponent<Renderer>().bounds.size.y;

        for (int i = 0; i < amount; i++) {

            GameObject cone = Instantiate(Resources.Load<GameObject>("Models/Obstacles/Cone"));
            cone.transform.position = new Vector3(xPos, 0, zPos);
            cone.transform.localScale = Vector3.one * scale;
            cone.transform.parent = cones.transform;

            for(int j = 0; j < cone.transform.childCount; j++) {
                cone.transform.GetChild(j).gameObject.AddComponent<MeshCollider>().convex = true;
            }

            cone.name = "Cone";

            xPos += distance;

        }

        cones.transform.parent = GetParent("Cones");

        return cones;

    }

    public static GameObject InitBarricade(float zPos) {
        string name = (Random.Range(0f, 1f) >= .5f ? "wood" : "stone");

        GameObject barricade = Instantiate(Resources.Load<GameObject>("Models/Obstacles/Barricades/" + name));

        Bounds bounds = (name.Equals("wood") ? barricade.transform.Find("Stands").GetComponent<Renderer>().bounds : barricade.GetComponent<Renderer>().bounds);

        float maxX = Road.pieceSize * .75f / 2 - bounds.size.x / 2;
        float xPos = Random.Range(-maxX, maxX);
        float scale = Player.playerSize.y * .65f / bounds.size.y;
        barricade.transform.localScale = Vector3.one * scale;
        barricade.transform.position = new Vector3(xPos, 0, zPos);

        if (barricade.transform.childCount > 0) {
            for(int i = 0; i < barricade.transform.childCount; i++) {
                barricade.transform.GetChild(i).gameObject.AddComponent<MeshCollider>().convex = true;
            } 
        } else {
            barricade.AddComponent<MeshCollider>().convex = true;
        }

        barricade.name = "Barricade_" + Road.pieceIndex;

        barricade.transform.parent = GetParent("Barricades");

        return barricade;
    }

    public static GameObject InitFence(float zPos) {

        GameObject fenceParent = new GameObject("Fence_" + Road.pieceIndex);

        GameObject leftSidePost = Instantiate(Resources.Load<GameObject>("Models/Obstacles/Fence/Post"));
        float scale = Road.pieceSize / 45 / leftSidePost.GetComponent<Renderer>().bounds.size.x;
        leftSidePost.transform.localScale = new Vector3(scale, 1, scale);
        leftSidePost.transform.position = new Vector3(-Road.pieceSize / 2 * .75f, leftSidePost.GetComponent<Renderer>().bounds.size.y / 2, zPos);

        GameObject rightSidePost = Instantiate(leftSidePost);
        rightSidePost.transform.position = new Vector3(-leftSidePost.transform.position.x, leftSidePost.transform.position.y, zPos);

        GameObject fence = Instantiate(Resources.Load<GameObject>("Models/Obstacles/Fence/Fence"));
        scale = rightSidePost.GetComponent<Renderer>().bounds.min.x - leftSidePost.GetComponent<Renderer>().bounds.max.x;
        scale /= fence.GetComponent<Renderer>().bounds.size.x;
        fence.transform.localScale = Vector3.one * scale;
        float yPos = Player.playerSize.y * .75f + fence.GetComponent<Renderer>().bounds.size.y / 2;
        fence.transform.position = new Vector3(0, yPos, zPos);

        float heightScale = (fence.GetComponent<Renderer>().bounds.max.y + leftSidePost.GetComponent<Renderer>().bounds.size.x) / leftSidePost.GetComponent<Renderer>().bounds.size.y;
        leftSidePost.transform.localScale = rightSidePost.transform.localScale = new Vector3(scale, heightScale, scale);
        scale = leftSidePost.transform.localScale.x;
        yPos = leftSidePost.GetComponent<Renderer>().bounds.size.y / 2;
        leftSidePost.transform.position = new Vector3(leftSidePost.transform.position.x, yPos, zPos);
        rightSidePost.transform.position = new Vector3(rightSidePost.transform.position.x, yPos, zPos);

        GameObject topPost = Instantiate(Resources.Load<GameObject>("Models/Obstacles/Fence/Post"));
        scale = leftSidePost.GetComponent<Renderer>().bounds.size.x * .625f
            / topPost.GetComponent<Renderer>().bounds.size.x;
        heightScale = (rightSidePost.GetComponent<Renderer>().bounds.min.x - leftSidePost.GetComponent<Renderer>().bounds.max.x)
            / topPost.GetComponent<Renderer>().bounds.size.y;
        topPost.transform.localScale = new Vector3(scale, heightScale, scale);
        topPost.transform.Rotate(Vector3.forward, 90);
        yPos = fence.GetComponent<Renderer>().bounds.max.y + topPost.GetComponent<Renderer>().bounds.size.y / 2;
        topPost.transform.position = new Vector3(0, yPos, zPos);

        GameObject bottomPost = Instantiate(topPost);
        yPos = fence.GetComponent<Renderer>().bounds.min.y - bottomPost.GetComponent<Renderer>().bounds.size.y / 2;
        bottomPost.transform.position = new Vector3(0, yPos, zPos);

        GameObject[] gameObjects = { leftSidePost, rightSidePost, fence, topPost, bottomPost };
        foreach (GameObject gameObject in gameObjects) {
            gameObject.transform.parent = fenceParent.transform;
            if (gameObject.Equals(fence)) {
                gameObject.AddComponent<BoxCollider>();
            } else {
                gameObject.AddComponent<CapsuleCollider>();
            }
        }

        fenceParent.transform.parent = GetParent("Fences");

        return fenceParent;

    }

    public static GameObject InitTrafficLights(float zPos) {

        GameObject trafficLights = new GameObject("TrafficLights_" + Road.pieceIndex);

        GameObject leftLight = Instantiate(Resources.Load<GameObject>("Models/Obstacles/TrafficLights"));
        leftLight.transform.position = new Vector3(-Road.pieceSize * .7f / 2, 0, zPos);
        leftLight.transform.Rotate(Vector3.up, 180);

        GameObject pole = leftLight.transform.Find("Lights_Pole").gameObject;
        float scale = Road.pieceSize / 90 / pole.GetComponent<Renderer>().bounds.size.x;
        float heightScale = Player.playerSize.y * 1.25f / pole.GetComponent<Renderer>().bounds.size.y;

        pole.transform.localScale = new Vector3(scale, heightScale, scale);
        pole.transform.position = new Vector3(leftLight.transform.position.x, pole.GetComponent<Renderer>().bounds.size.y, zPos);

        GameObject lightsFrame = leftLight.transform.Find("Lights_Frame").gameObject;
        leftLight.transform.Find("Lights_Red").parent = leftLight.transform.Find("Lights_Green").parent =
            lightsFrame.transform;

        scale = Road.pieceSize / 12.5f / lightsFrame.GetComponent<Renderer>().bounds.size.x;
        lightsFrame.transform.localScale = Vector3.one * scale;
        float yPos = pole.GetComponent<Renderer>().bounds.max.y + lightsFrame.GetComponent<Renderer>().bounds.size.y / 2;
        lightsFrame.transform.position = new Vector3(leftLight.transform.position.x, yPos, zPos);

        GameObject rightLight = Instantiate(leftLight);
        rightLight.transform.position = new Vector3(-leftLight.transform.position.x, 0, zPos);

        GameObject[] gameObjects = { leftLight, rightLight };
        foreach(GameObject gameObject in gameObjects) {
            gameObject.transform.parent = trafficLights.transform;
        }

        trafficLights.transform.parent = GetParent("Traffic Lights");

        return trafficLights;

    }

    private static Transform GetParent(string name) {
        if(GameObject.Find("Road").transform.Find("Obstacles").Find(name) != null) {
            return GameObject.Find("Road").transform.Find("Obstacles").Find(name);
        } else {
            GameObject parent = new GameObject(name);
            parent.transform.parent = GameObject.Find("Road").transform.Find("Obstacles");
            return parent.transform;
        }
    }

}
