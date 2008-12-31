using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Windows.Markup;

namespace SharpDevelop.XamlDesigner.Dom
{
	public class EventId : MemberId
	{
		internal EventId(EventDescriptor d)
		{
			Descriptor = d;
		}

		public EventDescriptor Descriptor { get; private set; }

		public override string Name
		{
			get { return Descriptor.Name; }
		}

		public override Type OwnerType
		{
			get { return Descriptor.ComponentType; }
		}

		public override Type ValueType
		{
			get { return typeof(string); }
		}

		public override bool IsReadOnly
		{
			get { return false; }
		}

		public override ValueSerializer ValueSerializer
		{
			get { return ValueSerializer.GetSerializerFor(typeof(string)); }
		}
	}
}
