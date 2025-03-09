using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    public static GameController instance;

    [SerializeField] public Color[] colors = { Color.cyan, Color.red };
    [SerializeField] List<string> decompteString = new List<string>();

    public List<Player> players = new List<Player>();
    private List<Player> playersReady = new List<Player>();
    
    
    WaitForSeconds second;
    public GameObject restroom;
    [SerializeField] TextMeshProUGUI startAnnouncementText;
    
    // ---------- KAILY trucs --------------
    [SerializeField] private GameObject finMenu;
    [SerializeField] private GameObject pauseMenu;
    
    private FMOD.Studio.EventInstance _waitMusic;
    private FMOD.Studio.EventInstance _music;
    
    private bool _isPaused = false;
    private bool _gameStarted = false;

    public List<FMOD.Studio.EventInstance> _decompteInstances;
    // --------------------
    
    private void Awake()
    {
        instance = this;
        _waitMusic = FMODUnity.RuntimeManager.CreateInstance("event:/Soundtrack/A destiny cannot wait");
        _music = FMODUnity.RuntimeManager.CreateInstance("event:/Soundtrack/Magical Destiny");
        
        // !!!!!! PAS TOUCHER !!!!!!! L'ORDRE EST GIGA IMPORTANT !!!!!!!!
        _decompteInstances = new List<FMOD.Studio.EventInstance>();
        _decompteInstances.Add(FMODUnity.RuntimeManager.CreateInstance("event:/Voix/Bait go"));
        _decompteInstances.Add(FMODUnity.RuntimeManager.CreateInstance("event:/Voix/Bait 1"));
        _decompteInstances.Add(FMODUnity.RuntimeManager.CreateInstance("event:/Voix/Bait 2"));
        _decompteInstances.Add(FMODUnity.RuntimeManager.CreateInstance("event:/Voix/Bait 3"));
        // !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        
        _waitMusic.start();
        second = new WaitForSeconds(1);
    }

    public void PlayerIsReady(Player p)
    {
        if(!playersReady.Contains(p)) playersReady.Add(p);
        if (playersReady.Count == 2 && !_gameStarted)
        {
            StartCoroutine(StartFight());
        }
    }

    IEnumerator StartFight()
    {
        _waitMusic.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        _gameStarted = true;
        
        Debug.Log("hihiha");
        startAnnouncementText.transform.parent.gameObject.SetActive(true);
        startAnnouncementText.text = decompteString[3];
        for (int i = decompteString.Count-1; i >= 0; i--)
        {
            startAnnouncementText.text = decompteString[i];
            _decompteInstances[i].start();
            
            yield return second;
        }
        _music.start();
        
        startAnnouncementText.transform.parent.gameObject.SetActive(false);
        yield return second;
        restroom.SetActive(false);
        
    }

    private void EndGame()
    {
        Time.timeScale = 0;
        finMenu.SetActive(true);
    }

    public void OnPause()
    {
        if (!_isPaused)
        {
            _isPaused = true;
            Time.timeScale = 0;
            _music.setPaused(true);
            pauseMenu.SetActive(true);
        }
        else
        {
            _isPaused = false;
            Time.timeScale = 1;
            _music.setPaused(false);
            pauseMenu.SetActive(false);
        }
    }
    
    public void RestartGame()
    {
        _music.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        _waitMusic.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void MainMenu()
    {
        _music.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        _waitMusic.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        
        SceneManager.LoadScene("Menu");
    }
}
