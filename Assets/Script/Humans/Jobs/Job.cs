using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace Assets.Script.Humans {

	public class Job
	{
        public bool IsActive;
		public string Name;
		public Action<Human> onJobComplete;
        public bool IsRepeated { get; private set; }

        Human human;
        List<Task> _tasks;
		int _activeTask;
		

		public Job(Human h)
		{
			IsActive = false;
			human = h;
		}

		public Job(Human h, string name, List<Task> jobs, bool isRepeated)
		{
			Name = name;
			IsActive = true;
			_tasks = jobs;
			IsRepeated = isRepeated;
			human = h;
		}

		public void StartJob()
		{
			if (_tasks.Count == 0) return;
            _tasks[_activeTask].OnStopTask += OnTaskComplete;
            human.SetTask(_tasks[0]);
		}

		public void StopJob()
		{
			_tasks[_activeTask].OnStopTask -= OnTaskComplete;
			_tasks[_activeTask].StopTask();
			IsActive = false;
		}

		public void AddTaskToJob(Task newTask, bool stopCurrentTask)
		{
			if (stopCurrentTask)
			{
				_tasks[_activeTask].OnStopTask?.Invoke();
			}
            _tasks.Add(newTask);
            if (_activeTask == _tasks.Count-1)
			{
                human.SetTask(_tasks[_activeTask]);
                _tasks[_activeTask].OnStopTask += OnTaskComplete;
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

        void OnTaskComplete()
        {
            if (_tasks.Count > 0)
            {
                _tasks[_activeTask].OnStopTask -= OnTaskComplete;
                _tasks[_activeTask].StopTask();
                _activeTask++;
            }

			if (_activeTask == _tasks.Count)
			{
				onJobComplete?.Invoke(human);
				if (IsRepeated) _activeTask = 0;
			}

            //jobText.text = "";
            if (_activeTask < _tasks.Count)
            {
				//human.StartJob(_tasks[_activeTask]);
				human.SetTask(_tasks[_activeTask]);
                _tasks[_activeTask].OnStopTask += OnTaskComplete;
            }
        }

    }
}