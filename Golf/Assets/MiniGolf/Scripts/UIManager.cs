using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// Script to control game UI
/// </summary>
public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    [SerializeField] private Image powerBar;        //ref to powerBar image
    [SerializeField] private Text shotText;         //ref to shot info text
    [SerializeField] private Text clubText;         //ref to shot info text
    [SerializeField] private GameObject mainMenu, gameMenu, gameOverPanel, birdsEyeMenu, retryBtn, nextBtn;   //important gameobjects
    [SerializeField] private GameObject container, lvlBtnPrefab;    //important gameobjects

    public Text ShotText { get { return shotText; } }   //getter for shotText
    public Text ClubText { get { return clubText; } }   //getter for shotText
    public Image PowerBar { get => powerBar; }          //getter for powerBar

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

        powerBar.fillAmount = 0;                        //set the fill amount to zero
    }

	public void ShowMenus(bool main, bool game, bool birdsEye,bool gameOver)
	{
	    mainMenu.SetActive(main);                                      //deactivate main menu
        gameMenu.SetActive(game);  	
		birdsEyeMenu.SetActive(birdsEye);
		gameOverPanel.SetActive(gameOver);              //activate game finish panel
	}

    void Start()
    {
        if (GameManager.singleton.gameStatus == GameStatus.None)    //if gamestatus is none
        {   
            CreateLevelButtons();                       //create level buttons
        }     //we check for game status, failed or complete
        else if (GameManager.singleton.gameStatus == GameStatus.Failed ||
            GameManager.singleton.gameStatus == GameStatus.Complete)
        {
			ShowMenus(false,false,false,false);
            //mainMenu.SetActive(false);                                      //deavtivate main menu
            //gameMenu.SetActive(false);                                       //activate game menu
            LevelManager.instance.SpawnLevel(GameManager.singleton.currentLevelIndex);  //spawn level
        }
    }

    /// <summary>
    /// Method which spawn levels button
    /// </summary>
    void CreateLevelButtons()
    {
        //total count is number of level datas
        for (int i = 0; i < LevelManager.instance.levelDatas.Length; i++)
        {
            GameObject buttonObj = Instantiate(lvlBtnPrefab, container.transform);   //spawn the button prefab
            buttonObj.transform.GetChild(0).GetComponent<Text>().text = "" + (i + 1);   //set the text child
            Button button = buttonObj.GetComponent<Button>();                           //get the button componenet
            button.onClick.AddListener(() => OnClick(button));                          //add listner to button
        }
    }

    /// <summary>
    /// Method called when we click on button
    /// </summary>
    void OnClick(Button btn)
    {
		ShowMenus(false,false,true,false);
        //mainMenu.SetActive(false);                                                      //deactivate main menu
        //gameMenu.SetActive(true);                                                       //activate game manu
        GameManager.singleton.currentLevelIndex = btn.transform.GetSiblingIndex(); ;    //set current level equal to sibling index on button
		GameManager.shotCount=0;
		GameManager.totalShotCount=0;
		//Debug.Break();
        LevelManager.instance.SpawnLevel(GameManager.singleton.currentLevelIndex);      //spawn level
    }

    /// <summary>
    /// Method call after level fail or win
    /// </summary>
    public void GameResult()
    {
        switch (GameManager.singleton.gameStatus)
        {
            case GameStatus.Complete:                       //if completed
                ShowMenus(false,false,false,true);          //activate game finish panel
                nextBtn.SetActive(true);                    //activate next button
                SoundManager.instance.PlayFx(FxTypes.GAMECOMPLETEFX);
                break;
            case GameStatus.Failed:                         //if failed
                ShowMenus(false,false,false,true);          //activate game finish panel
                retryBtn.SetActive(true);                   //activate retry button
                SoundManager.instance.PlayFx(FxTypes.GAMEOVERFX);
                break;
        }
    }

    //method to go to main menu
    public void HomeBtn()
    {
        GameManager.singleton.gameStatus = GameStatus.None;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    //method to reload scene
    public void NextRetryBtn()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    public void ZoomClick()
    {
        CameraFollow.instance.SetOffset(); 

    }
	public void ClubUp()
	{
			BallControl.instance.ClubUp();

	}
	public void ClubDown()
	{
			BallControl.instance.ClubDown();

	}

}
