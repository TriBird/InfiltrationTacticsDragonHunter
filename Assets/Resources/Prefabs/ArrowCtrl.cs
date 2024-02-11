using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class ArrowCtrl : MonoBehaviour{
	public GameMaster gameMaster;
	public int damage = 0;

	void Start(){
		Vector3 start_pos = transform.localPosition;
		Vector3 end_pos = gameMaster.Boss_Trans.localPosition;
		Vector3 via_pos = new Vector3((end_pos.x - start_pos.x) / 2 + start_pos.x, start_pos.y + Random.Range(50f, 500f), 0);

		transform.DOLocalPath(
			new[]{
				start_pos,
				via_pos,
				end_pos,
			},
			1f, PathType.CatmullRom, PathMode.Sidescroller2D
		)
		.SetOptions(false)
		.SetLookAt(0)
		.SetLink(gameObject)
		.OnComplete(()=>{
			gameMaster.UnitAttack(damage, gameObject);
			Destroy(gameObject);
		});
	}
}
