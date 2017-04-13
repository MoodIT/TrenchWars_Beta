using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelObstacleFlag : LevelObstacle
{
    [Header("Obstacle Flag")]
    [SerializeField]
    protected GameObject initialFlagEffect = null;

    protected GameObject initialFlag = null;

    [Serializable]
    public class FlagAnim
    {
        public Vector2 timeSpanSec = new Vector2(2, 6);
        public string paramName = "";
        public AudioClip sounds = null;
    }

    [SerializeField]
    protected List<FlagAnim> animList = new List<FlagAnim>();

    protected Animator anim = null;

    void Start ()
    {
        anim = GetComponentInChildren<Animator>();

        GameManager.Instance.RegisterFlag(this);

        initialFlag = ParticleManager.instance.CreateEffect(initialFlagEffect, transform.localPosition, Quaternion.identity);

        StartCoroutine(PlayNewAnim(-1));
    }

    protected IEnumerator waitTime(int animIdx, float sec)
    {
        yield return new WaitForSeconds(sec);
        StartCoroutine(PlayNewAnim(animIdx));
    }

    protected IEnumerator PlayNewAnim(int prevAnimIdx)
    {
        yield return null;

        int newIdx = 0;
        if (prevAnimIdx >= 0 && prevAnimIdx < animList.Count)
        {
            anim.ResetTrigger(animList[prevAnimIdx].paramName);

            List<int> idxList = new List<int>();
            for (int i = 0; i < animList.Count; i++)
            {
                if (i == prevAnimIdx)
                    continue;
                idxList.Add(i);
            }
            newIdx = idxList[UnityEngine.Random.Range(0, idxList.Count)];
        }
        anim.SetTrigger(animList[newIdx].paramName);
        SoundManager.instance.PlaySound(animList[newIdx].sounds, gameObject);
        StartCoroutine(waitTime(newIdx, UnityEngine.Random.Range(animList[newIdx].timeSpanSec.x, animList[newIdx].timeSpanSec.y)));
    }

    public override void Activate(Character_Base character)
    {
        base.Activate(character);

        Destroy(initialFlag);
        GameManager.Instance.FlagActivated(this);
    }
}
