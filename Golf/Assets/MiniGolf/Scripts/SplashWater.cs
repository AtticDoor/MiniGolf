using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplashWater : MonoBehaviour
{
	
	public Transform splash;
	
    // Start is called before the first frame update
    void Start()
    {
		Color32 color = new Color32(255, 255, 255, 255);
		Color32 color2 = new Color32(255, 255, 255, 0);
        StartCoroutine(DoAThingOverTime(color,color2,.25f));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
	
	
	IEnumerator DoAThingOverTime(Color start, Color end, float duration) 
	{
			
		
			
		for (float t=0f;t<duration;t+=Time.deltaTime) 
		{
			float normalizedTime = t/duration;
			//right here, you can now use normalizedTime as the third parameter in any Lerp from start to end
			splash.gameObject.GetComponent<Renderer>().material.color = Color.Lerp(start, end, normalizedTime);
			//scale it up
			transform.localScale=new Vector3(transform.localScale.x,normalizedTime,transform.localScale.z);
			yield return null;
		}
		splash.gameObject.GetComponent<Renderer>().material.color = end; //without this, the value will end at something like 0.9992367
		Destroy(gameObject);
	}
}
