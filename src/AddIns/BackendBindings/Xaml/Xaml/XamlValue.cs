using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace ICSharpCode.Xaml
{
	public abstract class XamlValue : XamlNode
	{
		public object Instance;

		public MemberNode ParentMember
		{
			get { return ParentNode as MemberNode; }
		}

		public ObjectNode ParentObject
		{
			get { return ParentMember != null ? ParentMember.ParentObject : null; }
		}
	}
}
