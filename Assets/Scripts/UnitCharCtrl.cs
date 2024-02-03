using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UnitCharCtrl : MonoBehaviour{

	public GameMaster gameMaster;
	public string unitname = "debug";
	private Sequence Move_Seq;

	private int AntiGavage = 0;

	void Start(){
		Move_Seq = DOTween.Sequence();
		Move_Seq.Append(
			transform.DOLocalMoveY(-290f, 0.5f)
				.OnComplete(()=>ChangeBuffATK(true))
		);
		Move_Seq.Append(
			transform.DOLocalMoveX(500f, 2.0f)
				.SetEase(Ease.Linear)
				.OnComplete(()=>ChangeBuffATK(false))
		);
		Move_Seq.Append(
			transform.DOLocalMove(new Vector2(Random.Range(400f, 600f), Random.Range(-150f, -180f)), 0.5f)
		);
		Move_Seq.Join(
			transform.DOLocalRotate(new Vector3(0, 0, Random.Range(-360f, 360f)), 0.5f)
		);
		Move_Seq.Append(transform.GetComponent<CanvasGroup>().DOFade(0, 0.5f));
		Move_Seq.SetLink(gameObject);
		Move_Seq.OnComplete(()=>{
			gameMaster.BossAttack(unitname, gameObject);
		});

		// きゃら固有能力
		// if(unitname == "弓兵") StartCoroutine("ArrowFire");
		if(unitname == "指揮官") transform.Find("AttackBuff").gameObject.SetActive(true);
	}

	// public void OnTriggerEnter2D(Collider2D other){
	// 	if(other.gameObject.name == "AttackBuff"){
	// 		if(unitname == "指揮官") return;
	// 		transform.Find("Attack").gameObject.SetActive(true);
	// 	}
	// }

	// バフ暴発抑制
	private void ChangeBuffATK(bool isBuffATK){
		if(unitname == "指揮官"){
			transform.Find("AttackBuff").gameObject.SetActive(isBuffATK);
		}
	}


	// 紅蓮の弓矢
	// private IEnumerator ArrowFire(){
	// 	yield return new WaitForSeconds(Random.Range(0f,0.5f));

	// 	while(true){
	// 		yield return new WaitForSeconds(1.0f);
	// 		GameObject tmp = Instantiate(gameMaster.Arrow_Prefab, transform.parent);
	// 		tmp.transform.localPosition = transform.localPosition;
	// 		tmp.GetComponent<ArrowCtrl>().gameMaster = gameMaster;
	// 		AntiGavage++;

	// 		// print("tamadeteruyo " + AntiGavage);
	// 		// ガベコレ対策
	// 		//if(AntiGavage <= 5) yield break;
	// 	}
	// }
}
