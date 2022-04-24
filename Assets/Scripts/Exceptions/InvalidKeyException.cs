using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvalidKeyException : Exception
{
    public InvalidKeyException() { 
           
    }

    public InvalidKeyException(string message) : base(string.Format("Invalid key used for weighted priority queue: %s\n", message)) { }
}
