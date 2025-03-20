using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonEvent : EventTrigger
{
    public UIInteractions uiInt;

    private void Start()
    {
        uiInt = GameObject.Find("UIManager").GetComponent<UIInteractions>(); 
    }
    public override void OnPointerDown(PointerEventData eventData)
    {
        if (LevelManager.levelPlaying) return;
        int btnIdx = 0;
        base.OnPointerDown(eventData);
        switch(this.name)
        {
            case "Button1":
                btnIdx = 0;
                StartCoroutine(uiInt.ButtonAnimation(0));
                break;
            case "Button2":
                btnIdx = 1;
                StartCoroutine(uiInt.ButtonAnimation(1));
                break;
            case "Button3":
                btnIdx = 2;
                StartCoroutine(uiInt.ButtonAnimation(2));
                break;
            case "Button4":
                btnIdx = 3;
                StartCoroutine(uiInt.ButtonAnimation(3));
                break;
        }

        if(uiInt.gameplayActive)
        FindObjectOfType<LevelManager>().CheckUserInput(btnIdx);
    }
}
