#pragma strict

//KG_AudioMusicTrigger.js by Kaleb Grace.

public var AudioMusicManager : GameObject;
public var MusicOnTriggerEnter : int = -1;
public var MusicClipLoop : boolean = true;
public var MusicClipVolume : float = 1.0;

function OnTriggerEnter (other : Collider) {
	if (other.CompareTag("Player")) {
		AudioMusicManager.SendMessage("SetMusicClipLoop", MusicClipLoop);
		AudioMusicManager.SendMessage("SetMusicClipVolume", MusicClipVolume);
		AudioMusicManager.SendMessage("PlayMusicClip", MusicOnTriggerEnter);
	}
}