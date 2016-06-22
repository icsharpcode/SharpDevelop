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
using System.IO;
using ICSharpCode.SharpDevelop.Editor.CodeCompletion;
using ICSharpCode.XmlEditor;
using NUnit.Framework;

namespace XmlEditor.Tests.Schema
{
	public abstract class SchemaTestFixtureBase
	{		
		XmlSchemaCompletion schemaCompletion;

		/// <summary>
		/// Gets the <see cref="XmlSchemaCompletionData"/> object generated
		/// by this class.
		/// </summary>
		/// <remarks>This object will be null until the <see cref="FixtureInitBase"/>
		/// has been run.</remarks>
		public XmlSchemaCompletion SchemaCompletion {
			get { return schemaCompletion; }
		}
		
		/// <summary>
		/// Creates the <see cref="XmlSchemaCompletionData"/> object from 
		/// the derived class's schema.
		/// </summary>
		/// <remarks>Calls <see cref="FixtureInit"/> at the end of the method.
		/// </remarks>
		[TestFixtureSetUp]
		public void FixtureInitBase()
		{
			schemaCompletion = CreateSchemaCompletionObject();
			FixtureInit();
		}
		
		/// <summary>
		/// Method overridden by derived class so it can execute its own
		/// fixture initialisation.
		/// </summary>
		public virtual void FixtureInit()
		{
		}
		
		/// <summary>
		/// Returns the schema that will be used in this test fixture.
		/// </summary>
		protected virtual string GetSchema()
		{
			return String.Empty;
		}
		
		/// <summary>
		/// Creates an <see cref="XmlSchemaCompletionData"/> object that 
		/// will be used in the test fixture.
		/// </summary>
		protected virtual XmlSchemaCompletion CreateSchemaCompletionObject()
		{
			StringReader reader = new StringReader(GetSchema());
			return new XmlSchemaCompletion(reader);
		}
	}
}
