using UnityEngine;

public class NetworkTimer
{
    float timer;
    public float MinTimeBetweenTicks { get; }
    public int CurrentTick { get; private set; }

    public NetworkTimer(float serverTickRate)
    {
        MinTimeBetweenTicks = 1f / serverTickRate;
    }

    public void Update(float deltaTime)
    {
        timer += deltaTime;
    }

    public bool ShouldTick()
    {
        if (timer >= MinTimeBetweenTicks)
        {
            timer -= MinTimeBetweenTicks;
            CurrentTick++;
            return true;
        }
        return false;
    }
}

public class CircularBuffer<T>
{
    T[] buffer;
    int bufferSize;
    int pointer;

    public CircularBuffer(int _bufferSize)
    {
        bufferSize = _bufferSize;
        buffer = new T[bufferSize];
    }

    public void Add(T item, int index)
    {
        pointer = index % bufferSize;
        buffer[pointer] = item;
    }
    public T Get(int index) => buffer[index % bufferSize];
    public bool TryGet(int index, out T value)
    {
        int i = index % bufferSize;
        if (i <= pointer || (pointer == 0 && index == bufferSize))
        {
            value = buffer[i];
            return true;
        }
        value = default;
        return false;
    }
    public void Clear()
    {
        pointer = 0;
        buffer = new T[bufferSize];
    }
}
