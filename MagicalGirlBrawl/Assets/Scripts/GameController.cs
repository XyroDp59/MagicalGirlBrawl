using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    public static GameController instance;

    [SerializeField] public Color[] colors = { Color.cyan, Color.red };
    [SerializeField] List<AudioClip> decompteClip = new List<AudioClip>();
    [SerializeField] List<string> decompteString = new List<string>();

    public List<Player> players = new List<Player>();
    private List<Player> playersReady = new List<Player>();


    AudioSource source;
    WaitForSeconds second = new WaitForSeconds(1);
    public GameObject restroom;
    [SerializeField] TextMeshProUGUI startAnnouncementText;


    private void Awake()
    {
        instance = this;
    }

    public void PlayerIsReady(Player p)
    {
        if(!playersReady.Contains(p)) playersReady.Add(p);
        if (playersReady.Count == 2)
        {
            StartCoroutine(StartFight());
        }
    }

    IEnumerator StartFight()
    {
        Debug.Log("hihiha");
        startAnnouncementText.transform.parent.gameObject.SetActive(true);
        for (int i = decompteClip.Count; i > 0; i--)
        {
            startAnnouncementText.text = decompteString[i];
            //source.clip = decompteClip[i];
            //source.Play();
            yield return second;
        }
        startAnnouncementText.transform.parent.gameObject.SetActive(false);
        yield return second;
        restroom.SetActive(false);
        
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
