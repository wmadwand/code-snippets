using UnityEngine;
using System.Collections;

public class DragBird : MonoBehaviour {
	
	public float stretchLimit = 1.0f;
	public LineRenderer stringBack;
	public LineRenderer stringFront;
	
	private SpringJoint2D spring;
	private bool clicked;
	private Transform slingshot;
	private Ray mouseRay;
	private Ray leftRay;
	private float stretchSquare;
	private float radius;
	private Vector2 velocityX;
	
	void Awake () {
		spring = GetComponent<SpringJoint2D> ();
		slingshot = spring.connectedBody.transform;
		
	}
	
	void Start () {
		StringSetup ();
		mouseRay = new Ray(slingshot.position, Vector3.zero);
		leftRay = new Ray(stringFront.transform.position, Vector3.zero);
		stretchSquare = stretchLimit * stretchLimit;
		CircleCollider2D circleColl = GetComponent<Collider2D>() as CircleCollider2D;
		radius = circleColl.radius;
		
	}
	
	void Update () {
		if (clicked)
			Dragging ();
		
		if (spring != null) {
			if (!GetComponent<Rigidbody2D>().isKinematic && velocityX.sqrMagnitude > GetComponent<Rigidbody2D>().velocity.sqrMagnitude) {
				Destroy (spring);
                GetComponent<Rigidbody2D>().velocity = velocityX;
			}
			
			if (!clicked)
				velocityX = GetComponent<Rigidbody2D>().velocity;
			
			StringUpdate ();
			
		}
		else {
			stringBack.enabled = false;
			stringFront.enabled = false;
		}
	}
	
	void StringSetup () {
		stringBack.SetPosition (0, stringBack.transform.position);
		stringFront.SetPosition (0, stringFront.transform.position);
		stringBack.sortingOrder = 1;
		stringFront.sortingOrder = 5;
		stringBack.sortingLayerName = "Foreground";
		stringFront.sortingLayerName = "Foreground";
	}
	
	void OnMouseDown () {
		spring.enabled = false;
		clicked = true;
		
	}
	
	void OnMouseUp () {
		spring.enabled = true;
		GetComponent<Rigidbody2D>().isKinematic = false;
		clicked = false;
	}
	
	void Dragging () {
		Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		Vector2 fromSlingshot = mousePos - slingshot.position;
		
		if (fromSlingshot.sqrMagnitude > stretchSquare) {
			mouseRay.direction = fromSlingshot;
			mousePos = mouseRay.GetPoint(stretchLimit);
		}
		
		mousePos.z = 0f;
		transform.position = mousePos;
	}
	
	void StringUpdate () {
		Vector2 projectile = transform.position - stringFront.transform.position;
		leftRay.direction = projectile;
		Vector3 hold = leftRay.GetPoint(projectile.magnitude + radius);
		stringBack.SetPosition(1, hold);
		stringFront.SetPosition(1, hold);
	}
}
