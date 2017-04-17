using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class SoundManager
{
    static SoundManager _instance;
    public static SoundManager instance
    {
        get
        {
            if (_instance == null)
                _instance = new SoundManager();
            return _instance;
        }
    }

    public GameObject PlayRandomSound(List<AudioClip> clips, GameObject owner = null, bool loop = false, string name = "", float starttime = 0, float volume = 1f)
    {
        if (clips.Count == 0)
            return null;

        AudioClip clip = clips[UnityEngine.Random.Range(0, clips.Count - 1)];
        return PlaySound(clip, owner, loop, name, starttime, volume);
    }

    public GameObject PlaySound(AudioClip clip, GameObject owner = null, bool loop = false, string name = "", float starttime = 0, float volume = 1f)
    {
		if (clip == null)
            return null;

        GameObject obj = new GameObject("Sound_" + clip.name);

        SoundUpdater soundUpdater = obj.AddComponent<SoundUpdater>();
        soundUpdater.owner = obj;

        AudioSource sound = obj.AddComponent<AudioSource>();

		//hack for now to get the sounds closer to the camera
		if(owner != null)
		    obj.transform.parent = owner.transform;

		//obj.transform.localPosition = new Vector3(0f, obj.transform.localPosition.y, obj.transform.localPosition.z);
		obj.transform.localPosition = new Vector3(0f, 0f, 0f);
		
        sound.loop = loop;
        sound.clip = clip;
        sound.time = starttime;
		sound.volume = volume;
		sound.Play();

		if(!loop)
            GameObject.Destroy(obj,clip.length+2f);

        return obj;
    }
}
