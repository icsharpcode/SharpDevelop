using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ICSharpCode.WpfDesign.Extensions
{
	[AttributeUsage(AttributeTargets.Class, Inherited = false)]
	public class ReplacementForAttribute : Attribute
	{
		public ReplacementForAttribute(Type targetType)
		{
			this.TargetType = targetType;
		}

		public Type TargetType { get; private set; }
	}
}
