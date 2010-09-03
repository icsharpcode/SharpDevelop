// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.IO;

using ICSharpCode.FormsDesigner;
using ICSharpCode.PythonBinding;
using ICSharpCode.Scripting;

namespace PythonBinding.Tests.Utils
{
	/// <summary>
	/// PythonDesignerLoader derived class that provides access to
	/// various protected methods so we can use them when testing.
	/// </summary>
	public class DerivedPythonDesignerLoader : PythonDesignerLoader
	{				
		public DerivedPythonDesignerLoader(IScriptingDesignerGenerator generator) : base(generator)
		{
		}

		public void CallPerformFlush(IDesignerSerializationManager serializationManager)
		{
			base.PerformFlush(serializationManager);
		}

		protected override void OnEndLoad(bool successful, ICollection errors)
		{
			if (errors != null) {
				foreach (object o in errors) {
					Exception ex = o as Exception;
					if (ex != null) {
						System.Console.WriteLine("DesignerLoader.OnEndLoad: Exception: " + ex.ToString());
					}
				}
			}
			base.OnEndLoad(successful, errors);
		}
		
		protected override void OnBeginLoad()
		{
			base.OnBeginLoad();
		}
		
		protected override void Initialize()
		{
			base.Initialize();
		}
		
		protected override void PerformLoad(IDesignerSerializationManager manager)
		{
			base.PerformLoad(manager);
		}
	}
}
