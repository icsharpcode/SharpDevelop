// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Resources;
using System.Windows.Forms;

using ICSharpCode.PythonBinding;
using ICSharpCode.Scripting;
using IronPython.Compiler.Ast;
using Microsoft.Scripting;
using NUnit.Framework;
using PythonBinding.Tests.Utils;

namespace PythonBinding.Tests.Designer
{
	/// <summary>
	/// Tests that the PythonFormVisitor throws an exception if no InitializeComponent or 
	/// InitializeComponent method can be found.
	/// </summary>
	[TestFixture]
	public class MissingInitializeComponentMethodTestFixture : IComponentCreator
	{		
		string pythonCode = "from System.Windows.Forms import Form\r\n" +
							"\r\n" +
							"class MainForm(System.Windows.Forms.Form):\r\n" +
							"    def __init__(self):\r\n" +
							"    self.MissingMethod()\r\n" +
							"\r\n" +
							"    def MissingMethod(self):\r\n" +
							"        pass\r\n";		
		[Test]
		[ExpectedException(typeof(PythonComponentWalkerException))]
		public void PythonFormWalkerExceptionThrown()
		{
			PythonComponentWalker walker = new PythonComponentWalker(this);
			walker.CreateComponent(pythonCode);
			Assert.Fail("Exception should have been thrown before this.");
		}
		
		/// <summary>
		/// Check that the PythonComponentWalker does not try to walk the class body if it is null.
		/// IronPython 2.6.1 now throws an ArgumentNullException if null is passed for the 
		/// class body.
		/// </summary>
		[Test]
		public void ClassWithNoBodyCannotBeCreated()
		{
			ArgumentNullException ex = 
				Assert.Throws<ArgumentNullException>(delegate { CreateClassWithNullBody(); });
			
			Assert.AreEqual("body", ex.ParamName);
		}
		
		void CreateClassWithNullBody()
		{
			ClassDefinition classDef = new ClassDefinition("classWithNoBody", null, null);
		}
		
		/// <summary>
		/// Make sure we do not get an ArgumentOutOfRangeException when walking the
		/// AssignmentStatement.
		/// </summary>
		[Test]
		public void NoLeftHandSideExpressionsInAssignment()
		{
			List<Expression> lhs = new List<Expression>();
			AssignmentStatement assign = new AssignmentStatement(lhs.ToArray(), null);
			PythonComponentWalker walker = new PythonComponentWalker(this);
			walker.Walk(assign);
		}
		
		public IComponent CreateComponent(Type componentClass, string name)
		{
			throw new NotImplementedException();
		}
		
		public void Add(IComponent component, string name)
		{
			throw new NotImplementedException();
		}
		
		public IComponent GetComponent(string name)
		{
			return null;
		}
		
		public IComponent RootComponent {
			get { return null; }
		}
		
		public object CreateInstance(Type type, ICollection arguments, string name, bool addToContainer)
		{
			throw new NotImplementedException();
		}
		
		public Type GetType(string typeName)
		{
			throw new NotImplementedException();
		}
		
		public PropertyDescriptor GetEventProperty(EventDescriptor e)
		{
			return null;
		}
		
		public object GetInstance(string name)
		{
			return null;
		}
	
		public IResourceReader GetResourceReader(CultureInfo info)
		{
			return null;
		}
		
		public IResourceWriter GetResourceWriter(CultureInfo info)
		{
			return null;
		}		
	}
}
