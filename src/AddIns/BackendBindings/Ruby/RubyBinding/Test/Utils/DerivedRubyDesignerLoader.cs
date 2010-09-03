// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.IO;
using ICSharpCode.RubyBinding;
using ICSharpCode.FormsDesigner;

namespace RubyBinding.Tests.Utils
{
	/// <summary>
	/// RubyDesignerLoader derived class that provides access to
	/// various protected methods so we can use them when testing.
	/// </summary>
	public class DerivedRubyDesignerLoader : RubyDesignerLoader
	{				
		public DerivedRubyDesignerLoader(IRubyDesignerGenerator generator) : base(generator)
		{
		}

		public void CallPerformFlush(IDesignerSerializationManager serializationManager)
		{
			base.PerformFlush(serializationManager);
		}

		protected override void OnEndLoad(bool successful, ICollection errors)
		{
			System.Console.WriteLine("DesignerLoader.OnEndLoad: successful: " + successful);
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
			System.Console.WriteLine("DesignerLoader.OnBeginLoad");
			base.OnBeginLoad();
		}
		
		protected override void Initialize()
		{
			System.Console.WriteLine("DesignerLoader.Initialize");
			base.Initialize();
		}
		
		protected override void PerformLoad(IDesignerSerializationManager manager)
		{
			System.Console.WriteLine("DesignerLoader.PerformLoad Before");
			base.PerformLoad(manager);
			System.Console.WriteLine("DesignerLoader.PerformLoad After");
		}
	}
}
