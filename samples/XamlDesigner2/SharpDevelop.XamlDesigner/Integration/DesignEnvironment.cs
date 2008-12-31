using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharpDevelop.XamlDesigner.Dom
{
	public class DesignEnvironment
	{
		public static DesignEnvironment Instance = new DesignEnvironment();

		public int ParseDelay = 3000;

		public virtual string GetDocumentation(MemberId member)
		{
			return null;
		}

		public virtual void CreateEventHandler(DesignProperty property)
		{
		}

		public virtual IFileWatcher CreateWatcher()
		{
			return null;
		}
	}
}
