using System;
using System.Collections.Generic;
using System.Linq;
using PB.SYSTEM;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[Serializable]
public class TaskData
{
    public string routine;
    public bool isCompleted;
    public string time;

    public TaskData()
    {
        routine = string.Empty;
        isCompleted = false;
        time = DateTime.Now.ToString("O");
    }

    public DateTime GetDateTime()
    {
        return DateTime.Parse(time);
    }

    public void SetDateTime(DateTime dateTime)
    {
        time = dateTime.ToString("O");
    }
}

[Serializable]
public class TaskList
{
    public List<TaskData> tasks = new();

    public void SaveTask(TaskData data)
    {
        if (tasks.Contains(data))
        {
            int index = tasks.IndexOf(data);
            tasks[index] = data;
        }
        else
        {
            tasks.Add(data);
        }
    }

    public void RemoveTask(TaskData data)
    {
        if (tasks.Contains(data) == false) return;
        tasks.Remove(data);
    }

    public void CompleteTask(TaskData data)
    {
        if (tasks.Contains(data) == false)
        {
            tasks.Add(data);
        }

        int index = tasks.IndexOf(data);
        tasks[index].isCompleted = true;
    }

    public List<TaskData> GetTasksByDate(DateTime date)
    {
        return tasks
            .Where(task => task.GetDateTime().Date == date.Date)
            .OrderBy(task => task.GetDateTime())
            .ToList();
    }
}

public class Task : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] TextMeshProUGUI tmpTitle;
    [SerializeField] TextMeshProUGUI tmpTime;
    [SerializeField] Color defaultColor;
    [SerializeField] Color completedColor;
    [SerializeField] Color timeOverColor;
    [SerializeField] Color inProgressColor;

    private Image imgBG;
    private TaskData taskData;

    private void Awake()
    {
        imgBG = GetComponent<Image>();
    }

    public void Init(TaskData data)
    {
        taskData = data;
        tmpTitle.text = data.routine;
        var time = data.GetDateTime();
        var minuteString = time.Minute.ToString("D2");
        var hourString = time.Hour.ToString("D2");
        tmpTime.text = hourString + ":" + minuteString;

        if (taskData.isCompleted)
        {
            imgBG.color = completedColor;
        }
        else if (DateTime.Now < time && DateTime.Now >= time.AddHours(-1))
        {
            imgBG.color = inProgressColor;
        }
        else if (time < DateTime.Now)
        {
            imgBG.color = timeOverColor;
        }
        else
        {
            imgBG.color = defaultColor;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        TaskManager.Instance.SelectTask(taskData);
    }
}

