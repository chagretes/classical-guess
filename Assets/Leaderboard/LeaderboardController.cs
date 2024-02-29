using System;
using System.Collections;
using DataSystem;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class LeaderboardController : MonoBehaviour
{
    [SerializeField]
    private ScoreData score;
    [SerializeField]
    private TextExtension instructions;
    [SerializeField]
    private TMP_InputField inputName;
    [SerializeField]
    private GameObject leaderboardList;

    void Start()
    {
        StartCoroutine(GetScores());
        StartCoroutine(AnnounceScore(score.Score));
        instructions.SetText($"YOUR SCORE WAS\n{score.Score}\nTYPE 3 Characters TO REGISTER YOUR SCORE");
        inputName.Select();
        inputName.onValueChanged.AddListener(inputChanged);
    }

    void Update()
    {
        if(!inputName.enabled)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                SceneManager.LoadScene("StartScene");
            }

        }
        else if(inputName.enabled && !inputName.isFocused)
        {
            inputName.Select();
        }
    }

    private void inputChanged(string input)
    {
        if(input.Length == 3)
        {
            inputName.enabled = false;
            StartCoroutine(PostScore());
        }
    }

    IEnumerator PostScore()
    {
        var name = inputName.text.ToUpperInvariant();
        string uri = $"http://63.142.241.180:5001/api/Score?name={name}&points={score.Score}";
        using UnityWebRequest request = UnityWebRequest.Post(uri, "", "application/json");
        yield return request.SendWebRequest();
        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError(request.error);
        }
        else
        {
            Debug.Log("Form upload complete!");
        }
        StartCoroutine(GetScores());
    }

    private IEnumerator GetScores()
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get("http://63.142.241.180:5001/api/Score"))
        {
            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();

            switch (webRequest.result)
            {
                case UnityWebRequest.Result.Success:
                    leaderboardList.SetActive(true);
                    ScoreWrapper scoresWrapper = new ();
                    JsonUtility.FromJsonOverwrite(webRequest.downloadHandler.text, scoresWrapper);
                    var scores = scoresWrapper.scores;

                    for (int i = 0;i<leaderboardList.GetComponent<Transform>().childCount;i++)
                    {
                        var name = scores[i].name;
                        var points = scores[i].points;
                        
                        var elements = leaderboardList.GetComponent<Transform>().GetChild(i);
                        elements.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().SetText($"{i+1} {name}");
                        elements.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>().SetText($"{points}");
                    }

                    break;
                default:
                    Debug.LogError("HTTP Error: " + webRequest.error);
                    break;
            }
        }
    }

    private IEnumerator AnnounceScore(int value)
    {
        int thousandValue = ((value / 1000) % 10) * 1000;
        int hundredValue = ((value / 100) % 10) * 100;
        int tenValue = ((value / 10) % 10) * 10;

        float waitingTime = SoundEffectManager.Instance.PlayAudioNamed("YourScoreWas", "ScoreAudio");
        yield return new WaitForSeconds(waitingTime);

        waitingTime = SoundEffectManager.Instance.PlayAudioNamed(thousandValue.ToString(), "ScoreAudio");
        yield return new WaitForSeconds(waitingTime);
        
        waitingTime = SoundEffectManager.Instance.PlayAudioNamed(hundredValue.ToString(), "ScoreAudio");
        yield return new WaitForSeconds(waitingTime);

        waitingTime = SoundEffectManager.Instance.PlayAudioNamed(tenValue.ToString(), "ScoreAudio");
        yield return new WaitForSeconds(waitingTime);

    }


    [Serializable]
    private class ScoreWrapper
    {
        public ScoreDTO[] scores;
    }

    [Serializable]
    private class ScoreDTO
    {
        public Guid id;
        public string name;
        public int points;
    }
}
