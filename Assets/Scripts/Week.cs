using System;
using System.Collections.Generic;
using PB.SYSTEM;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Week : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI tmpDateTitle;
    [SerializeField] PObjectPooler taskPool;
    [SerializeField] Image imgFrame;
    private DateTime date;
    private Color originalTitleColor;
    private Color originalFrameColor;

    private void Awake()
    {
        originalTitleColor = tmpDateTitle.color;
        originalFrameColor = imgFrame.color;
    }

    private void CheckIsToday()
    {
        if (date.Date == DateTime.Now.Date)
        {
            imgFrame.color = PColor.Yellow;
            tmpDateTitle.color = PColor.Yellow;
        }
        else
        {
            tmpDateTitle.color = originalTitleColor;
            imgFrame.color = originalFrameColor;
        }
    }

    public void Init(List<TaskData> tasks, DateTime date)
    {
        this.date = date;
        tmpDateTitle.text = date.ToString("MM.dd ddd");

        ClearTasks();
        CheckIsToday();

        foreach (var task in tasks)
        {
            AddTask(task);
        }
    }

    private void AddTask(TaskData data)
    {
        var taskObj = taskPool.Get<Task>();
        taskObj.Init(data);
    }

    public void ClearTasks()
    {
        taskPool.ReleaseAll();
    }
}
