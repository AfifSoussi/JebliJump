using UnityEngine;
using System.Collections;

public class MusicTrigger : MonoBehaviour {
	public GameObject musicManager;
	public int MusicOnTriggerEnter = -1;
	public bool MusicClipLoop = true;
	public float MusicClipVolume = 1.0f;


	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}


	
	void OnTriggerEnter2D (Collider2D other) {
		print ("trigger another sound");
		if (other.CompareTag("Player")) {
			musicManager.SendMessage("SetMusicClipLoop", MusicClipLoop);
			musicManager.SendMessage("SetMusicClipVolume", MusicClipVolume);
			musicManager.SendMessage("PlayMusicClip", MusicOnTriggerEnter);
		}
	}

}
