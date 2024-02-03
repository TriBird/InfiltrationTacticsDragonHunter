using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;

public class CardCtrl: MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler{

	public int card_index = 0;

	private Color32 default_color = new Color32(30, 30, 30, 255);
	private Color32 pointer_enter_color = new Color32(65, 40, 40, 255);

	public void OnPointerClick(PointerEventData eventData){
		GameObject.Find("Scripts").GetComponent<SelectUnits>().SelectCard(card_index);
	}

	public void OnPointerEnter(PointerEventData eventData){
		transform.GetComponent<Image>().color = pointer_enter_color;
	}

	public void OnPointerExit(PointerEventData eventData){
		transform.GetComponent<Image>().color = default_color;
	}

	void Start(){
		transform.GetComponent<Image>().color = default_color;
	}
}