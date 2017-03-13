using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private LevelBuilder levelBuilder = null;

    private LevelBlock curSelected = null;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hitInfo = new RaycastHit();
            bool hit = Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo, 50, 1 << levelBuilder.BlockLayer);
            if (hit)
            {
                LevelBlock block = hitInfo.collider.GetComponent<LevelBlock>();
                if(block)
                {
                    if (!block.IsDiggable)
                    {
                        Debug.Log(block.name + " Blocked");
                    }
                    else
                    {
                        if (!block.IsSelected)
                        {
                            if (curSelected != null)
                                curSelected.IsSelected = false;
                            block.IsSelected = true;
                            Debug.Log(block.name + " Selected");
                        }
                        else
                        {
                            block.IsSelected = false;
                            block.Dig();
                            curSelected = null;

                            Debug.Log(block.name + " Digged");
                        }
                    }
                }
                Debug.Log("Hit " + hitInfo.transform.gameObject.name + "    " + hitInfo.distance);
            }
        }
    }
}
