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
    public const float POSTURE_BROKEN_DAMAGE_MODIFIER = 2;
    public const float POSTURE_BROKEN_DURATION = 2;
    public const float SUPPRESS_REGEN_DURATION = 2;
}


