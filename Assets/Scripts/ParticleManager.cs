using System.Collections.Generic;
using UnityEngine;

public class ParticleManagerUpdater : MonoBehaviour
{
    public void Awake()
    {
        DontDestroyOnLoad(transform.gameObject);
    }

    public void Update()
    {
        ParticleManager.instance.Update();
    }


}

public class ParticleManager
{
    static ParticleManager _instance;
    public static ParticleManager instance
    {
        get
        {
            if (_instance == null)
                _instance = new ParticleManager();
            return _instance;
        }
    }

    private GameObject updater = null; //used as parent and updater to all effects

    public delegate void DoneEvent();

    private List<ParticleSystem> effects = new List<ParticleSystem>();
    private Dictionary<GameObject, DoneEvent> effectParents = new Dictionary<GameObject, DoneEvent>();

    public ParticleManager()
    {
        //make sure we have someone to update our manager
        if (GameObject.FindObjectOfType<ParticleManagerUpdater>() == null)
        {
            updater = new GameObject("ParticleManagerUpdater_" + System.Guid.NewGuid());
            updater.AddComponent<ParticleManagerUpdater>();
        }
    }

    public GameObject CreateEffect(GameObject prefab, Vector3 pos, Quaternion rotation, int renderQueue = 0, int layer = -1, DoneEvent callback = null)
    {
        if (prefab == null) return null;

        GameObject instance = GameObject.Instantiate(prefab, pos, rotation) as GameObject;
        if(updater != null)
            instance.transform.parent = updater.transform;

        if (layer != -1)
            instance.layer = layer;

        effectParents.Add(instance, callback);

        ParticleSystem[] particles = instance.GetComponents<ParticleSystem>();
        foreach (ParticleSystem particleSys in particles)
            AddParticleSystem(instance, particleSys, renderQueue, layer);

        ParticleSystem[] childsystems = instance.GetComponentsInChildren<ParticleSystem>();
        foreach (ParticleSystem particleSys in childsystems)
            AddParticleSystem(instance, particleSys, renderQueue, layer);

        return instance;
    }

    private void AddParticleSystem(GameObject instance, ParticleSystem particleSys, int renderQueue, int layer)
    {
        particleSys.gameObject.layer = layer == -1 ? instance.layer : layer;
        effects.Add(particleSys);
        if (renderQueue != 0)
            particleSys.GetComponent<Renderer>().material.renderQueue = renderQueue;
    }

    public void Update()
    {
        List<ParticleSystem> removedEffects = new List<ParticleSystem>();
        foreach (ParticleSystem system in effects)
        {
            if (system == null || system.isStopped || !system.emission.enabled)
                removedEffects.Add(system);
        }

        effects.Remove(null);
        foreach (ParticleSystem remove in removedEffects)
        {
            if (remove != null)
            {
                //Debug.Log("REMOVED " + remove.name);
                effects.Remove(remove);
                GameObject.Destroy(remove.gameObject);
            }
        }

        List<GameObject> removedParents = new List<GameObject>();
        foreach (KeyValuePair<GameObject, DoneEvent> pair in effectParents)
        {
            if (pair.Key == null || (pair.Key.transform.childCount == 0 && pair.Key.GetComponent<ParticleSystem>() == null))
                removedParents.Add(pair.Key);
        }

        foreach (GameObject parent in removedParents)
        {
            if (parent != null)
            {
                //Debug.Log("REMOVED " + parent.name);
                DoneEvent callback = effectParents[parent];
                if (callback != null)
                    callback();
                effectParents.Remove(parent);
                GameObject.Destroy(parent);
            }
        }
    }

    public void StopAll()
    {
        foreach (ParticleSystem system in effects)
        {
            if (system != null)
            {
                var particleModule = system.main;
                particleModule.loop = false;
                system.Stop();
            }
        }
    }

    public void RemoveAll()
    {
        while(updater.transform.childCount > 0)
            GameObject.DestroyObject(updater.transform.GetChild(0));
    }

    public void StopLoop(GameObject effect)
    {
        if (effect != null)
        {
            ParticleSystem[] systems = effect.GetComponents<ParticleSystem>();
                
            foreach (ParticleSystem system in systems)
            {
                var particleModule = system.main;
                particleModule.loop = false;
            }

            systems = effect.GetComponentsInChildren<ParticleSystem>();

            foreach (ParticleSystem system in systems)
            {
                var particleModule = system.main;
                particleModule.loop = false;
            }
        }
    }
}
