using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.VFX;

public class UIInteractions : MonoBehaviour
{
    public Button[] mainButtons;
    public AudioClip[] audioClips;

    [SerializeField] private Button _soundButton;
    [SerializeField] private Button _noAdsButton;

    [SerializeField] private Texture _soundOn, _soundOff;

    [SerializeField] private GameObject _gameNameText;
    [SerializeField] private TextMeshProUGUI _bestScoreText;
    public bool gameplayActive = false;
    private bool _startOfGame = true;

    private Material[] _buttonMat = new Material[4];
    private void Awake()
    {
        for (int i = 0; i < mainButtons.Length; i++)
        {
            _buttonMat[i] = mainButtons[i].GetComponent<Image>().material;
        }
    }
    void Start()
    {
        StartCoroutine(ButtonInitAnimation());
        AudioListener.volume = 1.0f; //default sound
        for (int i = 0; i < mainButtons.Length; i++)
        {
            _buttonMat[i].SetColor("_EmissionColor", _buttonMat[i].color);
        }
    }

    public void GameStarted(int gameType)
    {
        if (_startOfGame == false) return;
        SetGameTypeTextsStatus(false);
        gameplayActive = true;
        StartCoroutine(StartGameRoutine(gameType));

    }
    private IEnumerator StartGameRoutine(int gameType)
    {
        yield return new WaitForSeconds(2f);
        FindObjectOfType<LevelManager>().StartGame(gameType);
    }

    public IEnumerator ButtonAnimation(int buttonIndex)
    {
        //vfx stuff
        mainButtons[buttonIndex].gameObject.GetComponent<VisualEffect>().Play();
        //end
        //sound stuff
        AudioSource audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.loop = false;
        audioSource.playOnAwake = false;
        audioSource.clip = audioClips[buttonIndex];
        audioSource.Play();
        //end
        //material stuff
        Material buttonMat = mainButtons[buttonIndex].GetComponent<Image>().material;
        Color color = buttonMat.color;
        buttonMat.SetColor("_EmissionColor", new Vector4(color.r, color.g, color.b) * 2f);
        yield return new WaitForSeconds(0.3f);
        buttonMat.SetColor("_EmissionColor", new Vector4(color.r, color.g, color.b) * 1f);
        //end
        //destory audio source since its not needed later.
        Destroy(audioSource , 0.5f);
        
    }

    public void SoundSetting()
    {
        GameObject spriteObj = _soundButton.gameObject;
        if (spriteObj.GetComponent<RawImage>().texture.name == "soundON")
        {
            AudioListener.volume = 0f;
            spriteObj.GetComponent<RawImage>().texture = _soundOff;
        }
        else if (spriteObj.GetComponent<RawImage>().texture.name == "soundOFF")
        {
            AudioListener.volume = 1.0f;
            spriteObj.GetComponent<RawImage>().texture = _soundOn;
        }
        else
        {
            Debug.Log("Sound Button bug - Maybe sprite name is changed?");
        }
    }
    
    public IEnumerator ButtonInitAnimation()
    {
        while(true)
        {
            while (!gameplayActive)
            {
                for (int i = 0; i < mainButtons.Length; i++)
                {
                    _buttonMat[i].SetColor("_EmissionColor", _buttonMat[i].color * 2f);
                    yield return new WaitForSeconds(0.3f);
                    _buttonMat[i].SetColor("_EmissionColor", _buttonMat[i].color);
                }

            }
            yield return new WaitForEndOfFrame();
        }    
    }

    public void SetBestScore(int score)
    {
        _bestScoreText.SetText("Best: "+ score.ToString());
    }

    public void SetGameTypeTextsStatus(bool status)
    {
        for (int i = 0; i < mainButtons.Length; i++)
        {
            mainButtons[i].transform.GetChild(0).gameObject.SetActive(status);
        }
        GameObject.FindGameObjectWithTag("LevelCount").transform.GetChild(0).GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().SetText("?");
        _gameNameText.SetActive(status);
        _startOfGame = status;
    }

}
