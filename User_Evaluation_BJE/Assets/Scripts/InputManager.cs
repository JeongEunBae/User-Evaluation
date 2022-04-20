using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InputManager : MonoBehaviour
{
    /* Emotion Button */
    public GameObject[] emotionButton;
    public ProgramManager programManager;

    /* Error Button */
    public GameObject errorButton;

    /* Record */
    public int currentSelectedEmotion; // 사용자가 선택한 감정 기록
    public int replayTime; // replay를 한 횟수
    public float taskTime; // 하나의 감정을 평가하는 시간 (비디오 영상 시간 포함)
    public int error; // 애니메이션 Error (0, 1[에러])

    private void Start()
    {
        currentSelectedEmotion = 0;
        replayTime = 0;
        taskTime = 0f;
        error = 0;
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
        
        // Task Time
        taskTime += Time.deltaTime;
    }

    //  No.1 ~ No.7 버튼 선택
    public void OnClickEmotionButton()
    {
        for (int index = 0; index < emotionButton.Length; index++)
        {
            if(emotionButton[index] == EventSystem.current.currentSelectedGameObject) EmotionButtonPressed(index + 1);
        }

        // Highlighted Color 원래 색상으로 초기화 
        EventSystem.current.SetSelectedGameObject(null);
    }

    public void OnClickReplayButton()
    {
        FuncButtonPressed("Replay");

        // Highlighted Color 원래 색상으로 초기화
        EventSystem.current.SetSelectedGameObject(null);
    }

    public void OnClickNextButton()
    {
        FuncButtonPressed("Next");

        // Highlighted Color 원래 색상으로 초기화
        EventSystem.current.SetSelectedGameObject(null);
    }

    // Error 버튼 클릭 시 error = 1, 한번 더 클릭 시(error=1일때) error = 0
    public void onClickErrorButton()
    {
        if(error == 0)
        {
            error = 1;
            errorButton.transform.Find("Select").transform.Find("Check").GetComponent<RawImage>().color = new Color(1f, 0.0f, 0.0f);
        }
        else
        {
            error = 0;
            errorButton.transform.Find("Select").transform.Find("Check").GetComponent<RawImage>().color = new Color(0.6f, 0.6f, 0.6f);
        }
        

        // Highlighted Color 원래 색상으로 초기화
        EventSystem.current.SetSelectedGameObject(null);
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

    // Emotion 버튼 초기화
    public void EmotionButtonUnChecked()
    {
        foreach (var button in emotionButton)
        {
            button.transform.Find("Select").transform.Find("Check").GetComponent<RawImage>().color = new Color(0.6f, 0.6f, 0.6f);
        }

    }

    public void FuncButtonPressed(string funcCode)
    {
        if (funcCode == "Next")
        {
            // 동영상이 play 되고 있을 경우에는 넘어갈 수 없음
            if (!programManager.taskPanel.GetComponent<VideoHandler>().videoPlayer.isPlaying)
            {
                // error가 없고 선택한 감정이 없을 경우에는 넘어갈 수 없음
                if (error == 0 && currentSelectedEmotion == 0) return;

                EmotionButtonUnChecked();
                programManager.NextVideo();

                replayTime = 0;
                taskTime = 0;
                error = 0;
                currentSelectedEmotion = 0;

                // errorButton 초기화
                errorButton.transform.Find("Select").transform.Find("Check").GetComponent<RawImage>().color = new Color(0.6f, 0.6f, 0.6f);
            }
        }
        else if (funcCode == "Replay")
        {
            replayTime++;
            programManager.PlayVideo();
        }
    }

}
