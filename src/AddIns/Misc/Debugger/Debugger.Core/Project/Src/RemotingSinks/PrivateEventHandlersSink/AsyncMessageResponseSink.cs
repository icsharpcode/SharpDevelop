// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;
using System.Threading;

namespace CustomSinks
{
	class AsyncMessageResponseSink: IMessageSink
	{
		IMessage msgResponse;

		EventWaitHandle responseReceived = new EventWaitHandle(false, EventResetMode.ManualReset, "Waiting for response");
		EventWaitHandle messageLoop;

		static List<EventWaitHandle> messageLoopsSTA = new List<EventWaitHandle>();

		static object storeOneEventLock = new object();
		static object invokeEventOnceLock = new object();
		static EventForwarderEventArgs storedEvent;
		static EventWaitHandle recheckEventStatus = new EventWaitHandle(false, EventResetMode.AutoReset, "Waiting for remoting event to be handled");

		static AsyncMessageResponseSink()
		{
			// We may want to invoke some events ourseves
			EventForwarder.EventReceived += new EventForwarderEventHandler(OnEventReceived);
			// From now on, OnEventReceived can be called at ANY time; even more times at once!
		}

		public AsyncMessageResponseSink()
		{
			if (Thread.CurrentThread.GetApartmentState() != ApartmentState.MTA) {
				messageLoop = new EventWaitHandle(false, EventResetMode.AutoReset, "Waiting for message loop to be pumped");
				lock (messageLoopsSTA) {
					messageLoopsSTA.Add(messageLoop);
				}
			}
		}

		public IMessage WaitForResponse()
		{
			// We are going to invoke events only if we are running on ApartmentState.STA

			if (Thread.CurrentThread.GetApartmentState() == ApartmentState.MTA) {
				responseReceived.WaitOne();
			} else {
				
				// Message loop:
				while (true) {
					Thread.Sleep(100);
					//WaitHandle.WaitAny(new WaitHandle[] {messageLoop, responseReceived});
					//responseReceived.WaitOne(100, false);
					if (msgResponse != null) break;
					System.Windows.Forms.Application.DoEvents();
					InvokeStoredEvent();
				};
				// We have got the response now

				lock (messageLoopsSTA) {
					messageLoopsSTA.Remove(messageLoop);
				}

				// We might be the last message loop - this would release the event
				recheckEventStatus.Set();				
			}
			return msgResponse;
		}

		// This function can be called at any time any number of times
		void InvokeStoredEvent()
		{
			lock (invokeEventOnceLock)
			{
				if (storedEvent != null) {
					storedEvent.InvokeEvent();
					recheckEventStatus.Set();
				}
			}
		}

		// This function can be called at any time any number of times
		static void OnEventReceived(object sender, EventForwarderEventArgs args)
		{
			lock (storeOneEventLock) // Store just one event at a time
			{
				while ((messageLoopsSTA.Count > 0) && // While there is someone who can invoke us
					   (args.WasInvoked == false))    // and while the event was not invoked yet
				{
					// Store event
					storedEvent = args;
					// Pump all STA message loops
					lock (messageLoopsSTA) {
						foreach (EventWaitHandle loop in messageLoopsSTA) {
							loop.Set();
						}
					}

					// Wait until we are told to check conditions again
					recheckEventStatus.WaitOne(); 
				}
			} 
		}

		IMessage IMessageSink.SyncProcessMessage(IMessage msg)
		{
			if (System.Threading.Thread.CurrentThread.Name == null) {
				System.Threading.Thread.CurrentThread.Name = "Response thread";
			}
			Console.WriteLine("Remoting response: " + msg.Properties["__MethodName"]);
			msgResponse = msg;
			responseReceived.Set();
			return msgResponse;
		}

		IMessageCtrl IMessageSink.AsyncProcessMessage(IMessage msg, IMessageSink replySink)
		{
			return null; // Should not be called
		}

		IMessageSink IMessageSink.NextSink {
			get {
				return null; // Should not be called
			}
		}
	}
}
