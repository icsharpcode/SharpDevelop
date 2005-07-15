using System;
using System.Drawing;
using System.Text;
using System.CodeDom;
using System.Collections;

namespace ICSharpCode.NRefactory.Parser
{
	public class PreProcessingDirective : AbstractSpecial
	{
		string cmd;
		string arg;
		
		public string Cmd {
			get {
				return cmd;
			}
			set {
				cmd = value;
			}
		}
		
		public string Arg {
			get {
				return arg;
			}
			set {
				arg = value;
			}
		}
		
		public override string ToString()
		{
			return String.Format("[PreProcessingDirective: Cmd = {0}, Arg = {1}]",
			                     Cmd,
			                     Arg);
		}
		
		public PreProcessingDirective(string cmd, string arg, Point start, Point end)
			: base(start, end)
		{
			this.cmd = cmd;
			this.arg = arg;
		}
		
		public override object AcceptVisitor(ISpecialVisitor visitor, object data)
		{
			return visitor.Visit(this, data);
		}
	}
}
