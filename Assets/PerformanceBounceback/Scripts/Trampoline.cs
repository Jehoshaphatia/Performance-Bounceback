using UnityEngine;

public class Trampoline : MonoBehaviour {

    public ParticleSystem pSystem;

	void Start () {
		pSystem = GetComponentInChildren<ParticleSystem> ();
	}

    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.CompareTag("Throwable"))
        {
            //Score Point
			GameManager.score++;
            //Particle effect
            pSystem.Play();
        }
    }
}
