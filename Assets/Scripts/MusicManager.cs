using UnityEngine;
using System.Collections;

public class MusicManager : MonoBehaviour {


	
	public AudioClip[] MusicClips;
	public int MusicOnAwake = -1;
	public bool MusicOnAwakeLoop = true;
	public float MusicOnAwakeVolume = 1.0f;
	public int ChannelCount = 2; 
	public float CrossFadeSeconds = 3.0f;
	private GameObject[] channels;
	private int channelIndex = 0;
	private bool channelIndexRev = false;
	private bool musicClipLoop;
	private float musicClipVolume;
	//Crossfading.
	private bool crossFading  = false;
	public AudioSource caCurrent;
	private AudioSource caIn;
	private AudioSource caOut;
	private float fadeInMaxVolume;
	private float fadeOutMaxVolume;
	static int instances = 0;
	static MusicManager myInstance;
	
	public static MusicManager Instance {
		get {
			if (myInstance == null)
				myInstance = FindObjectOfType(typeof(MusicManager)) as MusicManager;
			
			return myInstance;
		}
	}

	// Use this for initialization
	void Start () {

		instances++;
		
		if (instances > 1)
			Debug.Log("Warning: There are more than one Music Manager at the level");
		else
			myInstance = this;

		//Intitialize the references to the channel GameObjects.
		channels = new GameObject[ChannelCount];
		for (int i = 0; i < ChannelCount; i++) {
			channels[i] = GameObject.Find(name + "/Channel" + (i + 1).ToString());
		}
		SetMusicClipLoop(MusicOnAwakeLoop);
		SetMusicClipVolume(MusicOnAwakeVolume);
		PlayMusicClip(MusicOnAwake);
	}

	int getNewChannelIndex () {
		return (channelIndex + (channelIndexRev ? -1 : 1) + ChannelCount) % ChannelCount;
	}

	void SetMusicClipLoop (bool loop) {
		musicClipLoop = loop;
	}
	
	void SetMusicClipVolume (float volume) {
		musicClipVolume = volume;
	}
	
	void PlayMusicClip (int clipIndex) { 
	//	if (clipIndex < 0)
	//		break;
	//	print ("PlayMusicClip  "+clipIndex);
		//"ca" stands for "channel audio".
		caCurrent = channels[channelIndex].GetComponent<AudioSource>();
		
		if (caCurrent.clip != null && caCurrent.isPlaying) {
			//There is music playing on this channel...
			if (caCurrent.clip == MusicClips[clipIndex]) {
				//And it's the same clip we're supposed to play anyway... 
				caCurrent.loop = musicClipLoop;
				//break; //Nothing else to do here.
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
			//		break; //Nothing else to do here.
				}
			} 
			//As said before, there is music on this channel, so proceed with a NEW crossfade...
			AudioSource caNew = channels[getNewChannelIndex()].GetComponent<AudioSource>(); 
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


	// Update is called once per frame
	void Update () {
		if (crossFading) {
			caIn.volume = fadeInMaxVolume * Mathf.Clamp01(caIn.volume / fadeInMaxVolume + ((CrossFadeSeconds / 10) * Time.deltaTime));
			caOut.volume = fadeOutMaxVolume * Mathf.Clamp01(caOut.volume / fadeOutMaxVolume - ((CrossFadeSeconds / 10) * Time.deltaTime));
			if (caIn.volume == fadeInMaxVolume && caOut.volume == 0) {
				caOut.Stop(); //Stop the music if it reaches 0 volume; there are cases that may arise later where we do not want to do this.
				crossFading = false;
			}
		}
	}
}
