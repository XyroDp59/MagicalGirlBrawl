using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    public static GameController instance;

    [SerializeField] Color colorP1 = Color.cyan;
    [SerializeField] Color colorP2 = Color.red;
    [SerializeField] List<AudioClip> decompteClip = new List<AudioClip>();
    [SerializeField] List<string> decompteString = new List<string>();

    AudioSource source;
    WaitForSeconds second = new WaitForSeconds(1);
    [SerializeField] GameObject restroom;
    TextMeshProUGUI startAnnouncementText;


    private void Awake()
    {
        instance = this;
    }

    private int playerReady = 0;
    public void PlayerIsReady()
    {
        playerReady++;
        if (playerReady == 2)
        {
            StartCoroutine(StartFight());
        }
    }

    IEnumerator StartFight()
    {
        for (int i = decompteClip.Count; i > 0; i--)
        {
            startAnnouncementText.text = decompteString[i];
            //source.clip = decompteClip[i];
            //source.Play();
            yield return second;
        }
        restroom.SetActive(false);
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
