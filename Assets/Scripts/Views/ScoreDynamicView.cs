using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class ScoreDynamicView : MonoBehaviour {

    private Text myText;

    public float addTime = 0.5f;
    
    private float targetValue = 0;
    private float lastValue = 0;
    private float displayedValue = 0;
    private float animRemaining = 0;

    private int stylecounter = 0;

	void Start () {
        myText = GetComponent<Text>();
	}
	
	void Update ()
    {
        updateAnimation();

        updateDisplay();

		if(targetValue == Director.PlayerScore)
        {
            if(animRemaining <= 0)
                stylecounter = 0;
            return;
        }

        stylecounter = Mathf.Clamp(stylecounter + 1, 0, 10);

        animRemaining = addTime + addTime * (stylecounter * stylecounter / 50f);
        targetValue = Director.PlayerScore;
	}

    private void updateAnimation()
    {
        if(animRemaining <= 0)
        {
            displayedValue = targetValue;
            return;
        }

        animRemaining -= Time.deltaTime;

        animRemaining = Mathf.Clamp(animRemaining, 0, float.MaxValue);
        
        displayedValue = Mathf.Lerp(displayedValue, targetValue, animRemaining);
    }

    private void updateDisplay()
    {
        myText.text = "Score: " + (int)(displayedValue);
    }
}
