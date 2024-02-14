using System;
using System.Collections;
using System.Collections.Generic;
using DataSystem;
using TMPro;
using UnityEngine;


public class LevelController : MonoBehaviour
{
    [SerializeField]
    private int levelNumber = 1;
    [SerializeField]
    private GameObject titleUI;
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
    
    private DataManager dataManager;
    private TextMeshProUGUI titleText;
    private AudioSource audioSource;
    private List<ComposerData> composers;
    private int roundNumber = 1;
    private bool isPlaying;
    private int roundComposerID;
    private DateTime startPlayTime;

    private enum RoundEndType { RightGuess, WrongGuess, Timeout };

    // Start is called before the first frame update
    void Start()
    {
        dataManager = new DataManager();
        titleText = titleUI.GetComponent<TextMeshProUGUI>();
        audioSource = GetComponent<AudioSource>();
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
            var text = composersUI[i].GetComponent<TextMeshProUGUI>();
            text.SetText(composers[i].Name);
        }
        StartCoroutine(StartRound());
    }

    IEnumerator StartRound(){
        var initialRound = roundNumber;
        isPlaying = true;
        roundComposerID = UnityEngine.Random.Range(0,4);
        var roundComposer = composers[roundComposerID];
        Debug.Log("Composer = " + roundComposer.Name);
        AudioClip roundSong = GetNextSong(roundComposer);
        Debug.Log("Song = " + roundSong.name);
        audioSource.clip = roundSong;
        audioSource.Play();
        startPlayTime = DateTime.Now;
        yield return new WaitForSeconds(songSeconds);
        if (isPlaying && roundNumber == initialRound) {
            EndRound(RoundEndType.Timeout);
        }
    }

    private void EndRound(RoundEndType endType)
    {
        isPlaying = false;
        audioSource.Stop();
        //Pontua ou não
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

    void Update(){
        if(isPlaying) {
            if (Input.GetKeyDown(KeyCode.A))
            {
                if(roundComposerID == 0) {
                    EndRound(RoundEndType.RightGuess);
                } else {
                    EndRound(RoundEndType.WrongGuess);
                }
            }

            if (Input.GetKeyDown(KeyCode.S))
            {
                if(roundComposerID == 1) {
                    EndRound(RoundEndType.RightGuess);
                } else {
                    EndRound(RoundEndType.WrongGuess);
                }
            }

            if (Input.GetKeyDown(KeyCode.D))
            {
                if(roundComposerID == 2) {
                    EndRound(RoundEndType.RightGuess);
                } else {
                    EndRound(RoundEndType.WrongGuess);
                }
            }

            if (Input.GetKeyDown(KeyCode.F))
            {
                if(roundComposerID == 3) {
                    EndRound(RoundEndType.RightGuess);
                } else {
                    EndRound(RoundEndType.WrongGuess);
                }
            }
        }
    }
}

