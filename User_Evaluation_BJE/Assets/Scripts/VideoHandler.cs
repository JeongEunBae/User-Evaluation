using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class VideoHandler : MonoBehaviour
{
    public RawImage videoScreen;
    public VideoPlayer videoPlayer;


    void Start()
    {
        StartCoroutine(PrepareVideo());
    }

    // ���� �غ�
    IEnumerator PrepareVideo()
    {
        
        videoPlayer.Prepare();

        while (!videoPlayer.isPrepared)
        {
            yield return null;
        }

        videoScreen.texture = videoPlayer.texture;
    }

    // ���� ���
    public void StartVideo(string clipName)
    {

        // ���� Ŭ�� ����
        videoPlayer.clip = Resources.Load<VideoClip>("Clips/" + clipName);

        if (videoPlayer != null && videoPlayer.isPrepared)
        {
            videoPlayer.Play();
        }
        
    }

}
