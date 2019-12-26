using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
using System.Linq;
using System;



public class Player : MonoBehaviour
{

    Text mDebugText;
    public string Text { //Using lazy setter/getter
        set {
            if (mDebugText == null) {
                mDebugText = GetComponentInChildren<Text>();   //Get Text Component if it has one
            }
            Debug.Assert(mDebugText != null, "No Text TextComponent found");
            mDebugText.text = value;
        }
        get {
            if (mDebugText == null) {
                mDebugText = GetComponentInChildren<Text>();   //Get Text Component if it has one
            }
            Debug.Assert(mDebugText != null, "No Text TextComponent found");
            return mDebugText.text;
        }
    }

    [SerializeField]
    float Radius = 50.0f;

    NavMeshAgent mAgent;

    int mMyWood = 0;

    enum State {
        PreStart
        ,Idle
        ,ScanForWood
        ,FoundWood
        ,WalkToWood
        ,GatherWood
    }


    State mCurrentState = State.PreStart;

    void   EnterState(State vState,object vData=null) { //object is any class, we cast it later to what we want
        if(vState!=mCurrentState) {
            mCurrentState = vState;
            Debug.LogFormat("Entering {0:s}", vState.ToString());
            switch (vState) {

                case State.FoundWood: //When Scan finds wood set AI destination
                    mAgent.destination = ((Wood)vData).transform.position; //Send to new location, note cast
                    EnterState(State.WalkToWood);
                    break;

                case State.GatherWood: {
                        int tTakeAmount = Mathf.Min(10, ((Wood)vData).WoodCount); //Grab max 10 min what the pile has
                        mMyWood += tTakeAmount;
                        ((Wood)vData).WoodCount -= tTakeAmount; //Grab Wood
                        Text = string.Format("My Wood:{0}", mMyWood);   
                        EnterState(State.Idle); //Go back to Idle
                    }
                    break;
            }
        }
    }

    IEnumerator    DuringState() { //Run to process timed events inside a state
        for(; ; ) {
            Debug.LogFormat("InState {0:s}", mCurrentState.ToString());
            switch (mCurrentState) {
                case State.Idle: {
                        Wood tWood = ClosestWood(Radius); //Find wood close by
                        if (tWood != null) {
                            EnterState(State.FoundWood, tWood);
                        }
                    }
                    break;
            }
            yield return new WaitForSeconds(0.1f);  //Process evert 10th of second
        }
    }


    LayerMask mWoodMask;

    // Start is called before the first frame update
    void Start()
    {
        mAgent = gameObject.AddComponent<NavMeshAgent>();

        mWoodMask = 1<<LayerMask.NameToLayer("Wood");   //Make bitmask for Wood Layer, makes search faster
        EnterState(State.Idle);
        StartCoroutine(DuringState());
        
    }


    Wood ClosestWood(float tRadius) {
        List<Wood> tWoodFound = FindWood(tRadius);
        if(tWoodFound.Count>0) { //First one is closestnon zero pile
            return tWoodFound[0];
        }
        return null;
    }

    List<Wood> FindWood(float tRadius) {

        Vector3 tFrom = transform.position;
        tFrom.y = 0;
        Collider[] tColliders = Physics.OverlapSphere(transform.position, tRadius, mWoodMask); //Array of colliders
        List<Wood> tWoodFound = new List<Wood>(); //List of items
        foreach(Collider tCollider in tColliders) {
            Wood tWood = tCollider.GetComponent<Wood>();
            if(tWood!=null) {
                if(tWood.WoodCount>3) { //Only add piles with >3 wood to list, so low stock is ignored
                    tWoodFound.Add(tWood);  //Turn list of colliders into list of Wood found
                }
            }
        }

        tWoodFound.Sort((t1, t2) => {   //Sort Wood list by closest item & most wood
            Vector3 tFirst = tFrom - t1.transform.position;
            Vector3 tSecond = tFrom - t2.transform.position;
            tFirst.y = 0;
            tSecond.y = 0;
            if (t2.WoodCount == 0) return 1;
            float tDifference =tSecond.magnitude/(t2.WoodCount+1) -  tFirst.magnitude / (t1.WoodCount + 1); //Smallest distance or most wood
            return Math.Sign(tDifference);
        }
        );
        return tWoodFound; //List of wood found in reverse order
    }


    private void OnTriggerEnter(Collider other) { //When we get close to wood enter gather state
        Wood tWood = other.GetComponent<Wood>();
        if (tWood != null) {
            if(mCurrentState==State.WalkToWood) {
                EnterState(State.GatherWood,tWood);
            }
        }
    }
}
