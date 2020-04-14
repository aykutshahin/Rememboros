using System.Collections; 
using System.Collections.Generic;
using UnityEngine;

public class Timer : MonoBehaviour
{
    //name of the animaton to be used with the timer 
    public List<string> timerName;
    //how many frames there is for the animation to end
    private List<int> currentFrame;
    //total frames of the animation converted from animationTime
    private List<int> totalFrame;  
    //total time of the animaton set in the inspector
    public List<float>  animationTime;
    //total frames of the cooldown converted from cooldownTime 
    //cooldownframe < 0 means it is available >= 0 means it's not
    private List<int> cooldownFrame;
    // total time of the cooldown set in the inspector
    public List<float>  cooldownTime;
    //number of timers

    public Timer()
    {
        timerName = new List<string>();
        currentFrame = new List<int>();
        totalFrame = new List<int>();
        animationTime = new List<float>();
        cooldownFrame = new List<int>();
        cooldownTime = new List<float>();
    }
    //Basically retırns given parameter time / fixed.DeltaTime which is 0.02f
    public int ReturnFrameCount(float time)
    {
        float frames = time / Time.fixedDeltaTime;
        int roundedFrames = Mathf.RoundToInt(frames);

        // if float is same as int like 10.0f for 10 then return 10
        if (Mathf.Approximately(frames, roundedFrames))
        {
            return roundedFrames;
        }
        //if not then return the least greater integer
        return Mathf.CeilToInt(frames);
    }

    // returns a value between 0 and 1 according to passed frames according to the Name
    public float ReturnNormalizedTime(string Name)
    { 
        for(int i = 0; i < timerName.Count; i++)
        {
            if(System.String.Equals(timerName[i], Name))
            {
                return (((float)(totalFrame[i] - currentFrame[i])) / totalFrame[i]);
            }
        } 
        //for the ide to accept the method
        return 1f;
    } 

    public void DecreaseCurrentFrame()
    { 
        for(int i = 0; i < timerName.Count; i++)
        { 
            if(currentFrame[i] > 0)
                currentFrame[i] -= 1; 
            if(cooldownFrame[i] > 0)
                cooldownFrame[i] -= 1;
        }
    } 

    //sets currentFrame to totalFrame according to the Name
    public void ResetCurrentFrame(string Name)
    {
        for (int i = 0; i < timerName.Count; i++)
        {
            if (System.String.Equals(timerName[i], Name))
            {
                currentFrame[i] = totalFrame[i];
            }
        }     
    } 

    //reset cooldownframe according to the Name
    public void ResetCooldownFrame(string Name)
    {
        for (int i = 0; i < timerName.Count; i++)
        {
            if (System.String.Equals(timerName[i], Name))
            {
                cooldownFrame[i] = ReturnFrameCount(cooldownTime[i]);
            }
        }
    }

    //returns true if there is cooldown, false otherwise according to the Name
    public bool isOnCooldown(string Name)
    {
        for (int i = 0; i < timerName.Count; i++)
        {
            if (System.String.Equals(timerName[i], Name))
            {
                return (cooldownFrame[i] > 0) ? true : false;
            }
        } 
        //for the ide to accept the method
        return false; 
    }

    private void Start()
    {
        //These don't need to be handled from the inspector 
       /* currentFrame = new List<int>(this.timerName.Count);
        totalFrame = new List<int>(this.timerName.Count);
        cooldownFrame = new List<int>(this.timerName.Count);*/

        for (int i = 0; i < timerName.Count; i++)
        {
            totalFrame.Add(ReturnFrameCount(animationTime[i]));
            currentFrame.Add(totalFrame[i]); //ResetCurrentFrame(timerName[i]);
            //because we want to start with no cooldown
            cooldownFrame.Add(0);
        }
    } 
    //Add the given values to the lists and add 0 for cooldown because we want to start with no cooldown 
    //And add totalframe to currentframe because that's its initial value
    public void addTimer(string timerName, float animationTime, float cooldownTime)
    {
        this.timerName.Add(timerName);
        this.animationTime.Add(animationTime);
        this.cooldownTime.Add(cooldownTime);
        this.cooldownFrame.Add(0);
        this.totalFrame.Add(ReturnFrameCount(animationTime));
        this.currentFrame.Add(ReturnFrameCount(animationTime));
    }
}