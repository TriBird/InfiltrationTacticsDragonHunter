using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Security.Cryptography;

public class SelectUnits: MonoBehaviour{

	public Transform card_holder_trans;

	private bool isAnimation = false;

	void Start(){
		
	}

	public void ChangeCards(){
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
		ChangeCards();
	}
}