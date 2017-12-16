using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Score : MonoBehaviour {

    public GameManager gameManager;
	public Text Text;
	// Use this for initialization

	void Start () {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
		Text text = GetComponentInChildren<Text>();

	}
	
	// Update is called once per frame
	void Update () {
		DisplayScore ();
	}

	void DisplayScore(){
		Text.text = "Score: " + gameManager.score.ToString();
	}
}
