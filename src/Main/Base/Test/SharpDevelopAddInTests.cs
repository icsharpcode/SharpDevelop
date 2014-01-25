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
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using System.Linq;
using ICSharpCode.Core;
using NUnit.Framework;

namespace ICSharpCode.SharpDevelop
{
	[TestFixture]
	public class SharpDevelopAddInTests
	{
		List<XDocument> addInFiles;
		
		[TestFixtureSetUp]
		public void SetUp()
		{
			string dir = @"..\..\AddIns";
			addInFiles = new List<XDocument>();
			foreach (string file in Directory.GetFiles(dir, "*.addin", SearchOption.AllDirectories)) {
				XDocument doc = XDocument.Load(file);
				doc.AddAnnotation(FileName.Create(file));
				addInFiles.Add(doc);
			}
		}
		
		[Test]
		public void FoundMainAddIn()
		{
			Assert.IsTrue(addInFiles.Any(addIn => addIn.Annotation<FileName>().GetFileName() == "ICSharpCode.SharpDevelop.addin"));
		}
		
		[Test]
		public void IsPreinstalled()
		{
			foreach (var addIn in addInFiles) {
				if (addIn.Annotation<FileName>().GetFileName() == "ICSharpCode.SharpDevelop.addin") {
					Assert.AreEqual("true", (string)addIn.Root.Attribute("addInManagerHidden"));
				} else {
					Assert.AreEqual("preinstalled", (string)addIn.Root.Attribute("addInManagerHidden"));
				}
			}
		}
		
		[Test]
		public void HaveIdentity()
		{
			foreach (var addIn in addInFiles) {
				var manifest = addIn.Root.Element("Manifest");
				Assert.IsNotNull(manifest, "<Manifest> missing in " + addIn.Annotation<FileName>());
				Assert.IsNotNull(manifest.Element("Identity"), "<Identity> missing in " + addIn.Annotation<FileName>());
			}
		}
	}
}
