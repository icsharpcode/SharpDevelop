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
	/// <summary>
	/// Tests for assembly info reading and writing
	/// </summary>
	[TestFixture]
	public class AssemblyInfoProviderTests
	{
		private const string AssemblyInfoSample1 =
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
			var assemblyInfoFile = AssemblyInfoSample1;

			var assemblyInfo = ReadAssemblyInfo(assemblyInfoFile);

			Assert.AreEqual("SharpDevelop", assemblyInfo.Title);
			Assert.AreEqual("OpenSource IDE" , assemblyInfo.Description);
			Assert.AreEqual("Company", assemblyInfo.Company);
			Assert.AreEqual("Product", assemblyInfo.Product);
			Assert.AreEqual("Copyright 2014", assemblyInfo.Copyright);
			Assert.AreEqual("Trademark", assemblyInfo.Trademark);
			Assert.AreEqual("Alias", assemblyInfo.DefaultAlias);
			Assert.AreEqual(new Version(1, 2, 3, 1).ToString(), assemblyInfo.AssemblyVersion);
			Assert.AreEqual(new Version(1, 2, 3, 2).ToString(), assemblyInfo.AssemblyFileVersion);
			Assert.AreEqual(new Version(1, 2, 3, 3).ToString(), assemblyInfo.InformationalVersion);
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
			Assert.AreEqual("Incorrect version", assemblyInfo.AssemblyVersion);
			Assert.AreEqual("Incorrect version", assemblyInfo.AssemblyFileVersion);
			Assert.AreEqual("Incorrect version", assemblyInfo.InformationalVersion);
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
			Assert.AreEqual("", assemblyInfo.AssemblyVersion);
			Assert.AreEqual("", assemblyInfo.AssemblyFileVersion);
			Assert.AreEqual("", assemblyInfo.InformationalVersion);
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

		[TestCase]
		public void ReadAssemblyInfoWithAttributePostfixTest()
		{
			var assemblyInfoFile =
@"using System;
using System.Reflection;
using System.Runtime.InteropServices;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitleAttribute (""SharpDevelop"")]
[assembly: AssemblyDescriptionAttribute (""OpenSource IDE"")]
[assembly: AssemblyCompanyAttribute (""Company"")]
[assembly: AssemblyProductAttribute (""Product"")]
[assembly: AssemblyCopyrightAttribute (""Copyright 2014"")]
[assembly: AssemblyTrademarkAttribute (""Trademark"")]
[assembly: AssemblyDefaultAliasAttribute (""Alias"")]
// This sets the default COM visibility of types in the assembly to invisible.
// If you need to expose a type to COM, use [ComVisible(true)] on that type.
[assembly: ComVisibleAttribute (true)]
// The assembly version has following format :
//
// Major.Minor.Build.Revision
//
// You can specify all the values or you can use the default the Revision and 
// Build Numbers by using the '*' as shown below:
[assembly: AssemblyVersionAttribute (""1.2.3.1"")]
[assembly: AssemblyFileVersionAttribute (""1.2.3.2"")]
[assembly: AssemblyInformationalVersionAttribute (""1.2.3.3"")]
[assembly: GuidAttribute (""0c8c889f-ced2-4167-b155-2d48a99d8c72"")]
[assembly: NeutralResourcesLanguageAttribute (""ru-RU"")]
[assembly: AssemblyFlagsAttribute (32769)]
[assembly: CLSCompliantAttribute (true)]";

			var assemblyInfo = ReadAssemblyInfo(assemblyInfoFile);

			Assert.AreEqual("SharpDevelop", assemblyInfo.Title);
			Assert.AreEqual("OpenSource IDE", assemblyInfo.Description);
			Assert.AreEqual("Company", assemblyInfo.Company);
			Assert.AreEqual("Product", assemblyInfo.Product);
			Assert.AreEqual("Copyright 2014", assemblyInfo.Copyright);
			Assert.AreEqual("Trademark", assemblyInfo.Trademark);
			Assert.AreEqual("Alias", assemblyInfo.DefaultAlias);
			Assert.AreEqual(new Version(1, 2, 3, 1).ToString(), assemblyInfo.AssemblyVersion);
			Assert.AreEqual(new Version(1, 2, 3, 2).ToString(), assemblyInfo.AssemblyFileVersion);
			Assert.AreEqual(new Version(1, 2, 3, 3).ToString(), assemblyInfo.InformationalVersion);
			Assert.AreEqual(new Guid("0c8c889f-ced2-4167-b155-2d48a99d8c72"), assemblyInfo.Guid);
			Assert.AreEqual("ru-RU", assemblyInfo.NeutralLanguage);
			Assert.IsTrue(assemblyInfo.ComVisible);
			Assert.IsTrue(assemblyInfo.ClsCompliant);
			Assert.IsTrue(assemblyInfo.JitOptimization);
			Assert.IsTrue(assemblyInfo.JitTracking);
		}

		[TestCase]
		public void ReadAssemblyFlagsTests()
		{
			var assemblyInfo = ReadAssemblyInfo("[assembly: AssemblyFlags (32769)]");
			Assert.IsTrue(assemblyInfo.JitOptimization);
			Assert.IsTrue(assemblyInfo.JitTracking);

			assemblyInfo = ReadAssemblyInfo("[assembly: AssemblyFlags (16385)]");
			Assert.IsFalse(assemblyInfo.JitOptimization);
			Assert.IsFalse(assemblyInfo.JitTracking);

			assemblyInfo = ReadAssemblyInfo("[assembly: AssemblyFlags (49153)]");
			Assert.IsFalse(assemblyInfo.JitOptimization);
			Assert.IsTrue(assemblyInfo.JitTracking);

			assemblyInfo = ReadAssemblyInfo("[assembly: AssemblyFlags (0)]");
			Assert.IsTrue(assemblyInfo.JitOptimization);
			Assert.IsFalse(assemblyInfo.JitTracking);

			assemblyInfo = ReadAssemblyInfo(
	"[assembly: AssemblyFlags(AssemblyNameFlags.EnableJITcompileTracking)]");
			Assert.IsTrue(assemblyInfo.JitOptimization);
			Assert.IsTrue(assemblyInfo.JitTracking);

			assemblyInfo = ReadAssemblyInfo(
	"[assembly: AssemblyFlags(AssemblyNameFlags.EnableJITcompileOptimizer)]");
			Assert.IsFalse(assemblyInfo.JitOptimization);
			Assert.IsFalse(assemblyInfo.JitTracking);

			assemblyInfo = ReadAssemblyInfo(
	"[assembly: AssemblyFlags()]");
			Assert.IsTrue(assemblyInfo.JitOptimization);
			Assert.IsFalse(assemblyInfo.JitTracking);

			assemblyInfo = ReadAssemblyInfo(
	"[assembly: AssemblyFlags(AssemblyNameFlags.EnableJITcompileOptimizer | AssemblyNameFlags.EnableJITcompileTracking)]");
			Assert.IsFalse(assemblyInfo.JitOptimization);
			Assert.IsTrue(assemblyInfo.JitTracking);
		}

		[TestCase]
		public void WriteAssemblyFlagsTests()
		{
			var assemblyInfo = new AssemblyInfo { JitOptimization = false, JitTracking = true };
			Assert.AreEqual(
@"using System;
using System.Reflection;

[assembly: AssemblyFlags (AssemblyNameFlags.PublicKey | AssemblyNameFlags.EnableJITcompileOptimizer | AssemblyNameFlags.EnableJITcompileTracking)]
",
				WriteAssemblyInfoFile(assemblyInfo, string.Empty));

			assemblyInfo = new AssemblyInfo { JitOptimization = true, JitTracking = true };
			Assert.AreEqual(
@"using System;
using System.Reflection;

[assembly: AssemblyFlags (AssemblyNameFlags.PublicKey | AssemblyNameFlags.EnableJITcompileTracking)]
",
				WriteAssemblyInfoFile(assemblyInfo, string.Empty));

			assemblyInfo = new AssemblyInfo { JitOptimization = true, JitTracking = false };
			Assert.AreEqual("using System;\r\nusing System.Reflection;\r\n\r\n", WriteAssemblyInfoFile(assemblyInfo, string.Empty));

			assemblyInfo = new AssemblyInfo { JitOptimization = false, JitTracking = false };
			Assert.AreEqual(
@"using System;
using System.Reflection;

[assembly: AssemblyFlags (AssemblyNameFlags.PublicKey | AssemblyNameFlags.EnableJITcompileOptimizer)]
",
				WriteAssemblyInfoFile(assemblyInfo, string.Empty));
		}

		[TestCase] 
		public void WriteDefaultAssemblyInfoToEmptyAssemblyInfoFileTest()
		{
			var assemblyInfoFile = "using System;";
			var assemblyInfo = new AssemblyInfo { JitOptimization = true };
			var result = WriteAssemblyInfoFile(assemblyInfo, assemblyInfoFile);

			Assert.AreEqual("using System;\r\nusing System.Reflection;\r\n\r\n", result);
		}

		[TestCase]
		public void WriteNotDefaultAssemblyInfoToEmptyAssemblyInfoFileTest()
		{
			var assemblyInfoFile = "using System.Reflection;";

			var assemblyInfo = new AssemblyInfo
			{
				Title = "SharpDevelop",
				Description = "OpenSource IDE",
				Company = "Company",
				Product = "Product",
				Copyright = "Copyright 2014",
				Trademark = "Trademark",
				DefaultAlias = "Alias",
				AssemblyVersion = new Version(1, 2, 3, 4).ToString(),
				AssemblyFileVersion = new Version(1, 2, 3, 4).ToString(),
				InformationalVersion = new Version(1, 2, 3, 4).ToString(),
				Guid = new Guid("0c8c889f-ced2-4167-b155-2d48a99d8c72"),
				NeutralLanguage = "ru-RU",
				ComVisible = true,
				ClsCompliant = true,
				JitOptimization = true,
				JitTracking = true
			};

			var result = WriteAssemblyInfoFile(assemblyInfo, assemblyInfoFile);

			Assert.AreEqual(
@"using System;
using System.Reflection;
using System.Resources;
using System.Runtime.InteropServices;

[assembly: AssemblyTitle (""SharpDevelop"")]
[assembly: AssemblyDescription (""OpenSource IDE"")]
[assembly: AssemblyCompany (""Company"")]
[assembly: AssemblyProduct (""Product"")]
[assembly: AssemblyCopyright (""Copyright 2014"")]
[assembly: AssemblyTrademark (""Trademark"")]
[assembly: AssemblyDefaultAlias (""Alias"")]
[assembly: AssemblyVersion (""1.2.3.4"")]
[assembly: AssemblyFileVersion (""1.2.3.4"")]
[assembly: AssemblyInformationalVersion (""1.2.3.4"")]
[assembly: Guid (""0c8c889f-ced2-4167-b155-2d48a99d8c72"")]
[assembly: NeutralResourcesLanguage (""ru-RU"")]
[assembly: ComVisible (true)]
[assembly: CLSCompliant (true)]
[assembly: AssemblyFlags (AssemblyNameFlags.PublicKey | AssemblyNameFlags.EnableJITcompileTracking)]
", result);
		}

		[TestCase]
		public void WriteNotDefaultAssemblyInfoToNotEmptyAssemblyInfoFileTest()
		{
			var assemblyInfoFile = AssemblyInfoSample1;

			var assemblyInfo = new AssemblyInfo
			{
				Title = "SharpDevelop-changed",
				Description = "OpenSource IDE-changed",
				Company = "Company-changed",
				Product = "Product-changed",
				Copyright = "Copyright 2014-changed",
				Trademark = "Trademark-changed",
				DefaultAlias = "Alias-changed",
				AssemblyVersion = new Version(4, 3, 2, 1).ToString(),
				AssemblyFileVersion = new Version(4, 3, 2, 1).ToString(),
				InformationalVersion = new Version(4, 3, 2, 1).ToString(),
				Guid = new Guid("dc8c889f-ced2-4167-b155-2d48a99d8c72"),
				NeutralLanguage = "en-US",
				ComVisible = false,
				ClsCompliant = false,
				JitOptimization = false,
				JitTracking = false
			};

			var result = WriteAssemblyInfoFile(assemblyInfo, assemblyInfoFile);

			Assert.AreEqual(
@"using System;
using System.Reflection;
using System.Resources;
using System.Runtime.InteropServices;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle (""SharpDevelop-changed"")]
[assembly: AssemblyDescription (""OpenSource IDE-changed"")]
[assembly: AssemblyCompany (""Company-changed"")]
[assembly: AssemblyProduct (""Product-changed"")]
[assembly: AssemblyCopyright (""Copyright 2014-changed"")]
[assembly: AssemblyTrademark (""Trademark-changed"")]
[assembly: AssemblyDefaultAlias (""Alias-changed"")]
// This sets the default COM visibility of types in the assembly to invisible.
// If you need to expose a type to COM, use [ComVisible(true)] on that type.
[assembly: ComVisible (false)]
// The assembly version has following format :
//
// Major.Minor.Build.Revision
//
// You can specify all the values or you can use the default the Revision and 
// Build Numbers by using the '*' as shown below:
[assembly: AssemblyVersion (""4.3.2.1"")]
[assembly: AssemblyFileVersion (""4.3.2.1"")]
[assembly: AssemblyInformationalVersion (""4.3.2.1"")]
[assembly: Guid (""dc8c889f-ced2-4167-b155-2d48a99d8c72"")]
[assembly: NeutralResourcesLanguage (""en-US"")]
[assembly: AssemblyFlags (AssemblyNameFlags.PublicKey | AssemblyNameFlags.EnableJITcompileOptimizer)]
[assembly: CLSCompliant (false)]
", result);
		}

		private AssemblyInfo ReadAssemblyInfo(string assemblyInfoFile)
		{
			using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(assemblyInfoFile)))
			{
				var assemblyInfoProvider = new AssemblyInfoProvider();
				return assemblyInfoProvider.ReadAssemblyInfo(stream);
			}
		}

		private string WriteAssemblyInfoFile(AssemblyInfo assemblyInfo, string sourceFile)
		{
			using (var inputStream = new MemoryStream(Encoding.UTF8.GetBytes(sourceFile)))
			{
				var assemblyInfoProvider = new AssemblyInfoProvider();
				return assemblyInfoProvider.MergeAssemblyInfo(assemblyInfo, inputStream);
			}
		}
	}
}
