using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class SkillModel: MonoBehaviour{

	public static List<SkillMaster> skillmasters = new List<SkillMaster>();
	public static List<SkillMaster> HavingSkills = new List<SkillMaster>();

	public void Awake(){
		skillmasters = new List<SkillMaster>{
			new SkillMaster("徴兵用紙", "各フェーズの選択可能部隊数が１増える"),
			new SkillMaster("友情の鎖", "部隊の人数+30%"),
			new SkillMaster("偶数の力", "部隊の数を偶数で調整すると攻撃体力２倍"),
			new SkillMaster("英知のゴーグル", "立ち止まったほうがいいことに気づく"),
			new SkillMaster("複合連立魔法", "３つの中から一度に２つを使用する"),
			new SkillMaster("天啓", "無敵が発動した際、ダメージの一部をドラゴンに反射"),
			new SkillMaster("絶対守護防壁", "起動すると数秒間無敵を得る"),
		};
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