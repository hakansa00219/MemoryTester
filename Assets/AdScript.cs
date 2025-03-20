using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;

public class AdScript : MonoBehaviour, IUnityAdsListener
{
    string GooglePlay_ID = "3790421";
    bool testMode = false;

    string myPlacementId = "rewardedVideo";

    [SerializeField] private GameObject _adButton;
    private bool ad_loaded = false;

    void Start()
    {
        Advertisement.AddListener(this);
        Advertisement.Initialize(GooglePlay_ID, testMode);
    }

    public void ShowIntAd()
    {
        Advertisement.Show();
    }
    
    public void ShowRewardAd()
    {
        if(FindObjectOfType<PlayerData>().GetAdInfo()) 
        {
            FindObjectOfType<LevelManager>().GameContinued();
        } 
        else
        {
            Advertisement.Show(myPlacementId);
        }
        
    }

    // Implement IUnityAdsListener interface methods:
    public void OnUnityAdsDidFinish(string placementId, ShowResult showResult)
    {
        if(placementId == "rewardedVideo")
        {
            //_adButton.SetActive(false);
            ad_loaded = false;
            // Define conditional logic for each ad completion status:
            if (showResult == ShowResult.Finished)
            {
                FindObjectOfType<LevelManager>().GameContinued();
                Debug.Log("reward");
                // Reward the user for watching the ad to completion.
                //TODO:continue bastıktan sonra 1 hak ver.

            }
            else if (showResult == ShowResult.Skipped)
            {
                // Do not reward the user for skipping the ad.
                Debug.Log("skipped");
                FindObjectOfType<LevelManager>().GameEnded();
            }
            else if (showResult == ShowResult.Failed)
            {
                FindObjectOfType<LevelManager>().GameEnded();
                Debug.Log("The ad did not finish due to an error.");
            }
            else
            {
                FindObjectOfType<LevelManager>().GameEnded();
            }
            //SetAdButtonOff()
        }

    }

    public void OnUnityAdsReady(string placementId)
    {
        // If the ready Placement is rewarded, show the ad:
        if (placementId == myPlacementId)
        {
            ad_loaded = true;
            // Optional actions to take when the placement becomes ready(For example, enable the rewarded ads button)
            // TODO: Enable continue button ?
            Debug.Log("ad ready");
            //_adButton.SetActive(true);
        }
        else
        {
            Debug.Log("int ad ready");
        }

    }

    public bool GetAdStatus() { return ad_loaded; }
    public void OnUnityAdsDidError(string message)
    {
        // Log the error.
    }

    public void OnUnityAdsDidStart(string placementId)
    {
        // Optional actions to take when the end-users triggers an ad.
    }

    // When the object that subscribes to ad events is destroyed, remove the listener:

    public void OnDestroy()
    {
        Advertisement.RemoveListener(this);
    }
}
