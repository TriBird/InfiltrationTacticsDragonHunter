using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class CardCtrl: MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler{

	public int card_index = 0;
	public int select_limits = 0;

	private Color32 default_color = new Color32(30, 30, 30, 255);
	private Color32 pointer_enter_color = new Color32(65, 40, 40, 255);
	private string scene_name = "";

	public bool is_selected = false;

	void Start(){
		is_selected = false;
		scene_name = SceneManager.GetActiveScene().name;
		transform.GetComponent<Image>().color = default_color;
	}

	public void OnPointerClick(PointerEventData eventData){
		if(scene_name == "UpGrade"){
			if(!is_selected){
				// check limit
				int selected_num = 0;
				foreach(Transform tmp in transform.parent){
					if(tmp.GetComponent<CardCtrl>().is_selected){
						selected_num += 1;
					}
				} 
				if(selected_num >= select_limits) return;

				is_selected = true;
				transform.GetComponent<Image>().color = pointer_enter_color;
			}else{
				is_selected = false;
				transform.GetComponent<Image>().color = default_color;
			}
			return;
		}
		GameObject.Find("Scripts").GetComponent<SelectUnits>().SelectCard(card_index);
	}

	public void OnPointerEnter(PointerEventData eventData){
		if(scene_name == "UpGrade") return;
		transform.GetComponent<Image>().color = pointer_enter_color;
	}

	public void OnPointerExit(PointerEventData eventData){
		if(scene_name == "UpGrade") return;
		transform.GetComponent<Image>().color = default_color;
	}
}