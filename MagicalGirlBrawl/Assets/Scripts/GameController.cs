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
    WaitForSeconds second;
    public GameObject restroom;
    [SerializeField] TextMeshProUGUI startAnnouncementText;
    
    // FMOD
    private FMOD.Studio.EventInstance _waitMusic;
    private FMOD.Studio.EventInstance _music;

    private void Awake()
    {
        instance = this;
        _waitMusic = FMODUnity.RuntimeManager.CreateInstance("event:/Soundtrack/A destiny cannot wait");
        _music = FMODUnity.RuntimeManager.CreateInstance("event:/Soundtrack/Magical Destiny");
        
        _waitMusic.start();
        second = new WaitForSeconds(1);
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
        _waitMusic.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        
        Debug.Log("hihiha");
        startAnnouncementText.transform.parent.gameObject.SetActive(true);
        startAnnouncementText.text = decompteString[3];
        for (int i = decompteString.Count-1; i > 0; i--)
        {
            startAnnouncementText.text = decompteString[i];
            //source.clip = decompteClip[i];
            //source.Play();
            yield return second;
        }
        _music.start();
        
        startAnnouncementText.transform.parent.gameObject.SetActive(false);
        yield return second;
        restroom.SetActive(false);
        
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
