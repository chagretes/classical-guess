using System;
using System.Collections;
using System.Collections.Generic;
using DataSystem;
using UnityEngine;

public class LevelController : MonoBehaviour
{
    [SerializeField]
    private int levelNumber = 1;
    [SerializeField]
    private GameObject titleUI;
    [SerializeField]
    private TextExtension titleText;
    [SerializeField]
    private GameObject levelUI;
    [SerializeField]
    private List<GameObject> composersUI;
    [SerializeField]
    private int songSeconds = 20;
    [SerializeField]
    private int roundsPerLevel = 5;
    [SerializeField]
    private int maxLevel = 1;
    [SerializeField]
    private int pointsPerRound = 500;
    [SerializeField]
    private ScoreData score;
    
    private DataManager dataManager;
    private AudioSource audioSource;
    private SoundEffectManager soundEffectManager;
    private List<ComposerData> composers;
    private int roundNumber = 1;
    private bool isPlaying;
    private int roundComposerID;
    private int playerIdGuess = -1;
    private DateTime startPlayTime;

    private enum RoundEndType { RightGuess, WrongGuess, Timeout };

    // Start is called before the first frame update
    void Start()
    {
        dataManager = new DataManager();
        audioSource = GetComponent<AudioSource>();
        soundEffectManager = SoundEffectManager.Instance;
        soundEffectManager.PlayAudioNamed("Level "+levelNumber.ToString());
        score.ClearScore();
        StartCoroutine(StartSequence());
    }

    IEnumerator StartSequence()
    {
        titleText.SetText("Level " + levelNumber);
        titleUI.SetActive(true);
        levelUI.SetActive(false);
        yield return new WaitForSeconds(3);
        titleUI.SetActive(false);
        levelUI.SetActive(true);
        composers = dataManager.GetComposers(levelNumber);
        for(int i = 0; i < 4; i++) {
            // Tink isso aqui tem baixa performace, 
            // mas preguiça de fazer o cache do TextMeshPro agora
            composersUI[i].GetComponent<TextExtension>().SetText(composers[i].Name);
        }

        StartCoroutine(StartRound());
    }

    IEnumerator StartRound() {
        var initialRound = roundNumber;
        isPlaying = true;
        roundComposerID = UnityEngine.Random.Range(0,4);
        var roundComposer = composers[roundComposerID];
        Debug.Log("Composer = " + roundComposer.Name);

        AudioClip roundSong = GetNextSong(roundComposer);
        Debug.Log("Song = " + roundSong.name);
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
        if(roundNumber>roundsPerLevel) {
            levelNumber++;
            if(levelNumber>maxLevel) {
                EndGame();
            } else {
                roundNumber = 1;
                StartCoroutine(StartRound());
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
            Debug.Log("Diff = " + diffInSeconds + "Round Score = " + roundScore);
        }
    }

    private void EndGame()
    {
        throw new System.NotImplementedException();
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
        float waitingTime;
        composersUI[roundComposerID].GetComponent<TextExtension>().ChangeColor(Color.green);

        if (endType is RoundEndType.RightGuess)
        {
            waitingTime = soundEffectManager.PlayAudioNamed("Correct");
            yield return new WaitForSeconds(waitingTime);

            waitingTime = soundEffectManager.PlayPositiveFeedback();
            //mensagem na tela com a mesma mensagem do áudio
            yield return new WaitForSeconds(waitingTime);
        }
        else if (endType is RoundEndType.WrongGuess)
        {
            composersUI[playerIdGuess].GetComponent<TextExtension>().ChangeColor(Color.red);

            waitingTime = soundEffectManager.PlayAudioNamed("Wrong");
            yield return new WaitForSeconds(waitingTime);

            //mensagem na tela Not This Time
            waitingTime = soundEffectManager.PlayAudioNamed("Not This Time");
            yield return new WaitForSeconds(waitingTime);

        }
        else if (endType is RoundEndType.Timeout)
        {
            waitingTime = soundEffectManager.PlayAudioNamed("Wrong");
            yield return new WaitForSeconds(waitingTime);

            //mensagem na tela The Time Is Up
            waitingTime = soundEffectManager.PlayAudioNamed("The Time Is Up");
            yield return new WaitForSeconds(waitingTime);
        }

        Debug.Log($"The composer was {composers[roundComposerID]}");
        waitingTime = soundEffectManager.PlayAudio(composers[roundComposerID].AnswerAudio);
        
        //mensagem na tela "The composer was 'name'!"

        yield return new WaitForSeconds(waitingTime);
        ResetComposerNameColors();
        playerIdGuess = -1;
    }

    void Update(){
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
}

