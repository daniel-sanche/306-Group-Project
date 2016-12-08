using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class DayNightSystem : MonoBehaviour {

    [SerializeField]
    public Stat day;
    [SerializeField]
    public Stat night;
    [SerializeField]
    Image overLay;
    [SerializeField]
    float dayLength;
    [SerializeField]
    int numDays;
    [SerializeField]
    int daysLeft;

    bool isDay;
    float timeLimit;

	private Text leftText;

    //AudioSource
    public AudioClip[] dayClips;
    public AudioClip[] nightClips;
    public AudioClip ending;
    public AudioSource music;



    // Use this for initialization
    void Start()
    {
        isDay = true;
        timeLimit = numDays * 2 * dayLength;
        InvokeRepeating("UpdateTime", 1, 1);
        InvokeRepeating("ChangeTimeType", 0, dayLength);
        daysLeft = numDays;
		leftText = GetComponentInChildren<Text> ();
		leftText.text = daysLeft.ToString();
        
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
    }


    // Changes the games time from day to night or vice versa
    void ChangeTimeType()
    {
        if (isDay)
        {
            isDay = false;
            // Makes the overlay disappear so that everything appears lighter
            overLay.GetComponent<CanvasGroup>().alpha = 0;
            //Changes from keeping track of how much night is left to how much day is left
			night.bar.GetComponent<CanvasGroup>().alpha = 0;
            day.bar.GetComponent<CanvasGroup>().alpha = 1;
			leftText.text = daysLeft.ToString();
			if (daysLeft == 0)
			{
                music.clip = ending;
                music.loop = false;
                music.Play();
                SceneManager.LoadScene(2);
			}
        }
        else
        {
            isDay = true;
            // Makes the overlay appear so that everything appears darker
            overLay.GetComponent<CanvasGroup>().alpha = 1;
            //Changes from keeping track of how much day is left to how much night is left
            day.bar.GetComponent<CanvasGroup>().alpha = 0;
            night.bar.GetComponent<CanvasGroup>().alpha = 1;
            daysLeft = daysLeft - 1;
        }
        UpdateNPCs();
        UpdateMusic();
    }

    // counts how long the day or night has lasted
    // night.MaxVal - night.CurrentVal is how close the night is
    // day.MaxVal - day.CurrentVal is how close the night is
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

    //tells NPCs to switch between friendly and enemy forms
    void UpdateNPCs()
    {
        GameObject[] objects;
        objects = GameObject.FindGameObjectsWithTag("NPC");

        for(int i =0; i< objects.Length; i++)
        {
            objects[i].SendMessage("ChangeForm");
        }

    }

    //Simple switch statement chooses a sample to loop based on whether its day/night and how many days are left
    //Each loop should be precisely 16 sec long
    void UpdateMusic()
    {
        switch (daysLeft)
        {
            case 5:
                if (!isDay) music.clip = dayClips[0];
                else music.clip = nightClips[0];
                music.Play();
                break;
            case 4:
                if (!isDay) music.clip = dayClips[1];
                else music.clip = nightClips[0];
                music.Play();
                break;
            case 3:
                if (!isDay) music.clip = dayClips[2];
                else music.clip = nightClips[1];
                music.Play();
                break;
            case 2:
                if (!isDay) music.clip = dayClips[3];
                else music.clip = nightClips[2];
                music.Play();
                break;
            case 1:
                if (!isDay) music.clip = dayClips[4];
                else music.clip = nightClips[3];
                music.Play();
                break;
            case 0:
                music.clip = nightClips[4];
                music.Play();
                break;
            default:
                break;
        }
    }
}
