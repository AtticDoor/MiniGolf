using UnityEngine;
using System.Collections;

/// <summary>
/// Script responsible for managing level, like spawning level, spawning balls, deciding game win/loss status and more
/// </summary>
public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;

    public GameObject ballPrefab;           //reference to ball prefab
    public Vector3 ballSpawnPos;            //reference to spawn position of ball
    private GameObject ball;
    public Camera BirdsEyeCam;
    public LevelData[] levelDatas;          //list of all the available levels

    //public int shotCount = 0;              //count to store available shots
	//public int totalShotCount=0;

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
    }

    /// <summary>
    /// Method to spawn level
    /// </summary>
    public void SpawnLevel(int levelIndex)
    {
        Instantiate(BirdsEyeCam, Vector3.zero, transform.rotation);
        //we spawn the level prefab at required position
        Instantiate(levelDatas[levelIndex].levelPrefab, Vector3.zero, Quaternion.identity);
        GameManager.shotCount = levelDatas[levelIndex].shotCount;                                   //set the available shots
        UIManager.instance.ShotText.text = ""+GameManager.shotCount;		                       //set the ShotText text

        GameObject birdsEyeStart = GameObject.Find("birdsEyeStart");
        //then we Instantiate the ball at spawn position
         ball = Instantiate(ballPrefab, ballSpawnPos, Quaternion.identity);
        //ball= Instantiate(ballPrefab, birdsEyeStart.transform.position, Quaternion.identity);
        CameraFollow.instance.SetTarget(ball);                      //set the camera target
        GameManager.singleton.gameStatus = GameStatus.Playing;      //set the game status to playing
       
        //StartCoroutine(DoAThingOverTime(ball.transform.position,ballSpawnPos, 2.0f));
    }

    /// <summary>
    /// Method used to reduce shot
    /// </summary>
    public void ShotTaken()
    {
            GameManager.shotCount++;    
            GameManager.totalShotCount++;                                            //reduce it by 1
			//Debug.Log(""+GameManager.shotCount+"  " +GameManager.totalShotCount);
            UIManager.instance.ShotText.text = "" + GameManager.shotCount;      //set the text

    }
	public void Penalty()
	{
			ShotTaken();
			//shotCount++;                                            //reduce it by 1
			//UIManager.instance.ShotText.text = "" + shotCount;      //set the text

	}
		
    /// <summary>
    /// Method called when player failed the level
    /// </summary>
    public void LevelFailed()
    {
        if (GameManager.singleton.gameStatus == GameStatus.Playing) //check if the gamestatus is playing
        {
            GameManager.singleton.gameStatus = GameStatus.Failed;   //set gamestatus to failed
            UIManager.instance.GameResult();                        //call GameResult method
        }
    }

		

    /// <summary>
    /// Method called when player win the level
    /// </summary>
    public void LevelComplete()
    {
        if (GameManager.singleton.gameStatus == GameStatus.Playing) //check if the gamestatus is playing
        {    //check if currentLevelIndex is less than total levels available
            if (GameManager.singleton.currentLevelIndex < levelDatas.Length)    
            {
                GameManager.singleton.currentLevelIndex++;  //increase the count by 1
            }
            else
            {
                //else start from level 0
                GameManager.singleton.currentLevelIndex = 0;
            }
            GameManager.singleton.gameStatus = GameStatus.Complete; //set gamestatus to Complete
            UIManager.instance.GameResult();                        //call GameResult method
        }
    }


    IEnumerator DoAThingOverTime(Vector3 start, Vector3 end, float duration)
    {



        for (float t = 0f; t < duration; t += Time.deltaTime)
        {
            float normalizedTime = t / duration;
            //right here, you can now use normalizedTime as the third parameter in any Lerp from start to end
            ball.transform.position= Vector3.Lerp(start, end, normalizedTime);
            yield return null;
        }
        ball.transform.position = end; //without this, the value will end at something like 0.9992367
        //Destroy(gameObject);
        CameraFollow.instance.SetTarget(ball);                      //set the camera target
    }
}
