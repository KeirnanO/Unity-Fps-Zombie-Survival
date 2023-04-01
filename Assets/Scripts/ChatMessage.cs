using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChatMessage
{
    string name;
    string message;

    public ChatMessage(string _name, string _message)
    {
        name = _name;
        message = _message;
    }

    public string GetName()
    {
        return name;
    }

    public string GetMessage()
    {
        return message;
    }
}
