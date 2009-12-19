// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Xml.Schema;
using ICSharpCode.XmlEditor;

namespace XmlEditor.Tests.Utils
{
	public class DerivedXmlSchemaObjectLocation : XmlSchemaObjectLocation
	{
		bool jumpToFilePositionCalled;
		string jumpToFilePositionMethodFileNameParameter;
		int jumpToFilePositionMethodLineParameter = -1;
		int jumpToFilePositionMethodColumnParameter = -1;
		
		public DerivedXmlSchemaObjectLocation(XmlSchemaObject schemaObject)
			: base(schemaObject)
		{
		}
		
		public bool IsDerivedJumpToFilePositionMethodCalled {
			get { return jumpToFilePositionCalled; }
		}
		
		public string JumpToFilePositionMethodFileNameParameter {
			get { return jumpToFilePositionMethodFileNameParameter; }
		}
		
		public int JumpToFilePositionMethodLineParameter {
			get { return jumpToFilePositionMethodLineParameter; }
		}
		
		public int JumpToFilePositionMethodColumnParameter {
			get { return jumpToFilePositionMethodColumnParameter; }
		}
		
		public void CallJumpToFilePosition(string fileName, int line, int column)
		{
			JumpToFilePosition(fileName, line, column);
		}
		
		protected override void JumpToFilePosition(string fileName, int line, int column)
		{
			jumpToFilePositionCalled = true;
			jumpToFilePositionMethodFileNameParameter = fileName;
			jumpToFilePositionMethodLineParameter = line;
			jumpToFilePositionMethodColumnParameter = column;
		}
	}
}
