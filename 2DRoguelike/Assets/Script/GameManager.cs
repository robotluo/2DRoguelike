using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

	public static GameManager _Instance;
	public static GameManager Instance{
		get {
			return _Instance;
		}
	}
		
	public int level = 1;
	public int Hp = 100;
	public bool isExit = false;
	public List<Enemy> enemyList = new List<Enemy>();

	private int playMoveNumber = 0;
	private Text foodText;  // 食物 UI
	private Text prompt; 	// GameOver 
	private Image dayImage;	// 天数背景
	private Text dayText;	// 天数UI
	private Player player;
	private int rowMax = 9; // 行
	private int colMax = 9; // 列
	private MapManage mapManage;

	void Awake(){
		_Instance = this;
		DontDestroyOnLoad (this.gameObject);
		initGame ();
	}

	//统一创建
	public void initGame(){

		//先创建地图
		mapManage =GameObject.Find("MapManage").GetComponent<MapManage> ();
		mapManage.createMap ();

		//创建 UI
		foodText = GameObject.Find ("FoodText").GetComponent<Text> ();
		prompt = GameObject.Find ("Prompt").GetComponent<Text> ();
		prompt.enabled = false;
		foodText.text = "Food : " + Hp;
		player = GameObject.FindGameObjectWithTag ("Player").GetComponent<Player>();
		dayImage = GameObject.Find ("dayImage").GetComponent<Image> ();
		dayText = GameObject.Find ("dayText").GetComponent<Text> ();
		dayText.text = "Day: " + level; 
		Invoke ("dayShow",1);

		//初始化信息
		isExit = false;
		enemyList.Clear ();
	}

	//需要保存的信息
	void OnLevelWasLoaded(int scenesLebel){
		level++;
		initGame ();
	}

	public void showHp(){
		foodText.text = "Food : " + Hp;
	}

	//改变 Hp
	public void setHp(int hp){
		Hp += hp;
		if (Hp <= 0) {
			prompt.enabled = true;
		}
	}

	//添加敌人
	public void setEnemy(Enemy other){
		enemyList.Add (other);
	}

	//玩家移动
	public void playMove(){
		playMoveNumber++;
		if (playMoveNumber % 2 == 0) {
			foreach (var t in enemyList) {
				t.Move();
			}
		}
		if (player.movePosition.x == rowMax - 1 && player.movePosition.y == colMax - 1) {
			isExit = true;
			Debug.Log ("dasdsa");
			//加载下一关
			SceneManager.LoadScene(0);
		}
	}

	void dayShow(){
		dayImage.gameObject.SetActive (false);
	}
}
