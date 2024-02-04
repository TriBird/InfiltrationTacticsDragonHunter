using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UnitCharCtrl : MonoBehaviour{

	public GameMaster gameMaster;
	public string unitname = "debug";
	public bool is_arrive_dragon = false;

	private int current_hp = -1;

	private UnitMaster unit_master;
	private Sequence Move_Seq;
	private Coroutine col;

	void Start(){
		// 自分が何者かを知る
		unit_master = UnitDist.name_to_master(unitname);
		current_hp = unit_master.hitpoint;

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

	public void GetDamage(int damage){
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

	public void DeadUnit(){
		StopCoroutine(col);
		Move_Seq.Kill();

		// dead animation
		Move_Seq = DOTween.Sequence();
		Move_Seq.Append(
			transform.DOLocalMove(new Vector2(Random.Range(400f, 600f), Random.Range(-150f, -180f)), 0.5f)
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

	// main routine
	public IEnumerator Cronus(){
		while(true){
			gameMaster.UnitAttack(unitname, gameObject);
			yield return new WaitForSeconds(0.5f);
		}
	}

	// arrow
	private IEnumerator ArrowFire(){
		yield return new WaitForSeconds(Random.Range(0f, 0.5f));

		while(true){
			GameObject tmp = Instantiate(gameMaster.Arrow_Prefab, transform.parent);
			tmp.transform.localPosition = transform.localPosition;
			tmp.GetComponent<ArrowCtrl>().gameMaster = gameMaster;

			yield return new WaitForSeconds(1.0f);
		}
	}

}
