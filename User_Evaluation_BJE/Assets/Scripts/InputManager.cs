using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InputManager : MonoBehaviour
{
    /* Emotion Button */
    public GameObject[] emotionButton;
    public int currentSelectedEmotion;
    public ProgramManager programManager;

    private void Start()
    {
        currentSelectedEmotion = 0;
    }

    private void Update()
    {

        // keyboard No.1 ~ No.7 입력
        if (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Keypad1)) EmotionButtonPressed(1);
        else if (Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Keypad2)) EmotionButtonPressed(2);
        else if (Input.GetKeyDown(KeyCode.Alpha3) || Input.GetKeyDown(KeyCode.Keypad3)) EmotionButtonPressed(3);
        else if (Input.GetKeyDown(KeyCode.Alpha4) || Input.GetKeyDown(KeyCode.Keypad4)) EmotionButtonPressed(4);
        else if (Input.GetKeyDown(KeyCode.Alpha5) || Input.GetKeyDown(KeyCode.Keypad5)) EmotionButtonPressed(5);
        else if (Input.GetKeyDown(KeyCode.Alpha6) || Input.GetKeyDown(KeyCode.Keypad6)) EmotionButtonPressed(6);
        else if (Input.GetKeyDown(KeyCode.Alpha7) || Input.GetKeyDown(KeyCode.Keypad7)) EmotionButtonPressed(7);

        // Next, Replay Function
        if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.Space)) FuncButtonPressed("Next");
        if(Input.GetKeyDown(KeyCode.R)) FuncButtonPressed("Replay");
        
    }

    //  No.1 ~ No.7 버튼 선택
    public void OnClickEmotionButton()
    {
        for (int index = 0; index < emotionButton.Length; index++)
        {
            if(emotionButton[index] == EventSystem.current.currentSelectedGameObject) EmotionButtonPressed(index + 1);
        }
            
    }

    // 선택된 Emotion 버튼 색 변경
    public void EmotionButtonPressed(int taskNum)
    {
        currentSelectedEmotion = taskNum;

        foreach (var button in emotionButton)
        {
            if(emotionButton[taskNum - 1].name == button.name) button.transform.Find("Select").transform.Find("Check").GetComponent<RawImage>().color = new Color(0.5f, 1f, 0f);
            else button.transform.Find("Select").transform.Find("Check").GetComponent<RawImage>().color = new Color(0.6f, 0.6f, 0.6f);
        }

    }

    public void FuncButtonPressed(string funcCode)
    {
        if(funcCode == "Next") programManager.NextVideo();
        else if(funcCode == "Replay") programManager.PlayVideo();
    }

}
