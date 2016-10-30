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

    // Use this for initialization
    void Start()
    {
        isDay = true;
        timeLimit = numDays * 2 * dayLength;
        InvokeRepeating("ChangeTimeType", 0, dayLength);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void Awake()
    {
        day.Initialize();
        night.Initialize();
    }

    void ChangeTimeType()
    {
        if (isDay)
        {
            isDay = false;
            overLay.GetComponent<CanvasGroup>().alpha = 0;
        }
        else
        {
            isDay = true;
            overLay.GetComponent<CanvasGroup>().alpha = 1;
        }
    }
}
