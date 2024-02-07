using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UnitMaster{

	public string unit_name = "debug";
	public int hitpoint = 10;
	public int attack = 10;
	public int rank = 0;
	public int num = 0;
	public string unit_desc = "";

	public UnitMaster(int hp, int atk, int rank, string name, string desc){
		unit_name = name;
		hitpoint = hp;
		attack = atk;
		this.rank = rank;
		unit_desc = desc;
	}
}

public class UnitDist{
	public static List<UnitMaster> unit_masters = new List<UnitMaster>(){
		new UnitMaster(1, 1, 0, "歩兵", "数の多さで自己の弱さをカバーする。"),
		new UnitMaster(1, 1, 1, "弓兵", "突っ込む移動中にも攻撃してくれる"),
		new UnitMaster(1, 1, 1, "魔術師", "攻撃魔法、弱体化魔法、防衛魔法のどれかを使用する"),
		new UnitMaster(10, 5, 2, "騎兵", "１回攻撃を避けることができる"),
		new UnitMaster(10, 1, 2, "指揮官", "味方の攻撃力を上昇させる。"),
		new UnitMaster(500, 50, 3, "ジークフリート", "つよい…！"),
	};

	/// <summary>
	/// The class of finding UnitMaster from just name
	/// </summary>
	/// <param name="name">The name of unit</param>
	/// <returns>UnitMaster</returns>
	public static UnitMaster name_to_master(string name){
		foreach(UnitMaster master in UnitDist.unit_masters){
			if(master.unit_name == name){
				return master;
			}
		}

		return null;
	}

	public static int[][] unitnums = {
		new int[]{30, 50},
		new int[]{10, 30},
		new int[]{5, 10},
		new int[]{1, 3},
	};
}