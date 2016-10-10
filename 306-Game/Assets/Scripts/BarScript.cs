using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class BarScript : MonoBehaviour {

    private float fillAmount;

    [SerializeField]
    private float lerpSpeed;

    [SerializeField]
    private Image content;

    public float MaxValue { get; set; }

    public float Value
    {
        set
        {
            fillAmount = Map(value, 0, MaxValue, 0, 1);
        }
    }


	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        HandleBar();
	}


    //Set the amount of the bar shown to be the amount represented by fillAmount
    void HandleBar()
    {
        if (fillAmount != content.fillAmount)
        {
            content.fillAmount = Mathf.Lerp(content.fillAmount,fillAmount,Time.deltaTime*lerpSpeed);
        }
    }

    float Map(float value, float inMin, float inMax, float outMin, float outMax)
    {
        return ((value - inMin) * outMax-outMin) / ((inMax-inMin) + outMin);
    }
}
