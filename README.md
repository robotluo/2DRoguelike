# 2DRoguelike
同样的，实现过程只记大致思路与新知识点，不记细节。  
导入路径 Assets/scence/BattlePlane.unity 
简书: http://www.jianshu.com/p/f2eecdab720e 
大致实现功能：  
　1.创建主角和敌人，创建地图  
　2.随机生成敌人和食物  
　3.获取食物、碰撞检测  
　4.与敌人碰撞时状态机的变换  
　5.血量控制、关卡转换  
　6.添加音效  

　　2DRoguelike 是官方的案例之一，与之前所用的有所区别在于使用了状态机来控制人物动画。  
　　状态机保存了许多不同的状态，事件触发可以改变状态机的状态，实现效果的转换。  

####创建地图：  
　　将素材装入一个数组，随机一个索引并创建实例。  
```
// outWalls 为外墙数组  
// Map 为一个空 GameOnject 的 Transform，用于装载地图，比较整洁   
int outWallIndex = Random.Range (0, outWalls.Length);  
GameObject tmp = GameObject.Instantiate (outWalls [outWallIndex], new Vector3 (row, col, 0), Quaternion.identity) as GameObject;  
tmp.transform.SetParent (Map);  
```

　　当我们有许多实例需要随机生成时，例如，敌人、食物、障碍物等都需要随机生成在地图中，因此可以写一个专门的方法。
先将地图上能存实例的坐标存入 mapList：
```
private List<Vector2> mapList = new List<Vector2>();
//将地图上能放置物品的坐标存入 mapList
		mapList.Clear ();
		for (int row = 2; row <= rowMax - 2; row++) {
			for(int col =2 ;col <= colMax - 2; col++) {
				mapList.Add (new Vector2 (row, col));
			}
		}
```
专门用于创建的方法：
```
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
```
调用时只需要传入数量和数组即可。  


####状态机的创建：  
　　将素材多选直接拖入Hierarchy 中，会自动生成两个文件。一个是动画，这里命名为 Animation ，另一个是控制器，这里命名为AnimatorController。状态机的逻辑非常简单，将每个状态的具体实现封装起来，当满足某个条件，将状态从当前状态转换到另一状态，状态内部与状态转换之间不互相影响。
　　如果一个主角有多个状态，例如有静止、移动、攻击、受伤等等，只需要将其全部拖到同一个物体上，如果直接拖到 Hierarchy 会产生多个状态机。  
**共用状态机：**
　　状态机分为两部分，动画和控制。在实际中我们可能需要多个角色共用同一状态机。例如，两种不同的怪物，他们的动画不同，但是处理状态的逻辑是完全一样的，这时候如果分别使用两个状态机就有点多余，所以需要共用。
　　我们可以这么做，如果我们有了一个 Controller_1 想要重用,首先在同一文件夹下右键 -> create -> Animator Override Controller，命名为 Controller_2 ，点击 Controller_2 会看到 Inspector 视图下有一个 Controller 需要赋值，将 Controller_1 赋值到此处，再将新的动画进行对应赋值。这样，Controller_1 与 Controller_2 就共用一套逻辑。
**状态切换：**
　　双击一个控制器，选中一个状态，右键 Make Transition，会出现一个箭头，此时连接到需要转换的状态。
例如：
![状态机.png](http://upload-images.jianshu.io/upload_images/3254839-93017a9f360c137c.png?imageMogr2/auto-orient/strip%7CimageView2/2/w/1240)
连线代表能够转换，现在需要自定义转换的条件。如下图，添加两个 Trigger
![Trigger.png](http://upload-images.jianshu.io/upload_images/3254839-683a287591642e8d.png?imageMogr2/auto-orient/strip%7CimageView2/2/w/1240)

选中连线，再在 Inspector 视图下 Conditions 添加 所需Trigger，改变Has Exit Time 勾选，Has Exit Time表示播放完变换，当我们攻击时应该是立刻做出反应，所以不需要勾选。而从攻击状态变为普通状态只需要播放完即可，不需要Trigger。

###主角移动：
　　添加 Rigidbody ，用 MovePosition 方法移动
```
myRigidbody = GetComponent<Rigidbody2D> ();
myRigidbody.MovePosition (Vector2.Lerp(transform.position, movePosition, moveSpeed * Time.deltaTime));
//（当前位置，目标位置，移动速度）
```
**碰撞检测：**
　　这里采用射线的方式进行碰撞检测，向目标位置发射射线，判断所碰撞物体种类再进行相关处理。
```
myCollider.enabled = false;	//去掉当前碰撞器，防止检测到自身
RaycastHit2D hit = Physics2D.Linecast (movePosition,movePosition + new Vector2(hor,ver));
myCollider.enabled = true; //重新启用碰撞器
```
对 hit 进行判断和相关处理即可。
例如：攻击墙体，需要向其发送一个信息，以及播放攻击动画
```
private Animator myAnimator;
myAnimator = GetComponent<Animator> ();
myAnimator.SetTrigger ("Attack"); //改变状态机

hit.collider.SendMessage ("Damager");//调用墙体的 Damager fangfa
```

**怪物控制：**
　　怪物与主角的控制方式不同，主角是按键控制，而怪物是自己移动。
在一个单例 GameManager 中用一个 List 储存所有敌人，当主角移动时，调用 GameManager 中的 相应方法，遍历 List 通知所有敌人移动。

**Text 显示信息：**
　　我们需要几个 UI Text 来显示信息，需要显示当前食物的剩余、提示当前关卡、游戏结束时显示 Game Over。　
例如显示食物数量：
```
private Text foodText;  //
foodText = GameObject.Find ("FoodText").GetComponent<Text> ();//按名称查找
foodText.text = "Food : " + Hp;
```

###关卡变换：
　　关卡的变换首先要判断是否到达这种，这里有一个细节。我们判断是否到达终点需要知道主角的位置，所以在 GameManager 中要先获得主角的引用。但若是在 Awake 中获取，MapManager 可能并没有把主角创建出来，此时会报错。所以地图的创建一定要在 MapMahager 中的 Awake，而主角的获取需要在 GameManager 中的 Start。
　　当然，还有一种方式就是在 GameManage 中统一控制，先创建地图再创建 UI。
　　关卡变换涉及到一个场景的切换(新知识)，关卡变换地图需要重新生成，但血量，关卡数需要保留。
　　为了保留血量和关卡，我们在进入新的关卡时不能销毁当前 GameManager 。
```
DontDestroyOnLoad (this.gameObject);//在 GameManager 中使用此方法，重新加载时不销毁 GamaManager
```
```
void OnLevelWasLoaded(int scenesLevel) { 
//每次加载关卡时都会调用，可以在其中写一些需要更新的信息，具体可以查看API
  level++；
  initGame (); //初始化场景
}
```
此时会发现，虽然当前 GameManager 没有被销毁，但是出现了两个 GamaManager 。所以我们需要将 GameManager  做成 Prefabs ，不让它存在场景中，而是用创建的方式。
