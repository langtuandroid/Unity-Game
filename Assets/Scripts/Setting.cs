using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using System;

public static class Setting
{
    // Game Settings
    public const int TARGET_FRAME_RATE = 120;

    // Attack info duration (seconds)
    public const float EXPIRE_ATTACK_TIME = 3;

    // Tags
    public const string TAG_ENTITY = "entity";
    public const string TAG_PROJECTILE = "projectile";
    public static string TAG_PLAYER = "player";
    public static string TAG_INTERACTABLE = "interactable";
    public static string TAG_INTERACTOR = "interactor";

    // 
    public const int ENTITY_DNE = -1;

    public enum CoroutineStatus
    {
        COROUTINE_OFF = 0,
        COROUTINE_ON = 1
    }
}


