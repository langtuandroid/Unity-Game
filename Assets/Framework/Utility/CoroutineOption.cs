using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LobsterFramework
{
    public class CoroutineOption
    {
        public bool exit;
        public bool reset;
        public float waitTime;

        public static CoroutineOption Exit = new CoroutineOption(0, true);
        public static CoroutineOption Reset = new CoroutineOption(0, false, true);

        public static CoroutineOption Wait(float time)
        {
            return new CoroutineOption(time);
        }

        public CoroutineOption(float time, bool exit = false, bool reset = false)
        {
            waitTime = time;
            this.reset = reset;
            this.exit = exit;
        }
    }
}
