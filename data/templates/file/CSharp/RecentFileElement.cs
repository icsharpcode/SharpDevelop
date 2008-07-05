${StandardHeader.C#}
using System;   
using System.Configuration;

namespace ${StandardNamespace}
{
	/// <summary>
	/// An xml representation of a recent element.
	/// </summary>
	public sealed class RecentFileElement : ConfigurationElement
	{
		/// <summary>
		/// The full path of the recently opened file.
		/// </summary>
		[ConfigurationProperty("fileName", IsKey = true, IsRequired = true)]
		public string Name
		{
			get { return (string)this["fileName"]; }
			set { this["fileName"] = value; }
		}
		
		internal RecentFileElement() : base()
		{}
		
		internal RecentFileElement(string FileName)  : base()
		{
			this.Name = FileName;
		}
	}
	
}

