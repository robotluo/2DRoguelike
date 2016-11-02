using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class MapManage : MonoBehaviour {

	public GameObject[] outWalls; 	//外墙
	public GameObject[] floors;		//地板
	public GameObject[] walls;	 	//障碍物
	public GameObject[] foods;		//食物
	public GameObject[] enemys;		//敌人
	public GameObject   exit;		//出口
	public GameObject player;		//主角

	public int wallNumber = 5; 		//障碍物数量
	public int foodNumber = 2; 		//食物数量
	public int enemyNumber = 2; 	//敌人数量


	private int rowMax = 9; //行
	private int colMax = 9; //列
	private Transform Map; 	//装载地板和围墙
	private List<Vector2> mapList = new List<Vector2>();

	/*
	// Use this for initialization
	void Awake () {	
		createMap();
	}*/

	public void createMap(){
		Map = new GameObject ("Map").transform;
		//创建外墙和地板
		for (int row = 0; row <= rowMax; row++) {
			for (int col = 0; col <= colMax; col++) {
				if (row == 0 || row == rowMax || col == 0 || col == colMax) {
					int outWallIndex = Random.Range (0, outWalls.Length);
					GameObject tmp = GameObject.Instantiate (outWalls [outWallIndex], new Vector3 (row, col, 0), Quaternion.identity) as GameObject;
					tmp.transform.SetParent (Map);
				} else {
					int floorIndex = Random.Range (0, floors.Length);
					GameObject tmp =GameObject.Instantiate (floors [floorIndex], new Vector3 (row, col, 0), Quaternion.identity) as GameObject;
					tmp.transform.SetParent (Map);
				}
			}
		}

		//将地图上能放置物品的坐标存入 mapList
		mapList.Clear ();
		for (int row = 2; row <= rowMax - 2; row++) {
			for(int col =2 ;col <= colMax - 2; col++) {
				mapList.Add (new Vector2 (row, col));
			}
		}
			
		//创建障碍物
		creatElement(wallNumber,walls);
		//创建食物
		creatElement(foodNumber,foods);
		//创建敌人
		creatElement(enemyNumber,enemys);

		//创建出口
		GameObject.Instantiate(exit,new Vector2(rowMax-1 , colMax-1),Quaternion.identity);
		//创建主角
		GameObject.Instantiate(player,new Vector2( 1, 1 ),Quaternion.identity);
	}

	//创建素材
	void creatElement(int count, GameObject[] array){
		for (int i = 0; i < count; i++) {
			int listIndex = Random.Range (0, mapList.Count ); //随机取得放置点
			int arrIndex = Random.Range (0, array.Length);  //随机取得对象

			Vector2 pos = mapList [listIndex]; //取出放置点
			mapList.Remove(pos);			// list 中去掉该位置
			GameObject tmp = GameObject.Instantiate (array[arrIndex],pos,Quaternion.identity) as GameObject;
			tmp.transform.SetParent (Map);
		}
	}
}
