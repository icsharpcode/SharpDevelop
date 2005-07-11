using System;
using System.Collections.Generic;
using System.Text;

namespace CustomSinks
{
	public class EventForwarder:MarshalByRefObject
	{
		Delegate realEvent;

		public EventForwarder(Delegate realEvent)
		{
			this.realEvent = realEvent;
		}

		public void ForwardEvent0()
		{
			realEvent.DynamicInvoke(new object[] {});
		}

		public void ForwardEvent1(object p1)
		{
			realEvent.DynamicInvoke(new object[] { p1});
		}

		public void ForwardEvent2(object p1, object p2)
		{
			realEvent.DynamicInvoke(new object[] { p1, p2});
		}

		public void ForwardEvent3(object p1, object p2, object p3)
		{
			realEvent.DynamicInvoke(new object[] { p1, p2, p3});
		}

		public void ForwardEvent4(object p1, object p2, object p3, object p4)
		{
			realEvent.DynamicInvoke(new object[] { p1, p2, p3, p4});
		}

		public void ForwardEvent5(object p1, object p2, object p3, object p4, object p5)
		{
			realEvent.DynamicInvoke(new object[] { p1, p2, p3, p4, p5});
		}

		public void ForwardEvent6(object p1, object p2, object p3, object p4, object p5, object p6)
		{
			realEvent.DynamicInvoke(new object[] { p1, p2, p3, p4, p5, p6});
		}

		public void ForwardEvent7(object p1, object p2, object p3, object p4, object p5, object p6, object p7)
		{
			realEvent.DynamicInvoke(new object[] { p1, p2, p3, p4, p5, p6, p7});
		}

		public void ForwardEvent8(object p1, object p2, object p3, object p4, object p5, object p6, object p7, object p8)
		{
			realEvent.DynamicInvoke(new object[] { p1, p2, p3, p4, p5, p6, p7, p8});
		}

		public void ForwardEvent9(object p1, object p2, object p3, object p4, object p5, object p6, object p7, object p8, object p9)
		{
			realEvent.DynamicInvoke(new object[] { p1, p2, p3, p4, p5, p6, p7, p8, p9});
		}

		public void ForwardEvent10(object p1, object p2, object p3, object p4, object p5, object p6, object p7, object p8, object p9, object p10)
		{
			realEvent.DynamicInvoke(new object[] { p1, p2, p3, p4, p5, p6, p7, p8, p9, p10});
		}
	}
}
