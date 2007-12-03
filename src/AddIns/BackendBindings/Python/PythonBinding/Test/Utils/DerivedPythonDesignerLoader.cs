// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.ComponentModel.Design;
using System.IO;
using ICSharpCode.PythonBinding;
using ICSharpCode.FormsDesigner;
using ICSharpCode.TextEditor.Document;
using IronPython.CodeDom;

namespace PythonBinding.Tests.Utils
{
	/// <summary>
	/// PythonDesignerLoader derived class that provides access to
	/// various protected methods so we can use them when testing.
	/// </summary>
	public class DerivedPythonDesignerLoader : PythonDesignerLoader
	{
		bool loadingBeforeBeginLoad;
		bool loadingAfterBeginLoad;
		bool loadingBeforeOnEndLoad;
		bool loadingAfterOnEndLoad;
		CodeCompileUnit codeCompileUnit;
		
		public DerivedPythonDesignerLoader(IDocument document, IDesignerGenerator generator) : base(document, generator)
		{
		}
		
		public bool IsLoadingBeforeBeginLoad {
			get { return loadingBeforeBeginLoad; }
		}
		
		public bool IsLoadingAfterBeginLoad {
			get { return loadingAfterBeginLoad; }
		}
		
		public bool IsLoadingBeforeOnEndLoad {
			get { return loadingBeforeOnEndLoad; }
		}
		
		public bool IsLoadingAfterOnEndLoad {
			get { return loadingAfterOnEndLoad; }
		}
				
		/// <summary>
		/// Calls the PythonDesignerLoader's Write method.
		/// </summary>
		public void CallWrite(CodeCompileUnit unit)
		{
			base.Write(unit);
		}
		
		/// <summary>
		/// Calls the PythonDesignerLoader's Parse method.
		/// </summary>
		public CodeCompileUnit CallParse()
		{
			return base.Parse();
		}
		
		/// <summary>
		/// Gets the last code compile unit returned from the 
		/// designer loader's Parse method.
		/// </summary>
		public CodeCompileUnit CodeCompileUnit {
			get { return codeCompileUnit; }
		}
				
		public CodeDomProvider GetCodeDomProvider()
		{
			return base.CodeDomProvider;
		}
		
		public ITypeResolutionService GetTypeResolutionService()
		{
			return TypeResolutionService;
		}
				
		public override void BeginLoad(System.ComponentModel.Design.Serialization.IDesignerLoaderHost host)
		{
			loadingBeforeBeginLoad = base.Loading;
			base.BeginLoad(host);
			loadingAfterBeginLoad = base.Loading;
		}
		
		protected override void OnBeginUnload()
		{
			Console.WriteLine("DesignerLoader.OnBeginUnload");
			base.OnBeginUnload();
		}
		
		protected override void OnModifying()
		{
			Console.WriteLine("DesignerLoader.OnModifying");
			base.OnModifying();
		}
		
		protected override void OnEndLoad(bool successful, ICollection errors)
		{
			Console.WriteLine("DesignerLoader.OnEndLoad: successful: " + successful);
			if (errors != null) {
				foreach (object o in errors) {
					Exception ex = o as Exception;
					if (ex != null) {
						Console.WriteLine("DesignerLoader.OnEndLoad: Exception: " + ex.ToString());
					}
				}
			}
			loadingBeforeOnEndLoad = base.Loading;
			base.OnEndLoad(successful, errors);
			loadingAfterOnEndLoad = base.Loading;
		}
		
		protected override void OnBeginLoad()
		{
			Console.WriteLine("DesignerLoader.OnBeginLoad");
			base.OnBeginLoad();
		}
		
		protected override void Initialize()
		{
			Console.WriteLine("DesignerLoader.Initialize");
			base.Initialize();
		}
		
		protected override void PerformLoad(System.ComponentModel.Design.Serialization.IDesignerSerializationManager manager)
		{
			Console.WriteLine("DesignerLoader.PerformLoad Before");
			base.PerformLoad(manager);
			Console.WriteLine("DesignerLoader.PerformLoad After");
		}

		/// <summary>
		/// Saves the last CodeCompileUnit created by the
		/// PythonDesignerLoader.
		/// </summary>
		protected override CodeCompileUnit Parse()
		{
			codeCompileUnit = base.Parse();
			return codeCompileUnit;
		}		
	}
}
