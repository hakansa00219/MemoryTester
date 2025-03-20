using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.VFX;

public class LevelManager : MonoBehaviour
{
    public int currentLevel = 0;
    private float waitTime = 0.5f;

    public static bool levelPlaying = false;

    private List<int> levelButtonSequence = new List<int>();

    [SerializeField] private UIInteractions _uiInt;
    [SerializeField] private GameObject _loseScreen;
    [SerializeField] private UnityEngine.UI.Button _adButton;
    [SerializeField] private TextMeshProUGUI _scoreText;

    [SerializeField] private AudioClip _lostAudio;

    private Coroutine waitLevelAnimationRoutine;

    public bool is_able_to_continue = true;

    public void Start()
    {
        currentLevel = 1;
        buttonCount = 0;
        
    }

    public void StartGame(int gameType)
    {
        is_able_to_continue = true;
        currentLevel = 1;
        buttonCount = 0;
        GameObject.FindGameObjectWithTag("LevelCount").transform.GetChild(0).GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().SetText("1");
        VisualEffect[] rippleEffects = FindObjectsOfType<VisualEffect>();
        switch(gameType)
        {
            case 0: //slow
                waitTime = 1f;
                foreach (VisualEffect vfx in rippleEffects) vfx.SetInt("Speed", 3000);
                break;
            case 1: //normal
                waitTime = 0.75f;
                foreach (VisualEffect vfx in rippleEffects) vfx.SetInt("Speed", 5000);
                break;
            case 2: //fast
                waitTime = 0.5f;
                foreach (VisualEffect vfx in rippleEffects) vfx.SetInt("Speed", 7500);
                break;
            case 3: //pro
                waitTime = 0.25f;
                foreach (VisualEffect vfx in rippleEffects) vfx.SetInt("Speed", 10000);
                break;
        }
        ChooseRandomButtons(currentLevel);
        StartCoroutine(PlayLevel(levelButtonSequence));
    }

    private void ChooseRandomButtons(int currentLevel)
    {
        levelButtonSequence.Clear();

        for(int i = currentLevel; i != 0; i--)
        {
            levelButtonSequence.Add(Random.Range(0, 4));
        }
    }

    private IEnumerator PlayLevel(List<int> buttonSequence)
    {
        if (buttonSequence == null) yield break;

        levelPlaying = true;

        //animasyon bekleme efekti
        waitLevelAnimationRoutine = StartCoroutine(WaitLevelAnimation());
        //end

        yield return new WaitForSeconds(2);
        for (int i = 0; i < buttonSequence.Count ; i++)
        {
            StartCoroutine(_uiInt.ButtonAnimation(buttonSequence[i]));
            yield return new WaitForSeconds(waitTime);
        }

        //animasyon bekleme efekti
        StopCoroutine(waitLevelAnimationRoutine);
        GameObject.FindGameObjectWithTag("LevelCount").transform.GetChild(0).GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().SetText(currentLevel.ToString());
        //end
        levelPlaying = false;
    }
    public int buttonCount = 0;
    public void CheckUserInput(int buttonIndex)
    {
        if (buttonIndex == levelButtonSequence[buttonCount++])
        {
            Debug.Log("OK!");
            if(currentLevel == buttonCount)
            {
                LoadNextLevel();
            }
        }
        else
        {
            //yanma kodu
            StartCoroutine(GameOver());
        }
        
    }

    private void LoadNextLevel()
    {
        buttonCount = 0;
        currentLevel++;
        GameObject.FindGameObjectWithTag("LevelCount").transform.GetChild(0).GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().SetText(currentLevel.ToString());
        ChooseRandomButtons(currentLevel);
        StartCoroutine(PlayLevel(levelButtonSequence));
        Debug.Log("Next Level:" + currentLevel);
    }

    public IEnumerator GameOver()
    {
        AudioSource audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.loop = false;
        audioSource.playOnAwake = false;
        audioSource.clip = _lostAudio;
        audioSource.Play();

        levelPlaying = true;

        yield return new WaitForSeconds(1);
        Destroy(audioSource, 0.5f);
        if (!FindObjectOfType<PlayerData>().GetAdInfo())  
        {
            FindObjectOfType<AdScript>().ShowIntAd();

           _adButton.interactable = FindObjectOfType<AdScript>().GetAdStatus() & is_able_to_continue;


        } 
        else
        {
            _adButton.GetComponentInChildren<TextMeshProUGUI>().SetText("RETRY");
            _adButton.interactable = is_able_to_continue;
                   
        }
        

        buttonCount = 0;
        //GameObject.FindGameObjectWithTag("LevelCount").transform.GetChild(0).GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().SetText("X");
        _scoreText.SetText("SCORE: " + (currentLevel - 1).ToString());
        _loseScreen.SetActive(true);
        levelPlaying = false;
        
        
        //continue e basilirsa odul kazandiysa tekrar cal PlayLevel()
        //odul kazanmadan kapattiysa back napiyorsa o
        Debug.Log("Game Over");
    }

    private IEnumerator WaitLevelAnimation()
    {
        TMPro.TextMeshProUGUI levelText = GameObject.FindGameObjectWithTag("LevelCount").transform.GetChild(0).GetChild(0).GetComponent<TMPro.TextMeshProUGUI>();
        while (true)
        {                   
            levelText.SetText(".");
            yield return new WaitForSeconds(0.2f);
            levelText.SetText("..");
            yield return new WaitForSeconds(0.2f);
            levelText.SetText("...");
            yield return new WaitForSeconds(0.2f);
        }
        

    }

    public void GameEnded()
    {
        _uiInt.gameplayActive = false;
        
        FindObjectOfType<PlayerData>().UpdateScore(currentLevel - 1);
        _uiInt.SetGameTypeTextsStatus(true);
        //back e basilirsa clear currentLevel
        //back e basilirse clear currentLevelSequence
        //gamePlayActive = false
    }

    public void GameContinued()
    {
        is_able_to_continue = false;
        StartCoroutine(PlayLevel(levelButtonSequence));
    }
}
