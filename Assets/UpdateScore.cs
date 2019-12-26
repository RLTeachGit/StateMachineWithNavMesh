using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpdateScore : MonoBehaviour
{

    int Score = 0;

    Text mText;

    // Start is called before the first frame update
    void Start()
    {
        mText = GetComponent<Text>();   //Get Text
        StartCoroutine(UpdateCoroutine(2.0f)); //Start CoRoutine which will keep going
    }

    IEnumerator UpdateCoroutine(float TimeOut) {

        while(true) {
            mText.text = string.Format("Score:{0}", Score);
            yield return new WaitForSeconds(TimeOut); //Wait and let other stuff run
            Score+=10;
        }
    }
}
