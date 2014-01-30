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
		/// <summary>
		/// Converts an async function that produces a result collection using callbacks,
		/// into an IObservable.
		/// </summary>
		/// <param name="func">
		/// The async function will run once for each subscription on the observable.
		/// </param>
		/// <remarks>
		/// The async function gets passed a cancellation token. This token will get signalled when the observable subscription is cancelled.
		/// </remarks>
		public static IObservable<T> CreateObservable<T>(Func<CancellationToken, Action<T>, Task> func)
		{
			return new AnonymousObservable<T>(observer => new TaskToObserverSubscription<T>(func, observer));
		}
		
		/// <summary>
		/// Converts an async function that produces a result collection using callbacks,
		/// into an IObservable.
		/// </summary>
		/// <param name="func">
		/// The async function will run once for each subscription on the observable.
		/// </param>
		/// <param name="progressMonitor">
		/// The progress monitor used to report progress. The monitor will be disposed after the async function
		/// completes.
		/// The async function is given a child monitor of this progress monitor: it forwards all
		/// calls to the parent progress monitor, but uses a cancellation token that gets signalled
		/// when the observable subscription is cancelled.
		/// </param>
		/// <remarks>
		/// Multiple subscriptions on the observable do not work well together with progress reporting.
		/// </remarks>
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
				func(cts.Token, Callback).ContinueWith(TaskCompleted).FireAndForget();
			}
			
			public TaskToObserverSubscription(Func<IProgressMonitor, Action<T>, Task> func, IProgressMonitor progressMonitor, IObserver<T> observer)
			{
				this.observer = observer;
				this.progressMonitor = progressMonitor;
				this.childProgressMonitor = progressMonitor.CreateSubTask(1, cts.Token);
				func(childProgressMonitor, Callback).ContinueWith(TaskCompleted).FireAndForget();
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
		
		#region First/Single/Last
		public static Task<T> FirstAsync<T>(this IObservable<T> source, CancellationToken cancellationToken = default(CancellationToken))
		{
			return FirstInternalAsync(source, cancellationToken, true);
		}
		
		public static Task<T> FirstOrDefaultAsync<T>(this IObservable<T> source, CancellationToken cancellationToken = default(CancellationToken))
		{
			return FirstInternalAsync(source, cancellationToken, false);
		}
		
		static async Task<T> FirstInternalAsync<T>(IObservable<T> source, CancellationToken cancellationToken, bool throwIfEmpty)
		{
			var tcs = new TaskCompletionSource<T>();
			Action onCompleted;
			if (throwIfEmpty)
				onCompleted = () => tcs.TrySetException(new InvalidOperationException());
			else
				onCompleted = () => tcs.TrySetResult(default(T));
			using (source.Subscribe(item => tcs.TrySetResult(item),
			                        exception => tcs.TrySetException(exception),
			                        onCompleted))
			{
				using (cancellationToken.Register(() => tcs.TrySetCanceled())) {
					return await tcs.Task.ConfigureAwait(false);
				}
			}
		}
		
		public static Task<T> SingleAsync<T>(this IObservable<T> source, CancellationToken cancellationToken = default(CancellationToken))
		{
			return SingleInternalAsync(source, cancellationToken, true);
		}
		
		public static Task<T> SingleOrDefaultAsync<T>(this IObservable<T> source, CancellationToken cancellationToken = default(CancellationToken))
		{
			return SingleInternalAsync(source, cancellationToken, false);
		}
		
		static async Task<T> SingleInternalAsync<T>(IObservable<T> source, CancellationToken cancellationToken, bool throwIfEmpty)
		{
			SemaphoreSlim gate = new SemaphoreSlim(0);
			T value = default(T);
			bool isEmpty = true;
			Exception ex = null;
			using (source.Subscribe(
				item => {
					if (isEmpty) {
						value = item;
						isEmpty = false;
					} else {
						ex = new InvalidOperationException("Sequence contains more than one element.");
						gate.Release();
					}
				},
				exception => {
					ex = exception;
					gate.Release();
				},
				() => {
					gate.Release();
				}))
			{
				await gate.WaitAsync(cancellationToken).ConfigureAwait(false);
			}
			if (ex != null)
				throw ex;
			if (isEmpty && throwIfEmpty)
				throw new InvalidOperationException("Sequence contains no elements.");
			return value;
		}
		
		public static Task<T> LastAsync<T>(this IObservable<T> source, CancellationToken cancellationToken = default(CancellationToken))
		{
			return LastInternalAsync(source, cancellationToken, true);
		}
		
		public static Task<T> LastOrDefaultAsync<T>(this IObservable<T> source, CancellationToken cancellationToken = default(CancellationToken))
		{
			return LastInternalAsync(source, cancellationToken, false);
		}
		
		static async Task<T> LastInternalAsync<T>(IObservable<T> source, CancellationToken cancellationToken, bool throwIfEmpty)
		{
			SemaphoreSlim gate = new SemaphoreSlim(0);
			T value = default(T);
			bool isEmpty = true;
			Exception ex = null;
			using (source.Subscribe(
				item => {
					value = item;
					isEmpty = false;
				},
				exception => {
					ex = exception;
					gate.Release();
				},
				() => {
					gate.Release();
				}))
			{
				await gate.WaitAsync(cancellationToken).ConfigureAwait(false);
			}
			if (ex != null)
				throw ex;
			if (isEmpty && throwIfEmpty)
				throw new InvalidOperationException("Sequence contains no elements.");
			return value;
		}
		#endregion
		
		#region ToListAsync
		/// <summary>
		/// Converts the observable into a List.
		/// </summary>
		public static async Task<List<T>> ToListAsync<T>(this IObservable<T> source, CancellationToken cancellationToken = default(CancellationToken))
		{
			var tcs = new TaskCompletionSource<List<T>>();
			List<T> results = new List<T>();
			using (source.Subscribe(item => results.Add(item),
			                        exception => tcs.TrySetException(exception),
			                        () => tcs.TrySetResult(results)))
			{
				using (cancellationToken.Register(() => tcs.TrySetCanceled())) {
					return await tcs.Task.ConfigureAwait(false);
				}
			}
		}
		#endregion
		
		#region ForEach
		/// <summary>
		/// Subscribes to the observable and runs an action for each element.
		/// </summary>
		public static async Task ForEachAsync<T>(this IObservable<T> source, Action<T> action)
		{
			TaskCompletionSource<object> tcs = new TaskCompletionSource<object>();
			using (source.Subscribe(delegate (T item) {
			                        	try {
			                        		action(item);
			                        	} catch (Exception ex) {
			                        		tcs.TrySetException(ex);
			                        	}
			                        },
			                        exception => tcs.TrySetException(exception),
			                        () => tcs.TrySetResult(null)))
			{
				await tcs.Task.ConfigureAwait(false);
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
