using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using MiniJSON;
using UnityEngine.UI;
using System.Linq;

public class QuestionDrawManager : MonoBehaviour
{
    public Sprite currQuestionImage;

    public GameObject quizForm;
    public GameObject carousel;

    Text[] answerLabels;

    public Question currentQuestion;
    public LinksManager linksScript;

    public Question SetNewQuestion(int questionNumber)
    {
        return currentQuestion = linksScript.questionDataManager.QuestionsList[questionNumber];
    }

    public void DrawAllQuestions()
    {
        if (linksScript.questionDataManager.QuestionsList != null)
        {
            DrawCarousel();

            int posX = 0;
            int questionNum = 1;

            currentQuestion = linksScript.questionDataManager.QuestionsList[0];

            foreach (Question currQuestion in linksScript.questionDataManager.QuestionsList)
            {
                DrawCurrentQuestion(currQuestion, questionNum, posX);

                posX += 730;
                questionNum++;
            }
        }
    }

    void DrawCarousel()
    {
        GameObject currentCarousel = Instantiate(carousel, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
        currentCarousel.transform.SetParent(GameObject.FindGameObjectWithTag("_QuizPanel").transform, false);
    }

    void DrawCurrentQuestion(Question currentQuestion, int questionNum, int posX)
    {
        GameObject currentQuestionForm = Instantiate(quizForm, new Vector3(posX, 0, 0), Quaternion.identity) as GameObject;

        var currentQuestionId = currentQuestion.id;
        GameObject dummyAnswerCoverGb = currentQuestionForm.transform.Find("DummyAnswersCover").gameObject;
        dummyAnswerCoverGb.name = "DummyAnswersCover" + currentQuestionId;

        if (questionNum == 1)
        {
            currentQuestionForm.GetComponent<Toggle>().isOn = true;
        }

        GameObject carouselContent = GameObject.FindGameObjectWithTag("CarouselContent") as GameObject;
        currentQuestionForm.transform.SetParent(carouselContent.transform, false);

        currentQuestionForm.gameObject.name = "QuestionForm" + questionNum;
        currentQuestionForm.GetComponent<Text>().text = currentQuestionId.ToString();

        GameObject currQuestionHeader = currentQuestionForm.transform.Find("Question").gameObject;
        currQuestionHeader.GetComponentInChildren<Text>().text = currentQuestion.questionText;

        if (currentQuestion.questionURLImage != "")
        {
            Image currImageComponent = currQuestionHeader.GetComponentInChildren<Image>();
            currImageComponent.enabled = true;
            linksScript.downloadImageManager.GetWebImageInSprite(linksScript.questionDataManager.urlMain + currentQuestion.questionURLImage, currImageComponent);
        }

        currentQuestionForm.transform.Find("QuestionCurrNumber").GetComponent<Text>().text = questionNum + "  из  " + linksScript.questionDataManager.QuestionsList.Count;

        answerLabels = new Text[4];

        int i = 0;

        foreach (Text gmobj_text in currentQuestionForm.transform.Find("Answers").GetComponentsInChildren<Text>().OrderBy(y => -y.transform.position.y))
        {
            answerLabels[i] = gmobj_text;
            i++;
        }

        currentQuestion.answersTfs = answerLabels;

        int answerNum = 0;

        foreach (Button btn in currentQuestionForm.transform.Find("Answers").GetComponentsInChildren<Button>().OrderBy(y => -y.transform.position.y))
        {
            int currAnswertID = currentQuestion.answers[answerNum].id;
            btn.onClick.AddListener(() => linksScript.quizGameManager.ClickAnswer(currentQuestionId, currAnswertID));
            btn.name = btn.name + "#" + currentQuestionId + "#" + currentQuestion.answers[answerNum].id;
            answerNum++;

            if (linksScript.quizGameManager.quizGameMode == QuizGameManager.QuizGameMode.rivalNet)
            {
                Image rivalPic = btn.transform.Find("RivalPic").GetComponent<Image>();
                Texture2D rivalImgTxtr = linksScript.opponentsManager.rivalAvatarTxtr;
                Sprite sprite = Sprite.Create(rivalImgTxtr, new Rect(0, 0, rivalImgTxtr.width, rivalImgTxtr.height), Vector2.zero);
                rivalPic.sprite = sprite;
            }
        }

        answerLabels[0].text = currentQuestion.answers[0].answerText;
        answerLabels[1].text = currentQuestion.answers[1].answerText;
        answerLabels[2].text = currentQuestion.answers[2].answerText;
        answerLabels[3].text = currentQuestion.answers[3].answerText;
    }
}
