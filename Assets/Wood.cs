using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Wood : MonoBehaviour
{

    public Color EmptyColour = Color.gray;
    public Color FullColour = Color.green;

    public int MaxWood=10;

    int mWoodCount = 0;

    public int  WoodCount {
        get {
            return mWoodCount;
        }
        set {
            mWoodCount = Mathf.Clamp(value, 0, MaxWood); //Keep woodcount sane
            Text = string.Format("I have {0} wood", mWoodCount); //Display
            Colour = Color.Lerp(InitialColour, FullColour, (float)mWoodCount / MaxWood);
        }
    }


    MeshRenderer mMR;


    //Lazy Getter/Setter
    Text mDebugText;
    public string Text {
        set {
            if (mDebugText == null) {
                mDebugText = GetComponentInChildren<Text>();   //Get Text Component if it has one
            }
            Debug.Assert(mDebugText != null, "No Text TextComponent found");
            mDebugText.text=value;
        }
        get {
            if (mDebugText == null) {
                mDebugText = GetComponentInChildren<Text>();   //Get Text Component if it has one
            }
            Debug.Assert(mDebugText != null, "No Text TextComponent found");
            return mDebugText.text;
        }
    }

    public Color Colour {
        set {
            if (mMR == null) {
                mMR = GetComponent<MeshRenderer>();   //Get MR Component if it has one
            }
            Debug.Assert(mMR != null, "No MeshRenderer found");
            mMR.material.color = value;
        }
        get {
            if (mMR == null) {
                mMR = GetComponent<MeshRenderer>();   //Get MR Component if it has one
            }
            Debug.Assert(mMR != null, "No MeshRenderer found");
            return  mMR.material.color;
        }
    }

    Color InitialColour;
    // Start is called before the first frame update
    void Start()
    {
        InitialColour = Colour; //Get Original colour
        StartCoroutine(GrowWood(Random.Range(2.0f,5.0f)));
        WoodCount = 0; //Display it
    }

    private void Update() {
    }


    IEnumerator GrowWood(float tRate) {
        for(; ; ) {
            yield return new WaitForSeconds(tRate);
            WoodCount++;
        }
    }
}
