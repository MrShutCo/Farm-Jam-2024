using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FibonacciHeap;
using System;
public interface IPriorityQueue<T, in TKey> where TKey : IComparable<TKey>
{
    void Insert(T item, TKey priority);
    T Top();
    T Pop();
}
public class PriorityQueue<TElement, TPriority> : IPriorityQueue<TElement, TPriority>
    where TPriority : IComparable<TPriority>
{
    private readonly FibonacciHeap<TElement, TPriority> heap;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="minPriority">Minimum value of the priority - to be used for comparing.</param>
    public PriorityQueue(TPriority minPriority)
    {
        heap = new FibonacciHeap<TElement, TPriority>(minPriority);
    }

    public void Insert(TElement item, TPriority priority)
    {
        heap.Insert(new FibonacciHeapNode<TElement, TPriority>(item, priority));
    }

    public TElement Top()
    {
        return heap.Min().Data;
    }

    public TElement Pop()
    {
        return heap.RemoveMin().Data;
    }
}


struct PriorityItem<T>
{
    public int element;
    public int priority;
}

