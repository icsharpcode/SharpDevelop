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
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Windows.Forms;

using ICSharpCode.PythonBinding;
using ICSharpCode.Scripting.Tests.Utils;
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
