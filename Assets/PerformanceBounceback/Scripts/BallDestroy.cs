using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[RequireComponent(typeof(XRGrabInteractable))]
public class BallDestroy : MonoBehaviour {

	[SerializeField]
	private float lifetime = 5f;

	private XRGrabInteractable grab;

	void Awake () {
		grab = GetComponent<XRGrabInteractable> ();
	}

	void OnEnable () {
		grab.selectEntered.AddListener (OnGrabbed);
		grab.selectExited.AddListener (OnReleased);
		ScheduleDespawn ();
	}

	void OnDisable () {
		grab.selectEntered.RemoveListener (OnGrabbed);
		grab.selectExited.RemoveListener (OnReleased);
		CancelInvoke ();
	}

	// A held ball must never be recycled out from under the interactor.
	void OnGrabbed (SelectEnterEventArgs args) {
		CancelInvoke (nameof (Despawn));
	}

	void OnReleased (SelectExitEventArgs args) {
		// A canceled exit means we are already being disabled; OnDisable cleans up.
		if (args.isCanceled)
			return;

		ScheduleDespawn ();
	}

	void ScheduleDespawn () {
		CancelInvoke (nameof (Despawn));
		Invoke (nameof (Despawn), lifetime);
	}

	void Despawn () {
		gameObject.SetActive (false);
	}
}
