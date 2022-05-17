using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour {

    private void Awake() {
        GameObject.Find("NewGame").GetComponent<Button>().onClick.AddListener(NewGame);   
    }

    private void NewGame() {
        SceneManager.LoadScene("Level");
    }

}
