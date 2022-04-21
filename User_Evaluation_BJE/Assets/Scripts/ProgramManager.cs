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

    /* Task Panel */
    public Text progressText;
    public Slider progressSlider;

    /* Participant Information */
    private string p_Dir;

    /* Video */
    private string[] videoList;
    private int videoIndex;
    private int currentVideoNum;
    private int videoTaskNum;
    private string[] solutionList;

    /* Record */
    private List<int> answerSelectedEmotionList;
    private List<int> replayTimeList;
    private List<float> taskTimeList;
    private List<int> errorList;
    private int answerSuccessCount;
    private float totalTaskTime;

    private void Start()
    {
        currentTaskNum = 1;
        videoTaskNum = 406;

        answerSelectedEmotionList = new List<int>();
        replayTimeList = new List<int>();
        taskTimeList = new List<float>();
        errorList = new List<int>();
        answerSuccessCount = 0;
        totalTaskTime = 0.0f;
    }

    // ������ �̸�, ID �Է¹ް� ���� ���� ��ư Ŭ�� �� Task List Panel�� ���� 
    public void OnShowTaskList()
    {
        if (p_Name.text != "" && p_ID.text != "")
        {
            errorText.text = "";

            InitProgram(p_Name.text, p_ID.text);

            informationPanel.SetActive(false);
            taskListPanel.SetActive(true);
            ViewTaskList();

            // ���� ����Ʈ & ���� ����Ʈ ��������
            GetVideoAndSolutionList();
        }
        else
        {
            if (p_Name.text == "" && p_ID.text == "") errorText.text = "������ �̸��� ���� ID�� �Էµ��� �ʾҽ��ϴ�.";
            else if (p_Name.text == "") errorText.text = "������ �̸��� �Էµ��� �ʾҽ��ϴ�.";
            else errorText.text = "������ ���� ID�� �Էµ��� �ʾҽ��ϴ�.";
        }
    }

    // ������ ������ ������ ���� ���� �� ������ �ʱⰪ ����
    private void InitProgram(string p_Name, string p_ID)
    {
        string path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        p_Dir = path + "\\Data\\" + p_ID;
        DirectoryInfo di = new DirectoryInfo(p_Dir);
        if (di.Exists == false) di.Create();

        // ������ ���� Task ���� ��Ȳ ���
        string tasklist_path = p_Dir + "\\" + "Tasklist";
        if (!File.Exists(tasklist_path))
        {
            using (FileStream fileStream = new FileStream(tasklist_path, FileMode.OpenOrCreate))
            {
                using (StreamWriter streamWriter = new StreamWriter(fileStream, System.Text.Encoding.UTF8))
                {

                    streamWriter.WriteLine("������ Name :" + p_Name);
                    streamWriter.WriteLine("Current Exp_Task_Num :" + currentTaskNum);
                }
            }
       }
    }

    private void SaveStartDemograpy()
    {
        // ������ ���� ���� ��� (���� ����)
        string demography_path = p_Dir + "\\" + "Demography";

        FileMode demograpyFilemode;
        if (File.Exists(demography_path)) demograpyFilemode = FileMode.Append;
        else demograpyFilemode = FileMode.OpenOrCreate;

        using (FileStream fileStream = new FileStream(demography_path, demograpyFilemode))
        {
            using (StreamWriter streamWriter = new StreamWriter(fileStream, System.Text.Encoding.UTF8))
            {
                streamWriter.WriteLine("�̸� : " + p_Name.text);
                streamWriter.WriteLine("���� ID : " + p_ID.text);
                streamWriter.WriteLine("Exp_Num : " + currentTaskNum);
                streamWriter.WriteLine("������۽ð� : " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss tt"));

            }
        }
    }

    private void SaveEndDemograpy()
    {
        // ������ ���� ���� ��� (���� ��)
        string demography_path = p_Dir + "\\" + "Demography";
        using (FileStream fileStream = new FileStream(demography_path, FileMode.Append))
        {
            using (StreamWriter streamWriter = new StreamWriter(fileStream, System.Text.Encoding.UTF8))
            {
                streamWriter.WriteLine("��������ð� : " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss tt"));
                streamWriter.WriteLine("����ð� : " + totalTaskTime);
                streamWriter.WriteLine("����� : " + answerSuccessCount);
                streamWriter.WriteLine("");
            }
        }
    }

    // TaskList �ҷ����� (�̹� ������ Task�� ��Ȱ��ȭ)
    private void ViewTaskList()
    {
        // ������ ���� Task ��Ȳ �ҷ�����
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

    // ������ Task Panel �ҷ�����
    public void OnShowTask()
    {
        int pressTask = Int32.Parse(EventSystem.current.currentSelectedGameObject.transform.Find("Text").GetComponent<Text>().text);
        
        // ������ Task ��ȣ�� Ŭ���� Task Button�� ���� ��쿡�� Task Panel ǥ��
        if(pressTask == currentTaskNum)
        {
            SaveStartDemograpy();

            taskListPanel.SetActive(false);
            taskPanel.SetActive(true);

            videoIndex = (currentTaskNum - 1) * videoTaskNum;
            PlayVideo();

            currentVideoNum = 0;
            SetProgress();

            progressSlider.value = 0.0f;
        }
    }

    private void GetVideoAndSolutionList()
    {
        // ���� ����Ʈ ��������
        string videoList_path = Application.dataPath + "/Resources/Files/cinema_list.csv";
        if (File.Exists(videoList_path))
        {
            using (FileStream fileStream = new FileStream(videoList_path, FileMode.Open, FileAccess.Read))
            {
                using (StreamReader streamReader = new StreamReader(videoList_path, false))
                {
                    string cinema_list_text = streamReader.ReadToEnd();
                    videoList = cinema_list_text.Split('\n');
                }
            }
        }
        else Debug.LogError("Not found cinema_list.csv");

        // ���� ����Ʈ ��������
        string solutionList_path = Application.dataPath + "/Resources/Files/solution.csv";
        if (File.Exists(solutionList_path))
        {
            using (FileStream fileStream = new FileStream(solutionList_path, FileMode.Open, FileAccess.Read))
            {
                using (StreamReader streamReader = new StreamReader(solutionList_path, false))
                {
                    string solution_list_text = streamReader.ReadToEnd();
                    solutionList = solution_list_text.Split('\n');
                }
            }
        }
        else Debug.LogError("Not found solution.csv");
    } 

    public void PlayVideo()
    {
        taskPanel.GetComponent<VideoHandler>().StartVideo(videoList[videoIndex].Remove(videoList[videoIndex].Length - 1));
    }

    public void NextVideo()
    {

        RecordAnswer();

        videoIndex++;
        PlayVideo();

        SetProgress();

        if (videoIndex >= currentTaskNum * videoTaskNum)
        {

            SaveExpTaskAnswer();
            SaveEndDemograpy();

            currentTaskNum++;

            // ������ ���� ��Ȳ OverWrite
            string tasklist_path = p_Dir + "\\" + "Tasklist";
            using (FileStream fileStream = new FileStream(tasklist_path, FileMode.Open, FileAccess.ReadWrite))
            {
                using (StreamWriter streamWriter = new StreamWriter(fileStream, System.Text.Encoding.UTF8))
                {

                    streamWriter.WriteLine("������ Name :" + p_Name.text);
                    streamWriter.WriteLine("Current Exp_Task_Num :" + currentTaskNum);
                }
            }
            taskPanel.SetActive(false);
            taskListPanel.SetActive(true);
            ViewTaskList();
            ClearRecordAnswerList();
            return;
        }
    }

    // ������ ��� ���� ���
    private void SaveExpTaskAnswer(bool isError = false)
    {
        string answer_path = p_Dir + "\\" + "Data" + currentTaskNum + ".csv";
        if (isError) answer_path = p_Dir + "\\" + "Dataerr" + currentTaskNum + ".csv";
        using (FileStream fileStream = new FileStream(answer_path, FileMode.OpenOrCreate))
        {
            using (StreamWriter streamWriter = new StreamWriter(fileStream, System.Text.Encoding.UTF8))
            {
                answerSuccessCount = 0;
                totalTaskTime = 0;

                streamWriter.WriteLine("anime_index,answer,task_time,replay_time,error,count");

                int saveLength = answerSelectedEmotionList.Count;
                for (int index = 0; index < saveLength; index++)
                {
                    streamWriter.WriteLine(videoList[((currentTaskNum - 1) * videoTaskNum) + index].Remove(videoList[((currentTaskNum - 1) * videoTaskNum) + index].Length - 1) + "," + answerSelectedEmotionList[index] + "," + taskTimeList[index] + "," + replayTimeList[index] + "," + errorList[index]);
                    if (answerSelectedEmotionList[index] == Int32.Parse(solutionList[((currentTaskNum - 1) * videoTaskNum) + index].Remove(solutionList[((currentTaskNum - 1) * videoTaskNum) + index].Length - 1))) answerSuccessCount++;
                    totalTaskTime += taskTimeList[index];
                }
                streamWriter.WriteLine(" , , , , ," + answerSuccessCount);
            }
        }
    }

    private void OnApplicationQuit()
    {
        SaveExpTaskAnswer(true);
    }

    private void RecordAnswer()
    {
        // ���� ��� 
        answerSelectedEmotionList.Add(taskPanel.GetComponent<InputManager>().currentSelectedEmotion); // ����ڰ� ������ Emotion
        replayTimeList.Add(taskPanel.GetComponent<InputManager>().replayTime); // replay�� �� Ƚ��
        taskTimeList.Add(taskPanel.GetComponent<InputManager>().taskTime); // �ϳ��� ������ ���ϴ� �ð� (���� ���� �ð� ����)
        errorList.Add(taskPanel.GetComponent<InputManager>().error); // �ִϸ��̼� ���� 0(����), 1(����)
    }

    private void ClearRecordAnswerList()
    {
        // List �ʱ�ȭ
        answerSelectedEmotionList.Clear();
        replayTimeList.Clear();
        taskTimeList.Clear();
        errorList.Clear();
    }

    public void SetProgress()
    {
        progressText.text = (++currentVideoNum) + " / " + videoTaskNum;

    }

    private void FixedUpdate()
    {
        if(taskPanel.GetComponent<VideoHandler>().videoPlayer.isPlaying)
        {
            // ���� ���� ���� 
            progressSlider.value = Convert.ToSingle(Math.Round(taskPanel.GetComponent<VideoHandler>().videoPlayer.time, 3) / Math.Round(taskPanel.GetComponent<VideoHandler>().videoPlayer.clip.length, 3));
        }
        else progressSlider.value = 1.0f;
    }
}
