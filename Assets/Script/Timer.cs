using System;
public class Timer
{
	public Action OnTrigger;
	public float SecondsToTrigger { get; private set; }

	bool isRunning;
	float timePassed;

	public Timer(float secondsToTrigger, bool _isRunning)
	{
		SecondsToTrigger = secondsToTrigger;
		timePassed = 0;
		isRunning = _isRunning;
	}

	public void Toggle()
	{
		isRunning = !isRunning;
		timePassed = 0;
	}

	public void Update(float deltaTime)
	{
		if (!isRunning) return;
		timePassed += deltaTime;
		if (timePassed > SecondsToTrigger)
		{
			OnTrigger?.Invoke();
			timePassed = 0;
		}
	}
}

