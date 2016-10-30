using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class DayNightSystem : MonoBehaviour {

    [SerializeField]
    Stat day;
    [SerializeField]
    Stat night;
    [SerializeField]
    Image overLay;
    [SerializeField]
    float dayLength;
    [SerializeField]
    int numDays;

    bool isDay;
    float timeLimit;
    float currTime;


    // Use this for initialization
    void Start()
    {
        isDay = true;
        timeLimit = numDays * 2 * dayLength;
        InvokeRepeating("UpdateTime", 1, 1);
        InvokeRepeating("ChangeTimeType", 0, dayLength);
    }

    // Update is called once per frame
    void Update()
    {
    }

    void Awake()
    {
        day.MaxVal = dayLength;
        night.MaxVal = dayLength;
        day.Initialize();
        night.Initialize();
        currTime = 0;
    }

    void ChangeTimeType()
    {
        if (isDay)
        {
            isDay = false;
            overLay.GetComponent<CanvasGroup>().alpha = 0;
            night.Bar.GetComponent<CanvasGroup>().alpha = 0;
            day.Bar.GetComponent<CanvasGroup>().alpha = 1;
        }
        else
        {
            isDay = true;
            overLay.GetComponent<CanvasGroup>().alpha = 1;
            day.Bar.GetComponent<CanvasGroup>().alpha = 0;
            night.Bar.GetComponent<CanvasGroup>().alpha = 1;
        }
    }

    void UpdateTime()
    {
        if (isDay)
        {
            night.CurrentVal++;
            day.CurrentVal = 0;
        }
        else
        {
            day.CurrentVal++;
            night.CurrentVal = 0;
        }
    }
}
