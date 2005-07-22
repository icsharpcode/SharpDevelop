// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.IO;
using System.Windows.Forms;
using System.Diagnostics;
using System.Drawing;
using System.Collections.Generic;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.SharpDevelop.Gui
{
	public class MemberNode : ExtTreeNode
	{
		int    line;
		int    column;
		ModifierEnum modifiers;
		IClass declaringType;
		
		string FileName {
			get {
				if (declaringType == null || declaringType.CompilationUnit == null) {
					return null;
				}
				return declaringType.CompilationUnit.FileName;
			}
		}
		public override bool Visible {
			get {
				ClassBrowserFilter filter = ClassBrowser.Instance.Filter;
				if ((modifiers & ModifierEnum.Public) != 0) {
					return (filter & ClassBrowserFilter.ShowPublic) != 0;
				}
				if ((modifiers & ModifierEnum.Protected) != 0) {
					return (filter & ClassBrowserFilter.ShowProtected) != 0;
				}
				if ((modifiers & ModifierEnum.Private) != 0) {
					return (filter & ClassBrowserFilter.ShowPrivate) != 0;
				}
				return (filter & ClassBrowserFilter.ShowOther) != 0;
			}
		}
		
		public MemberNode(IClass declaringType, IMethod method)
		{
			sortOrder = 10;
			
			this.declaringType = declaringType;
			modifiers = method.Modifiers;
			Text = AppendReturnType(GetAmbience().Convert(method), method.ReturnType);
			SelectedImageIndex = ImageIndex = ClassBrowserIconService.GetIcon(method);
			if (method.Region != null) {
				line   = method.Region.BeginLine;
				column = method.Region.BeginColumn;
			}
		}
		
		
		public MemberNode(IClass declaringType, IProperty property)
		{
			sortOrder = 12;
			
			this.declaringType = declaringType;
			modifiers = property.Modifiers;
			Text = AppendReturnType(GetAmbience().Convert(property), property.ReturnType);
			SelectedImageIndex = ImageIndex = ClassBrowserIconService.GetIcon(property);
			if (property.Region != null) {
				line   = property.Region.BeginLine;
				column = property.Region.BeginColumn;
			}
		}
		
		
		public MemberNode(IClass declaringType, IIndexer indexer)
		{
			sortOrder = 13;
			this.declaringType = declaringType;
			modifiers = indexer.Modifiers;
			Text = AppendReturnType(GetAmbience().Convert(indexer), indexer.ReturnType);
			SelectedImageIndex = ImageIndex = ClassBrowserIconService.GetIcon(indexer);
			if (indexer.Region != null) {
				line   = indexer.Region.BeginLine;
				column = indexer.Region.BeginColumn;
			}
		}
		
		
		public MemberNode(IClass declaringType, IField field)
		{
			sortOrder = 11;
			
			this.declaringType = declaringType;
			modifiers = field.Modifiers;
			Text = AppendReturnType(GetAmbience().Convert(field), field.ReturnType);
			SelectedImageIndex = ImageIndex = ClassBrowserIconService.GetIcon(field);
			if (field.Region != null) {
				line   = field.Region.BeginLine;
				column = field.Region.BeginColumn;
			}
		}
		
		public MemberNode(IClass declaringType, IEvent e)
		{
			sortOrder = 14;
			this.declaringType = declaringType;
			modifiers = e.Modifiers;
			Text = AppendReturnType(GetAmbience().Convert(e), e.ReturnType);
			SelectedImageIndex = ImageIndex = ClassBrowserIconService.GetIcon(e);
			if (e.Region != null) {
				line   = e.Region.BeginLine;
				column = e.Region.BeginColumn;
			}
		}
		
		IAmbience GetAmbience()
		{
			IAmbience ambience = AmbienceService.CurrentAmbience;
			ambience.ConversionFlags = ConversionFlags.None;
			return ambience;
		}
		
		string AppendReturnType(string text, IReturnType rt)
		{
			// TODO: Give user the possibility to turn off visibility of the return type
			return text + " : " + GetAmbience().Convert(rt);
		}
		
		public override void ActivateItem()
		{
			if (FileName != null) {
				FileService.JumpToFilePosition(FileName, line - 1, column - 1);
			}
		}
	}
}
