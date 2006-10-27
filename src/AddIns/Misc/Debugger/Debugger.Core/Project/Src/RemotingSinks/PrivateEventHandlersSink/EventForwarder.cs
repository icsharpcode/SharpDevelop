// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Reflection;
using System.Windows.Forms;

namespace CustomSinks
{
	public delegate void EventForwarderEventHandler(object sender, EventForwarderEventArgs args);

	public class EventForwarderEventArgs:EventArgs
	{
		bool invoked = false;
		Delegate realEvent;
		object[] args;

		public bool WasInvoked {
			get {
				return invoked;
			}
		}

		public void InvokeEvent()
		{
			if (invoked) {
				return;
			}

			realEvent.DynamicInvoke(args);

			invoked = true;
		}

		public EventForwarderEventArgs(Delegate realEvent, object[] args)
		{
			this.realEvent = realEvent;
			this.args = args;
		}
	}

	public class EventForwarder:MarshalByRefObject
	{
		public static event EventForwarderEventHandler EventReceived;

		Delegate realDelegate;
		Delegate proxyDelegate;

		public Delegate ProxyDelegate {
			get {
				return proxyDelegate;
			}
		}

		public EventForwarder(Delegate realDelegate)
		{
			this.realDelegate = realDelegate;

			Type type = realDelegate.GetType();
			MethodInfo proxyMethod = typeof(EventForwarder).GetMethod("ForwardEvent" + realDelegate.Method.GetParameters().Length);
			proxyDelegate = Delegate.CreateDelegate(type, this, proxyMethod);
		}

		void InvokeEvent(params object[] par)
		{
			Console.WriteLine("Remoting event received: " + realDelegate.Method.Name);
			EventForwarderEventArgs args = new EventForwarderEventArgs(realDelegate, par);

			if (EventReceived != null) {
				EventReceived(this, args);
			}
			
			// Invoke event if it was not already invoked
			if (!args.WasInvoked) {
				if (Application.OpenForms.Count > 0) {
					Application.OpenForms[0].Invoke(new EventHandler(delegate
					{
						args.InvokeEvent();
					}));
				} else {
					args.InvokeEvent();
				}
			}
		}

		public void ForwardEvent0()
		{
			InvokeEvent(new object[] {});
		}

		public void ForwardEvent1(object p1)
		{
			InvokeEvent(new object[] { p1});
		}

		public void ForwardEvent2(object p1, object p2)
		{
			InvokeEvent(new object[] { p1, p2});
		}

		public void ForwardEvent3(object p1, object p2, object p3)
		{
			InvokeEvent(new object[] { p1, p2, p3});
		}

		public void ForwardEvent4(object p1, object p2, object p3, object p4)
		{
			InvokeEvent(new object[] { p1, p2, p3, p4});
		}

		public void ForwardEvent5(object p1, object p2, object p3, object p4, object p5)
		{
			InvokeEvent(new object[] { p1, p2, p3, p4, p5});
		}

		public void ForwardEvent6(object p1, object p2, object p3, object p4, object p5, object p6)
		{
			InvokeEvent(new object[] { p1, p2, p3, p4, p5, p6});
		}

		public void ForwardEvent7(object p1, object p2, object p3, object p4, object p5, object p6, object p7)
		{
			InvokeEvent(new object[] { p1, p2, p3, p4, p5, p6, p7});
		}

		public void ForwardEvent8(object p1, object p2, object p3, object p4, object p5, object p6, object p7, object p8)
		{
			InvokeEvent(new object[] { p1, p2, p3, p4, p5, p6, p7, p8});
		}

		public void ForwardEvent9(object p1, object p2, object p3, object p4, object p5, object p6, object p7, object p8, object p9)
		{
			InvokeEvent(new object[] { p1, p2, p3, p4, p5, p6, p7, p8, p9});
		}

		public void ForwardEvent10(object p1, object p2, object p3, object p4, object p5, object p6, object p7, object p8, object p9, object p10)
		{
			InvokeEvent(new object[] { p1, p2, p3, p4, p5, p6, p7, p8, p9, p10});
		}
	}
}
