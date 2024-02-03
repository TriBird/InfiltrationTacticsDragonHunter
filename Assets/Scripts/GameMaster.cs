using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class GameMaster : MonoBehaviour{
	public GameObject UnitChara_Prefab;
	public GameObject Caution_obj, UnitNum_Obj;
	public Transform Boss_Trans, HPBar_Trans, VSBoss_Trans;

	private List<string> UnitList = new List<string>();

	public int CurrentUnitNum = 0;

	// ボス戦フラグ
	public static bool isBoss = false;
	public int MaxBossHP = 200;
	public int BossHP = 200;
	private int BossRemain = 10;
	private int CurrentPos = 0;

	// 開発者モード
	public bool DebugMode = true;

	/// <summary>
	/// 初期化
	/// </summary>
	void Start(){
		// ボス等初期状態
		Boss_Trans.localPosition = new Vector2(1330f, -160f);
		HPBar_Trans.localPosition = new Vector2(0f, 630f);

		// デバッグモード
		if(DebugMode){
			for(int i=0; i<100; i++){
				GetItem("歩兵", 1);
				if(i % 20 == 0) GetItem("指揮官", 1);
			}
		}

		BossEncounter();
	}

	/// <summary>
	/// 部隊アイテム取得
	/// </summary>
	public void GetItem(string unit, int num){
		// GameObject tmp = Instantiate(UnitCell_Prefab, Sequence_Trans);
		// tmp.GetComponent<Image>().sprite = Resources.Load<Sprite>("Chara/"+ unit);
		CurrentUnitNum += num;
		UnitNum_Obj.GetComponentInChildren<Text>().text = "x" + CurrentUnitNum;
		UnitList.Add(unit);
		//print("get unit [" + unit + "]");
	}

	/// <summary>
	/// ボス戦突入
	/// </summary>
	public void BossEncounter(){
		StartCoroutine("BossEncounter_Seq");
	}
	private IEnumerator BossEncounter_Seq(){
		// 自動地形生成システムをキル
		isBoss = true;

		// 警告画面表示
		Caution_obj.GetComponent<CanvasGroup>().alpha = 0;
		Caution_obj.SetActive(true);
		Caution_obj.transform.GetComponent<CanvasGroup>().DOFade(1, 0.5f);

		// Boss出てくる
		Boss_Trans.DOLocalMoveX(610f, 4f);

		// 地形がはけるまで5s待機
		yield return new WaitForSeconds(5.0f);

		// 警告画面消去
		Caution_obj.transform.GetComponent<CanvasGroup>().DOFade(0, 0.5f);
		Caution_obj.SetActive(false);

		yield return new WaitForSeconds(0.7f);

		// HPbarがズドン！
		HPBar_Trans.DOLocalMoveY(360f, 0.5f).SetEase(Ease.OutBounce);
		HPBar_Trans.Find("Fill").GetComponent<Image>().fillAmount = 1f;
		BossHP = MaxBossHP;

		// 隊列召喚！
		int unitnum = 0;
		while(true){
			yield return new WaitForSeconds(0.15f);

			// 画面下部の部隊も消していく
			//Destroy(Sequence_Trans.GetChild(0).gameObject);
			CurrentUnitNum--;
			UnitNum_Obj.GetComponentInChildren<Text>().text = "x" + CurrentUnitNum;

			unitnum++;
			GameObject tmp = Instantiate(UnitChara_Prefab, VSBoss_Trans);
			tmp.transform.localPosition = new Vector2(-700f, 670f);
			tmp.transform.GetComponent<Image>().sprite = Resources.Load<Sprite>("Chara/" + UnitList[unitnum-1]);
			tmp.transform.GetComponent<UnitCharCtrl>().gameMaster = this;
			tmp.GetComponent<UnitCharCtrl>().unitname = UnitList[unitnum-1];

			if(UnitList.Count <= unitnum) break;
		}

		yield break;
	}

	public void BossAttack(string unitname, GameObject obj){
		BossHP--;

		// ゲージを揺らす
		HPBar_Trans.Find("Fill").GetComponent<Image>().fillAmount = (float)BossHP / MaxBossHP;
		//Boss_Trans.DOShakePosition(0.1f, 10);
		Boss_Trans.DOLocalMoveX(550f, 0.5f).OnComplete(()=>{
			Boss_Trans.DOLocalMoveX(610f, 0.5f);
		});
		HPBar_Trans.DOShakePosition(0.1f, 5).OnComplete(() => Destroy(obj));
	}

}
