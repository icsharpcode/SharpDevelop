// CSharp Editor Example with Code Completion
// Copyright (c) 2006, Daniel Grunwald
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without modification, are
// permitted provided that the following conditions are met:
// 
// - Redistributions of source code must retain the above copyright notice, this list
//   of conditions and the following disclaimer.
// 
// - Redistributions in binary form must reproduce the above copyright notice, this list
//   of conditions and the following disclaimer in the documentation and/or other materials
//   provided with the distribution.
// 
// - Neither the name of the ICSharpCode team nor the names of its contributors may be used to
//   endorse or promote products derived from this software without specific prior written
//   permission.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS &AS IS& AND ANY EXPRESS
// OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY
// AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
// DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE,
// DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER
// IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT
// OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Threading;

using NRefactory = ICSharpCode.NRefactory;
using Dom = ICSharpCode.SharpDevelop.Dom;

namespace CSharpEditor
{
	partial class MainForm
	{
		[STAThread]
		public static void Main(string[] args)
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new MainForm());
		}
		
		internal Dom.ProjectContentRegistry pcRegistry;
		internal Dom.DefaultProjectContent myProjectContent;
		internal Dom.ParseInformation parseInformation = new Dom.ParseInformation();
		Dom.ICompilationUnit lastCompilationUnit;
		Thread parserThread;
		
		public static bool IsVisualBasic = false;
		
		/// <summary>
		/// Many SharpDevelop.Dom methods take a file name, which is really just a unique identifier
		/// for a file - Dom methods don't try to access code files on disk, so the file does not have
		/// to exist.
		/// SharpDevelop itself uses internal names of the kind "[randomId]/Class1.cs" to support
		/// code-completion in unsaved files.
		/// </summary>
		public const string DummyFileName = "edited.cs";
		
		static readonly Dom.LanguageProperties CurrentLanguageProperties = Dom.LanguageProperties.CSharp;
		
		public MainForm()
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			
			if (IsVisualBasic) {
				textEditorControl1.Text = @"
Class A
 Sub B
  Dim xx As String
  
 End Sub
End Class
";
				textEditorControl1.SetHighlighting("VBNET");
			} else {
				textEditorControl1.Text = @"using System;
class A
{
 void B()
 {
  string x;
  
 }
}
";
				textEditorControl1.SetHighlighting("C#");
			}
			textEditorControl1.ShowEOLMarkers = false;
			CodeCompletionKeyHandler.Attach(this, textEditorControl1);
			HostCallbackImplementation.Register(this);
			
			pcRegistry = new Dom.ProjectContentRegistry(); // Default .NET 2.0 registry
			
			// Persistence caches referenced project contents for faster loading.
			// It also activates loading XML documentation files and caching them
			// for faster loading and lower memory usage.
			pcRegistry.ActivatePersistence(Path.Combine(Path.GetTempPath(),
			                                            "CSharpCodeCompletion"));
			
			myProjectContent = new Dom.DefaultProjectContent();
			myProjectContent.Language = CurrentLanguageProperties;
		}
		
		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			
			parserThread = new Thread(ParserThread);
			parserThread.IsBackground = true;
			parserThread.Start();
		}
		
		void ParserThread()
		{
			BeginInvoke(new MethodInvoker(delegate { parserThreadLabel.Text = "Loading mscorlib..."; }));
			myProjectContent.AddReferencedContent(pcRegistry.Mscorlib);
			
			// do one initial parser step to enable code-completion while other
			// references are loading
			ParseStep();
			
			string[] referencedAssemblies = {
				"System", "System.Data", "System.Drawing", "System.Xml", "System.Windows.Forms", "Microsoft.VisualBasic"
			};
			foreach (string assemblyName in referencedAssemblies) {
				{ // block for anonymous method (capture assemblyNameCopy correctly)
					string assemblyNameCopy = assemblyName;
					BeginInvoke(new MethodInvoker(delegate { parserThreadLabel.Text = "Loading " + assemblyNameCopy + "..."; }));
				}
				myProjectContent.AddReferencedContent(pcRegistry.GetProjectContentForReference(assemblyName, assemblyName));
			}
			if (IsVisualBasic) {
				myProjectContent.DefaultImports = new Dom.DefaultUsing(myProjectContent);
				myProjectContent.DefaultImports.Usings.Add("System");
				myProjectContent.DefaultImports.Usings.Add("System.Text");
				myProjectContent.DefaultImports.Usings.Add("Microsoft.VisualBasic");
			}
			BeginInvoke(new MethodInvoker(delegate { parserThreadLabel.Text = "Ready"; }));
			
			// Parse the current file every 2 seconds
			while (!IsDisposed) {
				ParseStep();
				
				Thread.Sleep(2000);
			}
		}
		
		void ParseStep()
		{
			string code = null;
			Invoke(new MethodInvoker(delegate {
			                         	code = textEditorControl1.Text;
			                         }));
			TextReader textReader = new StringReader(code);
			Dom.ICompilationUnit newCompilationUnit;
			NRefactory.SupportedLanguage supportedLanguage;
			if (IsVisualBasic)
				supportedLanguage = NRefactory.SupportedLanguage.VBNet;
			else
				supportedLanguage = NRefactory.SupportedLanguage.CSharp;
			using (NRefactory.IParser p = NRefactory.ParserFactory.CreateParser(supportedLanguage, textReader)) {
				p.Parse();
				newCompilationUnit = ConvertCompilationUnit(p.CompilationUnit);
			}
			// Remove information from lastCompilationUnit and add information from newCompilationUnit.
			myProjectContent.UpdateCompilationUnit(lastCompilationUnit, newCompilationUnit, DummyFileName);
			lastCompilationUnit = newCompilationUnit;
			parseInformation.ValidCompilationUnit = newCompilationUnit;
		}
		
		Dom.ICompilationUnit ConvertCompilationUnit(NRefactory.Ast.CompilationUnit cu)
		{
			Dom.NRefactoryResolver.NRefactoryASTConvertVisitor converter;
			converter = new Dom.NRefactoryResolver.NRefactoryASTConvertVisitor(myProjectContent);
			cu.AcceptVisitor(converter, null);
			return converter.Cu;
		}
	}
}
