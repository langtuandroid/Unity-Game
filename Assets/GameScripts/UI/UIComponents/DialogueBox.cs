using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DialogueBox
{
    [Header("Dialogue Area")]
    [Tooltip("Main text area for displaying the dialogue")]
    [SerializeField] private TMP_Text mainText;

    [Header("Response Area")]
    [Tooltip("Layout group panel that governs the response buttons")]
    [SerializeField] private RectTransform responseLayoutGroup;
    [Tooltip("Button Template for displaying the responses, should be disabled before running.")]
    [SerializeField] private Button responseButtonTemplate;

    [Header("Speaker icon / name")]
    [SerializeField] private TMP_Text speakerName;
    [SerializeField] private Image speakerIcon;

    public TMP_Text MainText { 
        get { return mainText; }
    }

    public RectTransform ResponseLayoutGroup { 
        get { return responseLayoutGroup; }
    }

    public Button ResponseButtonTemplate { 
        get { return responseButtonTemplate; }
    }

    public TMP_Text SpeakerName { 
        get { return speakerName; }
    }

    public Image SpeakerIcon { 
        get { return speakerIcon; }
    }
}
