using UnityEngine;
using System.Collections;
using System.Collections.Generic;


namespace Assets.Script.Humans {

	public class Job
	{
        public bool IsActive;
		public string Name;

		Human human;
        List<Task> _tasks;
		int _activeTask;
		bool _isRepeated;

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
			_isRepeated = isRepeated;
			human = h;
		}

		public void StartJob()
		{
			if (_tasks.Count == 0) return;
            _tasks[_activeTask].OnStopTask += OnTaskComplete;
            human.SetTask(_tasks[0]);
		}

		public void AddTaskToJob(Task newTask, bool stopCurrentTask)
		{
			_tasks.Add(newTask);
			if (stopCurrentTask)
			{
				_tasks[_activeTask].OnStopTask?.Invoke();
			}
		}

		public void Update(double deltaTime)
		{
			_tasks[_activeTask].UpdateTask(human, deltaTime);
		}

		public void FixedUpdate(double deltaTime)
		{
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

            if (_isRepeated && _activeTask == _tasks.Count)
            {
                _activeTask = 0;
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