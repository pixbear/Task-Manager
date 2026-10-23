using System;
using PB.BUTTON;
using PB.UI;
using TMPro;
using UnityEngine;

public class Popup_Option : PPopupBase
{
    [SerializeField] TMP_InputField inputTask;
    [SerializeField] PCalendarView calendar;
    [SerializeField] PTimeSelector timeSelector;
    [SerializeField] PButton btnSave;
    [SerializeField] PButton btnDelete;
    [SerializeField] PButton btnComplete;
    [SerializeField] PButton btnClose;

    private TaskData task;
    private TextMeshProUGUI tmpCompletedBtnText;
    private bool isNewTask = false;

    private void Awake()
    {
        tmpCompletedBtnText = btnComplete.GetComponentInChildren<TextMeshProUGUI>();
        btnSave.onClick.AddListener(OnSave);
        btnDelete.onClick.AddListener(OnDelete);
        btnComplete.onClick.AddListener(OnComplete);
        btnClose.onClick.AddListener(() =>
        {
            btnClose.Interactable = false;
            Close();
        });
    }

    protected override void OnShow()
    {
        base.OnShow();
        btnClose.Interactable = true;
    }

    public void Init(TaskData data, bool isNew)
    {
        isNewTask = isNew;
        task = data;
        inputTask.text = data.routine;
        var time = data.GetDateTime();
        calendar.Init(time.Year, time.Month, time.Day);
        timeSelector.Init(time.Hour, time.Minute);
        
        var isCompleted = data.isCompleted;
        btnDelete.SetInteractable(!isNew);
        btnComplete.SetInteractable(!isCompleted);
        tmpCompletedBtnText.text = isCompleted ? "완료됨" : "완료";
    }

    private void OnSave()
    {
        if (string.IsNullOrEmpty(inputTask.text)) return;

        task.routine = inputTask.text;
        task.SetDateTime(new DateTime(
            calendar.SelectedYear,
            calendar.SelectedMonth,
            calendar.SelectedDay,
            timeSelector.SelectedHour,
            timeSelector.SelectedMinute,
            0));

        TaskManager.Instance.SaveTask(task);
        Close();
    }

    private void OnDelete()
    {
        TaskManager.Instance.DeleteTask(task);
        Close();
    }

    private void OnComplete()
    {
        if (isNewTask)
        {
            task.isCompleted = true;
            OnSave();
            return;
        }

        TaskManager.Instance.CompleteTask(task);
        Close();
    }
}
