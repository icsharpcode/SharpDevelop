// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Siegfried Pammer" email="sie_pam@gmx.at"/>
//     <version>$Revision$</version>
// </file>

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
			
			if (this.currentTask != null) {
				this.currentTask.Cancel();
				this.currentTask = null;
			}
		}

		public void Execute(Action backgroundAction, Action completionAction, Action failedAction)
		{
			if (backgroundAction == null)
				throw new ArgumentNullException("backgroundAction");
			
			this.Cancel();
			
			Task task = Task.Start(backgroundAction);
			this.currentTask = task;
			currentTask.RunWhenComplete(
				dispatcher,
				() => {
					// do not use task.IsCancelled because we do not
					// want to raise completionAction if the task
					// was successfully completed but another task
					// was started before we received the completion callback.
					if (this.currentTask == task) {
						this.currentTask = null;
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
