${StandardHeader.C#}
using System;   
using System.Configuration;

namespace ${StandardNamespace}
{
	/// <summary>
	/// Represents a single XML tag inside a ConfigurationSection
	/// or a ConfigurationElementCollection.
	/// </summary>
	public sealed class ${ClassName}Element : ConfigurationElement
	{
		/// <summary>
		/// The attribute <c>name</c> of a <c>${ClassName}Element</c>.
		/// </summary>
		[ConfigurationProperty("name", IsKey = true, IsRequired = true)]
		public string Name
		{
			get { return (string)this["name"]; }
			set { this["name"] = value; }
		}
	
	
		/// <summary>
		/// A demonstration of how to use a boolean property.
		/// </summary>
		[ConfigurationProperty("special")]
		public bool IsSpecial {
			get { return (bool)this["special"]; }
			set { this["special"] = value; }
		}
	}
	
}

