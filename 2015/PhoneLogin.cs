using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System;

public class PhoneLogin : MonoBehaviour
{
    public LinksManager linksScript;
    List<string> sendPhoneNumberResultList = new List<string>();
    List<string> createAccountResultList = new List<string>();

    public Text phoneNumberText;
    public Text confirmationCodeText;
    public GameObject telNumAlreadyExists;
    public GameObject erorrCodeConfLabel;
    public Button btnSendSMS;
    public Button btnCreateAccount;
    public Text countdownTimerText;
    public GameObject sendSMSAgainLink;
    public GameObject addTextTitleResendSMS;

    public WWW POST(string url, string phoneNumber, string resend, Action onComplete)
    {
        WWWForm form = new WWWForm();

        form.AddField("test_id", 1);
        form.AddField("phone", phoneNumber);

        if (resend == "true")
        {
            form.AddField("resend", resend);
        }

        WWW www = new WWW(url, form.data);

        StartCoroutine(SendPhoneNumber(www, onComplete));
        return www;
    }

    private IEnumerator SendPhoneNumber(WWW www, Action onComplete)
    {
        yield return www;
        if (www.error == null)
        {
            sendPhoneNumberResultList = ProcessSendPhoneNumberResponse(www.text);

            www.Dispose();
            www = null;

            onComplete();
        }
        else
        {
            Debug.Log("ERROR SendPhoneNumber: " + www.error);
        }
    }

    List<string> ProcessSendPhoneNumberResponse(string url)
    {
        sendPhoneNumberResultList.Clear();

        JSONObject jsonObject = new JSONObject(url);
        Debug.Log(jsonObject);

        string currResult = linksScript.socketIOManager.JsonToString(jsonObject.list[0].ToString(), "\"");
        sendPhoneNumberResultList.Add(currResult);

        if (jsonObject.list.Count > 1)
        {
            string currTextResult = linksScript.socketIOManager.JsonToString(jsonObject.list[1][0].ToString(), "\"");
            sendPhoneNumberResultList.Add(currTextResult);
        }

        return sendPhoneNumberResultList;
    }


    public void PostSendPhoneNumber(string targetURL, string phoneNumber, string resend)
    {
        POST(targetURL, phoneNumber, resend, () =>
        {
            if (sendPhoneNumberResultList[0] == "ok")
            {
                sendSMSAgainLink.SetActive(false);
                countdownTimerText.gameObject.SetActive(true);
                addTextTitleResendSMS.SetActive(true);

                btnCreateAccount.interactable = true;

                linksScript.loginRegManager.InputConfirmationCode();
                btnSendSMS.interactable = false;
                StartCoroutine(linksScript.timerManager.TimerCountdownForRegForm(0, 3));
            }

            else if (sendPhoneNumberResultList[0] == "error")
            {
                btnCreateAccount.interactable = false;

                if (sendPhoneNumberResultList[1] == "already_taken")
                {
                    telNumAlreadyExists.GetComponent<Text>().text = "Номер уже был зарегистрирован ранее.\nВоспользуйтесь страницей входа.";
                    telNumAlreadyExists.SetActive(true);
                }

                else if (sendPhoneNumberResultList[1] == "not_found")
                {
                    telNumAlreadyExists.GetComponent<Text>().text = "Номер телефона не найден.";
                    telNumAlreadyExists.SetActive(true);
                }
            }
        });
    }

    public WWW POSTCheckConfCode(string url, string phoneNumber, string confirmationCode, Action onComplete)
    {
        WWWForm form = new WWWForm();

        form.AddField("test_id", 1);
        form.AddField("phone", phoneNumber);
        form.AddField("code", confirmationCode);

        WWW www = new WWW(url, form.data);

        StartCoroutine(SendConfirmationCode(www, onComplete));
        return www;
    }

    private IEnumerator SendConfirmationCode(WWW www, Action onComplete)
    {
        yield return www;
        if (www.error == null)
        {
            createAccountResultList = ProcessCreateAccountResponse(www.text);

            if (createAccountResultList[0] == "ok")
            {
                linksScript.loginRegManager.newToken = linksScript.loginRegManager.ProcessToken(www.text);
            }

            www.Dispose();
            www = null;

            onComplete();
        }
        else
        {
            Debug.Log("ERROR SendConfirmationCode: " + www.error);
        }
    }

    public void PostCreateAccount(string targetURL, string phoneNumber, string confirmationCode)
    {
        POSTCheckConfCode(targetURL, phoneNumber, confirmationCode, () =>
        {
            if (createAccountResultList[0] == "ok")
            {
                linksScript.loginRegManager.LaunchGameAfterReg();
            }

            else if (createAccountResultList[0] == "error")
            {
                if (createAccountResultList[1] == "code_wrong")
                {
                    erorrCodeConfLabel.GetComponent<Text>().text = "Неверный код.";
                    erorrCodeConfLabel.SetActive(true);
                }

                else if (createAccountResultList[1] == "not_exist")
                {
                    erorrCodeConfLabel.GetComponent<Text>().text = "Номер телефона не найден.";
                    erorrCodeConfLabel.SetActive(true);
                }
            }
        });
    }

    List<string> ProcessCreateAccountResponse(string url)
    {
        createAccountResultList.Clear();

        JSONObject jsonObject = new JSONObject(url);
        Debug.Log(jsonObject);

        string currResult = linksScript.socketIOManager.JsonToString(jsonObject.list[0].ToString(), "\"");
        createAccountResultList.Add(currResult);

        if (jsonObject.list.Count > 1)
        {
            if (jsonObject.keys[1] == "token")
            {
                linksScript.loginRegManager.newToken = linksScript.socketIOManager.JsonToString(jsonObject.list[1].ToString(), "\"");
            }

            else if (jsonObject.keys[1] == "errors")
            {
                string currTextResult = linksScript.socketIOManager.JsonToString(jsonObject.list[1][0].ToString(), "\"");
                createAccountResultList.Add(currTextResult);
            }
        }

        return createAccountResultList;
    }
}
