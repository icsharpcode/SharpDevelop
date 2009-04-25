// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Windows.Forms;
using ICSharpCode.PythonBinding;
using IronPython.Compiler.Ast;
using NUnit.Framework;
using PythonBinding.Tests.Utils;

namespace PythonBinding.Tests.Designer
{
	/// <summary>
	/// Base class for all tests of the PythonCodeDeserialize when deserializing an 
	/// assignment.
	/// </summary>
	public abstract class DeserializeAssignmentTestFixtureBase
	{
		protected Node rhsAssignmentNode;
		protected object deserializedObject;
		protected MockDesignerLoaderHost mockDesignerLoaderHost;
		protected MockTypeResolutionService typeResolutionService;
		protected MockComponentCreator componentCreator;
		
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			componentCreator = new MockComponentCreator();

			AssignmentStatement assignment = PythonParserHelper.GetAssignmentStatement(GetPythonCode());
			rhsAssignmentNode = assignment.Right;
			
			mockDesignerLoaderHost = new MockDesignerLoaderHost();
			typeResolutionService = mockDesignerLoaderHost.TypeResolutionService;
			PythonCodeDeserializer deserializer = new PythonCodeDeserializer(componentCreator);
			deserializedObject = deserializer.Deserialize(rhsAssignmentNode);
		}
		
		public abstract string GetPythonCode();
	}
}
