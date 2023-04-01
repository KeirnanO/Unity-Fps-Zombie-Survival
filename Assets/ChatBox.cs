using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ChatBox : MonoBehaviour
{
    public static ChatBox instance;
    public GameObject chatboxObject;

    [SerializeField] List<ChatMessage> messages = new List<ChatMessage>();
    [SerializeField] TextMeshProUGUI messageBoxText;
    [SerializeField] TMP_InputField inputField;

    NetworkPlayerController localPlayer;

    float totalMessages = 14;

    float visibleLength = 3f;
    float timeToClose;

    private void Start()
    {
        instance = this;
    }

    private void Update()
    {
        //Handles Open Chat
        if (inputField.gameObject.activeSelf)
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                SubmitMessage();
                inputField.gameObject.SetActive(false);
            }

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                inputField.gameObject.SetActive(false);
            }

        }

        //Opens Chat Input
        if (Input.GetKeyDown(KeyCode.T))
        {
            if (!inputField.gameObject.activeSelf)
            {
                inputField.gameObject.SetActive(true);
                inputField.ActivateInputField();
            }
        }
    }

    public void AddMessage(string name, string message)
    {
        ChatMessage newMessage = new ChatMessage(name, message);

        messages.Add(newMessage);

        if (messages.Count > totalMessages)
            messages.Remove(messages[0]);

        SetMessageBoxText();
    }

    void SetMessageBoxText()
    {
        string chatString = "";

        foreach(ChatMessage message in messages)
        {   
            if(message.GetName() != "")
            {
                chatString += ("[" + message.GetName() + "] : " + message.GetMessage() + "\n");
            }
            else
            {
                chatString += (message.GetMessage() + "\n");
            }

            
        }

        messageBoxText.text = chatString;
    }

    void EnableField()
    {
        inputField.Select();
    }

    void SubmitMessage()
    {

        if (inputField.text == "")
            return;

        if(localPlayer)
            localPlayer.SendChatMessage(inputField.text);

        inputField.text = "";
    }

    public void SendServerMessage(string text)
    {
        localPlayer.SendServerMessage(text);
    }

    void SetActive(bool isActive)
    {
        chatboxObject.SetActive(isActive);
    }

    public void SetPlayer(NetworkPlayerController player)
    {
        localPlayer = player;
    }
}
