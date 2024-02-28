using System;
using System.Collections;
using System.Collections.Generic;
using DataSystem;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelController : MonoBehaviour
{
    [SerializeField]
    private int levelNumber = 1;
    [SerializeField]
    private GameObject titleUI;
    [SerializeField]
    private TextExtension titleText;
    [SerializeField]
    private TextExtension textOnScreen;
    [SerializeField]
    private GameObject levelUI;
    [SerializeField]
    private List<GameObject> composersUI;
    [SerializeField]
    private int songSeconds = 10;
    [SerializeField]
    private int roundsPerLevel = 5;
    [SerializeField]
    private int maxLevel = 3;
    [SerializeField]
    private int pointsPerRound = 500;
    [SerializeField]
    private ScoreData score;

    [SerializeField]
    private GameObject instructions;

    [SerializeField]
    private AudioClip generalExplanation;
    [SerializeField]
    private AudioClip level1explanation;
    [SerializeField] 
    private AudioClip level2explanation;
    [SerializeField]
    private AudioClip level3explanation;

    private DataManager dataManager;
    private AudioSource audioSource;
    private SoundEffectManager soundEffectManager;
    private List<ComposerData> composers;
    private int roundNumber = 1;
    private int songNumber = 1;
    private bool isLearning;
    private bool isPlaying;
    private int roundComposerID;
    private int playerIdGuess = -1;
    private DateTime startPlayTime;
    private readonly List<string> congratulationList = new() { "That Is Right", "Excellent", "Well Done"};
    //criar uma variedade de mensagens para o erro também. não apenas "Not this time"
    private enum RoundEndType { RightGuess, WrongGuess, Timeout };

    // Start is called before the first frame update
    void Start()
    {
        dataManager = new DataManager();
        audioSource = GetComponent<AudioSource>();
        soundEffectManager = SoundEffectManager.Instance;
        instructions.SetActive(false);
        score.ClearScore();
        StartCoroutine(StartSequence());
    }

    IEnumerator StartSequence()
    {
        soundEffectManager.PlayAudioNamed($"Level {levelNumber}");
        titleText.SetText($"Level {levelNumber}");
        titleUI.SetActive(true);
        levelUI.SetActive(false);
        yield return new WaitForSeconds(3);
        titleUI.SetActive(false);
        levelUI.SetActive(true);
        textOnScreen.SetText($"Level {levelNumber} composers");
        textOnScreen.gameObject.SetActive(true);

        //load text instructions
        instructions.SetActive(true);

        composers = dataManager.GetComposers(levelNumber);
        for (int i = 0; i < 4; i++)
        {
            // Tink isso aqui tem baixa performace, 
            // mas preguiça de fazer o cache do TextMeshPro agora
            composersUI[i].GetComponent<TextExtension>().SetText(composers[i].Name);
        }

        isLearning = true;
        yield return LoadAudioInstructions();
    }

    IEnumerator LoadAudioInstructions()
    {
        if (levelNumber == 1) { audioSource.clip = level1explanation; }
        else if (levelNumber == 2) { audioSource.clip = level2explanation; }
        else if (levelNumber == 3) { audioSource.clip = level3explanation; }

        audioSource.Play();
        yield return new WaitForSeconds(audioSource.clip.length);
        if(isLearning) {
            audioSource.clip = generalExplanation;
            audioSource.Play();
        }
    }

    IEnumerator StartRound() {
        instructions.SetActive(false);

        var initialRound = roundNumber;
        isLearning = false;
        isPlaying = true;
        textOnScreen.SetText($"Song {songNumber}");
        roundComposerID = UnityEngine.Random.Range(0,4);
        var roundComposer = composers[roundComposerID];
#if UNITY_EDITOR
        Debug.Log("Composer = " + roundComposer.Name);
#endif

        AudioClip roundSong = GetNextSong(roundComposer);
#if UNITY_EDITOR
        Debug.Log("Song = " + roundSong.name);
#endif
        startPlayTime = DateTime.Now;
        PlayRandomTenSeconds(roundSong);

        yield return new WaitForSeconds(songSeconds);
        if (isPlaying && roundNumber == initialRound) {
            yield return EndRound(RoundEndType.Timeout);
        }
    }

    private IEnumerator EndRound(RoundEndType endType)
    {
        isPlaying = false;
        audioSource.Stop();
        ScoreCalculation(endType);
        yield return ProvideFeedback(endType);
        roundNumber++;
        songNumber++;
        if (roundNumber>roundsPerLevel) {
            levelNumber++;
            if(levelNumber>maxLevel) {
#if UNITY_EDITOR
                Debug.Log($"Your score was {score.Score}");
#endif
                EndGame();
            } else {
                roundNumber = 1;
                StartCoroutine(StartSequence());
            }
        } else {
            StartCoroutine(StartRound());
        }
    }

    private void ScoreCalculation(RoundEndType endType)
    {
        if (endType.Equals(RoundEndType.RightGuess)) {
            TimeSpan diffInSeconds = DateTime.Now - startPlayTime;
            int roundScore = pointsPerRound;
            if(diffInSeconds.TotalSeconds > 1) {
                roundScore = (int)(pointsPerRound * (songSeconds - diffInSeconds.TotalSeconds)/songSeconds);
            }
            score.AddScore(roundScore);
#if UNITY_EDITOR
            Debug.Log("Diff = " + diffInSeconds + "Round Score = " + roundScore);
#endif
        }
    }

    private void EndGame()
    {
        SceneManager.LoadScene("LeaderboardsScene");
    }

    // Depois tem conferir se a música já saiu para não ter
    // música repetida no mesmo round
    private AudioClip GetNextSong(ComposerData roundComposer)
    {
        var songsSize = roundComposer.Songs.Count;
        return roundComposer.Songs[UnityEngine.Random.Range(0,songsSize)];
    }

    private void PlayRandomTenSeconds(AudioClip song)
    {
        float randomStartTime = UnityEngine.Random.Range(0, Mathf.Max(0, song.length - songSeconds));

        audioSource.clip = song;
        audioSource.time = randomStartTime;
        audioSource.Play();
    }

    private IEnumerator ProvideFeedback(RoundEndType endType)
    {
        string message;
        float waitingTime;
        composersUI[roundComposerID].GetComponent<TextExtension>().ChangeColor(Color.green);

        if (endType is RoundEndType.RightGuess)
        {
            message = congratulationList[UnityEngine.Random.Range(0, congratulationList.Count)];

            yield return LoadFeedback(message, "Correct");
        }

        else if (endType is RoundEndType.WrongGuess)
        {
            composersUI[playerIdGuess].GetComponent<TextExtension>().ChangeColor(Color.red);

            message = "Not This Time";
            yield return LoadFeedback(message, "Wrong");
        }

        else if (endType is RoundEndType.Timeout)
        {
            message = "The Time Is Up";
            yield return LoadFeedback(message, "Wrong");
        }

        message = $"The composer was {composers[roundComposerID].name}";
        WriteMensageOnScreen(message);

        waitingTime = soundEffectManager.PlayAudio(composers[roundComposerID].AnswerAudio);
        yield return new WaitForSeconds(waitingTime);

        ResetComposerNameColors();
        playerIdGuess = -1;
    }

    void Update(){

        if(isLearning)
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                soundEffectManager.PlayAudio(composers[0].SelectionAudio);
            }

            if (Input.GetKeyDown(KeyCode.S))
            {
                soundEffectManager.PlayAudio(composers[1].SelectionAudio);
            }

            if (Input.GetKeyDown(KeyCode.D))
            {
                soundEffectManager.PlayAudio(composers[2].SelectionAudio);
            }

            if (Input.GetKeyDown(KeyCode.F))
            {
                soundEffectManager.PlayAudio(composers[3].SelectionAudio);
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                isLearning = false;
                audioSource.Stop();
                StartCoroutine(StartRound());
            }
        }

        if(isPlaying) {
            if (Input.GetKeyDown(KeyCode.A))
            {
                playerIdGuess = 0;
                AnalizePlayerGuess();
            }

            if (Input.GetKeyDown(KeyCode.S))
            {
                playerIdGuess = 1;
                AnalizePlayerGuess();
            }

            if (Input.GetKeyDown(KeyCode.D))
            {
                playerIdGuess = 2;
                AnalizePlayerGuess();
            }

            if (Input.GetKeyDown(KeyCode.F))
            {
                playerIdGuess = 3;
                AnalizePlayerGuess();
            }
        }
    }
    private IEnumerator LoadFeedback(string message, string rightWrong)
    {
        float waitingTime;

        WriteMensageOnScreen(message);

        waitingTime = soundEffectManager.PlayAudioNamed(rightWrong);
        yield return new WaitForSeconds(waitingTime);

        waitingTime = soundEffectManager.PlayAudioNamed(message);
        yield return new WaitForSeconds(waitingTime);
    }

    private void AnalizePlayerGuess()
    {
        if (roundComposerID == playerIdGuess) { StartCoroutine(EndRound(RoundEndType.RightGuess)); }
        else { StartCoroutine(EndRound(RoundEndType.WrongGuess)); }
    }

    private void ResetComposerNameColors()
    {
        composersUI[roundComposerID].GetComponent<TextExtension>().ChangeColor(Color.white);
        if (playerIdGuess != -1) { composersUI[playerIdGuess].GetComponent<TextExtension>().ChangeColor(Color.white); }
    }

    private void WriteMensageOnScreen(string message)
    {
        textOnScreen.SetText(message);
    }
}

