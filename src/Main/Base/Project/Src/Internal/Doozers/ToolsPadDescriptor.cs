using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using ICSharpCode.Core;

namespace Internal.Doozers
{
	public class ToolsPadDescriptor
	{
		private ICSharpCode.Core.Codon _codon;
		private string _class;
		private string _id;

		public string Holder
		{
			get;
			private set;
		}

		public ToolsPadDescriptor(ICSharpCode.Core.Codon codon)
		{
			_id = codon.Id;
			_class = codon.Properties["class"];
			Holder = codon.Properties["holder"];
			_codon = codon;
		}

		internal object GetToolBoxContent()
		{
			Type singletonType = _codon.AddIn.FindType(_class);

			if (singletonType != null)
			{
				PropertyInfo getInstance = singletonType.GetProperty("Instance");
				if (getInstance == null)
				{
					LoggingService.Error(String.Format("Tool content {0} related to {1} view content, should be a singelton class", singletonType, Holder));
					return null;
				}
				return getInstance.GetValue(null, null);
			}

			LoggingService.Error(String.Format("Class {0} was not found", _class));
			return null;
		}
	}
}
