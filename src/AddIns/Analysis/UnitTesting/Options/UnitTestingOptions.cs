// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Widgets;

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
		
		static readonly UnitTestingOptions instance = new UnitTestingOptions(PropertyService.NestedProperties(AddInOptionsName));
		
		public static UnitTestingOptions Instance {
			get { return instance; }
		}

		Properties properties;
		
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
		
		public UnitTestingOptions Clone()
		{
			return new UnitTestingOptions(properties.Clone());
		}
	}
}
