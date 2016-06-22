// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using System.Windows.Threading;

namespace ICSharpCode.Profiler.Controls
{
	class SingleTask {
		Task currentTask;
		Dispatcher dispatcher;
		
		public SingleTask(Dispatcher dispatcher)
		{
			if (dispatcher == null)
				throw new ArgumentNullException("dispatcher");
			this.dispatcher = dispatcher;
			this.currentTask = null;
		}
		
		// task = local reference to the task instance of the current thread
		// currentUpdateTask = reference to the currently executed task
		//
		// Consider two tasks 1 and 2 running in parallel:
		//
		// These are the only two possible execution orders for two parallel updates.
		// Update and Complete cannot execute in parallel because they both run on the UI Thread
		//
		// Update(1)
		//  currentUpdateTask = 1
		// Update(2)
		//  currentUpdateTask = 2
		// Complete(1)
		//  currentUpdateTask = 2
		// Complete(2)
		//  currentUpdateTask = null
		//
		// Update(1)
		//  currentUpdateTask = 1
		// Update(2)
		//  currentUpdateTask = 2
		// Complete(2)
		//  currentUpdateTask = null
		// Complete(1)
		//  currentUpdateTask = null
		
		public void Cancel()
		{
			dispatcher.VerifyAccess();
			
			if (currentTask != null) {
				currentTask.Cancel();
				currentTask = null;
			}
		}

		public void Execute(Action backgroundAction, Action completionAction, Action failedAction)
		{
			if (backgroundAction == null)
				throw new ArgumentNullException("backgroundAction");
			
			Cancel();
			
			Task task = Task.Start(backgroundAction);
			currentTask = task;
			currentTask.RunWhenComplete(
				dispatcher,
				() => {
					// do not use task.IsCancelled because we do not
					// want to raise completionAction if the task
					// was successfully completed but another task
					// was started before we received the completion callback.
					if (currentTask == task) {
						currentTask = null;
						if (completionAction != null)
							completionAction();
					} else {
						if (failedAction != null)
							failedAction();
					}
				}
			);
		}
		
		public void Execute<T>(Func<T> backgroundAction, Action<T> completionAction, Action failedAction)
		{
			if (backgroundAction == null)
				throw new ArgumentNullException("backgroundAction");
			if (completionAction == null)
				throw new ArgumentNullException("completionAction");
			
			T returnValue = default(T);
			Execute(delegate { returnValue = backgroundAction(); },
			        delegate { completionAction(returnValue); },
			        failedAction
			       );
		}
	}

}
