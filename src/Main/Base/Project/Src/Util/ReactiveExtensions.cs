// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;

using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.SharpDevelop
{
	/// <summary>
	/// Provides extension methods for IObservable&lt;T&gt;.
	/// </summary>
	public static class ReactiveExtensions
	{
		#region ObserveOnUIThread
		public static IObservable<T> ObserveOnUIThread<T>(this IObservable<T> source)
		{
			return new UIThreadObservable<T>(source, SD.MainThread.SynchronizationContext);
		}
		
		class UIThreadObservable<T> : IObservable<T>
		{
			readonly IObservable<T> source;
			readonly SynchronizationContext synchronizationContext;
			
			public UIThreadObservable(IObservable<T> source, SynchronizationContext synchronizationContext)
			{
				this.source = source;
				this.synchronizationContext = synchronizationContext;
			}
			
			public IDisposable Subscribe(IObserver<T> observer)
			{
				return source.Subscribe(value => synchronizationContext.Post(state => observer.OnNext((T)state), value),
				                        ex => synchronizationContext.Post(state => observer.OnError((Exception)state), ex),
				                        () => synchronizationContext.Post(state => observer.OnCompleted(), null));
			}
		}
		#endregion
		
		#region Create Observable from Task + Callback Delegate
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
		#endregion
		
		#region ToListAsync
		/// <summary>
		/// Converts the observable into a List.
		/// </summary>
		public static async Task<List<T>> ToListAsync<T>(this IObservable<T> source, CancellationToken cancellationToken)
		{
			var tcs = new TaskCompletionSource<List<T>>();
			List<T> results = new List<T>();
			using (source.Subscribe(item => results.Add(item),
			                        exception => tcs.TrySetException(exception),
			                        () => tcs.TrySetResult(results)))
			{
				using (cancellationToken.Register(() => tcs.SetCanceled())) {
					return await tcs.Task.ConfigureAwait(false);
				}
			}
		}
		#endregion
		
		#region AnonymousObserver
		public static IDisposable Subscribe<T>(this IObservable<T> source, Action<T> onNext, Action<Exception> onError, Action onCompleted)
		{
			return source.Subscribe(new AnonymousObserver<T>(onNext, onError, onCompleted));
		}
		
		class AnonymousObservable<T> : IObservable<T>
		{
			readonly Func<IObserver<T>, IDisposable> subscribe;
			
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
		#endregion
	}
}
