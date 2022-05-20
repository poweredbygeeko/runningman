using System.Collections;
using System.Collections.Generic;

using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

using TMPro;

public class Controller : MonoBehaviour {

    public static GameObject HUD;
    public static GameObject gameOverHUD;

    private static Vector2 startSwipePos;

    public static int score = 0;
    private static int hiScore = 0;

    private static float startSwipeTime;
    private static float maxSwipeTime = .25f;
    private static float boostTime = 0;
    private static float maxBoostTime = 2.5f;

    public static bool isGameOver = false;
    public static bool isPaused = false;
    public static bool isCollisionsActive = true;

    public void Awake() {

        print("Started");

        if(PlayerPrefs.HasKey("HiScore")) {
            hiScore = PlayerPrefs.GetInt("HiScore");
        }

        HUD = Instantiate(Resources.Load<GameObject>("UI/UI_InGame"));
        HUD.transform.Find("HiScore").GetComponent<TextMeshProUGUI>().text = "HiScore: " + hiScore;
        gameOverHUD = Instantiate(Resources.Load<GameObject>("UI/UI_GameOver"));
        gameOverHUD.transform.Find("HiScore").GetComponent<TextMeshProUGUI>().text = "HiScore: " + hiScore;
        gameOverHUD.SetActive(false);

        HUD.transform.Find("Pause").GetComponent<Button>().onClick.AddListener(Pause);
        gameOverHUD.transform.Find("Retry").GetComponent<Button>().onClick.AddListener(Retry);
        gameOverHUD.transform.Find("Exit").GetComponent<Button>().onClick.AddListener(Exit);

        Road.pieceSize = Controller.GetScreenSize().x * 3;
        SetPlayerSize();


        Controller.score = 0;

        Road.zPos = 0;
        Road.pieceIndex = 0;

        Road.pieces.Clear();

        Obstacle.obstacles.Clear();

        Controller.isGameOver = false;

    }

    public void Update() {
        UpdateScore();
        setCameraPos();
        CheckBoostTime();
    }

    public static (bool, string) GetSwipe() {

        bool isSwipe = false;
        string swipeType = "";

        if (Input.GetMouseButtonDown(0)) {
            startSwipePos = Input.mousePosition;
            startSwipeTime = Time.time;
        }

        float swipeTime = Time.time - startSwipeTime;

        if (swipeTime < maxSwipeTime) {

            if (Input.GetMouseButtonUp(0) && !GameObject.Find("EventSystem").GetComponent<EventSystem>().IsPointerOverGameObject()) {
                Vector2 endSwipePos = Input.mousePosition;
                Vector2 displacement = endSwipePos - startSwipePos;
                float distance = Mathf.Sqrt(Mathf.Pow(displacement.x, 2) + Mathf.Pow(displacement.y, 2));
                isSwipe = distance > 30;
                if (isSwipe) {
                    swipeType = (displacement.y >= 0 ? "SWIPE_UP" : "SWIPE_DOWN");
                } else {
                    swipeType = "TAP";
                }
            }

        } else if (Input.GetMouseButton(0)) {
            return (false, "HOLD");
        }

        return (isSwipe, swipeType);

    }

    public static Vector2 GetScreenSize() {
        return Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, -Camera.main.transform.position.z)) * 2;
    }

    public static void setCameraPos() {
        Camera.main.transform.position = GameObject.Find("Player").transform.position + new Vector3(0, Player.playerSize.y * 2.5f, -Player.playerSize.z * 7.5f);
        Camera.main.transform.LookAt(GameObject.Find("Player").transform.position + new Vector3(0, 0, Player.playerSize.z * 12.5f));
    }

    private static void SetPlayerSize() {

        GameObject player = GameObject.Find("Player");
        Vector3 playerSize;

        playerSize.x = player.transform.Find("Character").GetComponent<Renderer>().bounds.max.x
            - player.transform.Find("Top").GetComponent<Renderer>().bounds.min.x;
        playerSize.y = player.transform.Find("Hair").GetComponent<Renderer>().bounds.max.y
            - player.transform.Find("Shoes").GetComponent<Renderer>().bounds.min.y;
        playerSize.z = player.transform.Find("Shoes").GetComponent<Renderer>().bounds.size.z;


        float scale = Road.pieceSize / 7.5f / playerSize.x;
        player.transform.localScale = Vector3.one * scale;

        Player.playerSize = playerSize * scale;

    }

    private static void CheckBoostTime() {
        if(Time.timeScale > 1) {
            boostTime += Time.deltaTime / Time.timeScale / Time.timeScale;
            if(boostTime >= maxBoostTime) {
                Time.timeScale = 1;
                boostTime = 0;
                isCollisionsActive = true;
            } else if(boostTime >= maxBoostTime - 1) {
                Time.timeScale -= (Time.timeScale - 1) * Time.deltaTime / Time.timeScale / Time.timeScale;
            }
        }
    }

    private static void UpdateScore() {
        if (Player.isRunning) {
            score += Mathf.RoundToInt(Time.deltaTime * 100);
            HUD.transform.Find("Score").GetComponent<TMPro.TextMeshProUGUI>().SetText(string.Format("{0:n0}", score));
        }
    }

    public static void GameOver() {

        if (!isGameOver) {

            GameObject.Find("Player").GetComponent<Rigidbody>().velocity = Vector3.zero;
            GameObject.Find("Player").GetComponent<Animator>().SetBool("isRunning", false);
            Player.isRunning = false;

            if (score > hiScore) {

                gameOverHUD.transform.Find("ScoreTitle").GetComponent<TextMeshProUGUI>().text = "NEW HIGH SCORE!";
                gameOverHUD.transform.Find("ScoreTitle").GetComponent<TextMeshProUGUI>().color = new Color(.7f, .15f, .15f);
                gameOverHUD.transform.Find("HiScore").GetComponent<TextMeshProUGUI>().text = "HiScore: " + hiScore;
                gameOverHUD.transform.Find("HiScore").gameObject.SetActive(false);

                PlayerPrefs.SetInt("HiScore", hiScore);
                hiScore = score;

                HUD.transform.Find("HiScore").GetComponent<TextMeshProUGUI>().text = "HiScore: " + hiScore;


            } else {

                if (!Controller.gameOverHUD.transform.Find("ScoreTitle").GetComponent<TextMeshProUGUI>().color.Equals(new Color(1, 1, 1))) {
                    Controller.gameOverHUD.transform.Find("ScoreTitle").GetComponent<TextMeshProUGUI>().text = "YOU SCORED";
                    Controller.gameOverHUD.transform.Find("ScoreTitle").GetComponent<TextMeshProUGUI>().color = new Color(1, 1, 1);
                    gameOverHUD.transform.Find("HiScore").gameObject.SetActive(true);
                }

            }

            HUD.transform.Find("Score").GetComponent<TextMeshProUGUI>().text = "0";
            HUD.SetActive(false);
            gameOverHUD.SetActive(true);
            gameOverHUD.transform.Find("Score").GetComponent<TextMeshProUGUI>().text = string.Format("{0:n0}", score);
            gameOverHUD.SetActive(true);


            isGameOver = true;


        }

    }

    public void Pause() {
        Time.timeScale = Controller.isPaused ? 1 : 0;
        Controller.isPaused = !Controller.isPaused;
    }

    public static void Retry() {

        GameObject player = GameObject.Find("Player");
        player.transform.position = Vector3.zero;
        player.transform.rotation = Quaternion.Euler(Vector3.zero);

        Transform[] parents = {
            GameObject.Find("Road").transform.Find("Pieces"),
            GameObject.Find("Road").transform.Find("Obstacles"),
            GameObject.Find("Road").transform.Find("Consumables")
        };

        foreach (Transform parent in parents) {
            for (int i = 0; i < parent.childCount; i++) {
                if (parent.GetChild(i).childCount > 0) {
                    for (int j = 0; j < parent.GetChild(i).childCount; j++) {
                        Destroy(parent.GetChild(i).GetChild(j).gameObject);
                    }
                } else {
                    Destroy(parent.GetChild(i).gameObject);
                }
            }
        }

        Road.zPos = 0;
        Road.pieceIndex = 0;

        Obstacle.obstacles.Clear();
        Road.pieces.Clear();


        for (int i = 0; i < Road.drawDistance * 2; i++) {
            Road.InitPiece();
        }

        Controller.score = 0;

        Controller.gameOverHUD.SetActive(false);
        Controller.HUD.SetActive(true);
        Controller.isGameOver = false;

    }

    private void Exit() {
        Controller.isGameOver = false;
        SceneManager.LoadScene("Menu");
    }

}