using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Security.Cryptography;
using UnityEngine.SceneManagement;

public class SelectUnits: MonoBehaviour{

	public Transform card_holder_trans, remain_text_trans;

	public static Dictionary<string, int> select_unit_dict = new Dictionary<string, int>(); 
	private bool isAnimation = false;
	private List<UnitMaster> card_list = new List<UnitMaster>();

	private int select_remain = 5;

	void Start(){
		ChangeCards();
	}

	public void Selected_units(){
		// discard cards
		card_holder_trans.DOLocalMoveY(1260f, 0.7f).OnComplete(()=>{
			SceneManager.LoadScene("Buttle");
		});
	}

	public void ChangeCards(){
		card_list = new List<UnitMaster>();

		// rot cards
		for(int i=0; i<3; i++){
			UnitMaster unit = UnitDist.unit_masters[Random.Range(0, UnitDist.unit_masters.Count)];
			int number = Random.Range(UnitDist.unitnums[unit.rank][0], UnitDist.unitnums[unit.rank][1]);
			unit.num = number;

			Transform card_ins = card_holder_trans.GetChild(i);
			card_ins.Find("Name").GetComponent<Text>().text = unit.unit_name;
			card_ins.Find("Num").GetComponent<Text>().text = "x" + number;
			card_ins.Find("Image").GetComponent<Image>().sprite = Resources.Load<Sprite>("Chara/" + unit.unit_name);
			card_ins.Find("Description").GetComponent<Text>().text = unit.unit_desc;

			card_list.Add(unit);
		}

		// rotate cards
		isAnimation = true;
		Sequence seq = DOTween.Sequence();
		foreach(Transform child in card_holder_trans){
			seq.Join(child.DOLocalRotate(new Vector3(0, 360f, 0), 0.5f, RotateMode.FastBeyond360));
		}
		seq.OnComplete(() => isAnimation = false);
		seq.SetLink(gameObject);
	}

	public void SelectCard(int index){
		if(isAnimation) return;

		UnitMaster unit = card_list[index];

		if(select_unit_dict.ContainsKey(unit.unit_name)) {
			select_unit_dict[unit.unit_name] += unit.num;
		}else{
			select_unit_dict.Add(unit.unit_name, 1);
		}

		select_remain -= 1;
		remain_text_trans.GetComponent<Text>().text = "残り選択可能 " +  select_remain;
		if(select_remain == 0){
			Selected_units();
		}

		ChangeCards();
	}
}