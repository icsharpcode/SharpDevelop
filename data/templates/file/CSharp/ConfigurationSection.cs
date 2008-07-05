${StandardHeader.C#}
using System;   
using System.Configuration;

namespace ${StandardNamespace}
{
	/// <summary>
	/// Configuration section &lt;${ClassName}&gt;
	/// </summary>
	/// <remarks>
	/// Assign properties to your child class that has the attribute 
	/// <c>[ConfigurationProperty]</c> to store said properties in the xml.
	/// </remarks>
	public sealed class ${ClassName}Settings : ConfigurationSection
	{
		System.Configuration.Configuration _Config;


		#region ConfigurationProperties
		
		/*
		 *  Uncomment the following section and add a Configuration Collection 
		 *  from the with the file named ${ClassName}.cs
		 */
		// /// <summary>
		// /// A custom XML section for an application's configuration file.
		// /// </summary>
		// [ConfigurationProperty("customSection", IsDefaultCollection = true)]
		// public ${ClassName}Collection ${ClassName}
		// {
		// 	get { return (${ClassName}Collection) base["customSection"]; }
		// }

		/// <summary>
		/// Collection of <c>${ClassName}Element(s)</c> 
		/// A custom XML section for an applications configuration file.
		/// </summary>
		[ConfigurationProperty("exampleAttribute", DefaultValue="exampleValue")]
		public string ExampleAttribute {
			get { return (string) this["exampleAttribute"]; }
			set { this["exampleAttribute"] = value; }
		}

		#endregion

		/// <summary>
		/// Private Constructor used by our factory method.
		/// </summary>
		private ${ClassName}Settings () : base () {
			// Allow this section to be stored in user.app. By default this is forbidden.
			this.SectionInformation.AllowExeDefinition =
				ConfigurationAllowExeDefinition.MachineToLocalUser;
		}

		#region Public Methods
		
		/// <summary>
		/// Saves the configuration to the config file.
		/// </summary>
		public void Save() {
			_Config.Save();
		}
		
		#endregion
		
		#region Static Members
		
		/// <summary>
		/// Gets the current applications &lt;${ClassName}&gt; section.
		/// </summary>
		/// <param name="ConfigLevel">
		/// The &lt;ConfigurationUserLevel&gt; that the config file
		/// is retrieved from.
		/// </param>
		/// <returns>
		/// The configuration file's &lt;${ClassName}&gt; section.
		/// </returns>
		public static ${ClassName}Settings GetSection (ConfigurationUserLevel ConfigLevel) {
			/* 
			 * This class is setup using a factory pattern that forces you to
			 * name the section &lt;${ClassName}&gt; in the config file.
			 * If you would prefer to be able to specify the name of the section,
			 * then remove this method and mark the constructor public.
			 */ 
			System.Configuration.Configuration Config = ConfigurationManager.OpenExeConfiguration
				(ConfigLevel);
			${ClassName}Settings o${ClassName}Settings;
			
			o${ClassName}Settings =
				(${ClassName}Settings)Config.GetSection("${ClassName}Settings");
			if (o${ClassName}Settings == null) {
				o${ClassName}Settings = new ${ClassName}Settings();
				Config.Sections.Add("${ClassName}Settings", o${ClassName}Settings);
			}
			o${ClassName}Settings._Config = Config;
			
			return o${ClassName}Settings;
		}
		
		#endregion
	}
}

