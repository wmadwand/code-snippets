using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using SocketIO;
using System;
using MiniJSON;

public class SocketIOManager : MonoBehaviour
{
    public LinksManager linksScript;

    private SocketIOComponent socketIO;
    public BattleQuiz foundBattleQuiz;
    public int currentRivalAnswerId;
    public int expUserCurrAnswer;
    public int expRivalCurrAnswer;

    public string token;

    void Start()
    {
        socketIO = GetComponent<SocketIOComponent>();

        socketIO.On("battle_found", OnBattleFound);

        if (linksScript.quizGameManager.quizGameMode == QuizGameManager.QuizGameMode.rivalNet)
        {
            socketIO.On("new_answer", OnNewAnswer);
        }

        StartCoroutine("SubscribeUser");
    }

    public IEnumerator SubscribeUser()
    {
        yield return new WaitForSeconds(1f);
        Debug.Log("SubscribeUser");

        string tokenString = "\"/activity/" + token + "\"";
        socketIO.Emit("subscribe", new JSONObject(tokenString));
    }

    public void OnBattleFound(SocketIOEvent obj)
    {
        int battleId = (int)Convert.ToUInt64(obj.data.GetField("battle")[0].ToString());

        int userId = (int)Convert.ToUInt64(obj.data.GetField("battle")[1][0].ToString());
        string userName = JsonToString(obj.data.GetField("battle")[1][1].ToString(), "\"");
        string userAvatarURL = JsonToString(obj.data.GetField("battle")[1][2].ToString(), "\"");

        int rivalId = (int)Convert.ToUInt64(obj.data.GetField("battle")[2][0].ToString());
        string rivalName = JsonToString(obj.data.GetField("battle")[2][1].ToString(), "\"");
        string rivalAvatarURL = JsonToString(obj.data.GetField("battle")[2][2].ToString(), "\"");

        foundBattleQuiz = new BattleQuiz(battleId, new UserQuiz(userId, userName, userAvatarURL), new UserQuiz(rivalId, rivalName, rivalAvatarURL));

        if (foundBattleQuiz != null)
        {
            linksScript.opponentsManager.ProcessOpponentsInfo(foundBattleQuiz);
            linksScript.dashboardManager.ShowVSPanel(foundBattleQuiz);
        }
    }

    public void OnNewAnswer(SocketIOEvent obj)
    {
        int currentUserId = (int)Convert.ToUInt64(obj.data.GetField("user")[0].ToString());

        if (currentUserId == foundBattleQuiz.rivalQuiz.id)
        {
            int currentRivalAnswerId = (int)Convert.ToUInt64(obj.data.GetField("answer")[0].ToString());
            linksScript.rivalManager.RivalAnswer(currentRivalAnswerId);

            expRivalCurrAnswer = (int)Convert.ToUInt64(obj.data.GetField("answer")[3].ToString());
            linksScript.rivalManager.AddRivalScore(expRivalCurrAnswer);
            Debug.Log("RIVAL EXP:" + expRivalCurrAnswer);
        }

        if (currentUserId == foundBattleQuiz.userQuiz.id)
        {
            expUserCurrAnswer = (int)Convert.ToUInt64(obj.data.GetField("answer")[3].ToString());
            linksScript.quizGameManager.AddUserScore(expUserCurrAnswer);
            Debug.Log("USER EXP:" + expUserCurrAnswer);
        }
    }

    public string JsonToString(string target, string s)
    {
        string[] newString = Regex.Split(target, s);
        return newString[1];
    }
}