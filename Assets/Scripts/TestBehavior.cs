using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Reflection;

public class TestBehavior : MonoBehaviour
{
    Parent c1;
    Child1 c2;
    MethodInfo method;
    void Start()
    {
        c1 = new Child1();
        c2 = new();
        Func(c1);
        Func(c2);
    }

    // Update is called once per frame
    void Update()
    {
    }

    void Func(Parent p) {
        Debug.Log("Called parent");
    }

    void Func(Child1 c)
    {
        Debug.Log("Called child");
    }
}

class Parent { 

}

class Child1 : Parent{ 

}
