using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharpDevelop.XamlDesigner.Commanding
{
	public interface IHasCommands
	{
		List<CommandBase> Commands { get; }
	}
}
