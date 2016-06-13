using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ShipScript : MonoBehaviour {

	static ShipScript _instance;
	public static ShipScript Instance {get { return _instance;}}

	public UnityEngine.Object bullet;
	public Transform Target;
	public Transform Ship;
	bool fadingOff;

	// Use this for initialization
	void Start () {
		_instance = this;
		fadingOff = true;
		Target.gameObject.SetActive (false);
		Color c = Ship.GetComponent<Image>().color;
		c.a = 1f;
		Ship.GetComponent<Image>().color = c;
	}

	// Update is called once per frame
	void Update () {
		if (Input.GetMouseButtonDown(0)) {
			setTarget(Input.mousePosition);
		}

		if (Input.GetMouseButtonDown (1)) {
			FireBullet(Input.mousePosition);
		}

		if (Input.GetKeyDown (KeyCode.R))
			Reset ();

		if (Input.GetKeyDown (KeyCode.Space))
			StartCoroutine(Fade());

		MoveMovingObjects ();
	}

	void Reset(){
		foreach (MovingObject item in MovingObject.MovingObjects) {
			if (item.target != Ship)
				Destroy(item.target.gameObject);
		}
		MovingObject.MovingObjects.RemoveRange (0,MovingObject.MovingObjects.Count);
		Ship.position = new Vector3 (Screen.width/2, Screen.height/2, 0);
		Ship.rotation = Quaternion.AngleAxis(90, Vector3.forward);
		Target.gameObject.SetActive (false);
	}

	void MoveMovingObjects(){
		try {
			foreach (MovingObject item in MovingObject.MovingObjects) {
				item.Move();
			}
		} catch (System.InvalidOperationException) { }
	}

	IEnumerator Fade(){
		float step = (fadingOff) ? -0.01f : 0.01f;
		float init = (fadingOff) ? 1f : 0f;

		for (float f = init; f >= 0f && f <=1f; f += step) {
			Color c = Ship.GetComponent<Image>().color;
			c.a = f;
			Ship.GetComponent<Image>().color = c;
			yield return new WaitForSeconds(.01f);
		}
		fadingOff = !fadingOff;
	}

	void setTarget(Vector3 pos) {
		Target.gameObject.SetActive (true);
		Target.position = pos;

		// -- Instant rotate
		//Vector3 moveDirection = Target.position - Ship.position;
		//float angle = Mathf.Atan2(moveDirection.y, moveDirection.x) * Mathf.Rad2Deg;
		//Ship.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

		new MovingObject (Ship.position, pos, Ship);
	}

	void FireBullet(Vector3 pos) {
		new MovingObject (Ship.position, pos);
	}

	public Transform InstantiateBullet() {
		Transform t = (Instantiate(bullet,Ship.position,Ship.rotation) as GameObject).transform;
		t.SetParent (this.transform);
		return t;
	}

	public void DestroyBullet(Transform bullet) {
		Destroy (bullet.gameObject);
	}
}
