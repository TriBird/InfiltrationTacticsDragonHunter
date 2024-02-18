using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;

/// <summary>
/// 結果表示のクラス
/// </summary>
public class ResultMaster: MonoBehaviour{
	
	public Transform result_trans;

	private int boss_id = -1;
	private int damage = -1;
	private int max_hp = -1;

	public void Start(){
		foreach(Transform tmp in result_trans){
			tmp.gameObject.SetActive(false);
		}

		ResultPipeLine(0, 5555, 5555);
	}

	public void ResultPipeLine(int _boss_id, int _damage, int _max_hp){
		boss_id = _boss_id;
		damage = _damage;
		max_hp = _max_hp;

		Vector2 pos = result_trans.Find("Next").localPosition;
		pos.x = result_trans.Find("dragon_" + (boss_id+1)).localPosition.x;
		result_trans.Find("Next").localPosition = pos;

		result_trans.Find("damage_info").GetComponent<TextMeshProUGUI>().text = "与えたダメージ 0/0 0%";

		StartCoroutine(ResultAnimation());
	}

	public IEnumerator ResultAnimation(){
		if(damage == max_hp){
			result_trans.Find("UIText_Win").gameObject.SetActive(true);
		}else{
			result_trans.Find("UIText_Lose").gameObject.SetActive(true);
		}

		yield return new WaitForSeconds(0.5f);

		for(int i=0; i<3; i++){
			result_trans.Find("dragon_" + i).gameObject.SetActive(true);
			yield return new WaitForSeconds(0.2f);
		}
		result_trans.Find("Next").gameObject.SetActive(true);

		yield return new WaitForSeconds(0.5f);

		result_trans.Find("damage_info").gameObject.SetActive(true);

		int display_damege_num = 0;
		int display_percent = 0;
		int percent = Mathf.FloorToInt(damage / max_hp * 100f);
		DOTween.To(() => 0, x => display_damege_num = x, damage, 3);
		DOTween.To(() => 0, x => display_percent = x, percent, 3);

		for(int i=0; i<300; i++){
			result_trans.Find("damage_info").GetComponent<TextMeshProUGUI>().text = "与えたダメージ " + display_damege_num + "/" + max_hp + " " + display_percent + "%";
			yield return new WaitForSeconds(0.01f);
		}

		yield return new WaitForSeconds(0.5f);

		result_trans.Find("return_units").gameObject.SetActive(true);

		yield break;
	}

	public void UpgradeSetter(Scene next, LoadSceneMode mode){
		int remain_num = boss_id;
		if(damage == max_hp){
			remain_num++;
		}
		GameObject.Find("Scripts").GetComponent<SkillEarn>().select_remain = remain_num;
		SceneManager.sceneLoaded -= UpgradeSetter;
	}
	
	public void ReturnUnitSelect(){
		SceneManager.sceneLoaded += UpgradeSetter;
		SceneManager.LoadScene("UpGrade");
	}

}