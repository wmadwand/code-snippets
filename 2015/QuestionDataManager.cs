using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Linq;
using System;
using System.Collections.Generic;
using MiniJSON;

public class QuestionDataManager : MonoBehaviour
{
    public LinksManager linksScript;
    public string urlMain = "";
    public string urlBattles = "";

    public int battleWithBotID;

    public class RightAnswer
    {
        public int idQuestion;
        public int idRightAnswer;

        public RightAnswer(int _idQuestion, int _idRightAnswer)
        {
            idQuestion = _idQuestion;
            idRightAnswer = _idRightAnswer;
        }
    }

    public List<Question> QuestionsList = new List<Question>();
    public List<RightAnswer> rightAnswersList = new List<RightAnswer>();

    public WWW GET(string url, Action onComplete)
    {
        WWW www = new WWW(url);
        StartCoroutine(BattleJoin(www, onComplete));
        return www;
    }

    public WWW POST(QuizState currQuizState, bool gameWithBot, string url, int currAnswerId, Action onComplete)
    {
        WWWForm form = new WWWForm();

        Dictionary<string, string> headers = new Dictionary<string, string>();
        headers.Add("X-Auth", linksScript.socketIOManager.token);

        form.AddField("test_id", 1);

        if (gameWithBot)
        {
            form.AddField("bot", "true");
        }

        if (currQuizState == QuizState.battleNewAnswer)
        {
            form.AddField("answer_id", currAnswerId);
            form.AddField("time", linksScript.timerManager.timerValueAnswer);
        }

        WWW www = new WWW(url, form.data, headers);

        if (currQuizState == QuizState.battleSearch)
        {
            StartCoroutine(BattleSearch(www, onComplete));
        }

        else if (currQuizState == QuizState.battleWithBotSearch)
        {
            StartCoroutine(BattleWithBotSearch(www, onComplete));
        }

        else if (currQuizState == QuizState.battleNewAnswer)
        {
            StartCoroutine(BattleNewAnswer(www, onComplete));
        }

        else if (currQuizState == QuizState.battleStop || currQuizState == QuizState.battleStopSearch)
        {
            StartCoroutine(BattleStop(www, onComplete));
        }

        return www;
    }

    private IEnumerator BattleJoin(WWW www, Action onComplete)
    {
        yield return www;
        if (www.error == null)
        {
            QuestionsList = ProcessQuestions(www.text);

            www.Dispose();
            www = null;

            onComplete();
        }
        else
        {
            Debug.Log("ERROR BattleJoin: " + www.error);
        }
    }

    public void GetBattleJoin(string foundBattleQuizId)
    {
        GET(urlMain + urlBattles + foundBattleQuizId, () =>
        {
            linksScript.questionDrawManager.DrawAllQuestions();
        });
    }

    private IEnumerator BattleSearch(WWW www, Action onComplete)
    {
        yield return www;
        if (www.error == null)
        {
            www.Dispose();
            www = null;

            onComplete();
        }
        else
        {
            Debug.Log("ERROR BattleSearch: " + www.error);
        }
    }

    public void PostBattleSearch()
    {
        POST(QuizState.battleSearch, false, urlMain + urlBattles + "", 0, () =>
        {
            //linksScript.questionManager.DrawAllQuestions();
            //Debug.Log("dddddd: " + QuestionsList);
        });
    }

    private IEnumerator BattleWithBotSearch(WWW www, Action onComplete)
    {
        yield return www;
        if (www.error == null)
        {
            battleWithBotID = GetBattleWithBotID(www.text);

            www.Dispose();
            www = null;

            onComplete();
        }
        else
        {
            Debug.Log("ERROR BattleWithBotSearch: " + www.error);
        }
    }

    public void PostBattleWithBotSearch()
    {
        POST(QuizState.battleWithBotSearch, true, urlMain + urlBattles + "", 0, () =>
        {
            linksScript.dashboardManager.ShowVSPanel(null);
            //Debug.Log("dddddd: " + QuestionsList);
        });
    }

    private IEnumerator BattleNewAnswer(WWW www, Action onComplete)
    {
        yield return www;
        if (www.error == null)
        {
            www.Dispose();
            www = null;

            onComplete();
        }
        else
        {
            Debug.Log("ERROR BattleNewAnswer: " + www.error);
        }
    }

    public void PostBattleNewAnswer(string currBattleId, int questionId, int answerId)
    {
        POST(QuizState.battleNewAnswer, false, urlMain + urlBattles + currBattleId + "" + questionId + "", answerId, () =>
        {
            //linksScript.questionManager.DrawAllQuestions();
            //Debug.Log("dddddd: " + QuestionsList);
        });
    }

    private IEnumerator BattleStop(WWW www, Action onComplete)
    {
        yield return www;
        if (www.error == null)
        {
            www.Dispose();
            www = null;

            onComplete();
        }
        else
        {
            Debug.Log("ERROR BattleStop OR BattleStopSearch: " + www.error);
        }
    }

    public void PostBattleStop()
    {
        POST(QuizState.battleStop, false, urlMain + urlBattles + linksScript.socketIOManager.foundBattleQuiz.id + "/stop", 0, () => { });
    }

    public void PostBattleStopSearch()
    {
        POST(QuizState.battleStopSearch, false, urlMain + urlBattles + "", 0, () => { });
    }

    List<Question> ProcessQuestions(string url)
    {
        IDictionary questionsList = (IDictionary)Json.Deserialize(url);
        IList questions = (IList)questionsList["questions"];

        int dummyCounter = 1;

        foreach (IDictionary question in questions)
        {
            List<Answer> currAnswers = new List<Answer>();

            IList answers = (IList)question["answers"];
            foreach (IDictionary answer in answers)
            {
                int currAnswerId = (int)Convert.ToUInt64(string.Format("{0}", answer["id"]));
                string currAnswerText = string.Format("{0}", answer["content"]);
                bool currAnswerRight = Convert.ToBoolean(string.Format("{0}", answer["right"]));
                currAnswers.Add(new Answer(currAnswerId, currAnswerText, currAnswerRight));
            }

            currAnswers.Sort(delegate (Answer ainf1, Answer ainf2)
            {
                return ainf1.id.CompareTo(ainf2.id);
            });

            int currQuestionId = (int)Convert.ToUInt64(string.Format("{0}", question["id"]));
            string currQuestionText = string.Format("{0}", question["content"]);
            string currQuestionUrlImage = string.Format("{0}", question["image"]);

            QuestionsList.Add(new Question(currQuestionId, currQuestionText, currQuestionUrlImage, currAnswers, null));

            rightAnswersList.Add(new RightAnswer(currQuestionId, currAnswers.Find(x => x.rightAnswer == true).id));
        }

        QuestionsList.Sort(delegate (Question q1, Question q2)
        {
            return q1.id.CompareTo(q2.id);
        });

        return QuestionsList;
    }

    public bool CorrectAnswerSelected(int questionId, int selectedAnswerID)
    {
        return selectedAnswerID == rightAnswersList.Find(x => x.idQuestion == questionId).idRightAnswer;
    }

    public int GetCorrectAnswerIdByQuestionId(int questionId)
    {
        int correctNum = rightAnswersList.Find(x => x.idQuestion == questionId).idRightAnswer;
        return correctNum;
    }

    int GetBattleWithBotID(string url)
    {
        var dict = Json.Deserialize(url) as Dictionary<string, object>;
        int currBattleID = (int)Convert.ToUInt64((long)dict["battle_id"]);

        return currBattleID;
    }

    public void OnApplicationQuit()
    {
        if (linksScript.socketIOManager.foundBattleQuiz != null)
        {
            PostBattleStop();
        }
        else
        {
            PostBattleStopSearch();
        }
    }
}
