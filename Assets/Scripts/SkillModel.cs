using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;

public class SkillModel: MonoBehaviour{

	public static List<SkillMaster> skillmasters = new List<SkillMaster>();
	public static List<SkillMaster> HavingSkills = new List<SkillMaster>();

	public void Awake(){
		skillmasters = new List<SkillMaster>{
			new SkillMaster("徴兵用紙", "各フェーズの選択可能部隊数が１増える"),
			new SkillMaster("友情の鎖", "部隊の人数+30%"),
			new SkillMaster("偶数の力", "部隊の数を偶数で調整すると歩兵の攻撃体力２倍"),
			new SkillMaster("英知のゴーグル", "弓兵が立ち止まったほうがいいことに気づく"),
			new SkillMaster("圧縮魔法", "魔術師は３つの魔法の中から一度に２つを使用する"),
			new SkillMaster("天啓", "騎士の無敵が発動した際、ダメージの一部をドラゴンに反射"),
			new SkillMaster("絶対守護防壁", "起動すると指揮官の周辺で数秒間無敵を得る"),
		};

		// loadskills
		HavingSkills = new List<SkillMaster>();
		List<string> splited_skill_names = new List<string>(PlayerPrefs.GetString("having_skills").Split(","));
		foreach(string skill_name in splited_skill_names){
			if(skillmasters.Any(item => item.skill_name == skill_name)){
				HavingSkills.Add(skillmasters.Where(item => item.skill_name == skill_name).FirstOrDefault());
			}
		}
	}

	public static void isSkill(string name, Action action){
		if(HavingSkills.Any(item => item.skill_name == name)){
			action.Invoke();
		}
	}

	public void OnDestroy(){
		// save as csv text
		string savetxt = "";
		foreach(SkillMaster skill in HavingSkills){
			savetxt += skill.skill_name + ",";
		}
		PlayerPrefs.SetString("having_skills", savetxt);
	}
}

public class SkillMaster{
	public string skill_name = "";
	public string skill_desc = "";
	public Sprite skill_image = null;

	public SkillMaster(string name, string desc){
		skill_name = name;
		skill_desc = desc;
		skill_image = Resources.Load<Sprite>("SkillIcon/"+skill_name);
	}
}