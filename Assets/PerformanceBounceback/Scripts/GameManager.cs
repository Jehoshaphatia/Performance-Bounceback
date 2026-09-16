using UnityEngine;

public class GameManager : MonoBehaviour {

	public static int score;

	// score is static, so it survives scene loads unless reset explicitly.
	void Awake () {
		score = 0;
	}
}
