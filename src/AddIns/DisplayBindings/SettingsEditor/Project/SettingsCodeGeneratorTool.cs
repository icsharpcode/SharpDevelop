// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.CodeDom;
using System.Configuration;
using System.Diagnostics;
using System.Reflection;
using System.Xml;

using ICSharpCode.EasyCodeDom;
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
			ccu.AddNamespace(setDoc.GeneratedClassNamespace).Types.Add(CreateClass(setDoc));
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
			
			c.AddProperty(f, "Default");
			
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
