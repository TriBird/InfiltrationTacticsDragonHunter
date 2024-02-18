using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Security.Cryptography;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class SelectUnits: MonoBehaviour{

	public GameObject unit_cell_prefab;
	public Transform card_holder_trans, remain_text_trans, unit_list_trans, upgrade_list_trans, description_trans;

	// public static Dictionary<UnitMaster, int> select_unit_dict = new Dictionary<UnitMaster, int>(); 
	public static List<UnitMaster> select_unit_lists = new List<UnitMaster>();
	private bool isAnimation = false;
	private bool isUnitListOpen = false;
	private List<UnitMaster> card_list = new List<UnitMaster>();
	private int select_remain = 5;

	void Start(){
		select_unit_lists = new List<UnitMaster>();
		card_holder_trans.localPosition = new Vector3(0f, -1080f, 0f);
		card_holder_trans.DOLocalMoveY(0f, 0.5f);
		SkillModel.isSkill("徴兵用紙", ()=>select_remain=6);
		remain_text_trans.GetComponent<Text>().text = "残り選択可能 " +  select_remain;
		description_trans.gameObject.SetActive(false);
		ChangeCards();
		UnitListUpdate();
		UpgradeListUpdate();
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
			UnitMaster unit = new UnitMaster(UnitDist.unit_masters[Random.Range(0, UnitDist.unit_masters.Count)]);
			// for rot fix
			// UnitMaster unit = UnitDist.name_to_master("歩兵");
			int number = Random.Range(UnitDist.unitnums[unit.rank][0], UnitDist.unitnums[unit.rank][1]);
			unit.num = number;
			SkillModel.isSkill("友情の鎖", ()=>{
				unit.num = Mathf.FloorToInt(unit.num * 1.3f);
			});

			Transform card_ins = card_holder_trans.GetChild(i);
			card_ins.Find("Name").GetComponent<Text>().text = unit.unit_name;
			card_ins.Find("Num").GetComponent<Text>().text = "x" + unit.num;
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

	public void UnitListUpdate(){
		foreach(Transform tmp in unit_list_trans.Find("UnitBox")) Destroy(tmp.gameObject);
		foreach(UnitMaster unit_m in select_unit_lists){
			GameObject obj = Instantiate(unit_cell_prefab, unit_list_trans.Find("UnitBox"));
			obj.transform.Find("Image").GetComponent<Image>().sprite = Resources.Load<Sprite>("Chara/" + unit_m.unit_name);
			obj.transform.Find("Name").GetComponent<Text>().text = unit_m.unit_name;
			obj.transform.Find("Num").GetComponent<Text>().text = unit_m.num.ToString();
		}
	}

	public void UpgradeDescShow(int index){
		description_trans.localPosition = new Vector3(120f * index + 70f - 750f, 323f, 0f);
		description_trans.Find("SkillTitle").GetComponent<Text>().text = SkillModel.skillmasters[index].skill_name;
		description_trans.Find("SkillDesc").GetComponent<Text>().text = SkillModel.skillmasters[index].skill_desc;
		description_trans.gameObject.SetActive(true);
	}
	public void UpgradeDescUnShow(){
		description_trans.gameObject.SetActive(false);
	}

	public void UpgradeListUpdate(){
		int counter = 0;
		foreach(SkillMaster master in SkillModel.skillmasters){
			EventTrigger e_trigger = upgrade_list_trans.GetChild(counter).GetComponent<EventTrigger>();
			e_trigger.transform.GetComponent<Image>().sprite = master.skill_image;
			if(SkillModel.HavingSkills.Any(skill => skill.skill_name == master.skill_name)){
				e_trigger.transform.GetComponent<Image>().color = new Color32(0xff, 0xff, 0xff, 0xff);
			}else{
				e_trigger.transform.GetComponent<Image>().color = new Color32(0xff, 0xff, 0xff, 0x30);
			}

			EventTrigger.Entry pointerEnterEntry = new EventTrigger.Entry();
			pointerEnterEntry.eventID = EventTriggerType.PointerEnter;
			int counter_ins = counter;
			pointerEnterEntry.callback.AddListener((data) => { UpgradeDescShow(counter_ins); });
			e_trigger.triggers.Add(pointerEnterEntry);

			EventTrigger.Entry pointerExitEntry = new EventTrigger.Entry();
			pointerExitEntry.eventID = EventTriggerType.PointerExit;
			pointerExitEntry.callback.AddListener((data) => { UpgradeDescUnShow(); });
			e_trigger.triggers.Add(pointerExitEntry);

			counter++;
		}
	}

	/// <summary>
	/// for chacking selected units
	/// </summary>
	public void UnitListToggle(){
		if(isAnimation) return;
		isUnitListOpen = !isUnitListOpen;
		isAnimation = true;
		float target_pos_y = -990f;
		string unit_list_uitxt = "▲ OPEN UNITS";
		if(isUnitListOpen){
			target_pos_y = 0f;
			unit_list_uitxt = "▼ CLOSE UNITS";
		}
		unit_list_trans.Find("UIText").GetComponent<Text>().text = unit_list_uitxt;
		unit_list_trans.DOLocalMoveY(target_pos_y, 0.5f).OnComplete(()=>{
			isAnimation = false;
		});
	}

	public void SelectCard(int index){
		if(isAnimation) return;

		UnitMaster unit = card_list[index];
		if(unit.unit_name == "歩兵" && unit.num % 2 == 0){
			SkillModel.isSkill("偶数の力", ()=>{
				unit.max_hitpoint = unit.max_hitpoint * 2;
				unit.current_hitpoint = unit.max_hitpoint;
				unit.attack = unit.attack * 2;
			});
		}

		select_unit_lists.Add(unit);

		select_remain -= 1;
		remain_text_trans.GetComponent<Text>().text = "残り選択可能 " +  select_remain;
		if(select_remain == 0){
			Selected_units();
		}

		ChangeCards();
		UnitListUpdate();
	}
}