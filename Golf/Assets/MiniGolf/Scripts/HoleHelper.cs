using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoleHelper : MonoBehaviour
{
	Transform ball;
	Quaternion targetRotation;
    // Start is called before the first frame update
    void Start()
    {
		ball=GameObject.Find("Ball(Clone)").transform;
        
    }

    // Update is called once per frame
    void Update()
    {
//        if(ball!=null)
//			transform.LookAt(ball.transform,Vector3.up);
Vector3 dir=new Vector3(ball.position.x, transform.position.y, ball.position.z);
transform.LookAt(dir);//(Vector3(ball.position.x, transform.position.y, ball.position.z)); 


//   targetRotation = Quaternion.LookRotation (ball.position - transform.position);
  // str = Mathf.Min (strength * Time.deltaTime, 1);
//   transform.rotation = Quaternion.Lerp (transform.rotation, targetRotation,1);// str);
}
}
