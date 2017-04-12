using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

class SoundUpdater : MonoBehaviour
{
    public AudioClip otherClip;
    public GameObject owner;

//    public IEnumerator Start()
//    {
//        yield return new WaitForSeconds(audio.clip == null ? 0 : audio.clip.length);
//        if (!audio.loop)
//        {
//            Destroy(owner);
//        }
//    }

    public void OnUpdate()
    {
        if(owner != null)
            transform.position = owner.transform.position;
    }
}