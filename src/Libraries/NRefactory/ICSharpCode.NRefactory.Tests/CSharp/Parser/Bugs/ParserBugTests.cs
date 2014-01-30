// 
// ParserBugTests.cs
//  
// Author:
//       Mike Krüger <mkrueger@xamarin.com>
// 
// Copyright (c) 2012 Xamarin Inc. (http://xamarin.com)
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
using System;
using NUnit.Framework;
using ICSharpCode.NRefactory.CSharp;
using System.Linq;

namespace ICSharpCode.NRefactory.CSharp.Parser.Bugs
{
	[TestFixture]
	public class ParserBugTests
	{
		/// <summary>
		/// Bug 4252 - override bug in mcs ast
		/// </summary>
		[Test]
		public void TestBug4252()
		{
			string code = @"
class Foo
{

    class Bar
    {
          override foo
    }

    public Foo () 
    {
    }
}";
			var unit = SyntaxTree.Parse(code);
			var type = unit.Members.First() as TypeDeclaration;
			var constructor = type.Members.Skip(1).First() as ConstructorDeclaration;
			var passed = !constructor.HasModifier(Modifiers.Override);
			if (!passed) {
				Console.WriteLine("Expected:" + code);
				Console.WriteLine("Was:" + unit);
			}
			Assert.IsTrue(passed);
		}
		
		/// <summary>
		/// Bug 4059 - Return statement without semicolon missing in the AST
		/// </summary>
		[Test]
		public void TestBug4059()
		{
			string code = @"
class Stub
{
    Test A ()
    {
        return new Test ()
    }
}";
			var unit = SyntaxTree.Parse(code);
			var type = unit.Members.First() as TypeDeclaration;
			var method = type.Members.First() as MethodDeclaration;
			bool passed = method.Body.Statements.FirstOrDefault() is ReturnStatement;
			if (!passed) {
				Console.WriteLine("Expected:" + code);
				Console.WriteLine("Was:" + unit);
			}
			Assert.IsTrue(passed);
		}
		
		/// <summary>
		/// Bug 4058 - Unattached parameter attributes should be included in the AST
		/// </summary>
		[Test]
		public void TestBug4058()
		{
			string code = @"
class TestClass
{
  TestClass([attr])
  {
  }
}";
			var unit = SyntaxTree.Parse(code);
			
			var type = unit.Members.First() as TypeDeclaration;
			var constructor = type.Members.First() as ConstructorDeclaration;
			bool passed = constructor.GetNodeAt<AttributeSection>(constructor.LParToken.StartLocation.Line, constructor.LParToken.StartLocation.Column + 1) != null;
			if (!passed) {
				Console.WriteLine("Expected:" + code);
				Console.WriteLine("Was:" + unit);
			}
			Assert.IsTrue(passed);
		}
		
		
		
		/// <summary>
		/// Bug 3952 - Syntax errors that causes AST not inserted 
		/// </summary>
		[Ignore("Still open 03/2013")]
		[Test]
		public void TestBug3952()
		{
			string code = @"
class Foo
{
	void Bar()
	{
		Test(new Foo (
	}
}";
			var unit = SyntaxTree.Parse(code);
			
			var type = unit.Members.First() as TypeDeclaration;
			var method = type.Members.First() as MethodDeclaration;
			bool passed = !method.Body.IsNull;
			if (!passed) {
				Console.WriteLine("Expected:" + code);
				Console.WriteLine("Was:" + unit);
			}
			Assert.IsTrue(passed);
		}
		
		/// <summary>
		/// Bug 3578 - For condition not in the AST.
		/// </summary>
		[Test]
		public void TestBug3578()
		{
			string code = 
@"class Foo
{
	void Bar ()
	{
		for (int i = 0; i < foo.bar)
	}
}";
			var unit = SyntaxTree.Parse(code);
			
			bool passed = @"class Foo
{
	void Bar ()
	{
		for (int i = 0; i < foo.bar;)
	}
}" == unit.ToString().Trim ();
			if (!passed) {
				Console.WriteLine("Expected:" + code);
				Console.WriteLine("Was:" + unit);
			}
			Assert.IsTrue(passed);
		}
		
		/// <summary>
		/// Bug 3517 - Incomplete conditional operator in the AST request.
		/// </summary>
		[Test]
		public void TestBug3517()
		{
			string code = 
@"class Test
{
	void Foo ()
	{
		a = cond ? expr
	}
}";
			var unit = SyntaxTree.Parse(code);
			
			var type = unit.Members.First() as TypeDeclaration;
			var method = type.Members.First() as MethodDeclaration;
			var exprStmt = method.Body.Statements.FirstOrDefault() as ExpressionStatement;
			var expr = exprStmt.Expression as AssignmentExpression;
			bool passed = expr != null && expr.Right is ConditionalExpression;
			
			if (!passed) {
				Console.WriteLine("Expected:" + code);
				Console.WriteLine("Was:" + unit);
			}
			Assert.IsTrue(passed);
		}
		
		[Test]
		public void TestBug3517Case2()
		{
			string code = 
@"class Test
{
	void Foo ()
	{
		a = cond ? expr :
	}
}";
			var unit = SyntaxTree.Parse(code);
			
			var type = unit.Members.First() as TypeDeclaration;
			var method = type.Members.First() as MethodDeclaration;
			var exprStmt = method.Body.Statements.FirstOrDefault() as ExpressionStatement;
			var expr = exprStmt.Expression as AssignmentExpression;
			bool passed = expr != null && expr.Right is ConditionalExpression;
			
			if (!passed) {
				Console.WriteLine("Expected:" + code);
				Console.WriteLine("Was:" + unit);
			}
			Assert.IsTrue(passed);
		}
		
		/// <summary>
		/// Bug 3468 - Local variable declarations are not inserted in ast & break declaring member locations.
		/// </summary>
		[Test]
		public void TestBug3468()
		{
			string code = 
@"class C
{
    public static void Main (string[] args)
    {
        string str = 
    }
}";
			var unit = SyntaxTree.Parse(code);
			
			var type = unit.Members.First() as TypeDeclaration;
			var method = type.Members.First() as MethodDeclaration;
			bool passed = !method.Body.IsNull;
			if (!passed) {
				Console.WriteLine("Expected:" + code);
				Console.WriteLine("Was:" + unit);
			}
			Assert.IsTrue(passed);
		}
		
		/// <summary>
		/// Bug 3288 - Try ... catch not added to the ast if catch block is missing
		/// </summary>
		[Test]
		public void TestBug3288()
		{
			string code = 
@"class Test
{
	public void Main(string[] args)
	{
		try {
		} catch (Exception name)
	}
}";
			var unit = SyntaxTree.Parse(code);
			
			var type = unit.Members.First() as TypeDeclaration;
			var method = type.Members.First() as MethodDeclaration;
			bool passed = method.Body.Statements.FirstOrDefault() is TryCatchStatement;
			if (!passed) {
				Console.WriteLine("Expected:" + code);
				Console.WriteLine("Was:" + unit);
			}
			Assert.IsTrue(passed);
		}
		
		/// <summary>
		/// Bug 3155 - Anonymous methods in variable declarations don't produce an ast, if the ';' is missing.
		/// </summary>
		[Test]
		public void TestBug3155()
		{
			string code = 
@"using System;

class Test
{
    void Foo ()
    {
        Action<int> act = delegate (int testMe) {

        }
    }
}
";
			var unit = SyntaxTree.Parse(code);
			
			bool passed = unit.ToString().Trim() == @"using System;

class Test
{
	void Foo ()
	{
		Action<int> act = delegate (int testMe) {
		};
	}
}";
			if (!passed) {
				Console.WriteLine("Expected:" + code);
				Console.WriteLine("Was:" + unit);
			}
			Assert.IsTrue(passed);
		}

		/// <summary>
		/// Bug 4556 - AST broken for unclosed invocation
		/// </summary>
		[Test]
		public void TestBug4556()
		{
			string code = 
@"using System;

class Foo
{
    public static void Main (string[] args)
    {
        Console.WriteLine (""foo"", 
    }
}
";
			var unit = SyntaxTree.Parse(code);
			var type = unit.Members.First(m => m is TypeDeclaration) as TypeDeclaration;
			var method = type.Members.First() as MethodDeclaration;
			bool passed = !method.Body.IsNull;
			if (!passed) {
				Console.WriteLine("Expected:" + code);
				Console.WriteLine("Was:" + unit);
			}
			Assert.IsTrue(passed);
		}
		
		/// <summary>
		/// Bug 5064 - Autocomplete doesn't include object initializer properties in yield return 
		/// </summary>
		[Test]
		public void TestBug5064()
		{
			string code = 
@"public class Bar
{
    public IEnumerable<Foo> GetFoos()
    {
        yield return new Foo { }
    }
}";
			var unit = SyntaxTree.Parse(code);
			
			string text = unit.ToString().Trim();
			string expected = @"public class Bar
{
	public IEnumerable<Foo> GetFoos()
	{
		yield return new Foo { };
	}
}";
			int i = 0, j = 0;
			while (i < text.Length && j < expected.Length) {
				if (char.IsWhiteSpace (text[i])) {
					i++;
					continue;
				}
				if (char.IsWhiteSpace (expected[j])) {
					j++;
					continue;
				}
				if (text [i] != expected [j]) {
					break;
				}
				i++;j++;
			}
			bool passed = i == text.Length && j == expected.Length;
			if (!passed) {
				Console.WriteLine("Expected:" + expected);
				Console.WriteLine("Was:" + unit);
			}
			Assert.IsTrue(passed);
		}

		/// <summary>
		/// Bug 5389 - Code completion does not work well inside a dictionary initializer
		/// </summary>
		[Test()]
		public void TestBug5389()
		{
			string code = 
				@"class Foo
{
	static Dictionary<Tuple<Type, string>, string> CreatePropertyMap ()
	{
		return new Dictionary<Tuple<Type, string>, string> {
			{ Tuple.Create (typeof (MainClass), ""Prop1""), ""Prop2"" },
			{ Tuple.C }
		}
	}
}
";
			var unit = SyntaxTree.Parse(code);
			
			var type = unit.Members.First() as TypeDeclaration;
			var method = type.Members.First() as MethodDeclaration;
			var stmt = method.Body.Statements.First () as ReturnStatement;
			bool passed = stmt.Expression is ObjectCreateExpression;
			if (!passed) {
				Console.WriteLine("Expected:" + code);
				Console.WriteLine("Was:" + unit);
			}
			Assert.IsTrue(passed);
		}

		[Test()]
		public void TestIncompleteParameter()
		{
			string code = 
				@"class Foo
{
	void Bar (params System.A) {}
}
";
			var unit = SyntaxTree.Parse(code);

			var type = unit.Members.First() as TypeDeclaration;
			var method = type.Members.First() as MethodDeclaration;
			bool passed = method.Parameters.Count == 1;
			if (!passed) {
				Console.WriteLine("Expected:" + code);
				Console.WriteLine("Was:" + unit);
			}
			Assert.IsTrue(passed);
		}

		[Test]
		public void TestMultipleNestedPartialClassesInSameFile ()
		{
			string code = @"public partial class PartialClassInSameFile
	{
		public partial class NestedPartial
		{
			void Foo ()
			{
			}
		}
	}

	public partial class PartialClassInSameFile
	{
		public partial class NestedPartial 
		{
			void FooBar ()
			{

			}
		}
	}
";
			var unit = SyntaxTree.Parse(code);
			Console.WriteLine(unit);
			var type = unit.Members.First() as TypeDeclaration;
			Assert.IsTrue(type.Members.FirstOrDefault() is TypeDeclaration, "1st nested partial not found."); 

			type = unit.Members.Skip (1).First() as TypeDeclaration;
			Assert.IsTrue(type.Members.FirstOrDefault() is TypeDeclaration, "2nd nested partial not found."); 

		}


		/// <summary>
		/// Bug 12383 - [AST] Non existing namespaces generated
		/// </summary>
		[Test]
		public void TestBug12383()
		{
			string code = @"using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using Kajabity.Tools.Java;
using Xamarin.Installer.Core;

namespace Xamarin.Installer.Core.Components.Android
{
	public class AndroidComponentSystemImage : AndroidComponentBase
	{
		public string Abi { get; private set; }

		public uint ApiLevel { get; private set; }

		public override string Name {
			get { return String.Format (SYSTEM_IMAGE_NAME_FORMAT, ApiLevel); }
		}

		public override string UserFriendlyName {
			get { return String.Format (""{0} System Image for API {1} ({2})"", LicenseVendor, ApiLevel, Abi); }
		}

		public override string LicenseTitle {
			get { return String.Format (""{0} System Image"", LicenseVendor); }
		}

		string LicenseVendor {
			get {
				string ul = Info.UsesLicense;
				if (String.IsNullOrWhiteSpace (ul))
					return ""Android"";
				if (ul.IndexOf (""intel"", StringComparison.OrdinalIgnoreCase) >= 0)
					return ""Intel"";
				else if (ul.IndexOf (""mips"", StringComparison.OrdinalIgnoreCase) >= 0)
					return ""MIPS"";
				else
					return ""Android"";
			
		}

		public override bool Init (AndroidComponentInfoBase componentInfo, AndroidSDKComponentInfo sdkInfo)
		{
			if (!base.Init (componentInfo, sdkInfo))
				return false;

			var info = componentInfo as AndroidComponentSystemImageInfo;
			if (info == null) {
				LogUnexpectedComponentInfoType (componentInfo);
				return false;
			}
			ApiLevel = info.ApiLevel;
			Abi = info.Abi;

			return true;
		}

		public override bool Install (DownloadedItem item, AndroidSdkInstance sdk)
		{
			if (item == null)
				throw new ArgumentNullException (""item"");
			if (sdk == null)
				throw new ArgumentNullException (""sdk"");

			bool success = true;
			string message = ""Android SDK Platform API level {0}, ABI {1} system image failed to install."";

			Log.Debug (""Installing system image for Android SDK Platform API {0}, ABI {1}."", ApiLevel, Abi);
			try {
				if (!DoInstall (item.FilePath, sdk.Path, ""system-images"", ""android-"" + ApiLevel, Abi)) {
					Log.Warning (message, ApiLevel, Abi);
					success = false;
				} else
					UpdateSourceProperties (sdk.Path);
			} catch (Exception ex) {
				Log.Warning (message, ex, ApiLevel, Abi);
				success = false;
			}

			return success;
		}

		public override bool IsInstallationNeeded (AndroidSdkInstance instance, AndroidPlatformSpecificToolNames toolNames)
		{
			Installed = false;
			if (instance == null)
				throw new ArgumentNullException (""instance"");

			string systemImagePath = Path.Combine (instance.Path, ""system-images"", ""android-"" + ApiLevel, Abi);
			AndroidDownloadsCharter charter = instance.DownloadCharter;
			JavaProperties props;

			if (!Directory.Exists (systemImagePath)) {
				charter.DownloadSystemImages = true;
				instance.AddUpdateReason (""System image {0}/{1} directory does not exist"", ApiLevel, Abi);
				return true;
			}

			if (!ReadJavaProperties (Path.Combine (systemImagePath, ""source.properties""), out props) || props == null) {
				charter.DownloadSystemImages = true;
				instance.AddUpdateReason (""Unable to read System Image {0}/{1} properties, assuming it's outdated"", ApiLevel, Abi);
				return true;
			}

			Installed = true;
			string rev;
			Version instv;
			if (!props.GetPkgRevision (out rev, out instv)) {
				charter.DownloadSystemImages = true;
				instance.AddUpdateReason (""Unable to read System Image {0}/{1} revision, assuming it's outdated"", ApiLevel, Abi);
				return true;
			}

			string value;
			if (!props.GetProperty<string> (""SystemImage.Abi"", out value) || String.IsNullOrEmpty (value)) {
				charter.DownloadSystemImages = true;
				instance.AddUpdateReason (""Unable to read System Image {0}/{1} ABI, assuming it's outdated"", ApiLevel, Abi);
				return true;
			}

			if (String.Compare (value, Abi, StringComparison.OrdinalIgnoreCase) != 0) {
				charter.DownloadSystemImages = true;
				instance.AddUpdateReason (""Installed System Image {0} ABI ({1}) is different ({2}), assuming it's outdated"", ApiLevel, Abi, value);
				return true;
			}

			if (Revision.AndroidTryParse (true) > instv) {
				charter.DownloadSystemImages = true;
				instance.AddUpdateReason (""Installed System Image {0}/{1} revision {2} is older than the required one ({3})"", ApiLevel, Abi, instv, Revision);
				return true;
			}

			bool needsDownload = false;
			if (!File.Exists (Path.Combine (systemImagePath, ""system.img""))) {
				if (!File.Exists (Path.Combine (systemImagePath, ""images"", Abi, ""system.img""))) {
					needsDownload = true;
					charter.DownloadSystemImages = true;
					instance.AddUpdateReason (""system.img not found for API {0}, ABI {1}"", ApiLevel, Abi);
				}
			}

			return needsDownload;
		}

		protected override void UpdateSourceProperties (string targetBasePath)
		{
			var props = new JavaProperties ();

			props.SetProperty (""Pkg.Desc"", Description);
			props.SetProperty (""Archive.Os"", OS.ToUpperInvariant ());
			props.SetProperty (""Archive.Arch"", Arch.ToUpperInvariant ());
			props.SetProperty (""AndroidVersion.ApiLevel"", ApiLevel.ToString ());
			props.SetProperty (""Pkg.Revision"", Revision.ToString ());
			props.SetProperty (""SystemImage.Abi"", Abi);
			props.SetProperty (""Pkg.SourceUrl"", SDKInfo.RepositoryUrl);

			SaveProperties (props, targetBasePath, ""system-images"", ""android-"" + ApiLevel, Abi);
		}
	}
}

";
			var unit = SyntaxTree.Parse(code);
			Assert.AreEqual(1, unit.DescendantNodesAndSelf().OfType<NamespaceDeclaration>().Count());

		}

		[Test]
		public void TestEmptyCollectionParsing()
		{
			string code = @"
class FooBar
{
	Dictionary<byte, string> foo = new Dictionary<byte, string>{
		{},
		{}
	};
}
";
			var unit = SyntaxTree.Parse(code);

			var type = unit.Members.First() as TypeDeclaration;
			var member = type.Members.First() as FieldDeclaration;
			var init = member.Variables.First().Initializer as ObjectCreateExpression;
			Assert.AreEqual(2, init.Initializer.Elements.Count);
		}

		[Test]
		public void AsyncAfterEnum() {
			string code = @"
using System.Threading.Tasks;
class C
{
	enum E {}
	async Task M() {
	}
}";

			var unit = SyntaxTree.Parse(code);
			var type = unit.Members.OfType<TypeDeclaration>().Single();
			var member = type.Members.OfType<MethodDeclaration>().SingleOrDefault(m => m.Name == "M");
			Assert.IsNotNull(member, "M() not found."); 
			Assert.That(member.Modifiers, Is.EqualTo(Modifiers.Async));
		}
	}
}

