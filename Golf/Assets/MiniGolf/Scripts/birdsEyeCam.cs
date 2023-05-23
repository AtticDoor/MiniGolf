using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class birdsEyeCam : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GameObject start = GameObject.Find("birdsEyeStart");
        GameObject end = GameObject.Find("birdsEyeEnd");
        if (start == null)
            Destroy(gameObject);
        else
        {
            transform.rotation = start.transform.rotation;
            StartCoroutine(MoveObject(transform,start.transform.position, end.transform.position, 8.0f));
        }
    }
    private void Update()
    {
        //quit birds eye view if player clicks
        if (Input.GetMouseButtonDown(0))
        { 
            UIManager.instance.ShowMenus(false, true, false, false);//activate game menu
            Destroy(gameObject);
        }
    }
    IEnumerator MoveObject (Transform thisTransform ,  Vector3 startPos,   Vector3 endPos, float time  ) 
	{
		float i = 0.0f;
		float rate = 1.0f/time;
		
		while (i < 1.0f && thisTransform!=null) 
		{
			i += Time.deltaTime * rate;
			thisTransform.position = Vector3.Lerp(startPos, endPos, i);
			yield return null;
		}
		
		UIManager.instance.ShowMenus(false,true,false,false);//activate game menu
        Destroy(gameObject);
	}


    //IEnumerator DoAThingOverTime(Vector3 start, Vector3 end, float duration)
    //{


		//for (float t = 0f; t < duration; t += Time.deltaTime)
  //      {
  //          float normalizedTime = t / duration;
  //          //right here, you can now use normalizedTime as the third parameter in any Lerp from start to end
  //          transform.position = Vector3.Lerp(start, end, Mathf.SmoothStep(0.0f, 1.0f, normalizedTime));
  //          yield return null;





  //      }
  //      transform.position = end; //without this, the value will end at something like 0.9992367
		//UIManager.instance.ShowMenus(false,true,false,false);//activate game menu
  //      Destroy(gameObject);
  //  }
}
