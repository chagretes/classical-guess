using UnityEngine;
using UnityEngine.SceneManagement;

public class StartController : MonoBehaviour
{
    private AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
        Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height, FullScreenMode.FullScreenWindow);
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if(Input.anyKeyDown) {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                SceneManager.LoadScene("Level");
            }
            else {
                audioSource.Play();
            }
        }
    }
     
}
