using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TimeManager : MonoBehaviour
{
    string lastEnter;
    private void Awake()
    {
        //마지막 접속 시간 불러오기
        lastEnter = PlayerPrefs.GetString("LastEnter", DateTime.UtcNow.ToString());
    }
    // Start is called before the first frame update
    void Start()
    {
        /*TimeSpan timestamp = DateTime.UtcNow;

        int past = PlayerPrefs.GetInt("sec",(int)timestamp.TotalSeconds);
        PlayerPrefs.SetInt("sec", (int)timestamp.TotalSeconds);
        Debug.Log((int)timestamp.TotalSeconds - past);*/
        TimeSpan restTime = DateTime.Parse(lastEnter) - DateTime.UtcNow;

        int restHour = (int)restTime.TotalHours;//접속 안 한 시간 - 식물에 악영향
        int restMinutes = (int)restTime.TotalMinutes;// 접속 안 한 분 - 보상 획득
        Debug.Log(DateTime.UtcNow);
        Debug.Log(restHour);
        Debug.Log(restMinutes);

    }

    //종료 전 현재 시간 접속 기능
    public void SaveTime()
    {
        PlayerPrefs.SetString("LastEnter", DateTime.UtcNow.ToString());
    }
}
