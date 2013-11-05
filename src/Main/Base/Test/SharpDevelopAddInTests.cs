// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
