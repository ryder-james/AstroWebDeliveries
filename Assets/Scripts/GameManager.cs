using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {
	#region Singleton
	public static GameManager Instance { get; private set; }

	private void Awake() {
		if (Instance == null) {
			Instance = this;
		} else {
			Destroy(this);
		}
		planets = FindObjectsOfType<Planet>();
	}
	#endregion

	[SerializeField]
	private float gameDurationSeconds = 5 * 60;

	[SerializeField]
	private TMP_Text timerLabel, scoreLabel, gameOverScoreLabel, highScoreLabel;

	[SerializeField]
	private GameObject gameOverScreen;

	[SerializeField]
	private Vector2 packagesPerSecond = Vector2.zero;

	[SerializeField]
	private float secondsToMaxSpeed = 60f;

	[SerializeField]
	private AnimationCurve packageSpawnCurve;

	public System.Action<Planet, Package> PackageSpawned { get; set; }

	private bool spawningPackages = true;

	private Planet[] planets;

	private float timeRemaining;

	private int Score { 
		get => score; 
		set {
			score = value;
			scoreLabel.SetText(score.ToString());
		}
	}

	private int previousIndex = -1;
	private int score, packages;

	public Planet GetRandomPlanet() {
		int rand = -1;
		for (int i = 0; i < 10; i++) {
			rand = Random.Range(0, planets.Length);
			if (rand != previousIndex)
				break;
		}
		previousIndex = rand;
		return planets[rand];
	}

	private Coroutine packageSpawner;

	private void Start() {
		packageSpawner = StartCoroutine(SpawnPackageLoop());

		previousIndex = -1;
		packages = 0;
		timeRemaining = gameDurationSeconds;
		Score = 0;
		gameOverScreen.SetActive(false);
	}

	public void GameOver() {
		StopCoroutine(packageSpawner);

		Time.timeScale = 0;
		gameOverScoreLabel.SetText($"Score: {Score}");
		int highScore = Mathf.Max(Score, PlayerPrefs.GetInt(nameof(Score), 0));
		PlayerPrefs.SetInt(nameof(Score), highScore);
		highScoreLabel.SetText($"High Score: {highScore}");
		gameOverScreen.SetActive(true);
	}

	public void Restart() {
		Time.timeScale = 1;
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
		Start();
	}

	private void Update() {
		timeRemaining -= Time.deltaTime;
		if (timeRemaining <= 0) {
			GameOver();
			return;
		}

		int minutes = Mathf.FloorToInt(timeRemaining / 60);
		int seconds = Mathf.FloorToInt(timeRemaining % 60);
		timerLabel.SetText($"{minutes}:{seconds:D2}");
	}

	private bool SpawnPackage() {
		if (packages >= 8) {
			return false;
		}

		Planet p = GetRandomPlanet();
		if (p.SpawnPackage(out Package pkg)) {
			pkg.DroppedOff += () => {
				Score++;
				packages--;
			};
			PackageSpawned?.Invoke(p, pkg);
			packages++;
			return true;
		}

		return false;
	}

	private IEnumerator SpawnPackageLoop() {
		float elapsedTime = 0f;

		while (spawningPackages) {
			float t = elapsedTime / secondsToMaxSpeed;
			float currentCallsPerSecond = Mathf.Lerp(packagesPerSecond.x, packagesPerSecond.y, packageSpawnCurve.Evaluate(t));
			float waitTime = 1f / currentCallsPerSecond;

			SpawnPackage();

			elapsedTime += waitTime;
			yield return new WaitForSeconds(waitTime);
		}
	}
}
