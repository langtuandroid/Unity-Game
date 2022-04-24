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
        queue.Enqueue(10, 0, 1);
        queue.Enqueue(12, 1, 1);
        queue.Enqueue(7, 0, 1);
        string expected = "[{V: 7, P: 0, W: 1}, {V: 10, P: 0, W: 1}, {V: 12, P: 1, W: 1}]";
        Assert.AreEqual(expected, queue.ToString());
        int item = queue.Dequeue();
        Assert.AreEqual(7, item);
        expected = "[{V: 10, P: 0, W: 1}, {V: 12, P: 1, W: 1}]";
        Assert.AreEqual(expected, queue.ToString());
        item = queue.Dequeue();
        Assert.AreEqual(10, item);
        expected = "[{V: 12, P: 1, W: 1}]";
        Assert.AreEqual(expected, queue.ToString());
        item = queue.Dequeue();
        Assert.AreEqual(12, item);
        Assert.AreEqual("[]", queue.ToString());
    }

    [Test]
    public void TestQueue2() {
        WeightedPriorityQueue<int> queue = new WeightedPriorityQueue<int>();
        queue.Enqueue(10, 0, 3);
        queue.Enqueue(12, 1, 1);
        queue.Enqueue(7, 0, 1);
        string expected = "[{V: 7, P: 0, W: 1}, {V: 10, P: 0, W: 3}, {V: 12, P: 1, W: 1}]";
        Assert.AreEqual(expected, queue.ToString());
        int item = queue.Dequeue();
        Assert.AreEqual(7, item);
        expected = "[{V: 10, P: 0, W: 3}, {V: 12, P: 1, W: 1}]";
        Assert.AreEqual(expected, queue.ToString());
        item = queue.Dequeue();
        Assert.AreEqual(10, item);
        expected = "[{V: 10, P: 0, W: 2}, {V: 12, P: 1, W: 1}]";
        Assert.AreEqual(expected, queue.ToString());
        item = queue.Dequeue();
        Assert.AreEqual(12, item);
        expected = "[{V: 10, P: 0, W: 2}]";
        Assert.AreEqual(expected, queue.ToString());
        item = queue.Dequeue();
        Assert.AreEqual(10, item);
        expected = "[{V: 10, P: 0, W: 1}]";
        Assert.AreEqual(expected, queue.ToString());
    }

    [Test]
    public void TestQueue3()
    {
        WeightedPriorityQueue<int> queue = new WeightedPriorityQueue<int>();
        int key = queue.Enqueue(10, 0, 3);
        queue.Enqueue(12, 1, 1);
        queue.Enqueue(7, 0, 1);

        string expected = "[{V: 7, P: 0, W: 1}, {V: 10, P: 0, W: 3}, {V: 12, P: 1, W: 1}]";
        Assert.AreEqual(expected, queue.ToString());

        int item = queue.Dequeue();
        Assert.AreEqual(7, item);
        expected = "[{V: 10, P: 0, W: 3}, {V: 12, P: 1, W: 1}]";
        Assert.AreEqual(expected, queue.ToString());

        item = queue.Dequeue();
        Assert.AreEqual(10, item);
        expected = "[{V: 10, P: 0, W: 2}, {V: 12, P: 1, W: 1}]";
        Assert.AreEqual(expected, queue.ToString());

        queue.SetWeight(key, 5);
        expected = "[{V: 10, P: 0, W: 5}, {V: 12, P: 1, W: 1}]";
        Assert.AreEqual(expected, queue.ToString());

        queue.SetValue(key, 3);
        expected = "[{V: 3, P: 0, W: 5}, {V: 12, P: 1, W: 1}]";
        Assert.AreEqual(expected, queue.ToString());

        item = queue.Dequeue();
        Assert.AreEqual(12, item);
        expected = "[{V: 3, P: 0, W: 5}]";
        Assert.AreEqual(expected, queue.ToString());

        item = queue.Dequeue();
        Assert.AreEqual(3, item);
        expected = "[{V: 3, P: 0, W: 4}]";
        Assert.AreEqual(expected, queue.ToString());

        queue.Dequeue();
        queue.Dequeue();
        queue.Dequeue();
        expected = "[{V: 3, P: 0, W: 1}]";
        Assert.AreEqual(expected, queue.ToString());

        queue.Dequeue();
        Assert.AreEqual("[]", queue.ToString());
    }

    [Test]
    public void TestQueue4()
    {
        WeightedPriorityQueue<int> queue = new WeightedPriorityQueue<int>();
        int key = queue.Enqueue(10, 0, 3);
        queue.Enqueue(12, 1, 1);

        string expected = "[{V: 10, P: 0, W: 3}, {V: 12, P: 1, W: 1}]";
        Assert.AreEqual(expected, queue.ToString());

        int item = queue.Dequeue();
        Assert.AreEqual(10, item);
        expected = "[{V: 10, P: 0, W: 2}, {V: 12, P: 1, W: 1}]";
        Assert.AreEqual(expected, queue.ToString());

        queue.SetWeight(key, -1);
        expected = "[{V: 10, P: 0, W: -1}, {V: 12, P: 1, W: 1}]";
        Assert.AreEqual(expected, queue.ToString());

        queue.SetValue(key, 3);
        expected = "[{V: 3, P: 0, W: -1}, {V: 12, P: 1, W: 1}]";
        Assert.AreEqual(expected, queue.ToString());

        item = queue.Dequeue();
        Assert.AreEqual(12, item);
        expected = "[{V: 3, P: 0, W: -1}]";
        Assert.AreEqual(expected, queue.ToString());
    }

    [Test]
    public void TestQueue5()
    {
        WeightedPriorityQueue<int> queue = new WeightedPriorityQueue<int>();
        int key = queue.Enqueue(10, 0, 3);
        queue.Enqueue(12, 1, 1);

        string expected = "[{V: 10, P: 0, W: 3}, {V: 12, P: 1, W: 1}]";
        Assert.AreEqual(expected, queue.ToString());

        queue.SetWeight(key, -1);
        expected = "[{V: 10, P: 0, W: -1}, {V: 12, P: 1, W: 1}]";
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
