using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using System.Linq;

public class UnitCharCtrl : MonoBehaviour{

	public GameMaster gameMaster;
	public string unitname = "debug";
	public bool is_arrive_dragon = false;
	public UnitMaster unit_master;

	private int current_hp = -1;
	private bool is_invisible = false;

	private Sequence Move_Seq;
	private Coroutine col;

	void Start(){
		// 自分が何者かを知る
		// unit_master = UnitDist.name_to_master(unitname);
		current_hp = unit_master.max_hitpoint;

		// char_skill
		if(unit_master.unit_name == "騎兵"){
			is_invisible = true;
		}

		// appear animation
		Move_Seq = DOTween.Sequence();
		Move_Seq.Append(
			transform.DOLocalMoveY(Random.Range(-400f, -300f), 0.5f)
				// ground
				.OnComplete(()=>{
					ChangeBuffATK(true);
					if(unitname == "弓兵") StartCoroutine("ArrowFire");
				})
		);
		Move_Seq.Append(
			transform.DOLocalMoveX(500f, 2.0f).SetEase(Ease.Linear)
		);
		Move_Seq.SetLink(gameObject);
		Move_Seq.OnComplete(()=>{
			col = StartCoroutine(Cronus());
			ChangeBuffATK(false);
			is_arrive_dragon = true;
		});
	}

	// Revieved boss attacking
	public void GetDamage(int damage){
		if(is_invisible){
			is_invisible = false;
			return;
		}
		current_hp -= damage;
		if(current_hp < 0){
			DeadUnit();
		}
	}

	// change buffer object
	private void ChangeBuffATK(bool isBuffATK){
		if(unitname == "指揮官"){
			transform.Find("Buffer").gameObject.SetActive(isBuffATK);
		}
	}

	public void OnTriggerEnter2D(Collider2D other){
		if(other.gameObject.name == "Buffer"){
			transform.Find("AttackBuff").gameObject.SetActive(true);
		}
	}

	/// <summary>
	/// gameover soldier
	/// </summary>
	public void DeadUnit(){
		if(col != null) StopCoroutine(col);
		Move_Seq.Kill();

		// dead animation
		Move_Seq = DOTween.Sequence();
		Move_Seq.Append(
			transform.DOLocalMove(
				new Vector2(
					Random.Range(transform.localPosition.x-100f, transform.localPosition.x+100f),
					Random.Range(transform.localPosition.y+130f, transform.localPosition.y+150f)
				), 0.5f
			)
		);
		Move_Seq.Join(
			transform.DOLocalRotate(new Vector3(0, 0, Random.Range(-360f, 360f)), 0.5f)
		);
		Move_Seq.Append(transform.GetComponent<CanvasGroup>().DOFade(0, 0.5f));
		Move_Seq.SetLink(gameObject);
		Move_Seq.OnComplete(()=>{
			Destroy(gameObject);
		});
	}

	// magics
	public void Magic_Attack(){
		gameMaster.UnitAttack(unit_master.attack, gameObject);
	}
	public void Magic_DefenceDown(){
		gameMaster.DefenceDown(5);
	}
	public void Magic_Heal(){
		// select heal target by randam
		int r = Random.Range(0, gameMaster.VSBoss_Trans.childCount);
		UnitCharCtrl ucc = gameMaster.VSBoss_Trans.GetChild(r).GetComponent<UnitCharCtrl>();
		ucc.current_hp += Mathf.FloorToInt(ucc.unit_master.max_hitpoint / 2);

		// animation
		Transform effect_trans = ucc.transform.Find("HealEffect");
		effect_trans.gameObject.SetActive(true);
		effect_trans.DOLocalMoveY(effect_trans.localPosition.y + 50f, 0.3f).OnComplete(()=>{
			DOVirtual.DelayedCall(0.3f, ()=>effect_trans.gameObject.SetActive(false)).SetLink(gameObject);
		}).SetLink(gameObject);
	}

	// main routine
	public IEnumerator Cronus(){
		if(unit_master.unit_name == "弓兵"){
			yield break;
		}

		while(true){
			if(unit_master.unit_name == "魔術師"){
				int index = Random.Range(0, 3);
				
				if(SkillModel.HavingSkills.Any(item => item.skill_name == "圧縮魔法")){
					switch(index){
						case 0:
							Magic_Attack();
							Magic_DefenceDown();
							break;
						case 1:
							Magic_Attack();
							Magic_DefenceDown();
							break;
						case 2:
							Magic_Attack();
							Magic_Heal();
							break;
					}
				}else{
					switch(index){
						case 0: Magic_Attack(); break;
						case 1: Magic_DefenceDown(); break;
						case 2: Magic_Heal(); break;
					}
				}
			}
			gameMaster.UnitAttack(unit_master.attack, gameObject);
			yield return new WaitForSeconds(0.5f);
		}
	}

	// arrow
	private IEnumerator ArrowFire(){
		yield return new WaitForSeconds(Random.Range(0f, 0.5f));

		while(true){
			GameObject tmp = Instantiate(gameMaster.Arrow_Prefab, gameMaster.ArrowLayer_Trans);
			tmp.transform.localPosition = transform.localPosition;
			tmp.GetComponent<ArrowCtrl>().gameMaster = gameMaster;

			yield return new WaitForSeconds(1.0f);
		}
	}

}
