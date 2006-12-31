// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Text;
using System.IO;
using System.Xml;
using System.Diagnostics;
using NUnit.Framework;
using ICSharpCode.WpfDesign.Designer.Xaml;

namespace ICSharpCode.WpfDesign.Designer.Tests
{
	[TestFixture]
	public class ModelTests : ModelTestHelper
	{
		[Test]
		public void SetButtonWidth()
		{
			DesignItem button = CreateCanvasContext("<Button/>");
			button.Properties["Width"].SetValue(100.0);
			AssertCanvasDesignerOutput(@"<Button Width=""100"" />", button.Context);
			AssertLog("");
		}
	}
}
