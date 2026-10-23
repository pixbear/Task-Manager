using System;
using System.Collections;
using System.IO;
using System.Threading.Tasks;
using PB.BUTTON;
using PB.MANAGER;
using PB.SYSTEM;
using TMPro;
using UnityEngine;

public class TaskManager : PMonoSingleton<TaskManager>
{
    [SerializeField] PButton btnToToday;
    [SerializeField] PButton btnPrevWeek;
    [SerializeField] PButton btnNextWeek;
    [SerializeField] PButton btnAddNewTask;
    [SerializeField] Popup_Option optionPopup;
    [SerializeField] TextMeshProUGUI tmpCurrentTime;
    [SerializeField] TextMeshProUGUI tmpShowedMonth;
    [SerializeField] TextMeshProUGUI tmpShowedYear;
    [SerializeField] Week[] weeks;
    private DateTime currentWeekStartDate;
    private TaskList taskList = new();

    protected override void Awake()
    {
        base.Awake();
        btnPrevWeek.onClick.AddListener(OnPrevWeek);
        btnNextWeek.onClick.AddListener(OnNextWeek);
        btnAddNewTask.onClick.AddListener(OnAddNewTask);
        btnToToday.onClick.AddListener(() => LoadWeek(DateTime.Now));
    }

    private async Awaitable Start()
    {
        StartCoroutine(UpdateTimeEveryMinute());
        // yield return LoadTasksCoroutine();
        await LoadTaskDataFromFirebase();
        LoadWeek(DateTime.Now);
    }

    private IEnumerator UpdateTimeEveryMinute()
    {
        while (true)
        {
            // ex 2025.11.28 금 - 12:30
            tmpCurrentTime.text = DateTime.Now.ToString("yyyy.MM.dd ddd - HH:mm");
            yield return new WaitForSecondsRealtime(60);
        }
    }

    private void OnApplicationQuit()
    {
        SaveData();
    }

    private void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            SaveData();
        }
        else
        {
            ReloadData();
        }
    }

    public async void SaveData()
    {
        await SaveTaskDataToFirebase();
    }

    public async void ReloadData()
    {
        await LoadTaskDataFromFirebase();
        LoadWeek(currentWeekStartDate);
    }

    private void LoadWeek(DateTime date)
    {
        // start from monday
        // ex 7월
        tmpShowedMonth.text = date.ToString("MMMM");
        tmpShowedYear.text = date.Year.ToString();
        currentWeekStartDate = date.AddDays(-(int)date.DayOfWeek + 1);
        for (int i = 0; i < weeks.Length; i++)
        {
            var weekDate = currentWeekStartDate.AddDays(i);
            var tasks = taskList.GetTasksByDate(weekDate);
            weeks[i].Init(tasks, weekDate);
        }
    }

    private void OnPrevWeek()
    {
        var prevWeek = currentWeekStartDate.AddDays(-7);
        LoadWeek(prevWeek);
    }

    private void OnNextWeek()
    {
        var nextWeek = currentWeekStartDate.AddDays(7);
        LoadWeek(nextWeek);
    }

    private void OnAddNewTask()
    {
        optionPopup.Show();
        optionPopup.Init(new TaskData(), true);
    }

    public void SelectTask(TaskData data)
    {
        optionPopup.Show();
        optionPopup.Init(data, false);
    }

    public void SaveTask(TaskData data)
    {
        taskList.SaveTask(data);
        LoadWeek(currentWeekStartDate);
    }

    public void DeleteTask(TaskData data)
    {
        taskList.RemoveTask(data);
        LoadWeek(currentWeekStartDate);
    }

    public void CompleteTask(TaskData data)
    {
        taskList.CompleteTask(data);
        LoadWeek(currentWeekStartDate);
    }

    public async Awaitable SaveTaskDataToFirebase()
    {
        await PFirebaseManager.Instance.SaveData("tasks", taskList);
    }

    public async Awaitable LoadTaskDataFromFirebase()
    {
        await PFirebaseManager.Instance.LoadData<TaskList>("tasks", loadedData =>
        {
            if (loadedData != null)
            {
                taskList = loadedData;
            }
        });
    }
}
