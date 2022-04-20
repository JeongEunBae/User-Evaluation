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

    // 비디오 준비
    IEnumerator PrepareVideo()
    {
        
        videoPlayer.Prepare();

        while (!videoPlayer.isPrepared)
        {
            yield return null;
        }

        videoScreen.texture = videoPlayer.texture;
    }

    // 비디오 재생
    public void StartVideo(string clipName)
    {

        // 비디오 클립 설정
        videoPlayer.clip = Resources.Load<VideoClip>("Clips/" + clipName);

        if (videoPlayer != null && videoPlayer.isPrepared)
        {
            videoPlayer.Play();
        }
        
    }

}
