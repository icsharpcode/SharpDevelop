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

using System.IO;
using System.Text;
using ICSharpCode.SharpDevelop.Gui.OptionPanels;
using NUnit.Framework;
using System;

namespace ICSharpCode.SharpDevelop.ProjectOptions
{
	[TestFixture]
	public class AssemblyInfoProviderTests
	{
		[TestCase]
		public void ReadEmptyAssemblyInfoTest()
		{
			var assemblyInfoFile = string.Empty;

			var assemblyInfo = ReadAssemblyInfo(assemblyInfoFile);

			Assert.IsNull(assemblyInfo.Title);
			Assert.IsNull(assemblyInfo.Description);
			Assert.IsNull(assemblyInfo.Company);
			Assert.IsNull(assemblyInfo.Product);
			Assert.IsNull(assemblyInfo.Copyright);
			Assert.IsNull(assemblyInfo.Trademark);
			Assert.IsNull(assemblyInfo.DefaultAlias);
			Assert.IsNull(assemblyInfo.AssemblyVersion);
			Assert.IsNull(assemblyInfo.AssemblyFileVersion);
			Assert.IsNull(assemblyInfo.InformationalVersion);
			Assert.IsNull(assemblyInfo.Guid);
			Assert.IsNull(assemblyInfo.NeutralLanguage);
			Assert.IsFalse(assemblyInfo.ComVisible);
			Assert.IsFalse(assemblyInfo.ClsCompliant);
			Assert.IsTrue(assemblyInfo.JitOptimization);
			Assert.IsFalse(assemblyInfo.JitTracking);
		}

		[TestCase]
		public void ReadNotEmptyAssemblyInfoTest()
		{
			var assemblyInfoFile =
@"using System;
using System.Reflection;
using System.Runtime.InteropServices;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle (""SharpDevelop"")]
[assembly: AssemblyDescription (""OpenSource IDE"")]
[assembly: AssemblyCompany (""Company"")]
[assembly: AssemblyProduct (""Product"")]
[assembly: AssemblyCopyright (""Copyright 2014"")]
[assembly: AssemblyTrademark (""Trademark"")]
[assembly: AssemblyDefaultAlias (""Alias"")]

// This sets the default COM visibility of types in the assembly to invisible.
// If you need to expose a type to COM, use [ComVisible(true)] on that type.
[assembly: ComVisible (true)]
// The assembly version has following format :
//
// Major.Minor.Build.Revision
//
// You can specify all the values or you can use the default the Revision and 
// Build Numbers by using the '*' as shown below:
[assembly: AssemblyVersion (""1.2.3.1"")]
[assembly: AssemblyFileVersion (""1.2.3.2"")]
[assembly: AssemblyInformationalVersion (""1.2.3.3"")]
[assembly: Guid (""0c8c889f-ced2-4167-b155-2d48a99d8c72"")]
[assembly: NeutralResourcesLanguage (""ru-RU"")]
[assembly: AssemblyFlags (32769)]
[assembly: CLSCompliant (true)]";

			var assemblyInfo = ReadAssemblyInfo(assemblyInfoFile);

			Assert.AreEqual("SharpDevelop", assemblyInfo.Title);
			Assert.AreEqual("OpenSource IDE" , assemblyInfo.Description);
			Assert.AreEqual("Company", assemblyInfo.Company);
			Assert.AreEqual("Product", assemblyInfo.Product);
			Assert.AreEqual("Copyright 2014", assemblyInfo.Copyright);
			Assert.AreEqual("Trademark", assemblyInfo.Trademark);
			Assert.AreEqual("Alias", assemblyInfo.DefaultAlias);
			Assert.AreEqual(new Version(1, 2, 3, 1), assemblyInfo.AssemblyVersion);
			Assert.AreEqual(new Version(1, 2, 3, 2), assemblyInfo.AssemblyFileVersion);
			Assert.AreEqual(new Version(1, 2, 3, 3), assemblyInfo.InformationalVersion);
			Assert.AreEqual(new Guid("0c8c889f-ced2-4167-b155-2d48a99d8c72"), assemblyInfo.Guid);
			Assert.AreEqual("ru-RU", assemblyInfo.NeutralLanguage);
			Assert.IsTrue(assemblyInfo.ComVisible);
			Assert.IsTrue(assemblyInfo.ClsCompliant);
			Assert.IsTrue(assemblyInfo.JitOptimization);
			Assert.IsTrue(assemblyInfo.JitTracking);
		}

		[TestCase]
		public void ReadAssemblyInfoWithIncorrectVersionsTest()
		{
			var assemblyInfoFile =
@"using System;
using System.Reflection;
using System.Runtime.InteropServices;

// This sets the default COM visibility of types in the assembly to invisible.
// If you need to expose a type to COM, use [ComVisible(true)] on that type.
[assembly: ComVisible (true)]
// The assembly version has following format :
//
// Major.Minor.Build.Revision
//
// You can specify all the values or you can use the default the Revision and 
// Build Numbers by using the '*' as shown below:
[assembly: AssemblyVersion (""Incorrect version"")]
[assembly: AssemblyFileVersion (""Incorrect version"")]
[assembly: AssemblyInformationalVersion (""Incorrect version"")]";

			var assemblyInfo = ReadAssemblyInfo(assemblyInfoFile);
			Assert.IsNull(assemblyInfo.AssemblyVersion);
			Assert.IsNull(assemblyInfo.AssemblyFileVersion);
			Assert.IsNull(assemblyInfo.InformationalVersion);
		}

		[TestCase]
		public void ReadAssemblyInfoWithIncorrectGuidTest()
		{
			var assemblyInfoFile =
@"using System;
using System.Reflection;
using System.Runtime.InteropServices;

// This sets the default COM visibility of types in the assembly to invisible.
// If you need to expose a type to COM, use [ComVisible(true)] on that type.
[assembly: ComVisible (true)]
[assembly: Guid (""Incorrect GUID"")]";

			var assemblyInfo = ReadAssemblyInfo(assemblyInfoFile);

			Assert.IsNull(assemblyInfo.Guid);
		}

		[TestCase]
		public void ReadAssemblyInfoWithEmptyStringsTest()
		{
			var assemblyInfoFile =
@"using System;
using System.Reflection;
using System.Runtime.InteropServices;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle ("""")]
[assembly: AssemblyDescription ("""")]
[assembly: AssemblyCompany ("""")]
[assembly: AssemblyProduct ("""")]
[assembly: AssemblyCopyright ("""")]
[assembly: AssemblyTrademark ("""")]
[assembly: AssemblyDefaultAlias ("""")]

// This sets the default COM visibility of types in the assembly to invisible.
// If you need to expose a type to COM, use [ComVisible(true)] on that type.
[assembly: ComVisible (true)]
// The assembly version has following format :
//
// Major.Minor.Build.Revision
//
// You can specify all the values or you can use the default the Revision and 
// Build Numbers by using the '*' as shown below:
[assembly: AssemblyVersion ("""")]
[assembly: AssemblyFileVersion ("""")]
[assembly: AssemblyInformationalVersion ("""")]
[assembly: Guid ("""")]
[assembly: NeutralResourcesLanguage ("""")]";

			var assemblyInfo = ReadAssemblyInfo(assemblyInfoFile);

			Assert.AreEqual("", assemblyInfo.Title);
			Assert.AreEqual("", assemblyInfo.Description);
			Assert.AreEqual("", assemblyInfo.Company);
			Assert.AreEqual("", assemblyInfo.Product);
			Assert.AreEqual("", assemblyInfo.Copyright);
			Assert.AreEqual("", assemblyInfo.Trademark);
			Assert.AreEqual("", assemblyInfo.DefaultAlias);
			Assert.IsNull(assemblyInfo.AssemblyVersion);
			Assert.IsNull(assemblyInfo.AssemblyFileVersion);
			Assert.IsNull(assemblyInfo.InformationalVersion);
			Assert.IsNull(assemblyInfo.Guid);
			Assert.AreEqual("", assemblyInfo.NeutralLanguage);
		}

		[TestCase]
		public void ReadAssemblyInfoWithFalseBooleanValuesTest()
		{
			var assemblyInfoFile =
@"using System;
using System.Reflection;
using System.Runtime.InteropServices;

// This sets the default COM visibility of types in the assembly to invisible.
// If you need to expose a type to COM, use [ComVisible(true)] on that type.
[assembly: ComVisible (false)]
[assembly: CLSCompliant (false)]";

			var assemblyInfo = ReadAssemblyInfo(assemblyInfoFile);

			Assert.IsFalse(assemblyInfo.ComVisible);
			Assert.IsFalse(assemblyInfo.ClsCompliant);
		}

		[TestCase]
		public void ReadAssemblyInfoWithTrueBooleanValuesTest()
		{
			var assemblyInfoFile =
@"using System;
using System.Reflection;
using System.Runtime.InteropServices;

// This sets the default COM visibility of types in the assembly to invisible.
// If you need to expose a type to COM, use [ComVisible(true)] on that type.
[assembly: ComVisible (true)]
[assembly: CLSCompliant (true)]";

			var assemblyInfo = ReadAssemblyInfo(assemblyInfoFile);

			Assert.IsTrue(assemblyInfo.ComVisible);
			Assert.IsTrue(assemblyInfo.ClsCompliant);
		}


		private AssemblyInfo ReadAssemblyInfo(string assemblyInfoFile)
		{
			var stream = new MemoryStream(Encoding.UTF8.GetBytes(assemblyInfoFile));
			var assemblyInfoProvider = new AssemblyInfoProvider();
			var assemblyInfo = assemblyInfoProvider.Read(stream);
			return assemblyInfo;
		}
	}
}
