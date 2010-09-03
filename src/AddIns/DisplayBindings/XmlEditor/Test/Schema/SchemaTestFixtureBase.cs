// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
