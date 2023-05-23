using UnityEngine;
using System.Collections;

/// <summary>
/// Script which controls the ball
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class BallControl : MonoBehaviour
{
    public static BallControl instance;                 

    [SerializeField] private LineRenderer lineRenderer;     //reference to lineRenderer child object
    [SerializeField] private SpriteRenderer spriteRenderer;     //reference to spriteRenderer child object
    [SerializeField] private float MaxForce;                //maximum force that an be applied to ball
    [SerializeField] private float forceModifier = 0.5f;    //multipliers of force
    [SerializeField] private GameObject areaAffector;       //reference to sprite object which show area around ball to click
    [SerializeField] private LayerMask rayLayer;            //layer allowed to be detected by ray

    private float force;                                    //actuale force which is applied to the ball
    private Rigidbody rgBody;                               //reference to rigidbody attached to this gameobject
    /// <summary>
    /// The below variables are used to decide the force to be applied to the ball
    /// </summary>
    private Vector3 startPos, endPos;
    private bool canShoot = false, ballIsStatic = true;    //bool to make shooting stopping ball easy
    private Vector3 direction;                              //direction in which the ball will be shot
	
	private Vector3 lastShotPos;
	public GameObject arrow;
	
	
	private int club;
	private float clubYForce=.15f;
	public void ClubUp()
	{
		club+=1;
		if (club>2) club=0;
		SetClub();
	}
	public void ClubDown()
	{
		club-=1;
		if (club<=0) club=2;
		SetClub();
	}
	public void SetClubName(string name)
	{
		UIManager.instance.ClubText.text = "" + name;      //set the text
		
	}
	public void SetClub()
	{
		switch(club) 
		{
		  case 0: SetClubName("Wood");  	clubYForce=.15f; 	MaxForce=15;	forceModifier=5; 	break;
		  case 1: SetClubName("Iron");  	clubYForce=.5f; 	MaxForce=6;		forceModifier=3; 	break;
		  case 2: SetClubName("Putter");  	clubYForce=.02f; 	MaxForce=4;		forceModifier=2; 	break;

		}
	}
	
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        rgBody = GetComponent<Rigidbody>();                 //get reference to the rigidbody
    }

    // Update is called once per frame
    void Update()
    {
        if (rgBody.velocity == Vector3.zero && !ballIsStatic &&!Resetting)   //if velocity is zero and ballIsStatic is false
        {
            ballIsStatic = true;                                //set ballIsStatic to true
            rgBody.angularVelocity = Vector3.zero;              //set angular velocity to zero
            areaAffector.SetActive(true);                       //activate areaAffector
        }
    }

    private void FixedUpdate()
    {
		direction = startPos - endPos;
        if (canShoot)                                               //if canSHoot is true
        {
            canShoot = false;                                       //set canShoot to false
            ballIsStatic = false;                                   //set ballIsStatic to false
            direction = startPos - endPos;                          //get the direction between 2 vectors from start to end pos
			Debug.Log("Force" +force.ToString());
			Debug.Log("Direction" +direction.ToString());
			direction.y=force*clubYForce;

            rgBody.AddForce(direction * force, ForceMode.Impulse);  //add force to the ball in given direction
			SoundManager.instance.PlayFx(FxTypes.BALLHIT);
			
            areaAffector.SetActive(false);                          //deactivate areaAffector
			
            LevelManager.instance.ShotTaken();                  //inform LevelManager of shot taken
            UIManager.instance.PowerBar.fillAmount = 0;             //reset the powerBar to zero
            force = 0;                                              //reset the force to zero
            startPos = endPos = Vector3.zero;                       //reset the vectors to zero
        }
    }

    // Unity native Method to detect colliding objects
    private void OnTriggerEnter(Collider other)
    {
        switch (other.name)
        {

            case "Destroyer": StartCoroutine(ResetBallPos(0, 0)); break;  //Level Failed
            case "Water": StartCoroutine(ResetBallPos(1, 2)); break;  //Level Failed
            case "Sand":
            {
                direction = Camera.main.transform.position - transform.position;
                GameObject.Instantiate(sandSplash, transform.position, Quaternion.LookRotation(direction));//, Vector3.up);
                break;
            }
            case "Hole":  LevelManager.instance.LevelComplete();   break;               //Level Complete          
        }
    }


    private bool Resetting = false;
    private void OnTriggerStay(Collider other)
    {
        if (other.name == "Sand")                              //if the object name is Destroyer
        {
            if(!Resetting)
            if (rgBody.velocity == Vector3.zero && !ballIsStatic)
                StartCoroutine(ResetBallPos(1, 3));
        }
    }
        

    public void MouseDownMethod()                                           //method called on mouse down by InputManager
    {
        if (Resetting) return;
        if(!ballIsStatic) return;                                           //no mouse detection if ball is moving
        startPos = ClickedPoint();                                          //get the vector in word space
		lastShotPos=startPos;
        lineRenderer.gameObject.SetActive(true);                            //activate lineRenderer
        lineRenderer.SetPosition(0, lineRenderer.transform.localPosition);  //set its 1st position
        //arrow.gameObject.SetActive(true);                            //activate lineRenderer
		
	//spriteRenderer.gameObject.SetActive(true);                            //activate spriteRenderer
       // spriteRenderer..SetPosition(0, spriteRenderer.transform.localPosition);  //set its 1st position
        
    }

    public void MouseNormalMethod()                                         //method called by InputManager
    {
		if(startPos == Vector3.zero) //if we haven't started a move, do  nothing
			return;
        if(!ballIsStatic) return;                                           //no mouse detection if ball is moving
        endPos = ClickedPoint();                                                //get the vector in word space
        force = Mathf.Clamp(Vector3.Distance(endPos, startPos) * forceModifier, 0, MaxForce);   //calculate the force
        UIManager.instance.PowerBar.fillAmount = force / MaxForce;              //set the powerBar image fill amount
        //we convert the endPos to local pos for ball as lineRenderer is child of ball
        lineRenderer.SetPosition(1, transform.InverseTransformPoint(endPos));   //set its 1st position
        spriteRenderer.transform.rotation = Quaternion.Euler(direction.x,direction.y,direction.z);//.SetPosition(1, transform.InverseTransformPoint(endPos));   //set its 1st position
		
		
		Vector3 arrowRot =direction;//new Vector3(1,(startPos - endPos).y,0);
		//Debug.Log(direction);
      arrow.transform.localEulerAngles =arrowRot;//startPos - endPos; 
    }

    public void MouseUpMethod()                                             //method called by InputManager
    {
		if(startPos == Vector3.zero) //if we haven't started a move, don't finish it
			return;
        if(!ballIsStatic) return;                                           //no mouse detection if ball is moving
        canShoot = true;                                                    //set canShoot true
        lineRenderer.gameObject.SetActive(false);                           //deactive lineRenderer
        spriteRenderer.gameObject.SetActive(false);                           //deactive spriteRenderer
        arrow.gameObject.SetActive(false);                            //activate lineRenderer
    }

    /// <summary>
    /// Method used to convert the mouse position to the world position in respect to Level
    /// </summary>
    Vector3 ClickedPoint()
    {
        Vector3 position = Vector3.zero;                                //get a new Vector3 varialbe
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);    //create a ray from camera in mouseposition direction
        RaycastHit hit = new RaycastHit();                              //create a RaycastHit
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, rayLayer))    //check for the hit 
        {
            position = hit.point;                                       //save the hit point in position
        }
        return position;                                                //return position
    }
	
	public GameObject splash;
	public GameObject sandSplash;
	IEnumerator  ResetBallPos(int penalty, int type)
	{
        Resetting = true;
        float delay = 2.0f;
            if (type<3) //don't do this for sand
			     //public void SetAllCollidersStatus (bool active) {
			     foreach(Collider c in GetComponents<Collider> ()) {
				     c.enabled = false;
			     }
 
 
			if(type==2) //water
			{
			    direction = Camera.main.transform.position - transform.position;

				GameObject.Instantiate(splash,transform.position,Quaternion.LookRotation(direction));//, Vector3.up);

			}
 
			if(type==3) //sand NOT CALLED ANYMORE
            {
                rgBody.velocity *= .5f;
                rgBody.angularVelocity *= .5f;
            delay = .5f;

              //  direction = Camera.main.transform.position - transform.position;

			//	GameObject.Instantiate(sandSplash,transform.position,Quaternion.LookRotation(direction));//, Vector3.up);

			}
			
			
			if(penalty>0)
			{
				yield return new WaitForSeconds(1.0f);
				LevelManager.instance.Penalty();
			}
			
			yield return new WaitForSeconds(delay);
			
			rgBody.velocity = new Vector3(0, 0, 0);
			rgBody.angularVelocity = new Vector3(0, 0, 0);
			transform.position = lastShotPos;
			
			//yield return new WaitForSeconds(0.0f);
			
			//collider.enabled=true;
         foreach(Collider c in GetComponents<Collider> ()) {
             c.enabled = true;
         }
 
 
			yield return new WaitForSeconds(0.0f);
        Resetting = false;

    }

	
#if UNITY_EDITOR

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 1.5f);
    }

#endif

}
