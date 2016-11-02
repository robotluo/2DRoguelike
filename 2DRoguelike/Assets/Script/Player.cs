using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {


	private Rigidbody2D myRigidbody; 	//rigidbody
	private float resetMoveTime = 0.3f; //移动间隔时间
	private BoxCollider2D myCollider; 	//colloder
	private Animator myAnimator; 		//状态机

	public float moveTime ;  			//距离上一次移动时间
	public Vector2 movePosition; 		//移动方向
	public float moveSpeed = 5;

	// Use this for initialization
	void Start () {
		myRigidbody = GetComponent<Rigidbody2D> ();
		movePosition = transform.position;
		moveTime = resetMoveTime;
		myCollider = GetComponent<BoxCollider2D> ();
		myAnimator = GetComponent<Animator> ();
	}
	 
	// Update is called once per frame
	void Update () {


		myRigidbody.MovePosition (Vector2.Lerp (transform.position, movePosition, moveSpeed * Time.deltaTime));

		if (GameManager.Instance.Hp <= 0 || GameManager.Instance.isExit) {
			return;
		}
		moveTime -= Time.deltaTime;
		//Debug.Log (moveTime);
		if (moveTime > 1e-6) {
			return;
		}

		float hor = Input.GetAxisRaw ("Horizontal");
		float ver = Input.GetAxisRaw ("Vertical");

		if (hor != 0) { //只让其在一个方向移动
			ver = 0;
		}
	
		if (ver != 0 || hor != 0) {
			moveTime = resetMoveTime;
			GameManager.Instance.setHp (-2); //每走一步减少的食物
			myCollider.enabled = false;	//去掉当前碰撞器，防止检测到自身
			RaycastHit2D hit = Physics2D.Linecast (movePosition, movePosition + new Vector2 (hor, ver));
			myCollider.enabled = true; //重新启用碰撞器

			if (hit.transform == null) {//没有碰撞到物体
				movePosition += new Vector2 (hor, ver);
			} else {
				switch (hit.collider.tag) {
				case "Wall":
					myAnimator.SetTrigger ("Attack");
					hit.collider.SendMessage ("Damager");
					break;
				case "OutWall":
					break;
				case "Food":
					GameManager.Instance.setHp (10);
					movePosition += new Vector2 (hor, ver);
					hit.collider.SendMessage ("DestroyFood");  //物体的销毁最好让其自己处理
					break;
				case "Suda":
					GameManager.Instance.setHp (20);
					movePosition += new Vector2 (hor, ver);
					hit.collider.SendMessage ("DestroyFood");
					break;
				}
			}
			GameManager.Instance.playMove ();
			GameManager.Instance.showHp ();
		}
	}

	public void Damager(int power){
		myAnimator.SetTrigger ("Damage");
		GameManager.Instance.setHp (-power);
		GameManager.Instance.showHp ();
	}
}
