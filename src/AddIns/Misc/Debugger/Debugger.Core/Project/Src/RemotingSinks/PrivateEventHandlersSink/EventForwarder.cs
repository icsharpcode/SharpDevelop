using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace CustomSinks
{
	public class EventForwarder:MarshalByRefObject
	{
		Delegate realEvent;

		public EventForwarder(Delegate realEvent)
		{
			this.realEvent = realEvent;
		}

		void DynamicInvoke(params object[] par)
		{
			// TODO: Walkaround - custom sinks do not work properly in same Process - they are ignored
			if (Application.OpenForms.Count > 0) {
				Application.OpenForms[0].Invoke(new EventHandler(delegate
				{
					realEvent.DynamicInvoke(par);
				}));
			} else {
				realEvent.DynamicInvoke(par);
			}
			//realEvent.DynamicInvoke(par);
		}

		public void ForwardEvent0()
		{
			DynamicInvoke(new object[] {});
		}

		public void ForwardEvent1(object p1)
		{
			DynamicInvoke(new object[] { p1});
		}

		public void ForwardEvent2(object p1, object p2)
		{
			DynamicInvoke(new object[] { p1, p2});
		}

		public void ForwardEvent3(object p1, object p2, object p3)
		{
			DynamicInvoke(new object[] { p1, p2, p3});
		}

		public void ForwardEvent4(object p1, object p2, object p3, object p4)
		{
			DynamicInvoke(new object[] { p1, p2, p3, p4});
		}

		public void ForwardEvent5(object p1, object p2, object p3, object p4, object p5)
		{
			DynamicInvoke(new object[] { p1, p2, p3, p4, p5});
		}

		public void ForwardEvent6(object p1, object p2, object p3, object p4, object p5, object p6)
		{
			DynamicInvoke(new object[] { p1, p2, p3, p4, p5, p6});
		}

		public void ForwardEvent7(object p1, object p2, object p3, object p4, object p5, object p6, object p7)
		{
			DynamicInvoke(new object[] { p1, p2, p3, p4, p5, p6, p7});
		}

		public void ForwardEvent8(object p1, object p2, object p3, object p4, object p5, object p6, object p7, object p8)
		{
			DynamicInvoke(new object[] { p1, p2, p3, p4, p5, p6, p7, p8});
		}

		public void ForwardEvent9(object p1, object p2, object p3, object p4, object p5, object p6, object p7, object p8, object p9)
		{
			DynamicInvoke(new object[] { p1, p2, p3, p4, p5, p6, p7, p8, p9});
		}

		public void ForwardEvent10(object p1, object p2, object p3, object p4, object p5, object p6, object p7, object p8, object p9, object p10)
		{
			DynamicInvoke(new object[] { p1, p2, p3, p4, p5, p6, p7, p8, p9, p10});
		}
	}
}
