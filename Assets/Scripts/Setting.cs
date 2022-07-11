using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Setting
{
    // Game Settings
    public const int TARGET_FRAME_RATE = 120;

    // Attack info duration (seconds)
    public const float EXPIRE_ATTACK_TIME = 10f;

    // Tags
    public const string TAG_ENTITY = "entity";
    public const string TAG_PROJECTILE = "projectile";
    public static string TAG_PLAYER = "player";

    // Id distribute keys
    public const string ID_ACTION = "action";
    public const string ID_QUEST = "quest";

    // 
    public const int ENTITY_DNE = -1;
    public const string NULL_COMPONENT_IDENTIFIER = "";

    // Action Arguments
    public const string HANDLING_COMPONENT = "_component";
    public const string IGNORE_TARGET = "ignore";

    // Action Ids

        // Std moveset
        public const string STD_CIRCLE_ATTACK = "circle_attack";
        public const string STD_GUARD = "guard";

    // Action Priorities
    public const int ACTION_ATTACK_PRIORITY = 1;
    public const int ACTION_DEFEND_PRIORITY = 2;

    // Action Cooldowns (seconds)
    public const float CD_CIRCLE_ATTACK = 1f;
    public const float CD_CIRCLE_GUARD = 1f;

    // Action Durations  (Frames)
    public const int DURATION_CIRCLE_ATTACK = 1;
    public const int DURATION_GUARD = TARGET_FRAME_RATE / 2;

    // Action Animation Durations  (Seconds)
    public const float ADURATION_GUARD = 0.5f;
}
