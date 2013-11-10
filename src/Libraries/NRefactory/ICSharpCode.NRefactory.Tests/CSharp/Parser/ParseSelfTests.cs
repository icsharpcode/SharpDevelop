// Copyright (c) 2010-2013 AlphaSierraPapa for the SharpDevelop Team
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
using System.IO;
using ICSharpCode.NRefactory.Editor;
using ICSharpCode.NRefactory.TypeSystem;
using NUnit.Framework;

namespace ICSharpCode.NRefactory.CSharp.Parser
{
	/// <summary>
	/// Test fixture that parses NRefactory itself,
	/// ensures that there are no parser crashes while doing so,
	/// and that the returned positions are consistent.
	/// </summary>
	[TestFixture]
	public class ParseSelfTests
	{
		string[] fileNames;
		
		[TestFixtureSetUp]
		public void SetUp()
		{
			string path = Path.GetFullPath (Path.Combine ("..", ".."));
			if (!File.Exists(Path.Combine(path, "NRefactory.sln")))
				throw new InvalidOperationException("Test cannot find the NRefactory source code in " + path);
			fileNames = Directory.GetFiles(path, "*.cs", SearchOption.AllDirectories);
		}
		
		[Test]
		public void GenerateTypeSystem()
		{
			IProjectContent pc = new CSharpProjectContent();
			CSharpParser parser = new CSharpParser();
			parser.GenerateTypeSystemMode = true;
			foreach (string fileName in fileNames) {
				SyntaxTree syntaxTree;
				using (var fs = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, FileOptions.SequentialScan)) {
					syntaxTree = parser.Parse(fs, fileName);
				}
				var unresolvedFile = syntaxTree.ToTypeSystem();
				foreach (var td in unresolvedFile.GetAllTypeDefinitions()) {
					Assert.AreSame(unresolvedFile, td.UnresolvedFile);
					foreach (var member in td.Members) {
						Assert.AreSame(unresolvedFile, member.UnresolvedFile);
						Assert.AreSame(td, member.DeclaringTypeDefinition);
					}
				}
				pc = pc.AddOrUpdateFiles(unresolvedFile);
			}
		}
		
		#region ParseAndCheckPositions
		
		[Test, Ignore("Positions still are incorrect in several cases")]
		public void ParseAndCheckPositions()
		{
			CSharpParser parser = new CSharpParser();
			foreach (string fileName in fileNames) {
				var currentDocument = new ReadOnlyDocument(File.ReadAllText(fileName));
				SyntaxTree syntaxTree = parser.Parse(currentDocument, fileName);
				if (parser.HasErrors)
					continue;
				ConsistencyChecker.CheckPositionConsistency(syntaxTree, fileName, currentDocument);
				ConsistencyChecker.CheckMissingTokens(syntaxTree, fileName, currentDocument);
			}
		}
		
		#endregion
	}
}
