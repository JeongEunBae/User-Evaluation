using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;
using UnityEngine.EventSystems;

public class ProgramManager : MonoBehaviour
{
    /* UI Panel */
    public GameObject informationPanel;
    public GameObject taskListPanel;
    public GameObject taskPanel;

    /* Information Panel */
    public InputField p_Name;
    public InputField p_ID;
    public Text errorText;

    /* TaskList Panel */
    private GameObject[] taskList;
    private int currentTaskNum;

    /* Participant Information */
    private string p_Dir;

    /* Video */
    private string[] videoList;
    private int videoIndex;

    private void Start()
    {
        currentTaskNum = 1;
    }

    // 피험자 이름, ID 입력받고 실험 시작 버튼 클릭 후 Task List Panel로 연결 
    public void OnShowTaskList()
    {
        if (p_Name.text != "" && p_ID.text != "")
        {
            errorText.text = "";

            InitProgram(p_Name.text, p_ID.text);

            informationPanel.SetActive(false);
            taskListPanel.SetActive(true);
            ViewTaskList();

            // 비디오 리스트 가져오기
            GetVideoList();
        }
        else
        {
            if (p_Name.text == "" && p_ID.text == "") errorText.text = "피험자 이름과 실험 ID가 입력되지 않았습니다.";
            else if (p_Name.text == "") errorText.text = "피험자 이름이 입력되지 않았습니다.";
            else errorText.text = "피험자 실험 ID가 입력되지 않았습니다.";
        }
    }

    // 피험자 데이터 저장할 폴더 생성 및 데이터 초기값 설정
    private void InitProgram(string p_Name, string p_ID)
    {
        string path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        p_Dir = path + "\\Data\\" + p_ID;
        DirectoryInfo di = new DirectoryInfo(p_Dir);
        if (di.Exists == false) di.Create();

        /*string demography_path = dir + "\\" + "Demography";
        using (FileStream fs = new FileStream(demography_path, FileMode.OpenOrCreate))
        {
            using (StreamWriter st = new StreamWriter(fs, System.Text.Encoding.UTF8))
            {
                st.WriteLine("이름 ," + p_Name);
                st.WriteLine("Exp_Num ," + currentTaskNum);
                st.WriteLine("실험시작시각 ," + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss tt"));

            }
        }*/

        // 피험자 실험 Task 현재 상황 기록
        string tasklist_path = p_Dir + "\\" + "Tasklist";
        if (!File.Exists(tasklist_path))
        {
            using (FileStream fileStream = new FileStream(tasklist_path, FileMode.OpenOrCreate))
            {
                using (StreamWriter streamWriter = new StreamWriter(fileStream, System.Text.Encoding.UTF8))
                {

                    streamWriter.WriteLine("피험자 Name :" + p_Name);
                    streamWriter.WriteLine("Current Exp_Task_Num :" + currentTaskNum);
                }
            }
       }
    }

    // TaskList 불러오기 (이미 수행한 Task는 비활성화)
    private void ViewTaskList()
    {
        // 피험자 실험 Task 상황 불러오기
        string tasklist_path = p_Dir + "\\" + "Tasklist";
        using (FileStream fileStream = new FileStream(tasklist_path, FileMode.Open, FileAccess.Read))
        {
            using (StreamReader streamReader = new StreamReader(tasklist_path, false))
            {
                string file_text = streamReader.ReadToEnd();
                currentTaskNum = Int32.Parse(file_text.Split(":")[2]);
            }
        }

        taskList = GameObject.FindGameObjectsWithTag("Task");

        for (int index = 1; index <= taskList.Length; index++)
        {
            if (currentTaskNum > index) taskList[index - 1].GetComponent<Button>().interactable = false;
        }
    }

    // 선택한 Task Panel 불러오기
    public void OnShowTask()
    {
        int pressTask = Int32.Parse(EventSystem.current.currentSelectedGameObject.transform.Find("Text").GetComponent<Text>().text);
        
        // 진행할 Task 번호와 클린한 Task Button이 같을 경우에만 Task Panel 표시
        if(pressTask == currentTaskNum)
        {
            taskListPanel.SetActive(false);
            taskPanel.SetActive(true);
            
            videoIndex = (currentTaskNum - 1) * 5;
            PlayVideo();
        }
    }

    private void GetVideoList()
    {
        string videoList_path = Application.dataPath + "/Resources/Clips/cinema_list.csv";
        if (File.Exists(videoList_path))
        {
            using (FileStream fileStream = new FileStream(videoList_path, FileMode.Open, FileAccess.Read))
            {
                using (StreamReader streamReader = new StreamReader(videoList_path, false))
                {
                    string cinema_list_text = streamReader.ReadToEnd();
                    Debug.Log(cinema_list_text);
                    videoList = cinema_list_text.Split('\n');
                }
            }
        }
        else Debug.LogError("Not found cinema_list.csv");
    } 

    public void PlayVideo()
    {
        taskPanel.GetComponent<VideoHandler>().StartVideo(videoList[videoIndex].Remove(videoList[videoIndex].Length - 1));
    }

    public void NextVideo()
    {
        if(videoIndex >= currentTaskNum * 5)
        {
            currentTaskNum++;

            // 피험자 실험 상황 OverWrite
            string tasklist_path = p_Dir + "\\" + "Tasklist";
            using (FileStream fileStream = new FileStream(tasklist_path, FileMode.Open, FileAccess.ReadWrite))
            {
                using (StreamWriter streamWriter = new StreamWriter(fileStream, System.Text.Encoding.UTF8))
                {

                    streamWriter.WriteLine("피험자 Name :" + p_Name);
                    streamWriter.WriteLine("Current Exp_Task_Num :" + currentTaskNum);
                }
            }

            taskPanel.SetActive(false);
            taskListPanel.SetActive(true);
            ViewTaskList();

            return;
        }            

        videoIndex++;
        PlayVideo();
    }
}
