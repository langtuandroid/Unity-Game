using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Setting;

public static class ActionDelegates
{
    public static ActionExecutor Executor(string id) {
        return id switch
        {
            STD_CIRCLE_ATTACK => StandardMoveSet.CircleAttack,
            STD_GUARD => StandardMoveSet.Guard,
            _ => null,
        };
    }

    public static ActionTerminator Terminator(string id)
    {
        return id switch
        {
            STD_GUARD => StandardMoveSet.ExitGuard,
            _ => StandardMoveSet.Exit,
        };
    }
}
