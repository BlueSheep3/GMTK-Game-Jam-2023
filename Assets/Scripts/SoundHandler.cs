using UnityEngine;

class SoundHandler : MonoBehaviour
{
	static SoundHandler inst;

	static AudioSource[] audioSources = new AudioSource[20];
	static int currentSource = 0;

	public AudioSource musicSource;


	void Awake() {
		if(inst != null) {
			Destroy(gameObject);
			return;
		}
		inst = this;
		DontDestroyOnLoad(gameObject);

		// create audio sources
		for(int i = 0; i < audioSources.Length; i++) {
			AudioSource a = gameObject.AddComponent<AudioSource>();
			a.dopplerLevel = 0;
			a.rolloffMode = AudioRolloffMode.Linear;
			a.minDistance = 500;
			a.maxDistance = 600;
			a.playOnAwake = false;
			audioSources[i] = a;
		}
	}

	public static void PlaySound(string clip, float volume) {
		#if UNITY_EDITOR
			CheckSoundHandlerExists();
		#endif

		audioSources[currentSource].clip = Resources.Load<AudioClip>($"Sounds/{clip}");
		audioSources[currentSource].volume = volume;
		audioSources[currentSource].Play();
		currentSource = (currentSource + 1) % audioSources.Length;
	}

	public static void PlayMusic(string clip) {
		#if UNITY_EDITOR
			CheckSoundHandlerExists();
		#endif

		if(inst.musicSource.clip?.name == clip) return;
		inst.musicSource.clip = Resources.Load<AudioClip>($"Music/{clip}");
		inst.musicSource.Play();
	}

	public static void StopMusic() {
		#if UNITY_EDITOR
			CheckSoundHandlerExists();
		#endif

		inst.musicSource.Stop();
	}

	#if UNITY_EDITOR
	static void CheckSoundHandlerExists() {
		if(inst == null) {
			Debug.Log("(SOUND): Creating SoundHandler Object...");
			GameObject sh = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/SoundHandler.prefab");
			inst = Instantiate(sh).GetComponent<SoundHandler>();
		}
	}
	#endif
}