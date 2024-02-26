using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class SoundMaster: MonoBehaviour{

	public static SoundMaster instance = null;

	public AudioClip BGM_Clip;
	public List<AudioClip> SE_Clips = new List<AudioClip>();

	public float MasterVolume = 0.3f;

	private AudioSource BGM_AS;
	private AudioSource SE_AS;

	private void Awake(){
		if(instance == null){
			instance = this;
			DontDestroyOnLoad(gameObject);
		}else{
			Destroy(gameObject);
		}
	}

	private void Start(){
		foreach(var se in SE_Clips){
			print(se.name);
		}

		AudioSource[] audioSources = transform.GetComponents<AudioSource>();
		BGM_AS = audioSources[0];
		SE_AS = audioSources[1];
	}

	public void PlayBGM(){
		BGM_AS.clip = BGM_Clip;
		BGM_AS.volume = MasterVolume;
		BGM_AS.loop = true;
		BGM_AS.Play();
	}

	public void PlaySE(string se_name){
		if(SE_Clips.Any(clip => clip.name == se_name)){
			SE_AS.clip = SE_Clips.Where(clip => clip.name == se_name).First();
			SE_AS.volume = MasterVolume * 0.6f;
			SE_AS.loop = false;
			SE_AS.Play();
		}
	}
}