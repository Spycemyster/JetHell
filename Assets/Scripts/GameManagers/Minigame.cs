using UnityEngine;

public class Minigame : MonoBehaviour
{
	public LevelHandler.LevelEvent OnCompleteMinigame {
		get { return level.OnCompleteLevel; }
		set { level.OnCompleteLevel = value; }
	}

	public LevelHandler.LevelEvent OnFailMinigame {
		get { return level.OnFailLevel; }
		set { level.OnFailLevel = value; }
	}

	[SerializeField] private LevelHandler level;
	public void InitializeLevel(PlayerController player, Camera mainCamera, float time)
	{
		level.InitializeLevel(player, time, mainCamera);
	}
}