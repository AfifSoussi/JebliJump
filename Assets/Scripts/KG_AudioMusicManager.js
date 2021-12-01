#pragma strict

//KG_AudioMusicManager.js by Kaleb Grace.

//Things to do:
//- Implement option to beat match outgoing track (check current playback position).

public var MusicClips : AudioClip[];
public var MusicOnAwake : int = -1;
public var MusicOnAwakeLoop : boolean = true;
public var MusicOnAwakeVolume : float = 1.0;
public var ChannelCount : int = 2; 
public var CrossFadeSeconds : float = 3.0;
private var channels : GameObject[];
private var channelIndex : int = 0;
private var channelIndexRev : boolean = false;
private var musicClipLoop : boolean;
private var musicClipVolume : float;
//Crossfading.
private var crossFading : boolean = false;
private var caIn : AudioSource;
private var caOut : AudioSource;
private var fadeInMaxVolume : float;
private var fadeOutMaxVolume : float;

function Start () { 
	//Intitialize the references to the channel GameObjects.
	channels = new GameObject[ChannelCount];
	for (var i : int = 0; i < ChannelCount; i++) {
		channels[i] = GameObject.Find(name + "/Channel" + (i + 1).ToString());
	}
	SetMusicClipLoop(MusicOnAwakeLoop);
	SetMusicClipVolume(MusicOnAwakeVolume);
	PlayMusicClip(MusicOnAwake);
}

function SetMusicClipLoop (loop : boolean) {
	musicClipLoop = loop;
}

function SetMusicClipVolume (volume : float) {
	musicClipVolume = volume;
}

function PlayMusicClip (clipIndex : int) { 
	if (clipIndex < 0)
		return false;

	//"ca" stands for "channel audio".
	var caCurrent : AudioSource = channels[channelIndex].GetComponent.<AudioSource>();
	
	if (caCurrent.clip != null && caCurrent.isPlaying) {
		//There is music playing on this channel...
		if (caCurrent.clip == MusicClips[clipIndex]) {
			//And it's the same clip we're supposed to play anyway... 
			caCurrent.loop = musicClipLoop;
			return true; //Nothing else to do here.
		}
		if (crossFading) { 
			//The music is already crossfading...
			if (caOut.clip != MusicClips[clipIndex]) { 
				//If the outgoing music is a completely unrelated clip, stop it.
				caOut.volume = 0;
				caOut.Stop(); 
				//Now jump down to below where we proceed with a NEW crossfade.
			}
			else { 
				//The clip that was fading out is the same one we need to fade in now!
				//So let's just reverse the roles (caCurrent is only used as placeholder in this "shell game").
				caCurrent = caOut;
				caOut = caIn;
				caIn = caCurrent;
				fadeOutMaxVolume = fadeInMaxVolume;
				fadeInMaxVolume = musicClipVolume;
				caIn.loop = musicClipLoop;
				channelIndexRev = !channelIndexRev;
				channelIndex = getNewChannelIndex(); 
				return true; //Nothing else to do here.
			}
		} 
		//As said before, there is music on this channel, so proceed with a NEW crossfade...
		var caNew : AudioSource = channels[getNewChannelIndex()].GetComponent.<AudioSource>(); 
		caNew.clip = MusicClips[clipIndex]; 
		caNew.loop = musicClipLoop;
		caNew.volume = 0;  
		caNew.Play();
		crossFading = true;
		caIn = caNew;
		caOut = caCurrent;
		fadeInMaxVolume = musicClipVolume;
		fadeOutMaxVolume = caCurrent.volume;
		channelIndex = getNewChannelIndex();
	}
	else {
		//There is NO music playing on this channel...
		caCurrent.clip = MusicClips[clipIndex]; 
		caCurrent.loop = musicClipLoop;
		caCurrent.volume = musicClipVolume;
		caCurrent.Play();
	}
}

function Update () {
	if (crossFading) {
		caIn.volume = fadeInMaxVolume * Mathf.Clamp01(caIn.volume / fadeInMaxVolume + ((CrossFadeSeconds / 10) * Time.deltaTime));
		caOut.volume = fadeOutMaxVolume * Mathf.Clamp01(caOut.volume / fadeOutMaxVolume - ((CrossFadeSeconds / 10) * Time.deltaTime));
		if (caIn.volume == fadeInMaxVolume && caOut.volume == 0) {
			caOut.Stop(); //Stop the music if it reaches 0 volume; there are cases that may arise later where we do not want to do this.
			crossFading = false;
		}
	}
}

function getNewChannelIndex () {
	return (channelIndex + (channelIndexRev ? -1 : 1) + ChannelCount) % ChannelCount;
}