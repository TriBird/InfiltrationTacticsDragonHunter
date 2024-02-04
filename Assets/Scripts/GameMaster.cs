using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Linq;
using UnityEngine.SceneManagement;

public class GameMaster : MonoBehaviour{
	public GameObject UnitChara_Prefab, Arrow_Prefab;
	public GameObject Caution_obj, UnitNum_Obj;
	public Transform Boss_Trans, HPBar_Trans, VSBoss_Trans, BuffBox_Trans, ArrowLayer_Trans;

	public int CurrentUnitNum = 0;

	// ボス戦フラグ
	public static bool isBoss = false;
	public int MaxBossHP = 500;
	public int BossHP = 500;
	private int current_boss_id = 0;
	private int DefenceDownEffect = 0;
	private Coroutine boss_atk_col = null;

	// 開発者モード
	public bool DebugMode = true;

	/// <summary>
	/// 初期化
	/// </summary>
	void Start(){
		// ボス等初期状態
		Boss_Trans.localPosition = new Vector2(1330f, -160f);
		HPBar_Trans.localPosition = new Vector2(0f, 630f);

		UnitNum_Obj.GetComponentInChildren<Text>().text = "x" + SelectUnits.select_unit_dict.Values.Sum();

		// for debug
		if(SelectUnits.select_unit_dict.Count == 0){
			SelectUnits.select_unit_dict.Add("弓兵", 10);
			SelectUnits.select_unit_dict.Add("指揮官", 1);
			SelectUnits.select_unit_dict.Add("魔術師", 100);
		}

		BossEncounter();
		DrawBuffs();
	}
	
	/// <summary>
	/// drawing buff box
	/// </summary>
	public void DrawBuffs(){
		// defence down
		BuffBox_Trans.Find("DefenceDown").GetComponentInChildren<Text>().text = DefenceDownEffect.ToString();
		BuffBox_Trans.Find("DefenceDown").gameObject.SetActive(DefenceDownEffect != 0);
	}

	/// <summary>
	/// enchant defence down
	/// </summary>
	public void DefenceDown(){
		DefenceDownEffect += 1;

		// visualize
		DrawBuffs();
	}

	/// <summary>
	/// ボス戦突入
	/// </summary>
	public void BossEncounter(){
		StartCoroutine("BossEncounter_Seq");
	}
	/// <summary>
	/// boss encount
	/// </summary>
	private IEnumerator BossEncounter_Seq(){
		// 自動地形生成システムをキル
		isBoss = true;
		Boss_Init();

		// 警告画面表示
		Caution_obj.GetComponent<CanvasGroup>().alpha = 0;
		Caution_obj.SetActive(true);
		Caution_obj.transform.GetComponent<CanvasGroup>().DOFade(1, 0.5f);

		// Boss出てくる
		Boss_Trans.DOLocalMoveX(610f, 2f);

		// 地形がはけるまで5s待機
		yield return new WaitForSeconds(2.0f);

		// 警告画面消去
		Caution_obj.transform.GetComponent<CanvasGroup>().DOFade(0, 0.5f);
		Caution_obj.SetActive(false);

		yield return new WaitForSeconds(0.7f);

		// HPbarがズドン！
		HPBar_Trans.DOLocalMoveY(360f, 0.5f).SetEase(Ease.OutBounce);
		HPBar_Trans.Find("Fill").GetComponent<Image>().fillAmount = 1f;
		BossHP = MaxBossHP;
		boss_atk_col = StartCoroutine(BossAttack());

		// 隊列召喚！
		int unitnum = SelectUnits.select_unit_dict.Values.Sum();
		while(SelectUnits.select_unit_dict.Count > 0){
			yield return new WaitForSeconds(0.15f);

			// unit extract
			int index = Random.Range(0, SelectUnits.select_unit_dict.Count);
			string unit_name = SelectUnits.select_unit_dict.ElementAt(index).Key;
			SelectUnits.select_unit_dict[unit_name] -= 1;
			if(SelectUnits.select_unit_dict[unit_name] <= 0){
				SelectUnits.select_unit_dict.Remove(unit_name);
			}

			unitnum--;
			UnitNum_Obj.GetComponentInChildren<Text>().text = "x" + unitnum;

			GameObject tmp = Instantiate(UnitChara_Prefab, VSBoss_Trans);
			tmp.transform.localPosition = new Vector2(-700f, 670f);
			tmp.transform.GetComponent<Image>().sprite = Resources.Load<Sprite>("Chara/" + unit_name);
			tmp.transform.GetComponent<UnitCharCtrl>().gameMaster = this;
			tmp.GetComponent<UnitCharCtrl>().unitname = unit_name;
		}

		yield break;
	}

	/// <summary>
	///  boss initialize
	/// </summary>
	public void Boss_Init(){
		string[] boss_names = new string[]{"翼竜ワイバーン", "古代竜アルビオン", "神龍エスカドレイク"};
		int[] boss_hp = new int[]{1, 1000, 10000};

		MaxBossHP = boss_hp[current_boss_id];
		BossHP = MaxBossHP;

		HPBar_Trans.Find("Fill").GetComponent<Image>().fillAmount = 1;
		HPBar_Trans.Find("Name").GetComponent<Text>().text = boss_names[current_boss_id];
	}

	/// <summary>
	/// boss attack routine
	/// </summary>
	public IEnumerator BossAttack(){
		// dragon skill
		// [dragon crow] attack units whose had arrived to dragon(max 5 units)
		void DragonCrow(){
			List<UnitCharCtrl> ucc = new List<UnitCharCtrl>();
			foreach(Transform tmp in VSBoss_Trans){
				UnitCharCtrl ucc_ins = tmp.GetComponent<UnitCharCtrl>();
				if(ucc_ins){
					if(ucc_ins.is_arrive_dragon){
						ucc.Add(ucc_ins);
					}
				}
				if(ucc.Count >= 10)	break;
			}
			if(ucc.Count > 0){
				// animation
				Boss_Trans.DOLocalMoveX(550f, 0.2f).OnComplete(()=>{
					Boss_Trans.DOLocalMoveX(610f, 0.2f);
				}).SetLink(Boss_Trans.gameObject);
				// damaging
				foreach(UnitCharCtrl ucc_instance in ucc){
					ucc_instance.GetDamage(10);
				}
			}
		}
		void EskaBrace(){

		}

		while(true){
			DragonCrow();

			yield return new WaitForSeconds(1.0f);
		}
	}

	public void UnitAttack(int damage, GameObject obj){
		if(BossHP <= 0) return;

		BossHP -= damage;

		if(BossHP <= 0){
			StopCoroutine(boss_atk_col);

			// dead animation
			Boss_Trans.DOLocalRotate(new Vector3(0f, 0, -100f), 1.0f);
			Boss_Trans.DOLocalJump(new Vector3(Boss_Trans.localPosition.x + 800f, -200f, 0f), 100, 1, 1.0f);
			// SceneManager.LoadScene("SelectUnits");
		}

		// ゲージを揺らす
		HPBar_Trans.Find("Fill").GetComponent<Image>().fillAmount = (float)BossHP / MaxBossHP;
		HPBar_Trans.DOShakePosition(0.1f, 5);
	}

}
