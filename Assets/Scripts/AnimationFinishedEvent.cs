using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// attach this script to both the avatar prefabs to fire the OnAnimationFinished after animations
/// call this on the animation event for each of the clip
/// </summary>
public class AnimationFinishedEvent : MonoBehaviour
{
    private ChekiLogic _chekiLogic;

    private void Start()
    {
        _chekiLogic = FindObjectOfType<ChekiLogic>();
    }

    public void OnAnimationFinished()
    {
        _chekiLogic.GoNextAnimation();
    }
}
