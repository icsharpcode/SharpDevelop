using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace ICSharpCode.Xaml
{
	[Serializable]
	public class XamlException : Exception
	{
		public XamlException()
		{
		}

		public XamlException(string message)
			: base(message)
		{
		}

		public XamlException(string message, Exception innerException)
			: base(message, innerException)
		{
		}

		protected XamlException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}

		public int LineNumber { get; set; }
		public int LinePosition { get; set; }
		public Uri BaseUri { get; set; }
	}
}
