using System;
using System.Drawing;

namespace ICSharpCode.NRefactory.Parser
{
	public class BlankLine : AbstractSpecial
	{
		public BlankLine(Point point) : base(point)
		{
		}
		
		public override object AcceptVisitor(ISpecialVisitor visitor, object data)
		{
			return visitor.Visit(this, data);
		}
	}
}
