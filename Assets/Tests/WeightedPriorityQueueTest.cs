using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class WeightedPriorityQueueTest
{
    // A Test behaves as an ordinary method
    [Test]
    public void TestQueue1()
    {
        // Use the Assert class to test conditions
        // Test argument: WeightedPriorityQueue.Enqueue(Value, Priority, Weight)
        WeightedPriorityQueue<int> queue = new WeightedPriorityQueue<int>();
        queue.Enqueue(10, 1, 0);
        queue.Enqueue(12, 1, 1);
        queue.Enqueue(7, 1, 0);
        string expected = "[{Value: 7, Priority: 0, Data: 1}, {Value: 10, Priority: 0, Data: 1}, {Value: 12, Priority: 1, Data: 1}]";
        Assert.AreEqual(expected, queue.ToString());
        int item = queue.Dequeue();
        Assert.AreEqual(7, item);
        expected = "[{Value: 10, Priority: 0, Data: 1}, {Value: 12, Priority: 1, Data: 1}]";
        Assert.AreEqual(expected, queue.ToString());
        item = queue.Dequeue();
        Assert.AreEqual(10, item);
        expected = "[{Value: 12, Priority: 1, Data: 1}]";
        Assert.AreEqual(expected, queue.ToString());
        item = queue.Dequeue();
        Assert.AreEqual(12, item);
        Assert.AreEqual("[]", queue.ToString());
    }

    [Test]
    public void TestQueue2() {
        WeightedPriorityQueue<int> queue = new WeightedPriorityQueue<int>();
        queue.Enqueue(10, 3, 0);
        queue.Enqueue(12, 1, 1);
        queue.Enqueue(7, 1, 0);
        string expected = "[{Value: 7, Priority: 0, Data: 1}, {Value: 10, Priority: 0, Data: 3}, {Value: 12, Priority: 1, Data: 1}]";
        Assert.AreEqual(expected, queue.ToString());
        int item = queue.Dequeue();
        Assert.AreEqual(7, item);
        expected = "[{Value: 10, Priority: 0, Data: 3}, {Value: 12, Priority: 1, Data: 1}]";
        Assert.AreEqual(expected, queue.ToString());
        item = queue.Dequeue();
        Assert.AreEqual(10, item);
        expected = "[{Value: 10, Priority: 0, Data: 2}, {Value: 12, Priority: 1, Data: 1}]";
        Assert.AreEqual(expected, queue.ToString());
        item = queue.Dequeue();
        Assert.AreEqual(12, item);
        expected = "[{Value: 10, Priority: 0, Data: 2}]";
        Assert.AreEqual(expected, queue.ToString());
        item = queue.Dequeue();
        Assert.AreEqual(10, item);
        expected = "[{Value: 10, Priority: 0, Data: 1}]";
        Assert.AreEqual(expected, queue.ToString());
    }

    [Test]
    public void TestQueue3()
    {
        WeightedPriorityQueue<int> queue = new WeightedPriorityQueue<int>();
        int key = queue.Enqueue(10, 3, 0);
        queue.Enqueue(12, 1, 1);
        queue.Enqueue(7, 1, 0);

        string expected = "[{Value: 7, Priority: 0, Data: 1}, {Value: 10, Priority: 0, Data: 3}, {Value: 12, Priority: 1, Data: 1}]";
        Assert.AreEqual(expected, queue.ToString());

        int item = queue.Dequeue();
        Assert.AreEqual(7, item);
        expected = "[{Value: 10, Priority: 0, Data: 3}, {Value: 12, Priority: 1, Data: 1}]";
        Assert.AreEqual(expected, queue.ToString());

        item = queue.Dequeue();
        Assert.AreEqual(10, item);
        expected = "[{Value: 10, Priority: 0, Data: 2}, {Value: 12, Priority: 1, Data: 1}]";
        Assert.AreEqual(expected, queue.ToString());

        queue.SetElementData(key, 5);
        expected = "[{Value: 10, Priority: 0, Data: 5}, {Value: 12, Priority: 1, Data: 1}]";
        Assert.AreEqual(expected, queue.ToString());

        queue.SetElementValue(key, 3);
        expected = "[{Value: 3, Priority: 0, Data: 5}, {Value: 12, Priority: 1, Data: 1}]";
        Assert.AreEqual(expected, queue.ToString());

        item = queue.Dequeue();
        Assert.AreEqual(12, item);
        expected = "[{Value: 3, Priority: 0, Data: 5}]";
        Assert.AreEqual(expected, queue.ToString());

        item = queue.Dequeue();
        Assert.AreEqual(3, item);
        expected = "[{Value: 3, Priority: 0, Data: 4}]";
        Assert.AreEqual(expected, queue.ToString());

        queue.Dequeue();
        queue.Dequeue();
        queue.Dequeue();
        expected = "[{Value: 3, Priority: 0, Data: 1}]";
        Assert.AreEqual(expected, queue.ToString());

        queue.Dequeue();
        Assert.AreEqual("[]", queue.ToString());
    }

    [Test]
    public void TestQueue4()
    {
        WeightedPriorityQueue<int> queue = new WeightedPriorityQueue<int>();
        int key = queue.Enqueue(10, 3, 0);
        queue.Enqueue(12, 1, 1);

        string expected = "[{Value: 10, Priority: 0, Data: 3}, {Value: 12, Priority: 1, Data: 1}]";
        Assert.AreEqual(expected, queue.ToString());

        int item = queue.Dequeue();
        Assert.AreEqual(10, item);
        expected = "[{Value: 10, Priority: 0, Data: 2}, {Value: 12, Priority: 1, Data: 1}]";
        Assert.AreEqual(expected, queue.ToString());

        queue.SetElementData(key, -1);
        expected = "[{Value: 10, Priority: 0, Data: -1}, {Value: 12, Priority: 1, Data: 1}]";
        Assert.AreEqual(expected, queue.ToString());

        queue.SetElementValue(key, 3);
        expected = "[{Value: 3, Priority: 0, Data: -1}, {Value: 12, Priority: 1, Data: 1}]";
        Assert.AreEqual(expected, queue.ToString());

        item = queue.Dequeue();
        Assert.AreEqual(12, item);
        expected = "[{Value: 3, Priority: 0, Data: -1}]";
        Assert.AreEqual(expected, queue.ToString());
    }

    [Test]
    public void TestQueue5()
    {
        WeightedPriorityQueue<int> queue = new WeightedPriorityQueue<int>();
        int key = queue.Enqueue(10, 3, 0);
        queue.Enqueue(12, 1, 1);

        string expected = "[{Value: 10, Priority: 0, Data: 3}, {Value: 12, Priority: 1, Data: 1}]";
        Assert.AreEqual(expected, queue.ToString());

        queue.SetElementData(key, -1);
        expected = "[{Value: 10, Priority: 0, Data: -1}, {Value: 12, Priority: 1, Data: 1}]";
        Assert.AreEqual(expected, queue.ToString());

        int item = queue.Dequeue();
        Assert.AreEqual(12, item);
        expected = "[]";
        Assert.AreEqual(expected, queue.ToString());
    }

    // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
    // `yield return null;` to skip a frame.
    [UnityTest]
    public IEnumerator WeightedPriorityQueueTestWithEnumeratorPasses()
    {
        // Use the Assert class to test conditions.
        // Use yield to skip a frame.
        yield return null;
    }
}
