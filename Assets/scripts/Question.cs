using UnityEngine;
using System.Collections;

public class Question {

	private string question;
	private string correctAnswer;
	private string[] dummyAnswers = new string[1];

	public Question(string question, string correctAnswer, string firstDummyAnswer)
	{
		this.question = question;
		this.correctAnswer = correctAnswer;
		dummyAnswers[0] = firstDummyAnswer;
	}

	public string getQuestion()
	{
		return question;
	}

	public string getCorrectAnswer()
	{
		return correctAnswer;
	}

	public string getDummyAnswer(int index)
	{
		return dummyAnswers[index];
	}
}