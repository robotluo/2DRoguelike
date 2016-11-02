using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour {

	private Transform player;			//主角
	private Rigidbody2D myRigidbody; 	//rigidbody
	private Vector2 afterPosition;		//移动位置
	private BoxCollider2D myCollider; 	//collider
	private Animator myAnimator;		//控制器
	public float moveSpeed = 3.0f; 		//移动速度
	public int power = 10;				//怪物攻击力

	// Use this for initialization
	void Start () {
		player = GameObject.FindGameObjectWithTag ("Player").transform;
		myRigidbody = GetComponent<Rigidbody2D> ();
		afterPosition = transform.position;
		myCollider = GetComponent<BoxCollider2D> ();
		myAnimator = GetComponent<Animator> ();
		GameManager.Instance.setEnemy (this);
	}
	
	// Update is called once per frame
	void Update () {
		myRigidbody.MovePosition (Vector2.Lerp (transform.position,afterPosition , moveSpeed * Time.deltaTime));
	}

	public void Move (){
		Vector2 offset = player.position - transform.position;

		if (offset.magnitude < 1.5f) {
			//攻击
			myAnimator.SetTrigger("Attack");
			player.SendMessage ("Damager",power);

		} else {
			//移动
			int x = 0, y = 0;
			if (Mathf.Abs (offset.x) > Mathf.Abs (offset.y)) {
				if (offset.x > 0) {
					x = 1;
				} else {
					x = -1;
				}
			} else {
				if (offset.y > 0) {
					y = 1;
				} else {
					y = -1;
				}
			}
			myCollider.enabled = false;	//去掉当前碰撞器，防止检测到自身
			RaycastHit2D hit = Physics2D.Linecast (afterPosition, afterPosition + new Vector2 (x, y));
			myCollider.enabled = true; //重新启用碰撞器
			if (hit.transform == null) {
				afterPosition += new Vector2 (x, y);
			} else {
				if (hit.transform.tag == "Food" || hit.transform.tag == "Suda" ) {
					afterPosition += new Vector2 (x, y);
				}
			}
		}
	}
}
