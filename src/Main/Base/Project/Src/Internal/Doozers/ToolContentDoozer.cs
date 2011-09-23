using System;
using System.Reflection;
using ICSharpCode.Core;

namespace Internal.Doozers
{
	public class ToolContentDoozer : IDoozer
	{
		private String _name;
		private String _className;
		private String _holder;
		
		public bool HandleConditions
		{
			get{ return false; }
		}
		
		public object BuildItem(BuildItemArgs args)
		{
			return new ToolsPadDescriptor(args.Codon);
		}
	}
}
