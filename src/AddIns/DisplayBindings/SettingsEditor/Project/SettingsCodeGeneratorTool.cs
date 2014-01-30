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
using System.CodeDom;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.Reflection;
using System.Xml;

using ICSharpCode.EasyCodeDom;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.SettingsEditor
{
	public class SettingsCodeGeneratorTool : ICustomTool
	{
		public void GenerateCode(FileProjectItem item, CustomToolContext context)
		{
			XmlDocument doc = new XmlDocument();
			doc.Load(item.FileName);
			SettingsDocument setDoc = new SettingsDocument(doc.DocumentElement, DummySettingsEntryHost.Instance);
			string customToolNamespace = item.GetEvaluatedMetadata("CustomToolNamespace");
			if (!string.IsNullOrEmpty(customToolNamespace)) {
				setDoc.GeneratedClassNamespace = customToolNamespace;
			}
			
			CodeCompileUnit ccu = new CodeCompileUnit();
			var ns = ccu.AddNamespace(setDoc.GeneratedClassNamespace);
			ns.Types.Add(CreateClass(setDoc));
			if (setDoc.UseMySettingsClassName) {
				ns.Types.Add(CreateMySettingsProperty(setDoc));
			}
			context.WriteCodeDomToFile(item, context.GetOutputFileName(item, ".Designer"), ccu);
		}
		
		public virtual CodeTypeDeclaration CreateClass(SettingsDocument setDoc)
		{
			CodeTypeDeclaration c = new CodeTypeDeclaration(setDoc.GeneratedClassName);
			c.AddAttribute(typeof(System.Runtime.CompilerServices.CompilerGeneratedAttribute));
			c.AddAttribute(typeof(System.CodeDom.Compiler.GeneratedCodeAttribute),
			               Easy.Prim(typeof(SettingsCodeGeneratorTool).FullName),
			               Easy.Prim(typeof(SettingsCodeGeneratorTool).Assembly.GetName().Version.ToString()));
			c.TypeAttributes = TypeAttributes.NotPublic | TypeAttributes.Sealed;
			c.IsPartial = true;
			c.BaseTypes.Add(Easy.TypeRef(typeof(ApplicationSettingsBase)));
			
			CodeMemberField f = c.AddField(Easy.TypeRef(c), "defaultInstance");
			f.Attributes = MemberAttributes.Private | MemberAttributes.Static;
			f.InitExpression = Easy.Type(typeof(ApplicationSettingsBase))
				.InvokeMethod("Synchronized", Easy.New(Easy.TypeRef(c)))
				.CastTo(Easy.TypeRef(c));
			
			var defaultProperty = c.AddProperty(f, "Default");
			
			if (setDoc.UseMySettingsClassName) {
				c.AddAttribute(typeof(EditorBrowsableAttribute), Easy.Prim(EditorBrowsableState.Advanced));
				AddAutoSaveLogic(c, defaultProperty);
			}
			
			foreach (SettingsEntry entry in setDoc.Entries) {
				Type entryType = entry.Type ?? typeof(string);
				SpecialSetting? specialSetting = null;
				foreach (SpecialTypeDescriptor desc in SpecialTypeDescriptor.Descriptors) {
					if (desc.type == entryType) {
						entryType = typeof(string);
						specialSetting = desc.specialSetting;
						break;
					}
				}
				EasyProperty p = c.AddProperty(entryType, entry.Name);
				if (entry.Scope == SettingScope.User) {
					p.AddAttribute(typeof(UserScopedSettingAttribute));
				} else {
					p.AddAttribute(typeof(ApplicationScopedSettingAttribute));
				}
				if (!string.IsNullOrEmpty(entry.Provider)) {
					p.AddAttribute(typeof(SettingsProviderAttribute),
					               Easy.TypeOf(new CodeTypeReference(entry.Provider)));
				}
				if (!string.IsNullOrEmpty(entry.Description)) {
					p.AddAttribute(typeof(SettingsDescriptionAttribute), Easy.Prim(entry.Description));
					Easy.AddSummary(p, entry.Description);
				}
				p.AddAttribute(typeof(DebuggerNonUserCodeAttribute));
				if (specialSetting != null) {
					p.AddAttribute(typeof(SpecialSettingAttribute),
					               Easy.Prim(specialSetting.Value));
				}
				if (entry.GenerateDefaultValueInCode) {
					p.AddAttribute(typeof(DefaultSettingValueAttribute), Easy.Prim(entry.SerializedValue));
				}
				if (entry.Scope == SettingScope.User && entry.Roaming) {
					p.AddAttribute(typeof(SettingsManageabilityAttribute),
					               Easy.Prim(SettingsManageability.Roaming));
				}
				p.Getter.Return(Easy.This.Index(Easy.Prim(entry.Name)).CastTo(entryType));
//				p.GetStatements.Add(new CodeMethodReturnStatement(
//					new CodeCastExpression(new CodeTypeReference(entryType),
//					                       new CodeIndexerExpression(new CodeThisReferenceExpression(),
//					                                                 new CodePrimitiveExpression(entry.Name))
//					                      )
//				));
				if (entry.Scope == SettingScope.User) {
					p.Setter.Assign(Easy.This.Index(Easy.Prim(entry.Name)), Easy.Value);
				}
			}
			
			return c;
		}
		
		void AddAutoSaveLogic(CodeTypeDeclaration c, CodeMemberProperty defaultProperty)
		{
			// VB auto-safe logic:
			
			c.Members.Add(new CodeSnippetTypeMember(
				@"		#Region ""Support for My.Application.SaveMySettingsOnExit""
		#If _MyType = ""WindowsForms"" Then
		Private Shared addedHandler As Boolean
		Private Shared addedHandlerLockObject As New Object

		<Global.System.Diagnostics.DebuggerNonUserCodeAttribute(), Global.System.ComponentModel.EditorBrowsableAttribute(Global.System.ComponentModel.EditorBrowsableState.Advanced)> _
		Private Shared Sub AutoSaveSettings(ByVal sender As Global.System.Object, ByVal e As Global.System.EventArgs)
			If My.Application.SaveMySettingsOnExit Then
				My.Settings.Save()
			End If
		End Sub
		#End If
		#End Region".Replace("\t", SD.EditorControlService.GlobalOptions.IndentationString)
			));
			
			defaultProperty.GetStatements.Insert(0, new CodeSnippetStatement(
				@"				#If _MyType = ""WindowsForms"" Then
				If Not addedHandler Then
					SyncLock addedHandlerLockObject
						If Not addedHandler Then
							AddHandler My.Application.Shutdown, AddressOf AutoSaveSettings
							addedHandler = True
						End If
					End SyncLock
				End If
				#End If".Replace("\t", SD.EditorControlService.GlobalOptions.IndentationString)
			));
		}
		
		class CodeLiteralDirective : CodeDirective
		{
			string text;
			
			public CodeLiteralDirective(string text)
			{
				this.text = text;
			}
			
			public override string ToString()
			{
				return text;
			}
		}
		
		CodeTypeDeclaration CreateMySettingsProperty(SettingsDocument setDoc)
		{
			CodeTypeDeclaration c = new CodeTypeDeclaration("MySettingsProperty");
			c.UserData["Module"] = true;
			c.AddAttribute(new CodeTypeReference("Microsoft.VisualBasic.HideModuleNameAttribute"));
			c.AddAttribute(typeof(DebuggerNonUserCodeAttribute));
			c.AddAttribute(typeof(System.Runtime.CompilerServices.CompilerGeneratedAttribute));
			c.TypeAttributes = TypeAttributes.NotPublic;
			
			CodeTypeReference r = new CodeTypeReference(setDoc.GeneratedFullClassName);
			var p = c.AddProperty(r, "Settings");
			p.Attributes = MemberAttributes.Assembly | MemberAttributes.Static;
			p.Getter.Return(Easy.Type(r).Property("Default"));
			p.AddAttribute(typeof(System.ComponentModel.Design.HelpKeywordAttribute), Easy.Prim("My.Settings"));
			return c;
		}
	}
	
	public class PublicSettingsCodeGeneratorTool : SettingsCodeGeneratorTool
	{
		public override CodeTypeDeclaration CreateClass(SettingsDocument setDoc)
		{
			CodeTypeDeclaration ctd = base.CreateClass(setDoc);
			ctd.TypeAttributes &= ~TypeAttributes.NotPublic;
			ctd.TypeAttributes |= TypeAttributes.Public;
			return ctd;
		}
	}
}
