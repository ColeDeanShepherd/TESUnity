using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

/// <summary>
/// Distributes work across frames.
/// </summary>
public class TemporalLoadBalancer
{
	public IEnumerator AddTask(IEnumerator taskCoroutine)
	{
		tasks.Add(taskCoroutine);

		return taskCoroutine;
	}
	public void CancelTask(IEnumerator taskCoroutine)
	{
		tasks.Remove(taskCoroutine);
	}
	public void RunTasks(float desiredWorkTime)
	{
		Debug.Assert(desiredWorkTime >= 0);

		if(tasks.Count == 0)
		{
			return;
		}

		stopwatch.Reset();
		stopwatch.Start();

		// Run at least one iteration of a task.
		do
		{
			if(!tasks[0].MoveNext())
			{
				tasks.RemoveAt(0);
			}
		} while((tasks.Count > 0) && (stopwatch.Elapsed.TotalSeconds < desiredWorkTime));

		stopwatch.Stop();
	}
	public void WaitForTask(IEnumerator taskCoroutine)
	{
		Debug.Assert(tasks.Contains(taskCoroutine));

		while(taskCoroutine.MoveNext()) { }

		tasks.Remove(taskCoroutine);
	}
	public void WaitForAllTasks()
	{
		while(tasks.Count > 0)
		{
			RunTasks(float.MaxValue);
		}
	}

	private List<IEnumerator> tasks = new List<IEnumerator>();
	private Stopwatch stopwatch = new Stopwatch();
}