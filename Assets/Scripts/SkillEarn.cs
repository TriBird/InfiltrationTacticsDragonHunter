using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Linq;
using UnityEngine.SceneManagement;

public class SkillEarn: MonoBehaviour{

	public Transform card_holder_trans, remain_text_trans, earn_error_trans;

	private bool isAnimation = false;
	private int select_remain = 2;

	private List<SkillMaster> card_list = new List<SkillMaster>();

	void Start(){
		card_holder_trans.localPosition = new Vector3(0f, -1080f, 0f);
		card_holder_trans.DOLocalMoveY(0f, 0.5f);
		earn_error_trans.gameObject.SetActive(false);
		GenerateCards();
	}

	public void EarnCards(){
		List<SkillMaster> selected_skills = new List<SkillMaster>();
		foreach(Transform tmp in card_holder_trans){
			if(tmp.GetComponent<CardCtrl>().is_selected){
				selected_skills.Add(card_list[tmp.GetComponent<CardCtrl>().card_index]);
			}
		} 
		
		// error
		if(selected_skills.Count < select_remain){
			earn_error_trans.gameObject.SetActive(true);
			DOVirtual.DelayedCall(1.5f, ()=>{
				earn_error_trans.gameObject.SetActive(false);
			});
			return;
		}

		foreach(SkillMaster skill in selected_skills){
			SkillModel.HavingSkills.Add(skill);
		}

		SceneManager.LoadScene("SelectUnits");
	}

	public void GenerateCards(){
		List<SkillMaster> deepCopiedList = SkillModel.skillmasters.Select(item => item).ToList();
		System.Random random = new System.Random();
		deepCopiedList.Select(x => new { Number = random.Next(), Item = x })
			.OrderBy(x => x.Number)
			.Select(x => x.Item)
			.ToList();

		card_list = deepCopiedList.Take(3).ToList();
		for(int i=0; i<3; i++){
			SkillMaster skill = card_list[i];
			Transform card_ins = card_holder_trans.GetChild(i);
			card_ins.GetComponent<CardCtrl>().select_limits = select_remain;
			card_ins.Find("Num").gameObject.SetActive(false);
			card_ins.Find("Name").GetComponent<Text>().text = skill.skill_name;
			card_ins.Find("Image").GetComponent<Image>().sprite = skill.skill_image;
			card_ins.Find("Description").GetComponent<Text>().text = skill.skill_desc;
		}
	}
}