using UnityEngine;
using System.Collections;

public class Wall : MonoBehaviour {

	public int hp =3;
	public Sprite damagerSprite;

	public void Damager(){
		hp--;
		GetComponent<SpriteRenderer> ().sprite = damagerSprite;
		if (hp <= 0) {
			Destroy (this.gameObject);
		}
	}
}
