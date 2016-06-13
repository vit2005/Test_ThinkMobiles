using UnityEngine.UI;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;

public class MovingObject {

	public static List<MovingObject> MovingObjects = new List<MovingObject>();

	int frames;
	int rotateFrames;
	float angle;
	float modulo;

	Vector3 d; // delta
	Vector3 tp; // Target position
	bool shouldDestroyTransform;

	public Transform target;

	public void Move(){
		if (target == null) {
			MovingObjects.Remove (this);
			return;
		}

		if (rotateFrames > 0) {
			Rotate();
			return;
		}

		target.localPosition += d;
		frames--;

		if (shouldDestroyTransform) {
			if (frames == 0)
				ShipScript.Instance.DestroyBullet (target);
			else
				return;
		}

		if ((tp.x - target.position.x)*d.x < 0 || (tp.y - target.position.y)*d.y < 0 ) {
			MovingObjects.Remove (this);
		}

	}

	void Rotate()
	{
		if (rotateFrames == 1 && modulo != 0){
			target.rotation *= Quaternion.AngleAxis(modulo, Vector3.forward); //target.eulerAngles = new Vector3(0, 0, modulo); //target.RotateAround (target.position, Vector3.up, modulo); //target.Rotate (Quaternion.AngleAxis(modulo, Vector3.forward));
		}
		else{
			if (modulo < 0)
				target.rotation *= Quaternion.AngleAxis(angle, Vector3.back);
			else
				target.rotation *= Quaternion.AngleAxis(angle, Vector3.forward); //target.eulerAngles = new Vector3(0, 0, angle);  //target.RotateAround (target.position, Vector3.up, angle);
		}
		rotateFrames--;
	}

	float TakeAngle(Vector3 from, Vector3 to)
	{
		return Mathf.Acos(Mathf.Clamp(Vector3.Dot(from.normalized, to.normalized), -1f, 1f)) * 57.29578f;
	}


	public MovingObject(Vector3 pos1, Vector3 pos2, Transform target = null, int rotatespeed = 10, int speed = 10 ){

		MovingObject sameTarget = MovingObjects.FirstOrDefault (mo => mo.target == target);
		if (sameTarget != null)
			MovingObjects.Remove (sameTarget);

		frames = 10000;
		d = pos2 - pos1;
		tp = pos2;

		float x = (d.x / (Mathf.Abs(d.x) + Mathf.Abs(d.y))) * speed;
		float y = (d.y / (Mathf.Abs(d.x) + Mathf.Abs(d.y))) * speed;
		d = new Vector3(x,y,0);

		shouldDestroyTransform = target == null;
		if (shouldDestroyTransform) {
			this.target = ShipScript.Instance.InstantiateBullet();
			rotateFrames = 0;
		} else {
			try{
				InitShip (pos1, pos2, target, rotatespeed, speed);
			} catch (Exception e) {
				Debug.LogError(e);
				return;
			}
		}

		MovingObjects.Add (this);
	}

	void InitShip(Vector3 pos1, Vector3 pos2, Transform target = null, int rotatespeed = 10, int speed = 10){
		//float tempAngle = Mathf.Atan2(d.y, d.x) * Mathf.Rad2Deg / rotatespeed;
		//float tempAngle = Vector3.Angle( d, target.forward );
		//float tempAngle = Quaternion.LookRotation(pos2 - pos1);
		//float tempAngle = Quaternion.Angle(target.rotation, Quaternion.AngleAxis(tempAngle, Vector3.forward));
		//float tempAngle = Vector2.Angle(new Vector2(pos1.x, pos1.y), new Vector2(pos2.x, pos2.y));
		
		//Debug.Log("atan()="+Mathf.Atan((pos1.y - pos2.y) / (pos1.x - pos2.x)) * Mathf.Rad2Deg);
		//Debug.Log("atan2()="+Mathf.Atan2(pos1.y - pos2.y, pos1.x - pos2.x) * Mathf.Rad2Deg);
		float ad; // Angle difference
		float atan1 = Mathf.Atan((pos1.y - pos2.y) / (pos1.x - pos2.x)) * Mathf.Rad2Deg;
		float atan2 = Mathf.Atan2(pos1.y - pos2.y, pos1.x - pos2.x) * Mathf.Rad2Deg;
		
		if (Mathf.Abs(atan1 - atan2) < 0.01f)
			ad = atan1 - target.eulerAngles.z + 180;
		else 
			ad = atan1 - target.eulerAngles.z;
		
		if (ad < -180)
			ad += 360;
		if (ad > 180)
			ad -= 360;
		
		//Debug.Log("rotation = "+target.eulerAngles.z);
		//Debug.Log("tempAngle = "+tempAngle.ToString());
		
		rotateFrames = Mathf.Abs(Convert.ToInt32(ad / rotatespeed));
		//Debug.Log("rotateFrames = "+rotateFrames.ToString());
		
		angle = rotatespeed;
		modulo = ad % rotatespeed;
		this.target = target;
	}
}
