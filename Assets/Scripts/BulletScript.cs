using UnityEngine;
using System.Collections;

public class BulletScript : MonoBehaviour {

	public void Update () {
		if (System.Math.Abs (transform.localPosition.x) > transform.parent.GetComponent<RectTransform> ().sizeDelta.x / 2 ||
			System.Math.Abs (transform.localPosition.y) > transform.parent.GetComponent<RectTransform> ().sizeDelta.y / 2)
			Destroy (gameObject);
	}

}
