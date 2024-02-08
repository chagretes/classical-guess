using System.Collections;
using TMPro;
using UnityEngine;

namespace LevelSystem
{
    public class LevelController : MonoBehaviour
    {
        [SerializeField]
        private int LevelNumber;
        [SerializeField]
        private GameObject TitleUI;
        [SerializeField]
        private GameObject LevelUI;
        // Start is called before the first frame update
        void Start()
        {
            var asd = TitleUI.GetComponent<TextMeshProUGUI>();
            asd.SetText("Level " + LevelNumber);
            StartCoroutine(StartSequence());
        }

        IEnumerator StartSequence()
        {
            yield return new WaitForSeconds(3);
            TitleUI.SetActive(false);
            LevelUI.SetActive(true);

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}