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

            // ���� ����Ʈ ��������
            GetVideoList();
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

        /*string demography_path = dir + "\\" + "Demography";
        using (FileStream fs = new FileStream(demography_path, FileMode.OpenOrCreate))
        {
            using (StreamWriter st = new StreamWriter(fs, System.Text.Encoding.UTF8))
            {
                st.WriteLine("�̸� ," + p_Name);
                st.WriteLine("Exp_Num ," + currentTaskNum);
                st.WriteLine("������۽ð� ," + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss tt"));

            }
        }*/

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

            // ������ ���� ��Ȳ OverWrite
            string tasklist_path = p_Dir + "\\" + "Tasklist";
            using (FileStream fileStream = new FileStream(tasklist_path, FileMode.Open, FileAccess.ReadWrite))
            {
                using (StreamWriter streamWriter = new StreamWriter(fileStream, System.Text.Encoding.UTF8))
                {

                    streamWriter.WriteLine("������ Name :" + p_Name);
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
