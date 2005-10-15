// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.NAntAddIn;
using NUnit.Framework;
using System;
using System.IO;
using System.Xml;

namespace ICSharpCode.NAntAddIn.Tests
{
	/// <summary>
	/// Tests the <see cref="NAntBuildFile"/> class.
	/// </summary>
	[TestFixture]
	public class InvalidNAntBuildFileTestFixture
	{
		NAntBuildFile buildFile;
		
		[Test]
		public void ReadFile()
		{
			StringReader reader = new StringReader("<project>");
			buildFile = new NAntBuildFile(reader);
			
			Assert.IsTrue(buildFile.HasError);
			
			NAntBuildFileError error = buildFile.Error;
			
			Assert.AreEqual("Unexpected end of file has occurred. The following elements are not closed: project. Line 1, position 10.", 
			                error.Message, 
			                "Error message is incorrect.");
			
			Assert.AreEqual(0, error.Line, "Error's line number is incorrect.");
			Assert.AreEqual(9, error.Column, "Error's column number is incorrect.");
		}
	}
}
