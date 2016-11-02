using UnityEngine;
using System.Collections;

public class Food : MonoBehaviour {
	//销毁自身
	public void DestroyFood() {
		Destroy (this.gameObject);
	}
}
