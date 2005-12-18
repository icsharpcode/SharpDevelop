// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="none" email=""/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Drawing;
using System.Collections.Generic;
using System.Globalization;

namespace ICSharpCode.NRefactory.Parser
{
	public class PreProcessingDirective : AbstractSpecial
	{
		public static void VBToCSharp(List<ISpecial> list)
		{
			for (int i = 0; i < list.Count; ++i) {
				if (list[i] is PreProcessingDirective)
					list[i] = VBToCSharp((PreProcessingDirective)list[i]);
			}
		}
		
		public static PreProcessingDirective VBToCSharp(PreProcessingDirective dir)
		{
			string cmd = dir.Cmd.ToLower(CultureInfo.InvariantCulture);
			string arg = dir.Arg;
			switch (cmd) {
				case "#end":
					if (arg.ToLower(CultureInfo.InvariantCulture).StartsWith("region")) {
						cmd = "#endregion";
						arg = "";
					}
					break;
			}
			return new PreProcessingDirective(cmd, arg, dir.StartPosition, dir.EndPosition);
		}
		
		public static void CSharpToVB(List<ISpecial> list)
		{
			for (int i = 0; i < list.Count; ++i) {
				if (list[i] is PreProcessingDirective)
					list[i] = CSharpToVB((PreProcessingDirective)list[i]);
			}
		}
		
		public static PreProcessingDirective CSharpToVB(PreProcessingDirective dir)
		{
			string cmd = dir.Cmd;
			string arg = dir.Arg;
			switch (cmd) {
				case "#region":
					cmd = "#Region";
					if (!arg.StartsWith("\"")) {
						arg = "\"" + arg.Trim() + "\"";
					}
					break;
				case "#endregion":
					cmd = "#End";
					arg = "Region";
					break;
			}
			return new PreProcessingDirective(cmd, arg, dir.StartPosition, dir.EndPosition);
		}
		
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
