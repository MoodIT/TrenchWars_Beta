using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelObstacleCow : LevelObstacle
{
    [Header("Obstacle Cow")]
    [SerializeField]
    protected Vector2 randomIdleTime = new Vector2(2, 6);

    [Header("Sounds")]
    [SerializeField]
    protected List<AudioClip> idleSounds = null;

    [SerializeField]
    protected List<AudioClip> touchSounds = null;

    HashSet<Character_Base> ignoreList = new HashSet<Character_Base>();

    void Start ()
    {
        StartCoroutine(playIdleSounds());
    }

    protected IEnumerator playIdleSounds()
    {
        while(true)
        {
            float time = UnityEngine.Random.Range(randomIdleTime.x, randomIdleTime.y);
            yield return new WaitForSeconds(time);

            SoundManager.instance.PlayRandomSound(idleSounds, gameObject);
        }
    }

    public override void Activate(Character_Base character)
    {
        base.Activate(character);

        if (ignoreList.Contains(character))
            return;

        SoundManager.instance.PlayRandomSound(touchSounds, gameObject);
        ignoreList.Add(character);
    }
}
