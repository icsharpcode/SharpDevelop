// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.Core;

namespace ICSharpCode.UnitTesting
{
	public class UnitTestingOptions
	{
		/// <summary>
		/// The name of the options as read from the PropertyService.
		/// </summary>
		public static readonly string AddInOptionsName = "UnitTesting.Options";
		
		/// <summary>
		/// The name of the noshadow property stored in SharpDevelop's options.
		/// </summary>
		public static readonly string NoShadowProperty = "NoShadow";

		/// <summary>
		/// The name of the nothread property stored in SharpDevelop's options.
		/// </summary>
		public static readonly string NoThreadProperty = "NoThread";
		
		/// <summary>
		/// The name of the nologo property stored in SharpDevelop's options.
		/// </summary>
		public static readonly string NoLogoProperty = "NoLogo";

		/// <summary>
		/// The name of the nodots property stored in SharpDevelop's options.
		/// </summary>
		public static readonly string NoDotsProperty = "NoDots";
		
		/// <summary>
		/// The name of the labels property stored in SharpDevelop's options.
		/// </summary>
		public static readonly string LabelsProperty = "Labels";
		
		/// <summary>
		/// The name of the create xml file property stored in SharpDevelop's options.
		/// </summary>
		public static readonly string CreateXmlOutputFileProperty = "CreateXmlOutputFile";

		Properties properties;
		
		public UnitTestingOptions()
			: this(PropertyService.Get(AddInOptionsName, new Properties()))
		{
		}
		
		public UnitTestingOptions(Properties properties)
		{
			this.properties = properties;
		}
		
		/// <summary>
		/// Disables the use of a separate thread for running tests.
		/// </summary>
		public bool NoThread {
			get { return properties.Get<bool>(NoThreadProperty, false); }
			set { properties.Set<bool>(NoThreadProperty, value); }
		}
		
		/// <summary>
		/// Disables shadow copying of the assemblies being tested.
		/// </summary>
		public bool NoShadow {
			get { return properties.Get<bool>(NoShadowProperty, false); }
			set { properties.Set<bool>(NoShadowProperty, value); }
		}		
		
		/// <summary>
		/// Disables the display of the NUnit logo when running NUnit-Console.
		/// </summary>
		public bool NoLogo {
			get { return properties.Get<bool>(NoLogoProperty, false); }
			set { properties.Set<bool>(NoLogoProperty, value); }
		}

		/// <summary>
		/// Disables the display of progress when running the unit tests.
		/// </summary>
		public bool NoDots {
			get { return properties.Get<bool>(NoDotsProperty, false); }
			set { properties.Set<bool>(NoDotsProperty, value); }
		}
		
		/// <summary>
		/// Labels each test in the console output.
		/// </summary>
		public bool Labels {
			get { return properties.Get<bool>(LabelsProperty, false); }
			set { properties.Set<bool>(LabelsProperty, value); }
		}
		
		/// <summary>
		/// Creates an XML output file.
		/// </summary>
		public bool CreateXmlOutputFile {
			get { return properties.Get<bool>(CreateXmlOutputFileProperty, false); }
			set { properties.Set<bool>(CreateXmlOutputFileProperty, value); }
		}		
	}
}
