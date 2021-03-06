using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System;
using SimpleJSON;



public class GameManager : MonoBehaviour {

	// public variables
	public Button startButton;
	public Button answer1Button;
	public Button answer2Button;
	public Text questionText;
	public Text score;
	public Text lives;
	public Text highScoreName;
	public AudioSource aMin;
	public AudioSource aMin7;
	public AudioSource bMin;
	public AudioSource cMaj;
	public AudioSource cMaj9;
	public AudioSource dLow;
	public AudioSource dMaj;
	public AudioSource eMin;
	public AudioSource eMin9;
	public AudioSource gMaj;
	public AudioSource wrong;
	public GameObject panel2;
	public GameObject panel3;
	public GameObject panel4;

	// private variables
	private System.Random rnd = new System.Random();
	private TextAsset questionsAsset;
	private string decryptedQuestions;
	private List<Question> questionList = new List<Question>();
	private int scoreValue = 0;
	private int livesValue = 3;
	private AudioSource[] primarySoundEffects = new AudioSource[5];
	private AudioSource[] secondarySoundEffects = new AudioSource[10];
	private int soundCounter = 0;

	void Start () {
		panel2.SetActive(false);
		panel3.SetActive(false);
		panel4.SetActive(false);
		DeactivateAnswerButtons ();

		primarySoundEffects[0] = gMaj;
		primarySoundEffects[1] = dMaj;
		primarySoundEffects[2] = dLow;
		primarySoundEffects[3] = cMaj9;
		primarySoundEffects[4] = cMaj;

		secondarySoundEffects[0] = aMin;
		secondarySoundEffects[1] = aMin7;
		secondarySoundEffects[2] = bMin;
		secondarySoundEffects[3] = cMaj;
		secondarySoundEffects[4] = cMaj9;
		secondarySoundEffects[5] = dLow;
		secondarySoundEffects[6] = dMaj;
		secondarySoundEffects[7] = eMin;
		secondarySoundEffects[8] = eMin9;
		secondarySoundEffects[9] = gMaj;

		questionsAsset = Resources.Load("JSONquestions") as TextAsset;
		
		var parsedQuestions = new string(questionsAsset.text.ToCharArray());
		var N = JSONNode.Parse(parsedQuestions);
		int length = N["questions"].Count;

		for(int i = 0; i < length; i++)
		{
			string question = N["questions"][i]["question"];
			string correctAnswer = N["questions"][i]["correctAnswer"];
			string dummy1 = N["questions"][i]["dummyAnswers"][0];
			questionList.Add(new Question(question, correctAnswer, dummy1));
		}
	}

	public void SceneTransition(string sceneName)
	{
		Application.LoadLevel(sceneName);
	}

	public void DeactivateAnswerButtons()
	{
		answer1Button.interactable = false;
		answer2Button.interactable = false;
	}
	
	public void ActivateAnswerButtons()
	{
		answer1Button.interactable = true;
		answer2Button.interactable = true;
	}

	public void ClearAnswerButtons()
	{
		Text answer1Text = answer1Button.GetComponentInChildren<Text>();
		answer1Text.text = "";
		answer1Button.image.color = Color.white;
		Text answer2Text = answer2Button.GetComponentInChildren<Text>();
		answer2Text.text = "";
		answer2Button.image.color = Color.white;
	}

	public void WrongAnswer(Button[] buttonArray, Button startButton, int correctAnswer)
	{
		wrong.Play();

		foreach(Button button in buttonArray)
		{
			button.onClick.RemoveAllListeners();
		}
		
		for(int i = 0; i < buttonArray.Length; i++)
		{
			if(i == correctAnswer)
			{
				buttonArray[i].image.color = Color.green;
			}
			else
			{
				buttonArray[i].image.color = Color.red;
			}
		}

		startButton.interactable = true;
		
		livesValue--;
		lives.text = "Попыток: " + livesValue.ToString();

		if(livesValue == 0)
		{
			// game over!
			DeactivateAnswerButtons();
			startButton.interactable = false;

			// check if player got a high score
			CheckHighScore();
		}
		else if(questionList.Count == 0)
		{
			panel4.SetActive(true);
			startButton.interactable = false;
		}
	}

	public void CheckHighScore()
    {
		if (panel4.activeSelf)
			panel4.SetActive(false);

		for (int i = 0; i <= 4; i++)
		{
			if (scoreValue > HighScoreTracker.highScoreTracker.getHighScore(i).getScore())
			{
				panel3.SetActive(true);
				break;
			}
		}

		// if no high score, pop up the normal end of game screen
		panel2.SetActive(true);
	}

	public void RecordHighScore()
	{
		string playerName;

		if(highScoreName.Equals(""))
		{
			playerName = "Игрок";
		}
		else
		{
			playerName = highScoreName.text;
		}

		for(int i = 0; i <= 4; i++)
		{
			if(scoreValue > (HighScoreTracker.highScoreTracker.getHighScore(i).getScore()))
			{
				HighScoreTracker.highScoreTracker.setHighScore(i, scoreValue, playerName);
				break;
			}
		}

		Application.LoadLevel ("highscores_scene");
	}

	public void RightAnswer(Button[] buttonArray, Button startButton, int correctAnswer)
	{
		if(soundCounter % 7 == 0)
			primarySoundEffects[0].Play();
		else
		{
			if((soundCounter % 4 == 0) || soundCounter % 5 == 0)
				primarySoundEffects[rnd.Next(primarySoundEffects.Length)].Play();
			else
				secondarySoundEffects[rnd.Next(primarySoundEffects.Length)].Play();
		}

		if(soundCounter == 0 || soundCounter == 1)
			soundCounter++;
		else
			soundCounter = 0;

		foreach(Button button in buttonArray)
		{
			button.onClick.RemoveAllListeners();
		}

		for(int i = 0; i < buttonArray.Length; i++)
		{
			if(i == correctAnswer)
			{
				buttonArray[i].image.color = Color.green;
			}
			else
			{
				buttonArray[i].image.color = Color.red;
			}
		}

		startButton.interactable = true;

		scoreValue++;
		score.text = "Баллов: " + scoreValue.ToString();

		if(questionList.Count == 0)
		{
			panel4.SetActive(true);
			startButton.interactable = false;
		}


	}
	
	public void StartGame()
	{
		startButton.interactable = false;
		ClearAnswerButtons();
		ActivateAnswerButtons ();
		int correctAnswer = rnd.Next(2);
		int randomQuestion = rnd.Next(questionList.Count);
		Debug.Log(randomQuestion);
		Question chosenQuestion = questionList[randomQuestion];
		questionList.RemoveAt(randomQuestion);
		Button[] buttonArray = {answer1Button, answer2Button};
		Text correctAnswerText = buttonArray[correctAnswer].GetComponentInChildren<Text>();
		correctAnswerText.text = chosenQuestion.getCorrectAnswer();
		questionText.text = chosenQuestion.getQuestion();
		buttonArray[correctAnswer].onClick.AddListener(() => { RightAnswer(buttonArray, startButton, correctAnswer);});

		int buttonFiller = 0;

		foreach(Button button in buttonArray)
		{
			if(button.GetComponentInChildren<Text>().text == "")
			{
				button.GetComponentInChildren<Text>().text = chosenQuestion.getDummyAnswer(buttonFiller);
				button.onClick.AddListener(() => { WrongAnswer (buttonArray, startButton, correctAnswer); });
				buttonFiller++;
			}
		}
	}
}