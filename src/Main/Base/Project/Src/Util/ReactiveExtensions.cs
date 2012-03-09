// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.SharpDevelop
{
	/// <summary>
	/// Provides extension methods for IObservable&lt;T&gt;.
	/// </summary>
	public static class ReactiveExtensions
	{
		public static IObservable<T> ObserveOnUIThread<T>(this IObservable<T> source)
		{
			return new AnonymousObservable<T>(
				observer => source.Subscribe(
					new AnonymousObserver<T>(
						value => WorkbenchSingleton.SafeThreadAsyncCall(delegate { observer.OnNext(value); }),
						exception => WorkbenchSingleton.SafeThreadAsyncCall(delegate { observer.OnError(exception); }),
						() => WorkbenchSingleton.SafeThreadAsyncCall(delegate { observer.OnCompleted(); })
					)
				)
			);
		}
		
		public static IObservable<T> CreateObservable<T>(Func<CancellationToken, Action<T>, Task> func)
		{
			return new AnonymousObservable<T>(observer => new TaskToObserverSubscription<T>(func, observer));
		}
		
		public static IObservable<T> CreateObservable<T>(Func<IProgressMonitor, Action<T>, Task> func, IProgressMonitor progressMonitor)
		{
			return new AnonymousObservable<T>(observer => new TaskToObserverSubscription<T>(func, progressMonitor, observer));
		}
		
		sealed class TaskToObserverSubscription<T> : IDisposable
		{
			readonly CancellationTokenSource cts = new CancellationTokenSource();
			readonly object syncLock = new object();
			readonly IObserver<T> observer;
			readonly IProgressMonitor childProgressMonitor;
			readonly IProgressMonitor progressMonitor;
			
			public TaskToObserverSubscription(Func<CancellationToken, Action<T>, Task> func, IObserver<T> observer)
			{
				this.observer = observer;
				func(cts.Token, Callback).ContinueWith(TaskCompleted);
			}
			
			public TaskToObserverSubscription(Func<IProgressMonitor, Action<T>, Task> func, IProgressMonitor progressMonitor, IObserver<T> observer)
			{
				this.observer = observer;
				this.progressMonitor = progressMonitor;
				this.childProgressMonitor = progressMonitor.CreateSubTask(1, cts.Token);
				func(childProgressMonitor, Callback).ContinueWith(TaskCompleted);
			}
			
			void Callback(T item)
			{
				// Needs lock because callbacks may be called in parallel, but OnNext may not.
				lock (syncLock)
					observer.OnNext(item);
			}
			
			void TaskCompleted(Task task)
			{
				if (childProgressMonitor != null)
					childProgressMonitor.Dispose();
				if (progressMonitor != null)
					progressMonitor.Dispose();
				if (task.Exception != null)
					observer.OnError(task.Exception.InnerExceptions[0]);
				else
					observer.OnCompleted();
			}
			
			public void Dispose()
			{
				cts.Cancel();
			}
		}
		
		public static IDisposable Subscribe<T>(this IObservable<T> source, Action<T> onNext, Action<Exception> onError, Action onCompleted)
		{
			return source.Subscribe(new AnonymousObserver<T>(onNext, onError, onCompleted));
		}
		
		public static List<T> ToList<T>(this IObservable<T> source, CancellationToken cancellation)
		{
			List<T> results = new List<T>();
			ManualResetEventSlim ev = new ManualResetEventSlim();
			Exception error = null;
			using (source.Subscribe(item => results.Add(item), exception => { error = exception; ev.Set(); }, () => ev.Set()))
				ev.Wait(cancellation);
			if (error != null)
				throw new TargetInvocationException(error);
			return results;
		}
		
		class AnonymousObservable<T> : IObservable<T>
		{
			Func<IObserver<T>, IDisposable> subscribe;
			
			public AnonymousObservable(Func<IObserver<T>, IDisposable> subscribe)
			{
				this.subscribe = subscribe;
			}
			
			public IDisposable Subscribe(IObserver<T> observer)
			{
				return subscribe(observer);
			}
		}
		
		class AnonymousObserver<T> : IObserver<T>
		{
			Action<T> onNext;
			Action<Exception> onError;
			Action onCompleted;
			
			public AnonymousObserver(Action<T> onNext, Action<Exception> onError, Action onCompleted)
			{
				if (onNext == null)
					throw new ArgumentNullException("onNext");
				if (onError == null)
					throw new ArgumentNullException("onError");
				if (onCompleted == null)
					throw new ArgumentNullException("onCompleted");
				this.onNext = onNext;
				this.onError = onError;
				this.onCompleted = onCompleted;
			}
			
			public void OnNext(T value)
			{
				onNext(value);
			}
			
			public void OnError(Exception error)
			{
				onError(error);
			}
			
			public void OnCompleted()
			{
				onCompleted();
			}
		}
	}
}
