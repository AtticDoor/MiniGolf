using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class mainLevelScript : MonoBehaviour
{
    public string levelName;
    public string LevelDescription;
    // Start is called before the first frame update
    void Start()
    {
        try
        {

            GameObject.Find("HoleName").GetComponent<Text>().text = levelName;
            GameObject.Find("HoleText").GetComponent<Text>().text = LevelDescription;
        }
        catch
        {
            Debug.Log("Couldn't set hole name or description:"+ levelName+" Desc:"+ LevelDescription);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
