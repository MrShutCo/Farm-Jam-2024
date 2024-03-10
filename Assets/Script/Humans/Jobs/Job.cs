using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

namespace Assets.Script.Humans
{

	public class Job
	{
		public bool IsActive;
		public string Name;
		public Action<Human, bool> onJobComplete;
		public bool IsRepeated { get; private set; }

		Human human;
		private Rigidbody2D _humanRigidBody;
		private Animator _humanAnimatorBody;
		List<Task> _tasks = new();
		int _activeTask;
		public string ActiveTaskText()
		{
			if (_tasks == null || _tasks.Count == 0) return "";
			return _tasks.Last().Name;
		}

		public Job(Human h, string name, List<Task> jobs, bool isRepeated)
		{
			Name = name;
			IsActive = false;
			_tasks = jobs;
			IsRepeated = isRepeated;
			human = h;
			_humanRigidBody = human.GetComponentInChildren<Rigidbody2D>();
			_humanAnimatorBody = human.anim;
		}

		public void StartJob()
		{
			if (_tasks.Count == 0) return;
			_tasks[_activeTask].OnStopTask += OnTaskComplete;
			_tasks[_activeTask].StartTask(_humanRigidBody, _humanAnimatorBody);
			IsActive = true;
		}

		public void StopJob()
		{
			if (_tasks.Count == 0) return;
			_tasks[_activeTask].OnStopTask -= OnTaskComplete;
			_tasks[_activeTask].StopTask();
			IsActive = false;
		}

		public void AddTaskToJob(Task newTask, bool stopCurrentTask)
		{
			_tasks.Add(newTask);
			if (stopCurrentTask && _tasks.Count > 0)
			{
				_tasks[_activeTask].OnStopTask?.Invoke(true);
			}
		}

		public void Update(double deltaTime)
		{
			if (!IsActive) return;
			_tasks[_activeTask].UpdateTask(human, deltaTime);
		}

		public void FixedUpdate(double deltaTime)
		{
			if (!IsActive) return;
			_tasks[_activeTask].FixedUpdateTask(human, deltaTime);
		}

		void OnTaskComplete(bool wasSuccessful)
		{
			if (_tasks.Count > 0)
			{
				_tasks[_activeTask].OnStopTask -= OnTaskComplete;
				_tasks[_activeTask].StopTask();
			}

			if (!wasSuccessful)
			{
				onJobComplete?.Invoke(human, false);
			}

			// If theres no more tasks, job is complete
			if (_activeTask >= _tasks.Count - 1)
			{
				onJobComplete?.Invoke(human, true);
				if (IsRepeated) _activeTask = 0;
				else return;
			}
			else
			{
				_activeTask++;
			}

			//jobText.text = "";
			if (_activeTask < _tasks.Count)
			{
				_tasks[_activeTask].OnStopTask += OnTaskComplete;
				_tasks[_activeTask].StartTask(_humanRigidBody, _humanAnimatorBody);
			}
		}

	}
}