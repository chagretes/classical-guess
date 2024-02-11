using System.Collections;
using System.Collections.Generic;
using DataSystem;
using TMPro;
using UnityEngine;

namespace LevelSystem
{
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
        private int songSeconds = 10;
        
        private DataManager dataManager;
        private TextMeshProUGUI titleText;
        private List<ComposerData> composers;
        private int roundNumber = 1;
        private ComposerData roundComposer;

        // Start is called before the first frame update
        void Start()
        {
            dataManager = new DataManager();
            titleText = titleUI.GetComponent<TextMeshProUGUI>();
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
        }

        // IEnumerator StartRound(){
        //     roundComposer = composers[Random.Range(1,4)];

        // }

        void Update(){

        }
    }
}