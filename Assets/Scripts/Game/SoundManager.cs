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

    public int poolSize = 15;
    
    Stack<GameObject> pool = new Stack<GameObject>();
	
	public SoundManager()
    {
        FillPool();
    }

    private void FillPool()
    {
        for (int i = 0; i < poolSize; i++)
        {
            GameObject obj = new GameObject("Pooled Sound");

            SoundUpdater soundUpdater = obj.AddComponent<SoundUpdater>();
            soundUpdater.owner = obj;

            obj.AddComponent<AudioSource>();

            obj.SetActive(false);

            pool.Push(obj);
        }
    }

	public GameObject PlaySound(AudioClip clip, GameObject owner = null, bool loop = false, string name = "", float starttime = 0, float volume = 1f)
    {
		if (clip == null)
            return null;

        GameObject obj = pool.Pop();
        obj.name = name.Length == 0 ? "Sound_" + clip.name : name;

        AudioSource sound = obj.GetComponent<AudioSource>();

		obj.SetActive(true);
		
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

        if (pool.Count < 5)
            FillPool();

		if(!loop) GameObject.Destroy(obj,clip.length+2f);

        return obj;
    }
}
