${StandardHeader.C#}

using System;   
using System.Configuration;

namespace ${StandardNamespace}
{
	/// <summary>
	/// Configuration settings for ${ClassName}.
	/// </summary>
public class ${ClassName}Section : ConfigurationSection
	{
		/// <summary>
		/// Collection of tables (auctually views) to generate 
		/// reports from.
		/// </summary>
		[ConfigurationProperty("customSection", IsDefaultCollection = true)]
		public ${ClassName}Collection ${ClassName}
		{
			get { return (${ClassName}Collection) base["customSection"]; }
		}
	}
}

