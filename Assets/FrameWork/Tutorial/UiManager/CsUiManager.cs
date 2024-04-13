﻿using System.Collections;
using System.Collections.Generic;
using FrameWork;
using UnityEngine;

public class CsUiManager : MonoBehaviour
{
    private UiActor _uiActor;
    // Start is called before the first frame update
    void Start()
    {
        var type = DllLoad.GetHoyUpdateDllType("FrameWork.InputCs");
        _uiActor=UiManager.Instance.ShowUi(type);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            EventManager.DispatchEvent(MessageType.UiMessage,UiMessageType.Show,new object[]{_uiActor.GetIndex()});
        }
        
        if (Input.GetKeyDown(KeyCode.S))
        {
            EventManager.DispatchEvent(MessageType.UiMessage,UiMessageType.Hide,new object[]{_uiActor.GetIndex()});
        }
        
        if (Input.GetKeyDown(KeyCode.D))
        {
            EventManager.DispatchEvent(MessageType.UiMessage,UiMessageType.Remove,new object[]{_uiActor.GetIndex()});
        }
    }
}
