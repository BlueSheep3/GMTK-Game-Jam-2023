using UnityEngine;
using System.IO;

static class Savedata
{
	static readonly string path = Application.persistentDataPath + "/savedata.json";
	public static Savefile savefile;

	public class Savefile
	{
		public int quality = 2;
		public int volume = 100; // 0 to 100
		public int screenSize = 0; // 0: fullscreen
		public int maxLevelCompleted = 0;
	}

	public static void Load() {
		savefile = JsonUtility.FromJson<Savefile>(File.ReadAllText(path)) ?? new();
		AudioListener.volume = savefile.volume / 100f;
		QualitySettings.SetQualityLevel(savefile.quality);
	}

	public static void Save() {
		File.WriteAllText(path, JsonUtility.ToJson(savefile));
	}
}