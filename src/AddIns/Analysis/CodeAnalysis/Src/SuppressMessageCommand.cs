// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;

using ICSharpCode.Core;
using ICSharpCode.NRefactory.Semantics;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.NRefactory.TypeSystem.Implementation;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Refactoring;

namespace ICSharpCode.CodeAnalysis
{
	public class SuppressMessageCommand : AbstractMenuCommand
	{
		public override void Run()
		{
			var view = (System.Windows.Controls.ListView)Owner;
			foreach (var t in view.SelectedItems.OfType<SDTask>().ToArray()) {
				FxCopTaskTag tag = t.Tag as FxCopTaskTag;
				if (tag == null)
					continue;
				CodeGenerator gen = tag.Project.LanguageBinding.CodeGenerator;
				ICompilation compilation;
				if (t.FileName != null)
					compilation = SD.ParserService.GetCompilationForFile(t.FileName);
				else
					compilation = SD.ParserService.GetCompilation(tag.Project);
				IAttribute attribute = CreateSuppressAttribute(compilation, tag);
				
				if (tag.MemberName == null)
					gen.AddAssemblyAttribute(tag.Project, attribute);
				else
					gen.AddAttribute(GetEntity(compilation, tag.TypeName, tag.MemberName), attribute);
			}
		}
		
		internal static DomRegion GetPosition(ICompilation compilation, string className, string memberName)
		{
			IEntity entity = GetEntity(compilation, className, memberName);
			return entity == null ? DomRegion.Empty : entity.Region;
		}
		
		static IEntity GetEntity(ICompilation compilation, string className, string memberName)
		{
			var typeName = new TopLevelTypeName(className);
			ITypeDefinition type = compilation.MainAssembly.GetTypeDefinition(typeName);
			if (type != null) {
				if (string.IsNullOrEmpty(memberName))
					return type;
				
				IMember m = GetMember(type, memberName);
				if (m != null)
					return m;
				
				return type;
			}
			return null;
		}
		
		static IMember GetMember(ITypeDefinition type, string memberName)
		{
			string fullName = type.ReflectionName + "." + memberName;
			return type.GetMembers(m => m.ReflectionName == fullName).FirstOrDefault();
		}
		
		const string FullAttributeName = "System.Diagnostics.CodeAnalysis.SuppressMessageAttribute";
		
		static IAttribute CreateSuppressAttribute(ICompilation compilation, FxCopTaskTag tag)
		{
			// [SuppressMessage("Microsoft.Performance", "CA1801:ReviewUnusedParameters", MessageId:="fileIdentifier"]
			IType attributeType = compilation.FindType(new FullTypeName(FullAttributeName));
			IType stringType = compilation.FindType(KnownTypeCode.String);
			
			KeyValuePair<IMember, ResolveResult>[] namedArgs = null;
			if (tag.MessageID != null) {
				IMember messageId = attributeType.GetProperties(p => p.Name == "MessageId").FirstOrDefault();
				namedArgs = new[] { new KeyValuePair<IMember, ResolveResult>(messageId, new ConstantResolveResult(stringType, tag.MessageID)) };
			}
			
			return new DefaultAttribute(
				attributeType, new[] {
					new ConstantResolveResult(stringType, tag.Category),
					new ConstantResolveResult(stringType, tag.CheckID)
				}, namedArgs);
		}
	}
}
