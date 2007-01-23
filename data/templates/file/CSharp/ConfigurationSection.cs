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
		/// Collection of <c>${ClassName}Element(s)</c> 
		/// A custom XML section for an applications configuration file.
		/// </summary>
		[ConfigurationProperty("customSection", IsDefaultCollection = true)]
		public ${ClassName}Collection ${ClassName}
		{
			get { return (${ClassName}Collection) base["customSection"]; }
		}


		/// <summary>
		/// Default Constructor.
		/// </summary>
		public PlaneDisasterSection () : base () {
			//Allow this section to be stored in user.app. By default this is forbidden.
			this.SectionInformation.AllowExeDefinition =
				ConfigurationAllowExeDefinition.MachineToLocalUser;
		}

	}
}

