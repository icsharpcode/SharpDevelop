// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Siegfried Pammer" email="siegfriedpammer@gmail.com"/>
//     <version>$Revision: 5529 $</version>
// </file>

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;

using ICSharpCode.AvalonEdit.Snippets;
using ICSharpCode.NRefactory.Ast;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Dom.Refactoring;
using ICSharpCode.SharpDevelop.Editor;

namespace SharpRefactoring.Gui
{
	public class CtorParamWrapper : INotifyPropertyChanged
	{
		IField field;
		
		public string Text {
			get { return field.ProjectContent.Language.GetAmbience().Convert(field); }
		}
		
		public bool IsNullable {
			get {
				return field.ReturnType.IsReferenceType == true ||
					field.ReturnType.IsConstructedReturnType && field.ReturnType.Name == "Nullable";
			}
		}
		
		public bool HasRange {
			get {
				return (field.ReturnType.IsConstructedReturnType &&
				        IsTypeWithRange(field.ReturnType.CastToConstructedReturnType().TypeArguments.First())
				       ) || IsTypeWithRange(field.ReturnType);
			}
		}
		
		public int Index { get; set; }
		
		bool isSelected;
		
		public bool IsSelected {
			get { return isSelected; }
			set {
				isSelected = value;
				OnPropertyChanged("IsSelected");
			}
		}
		
		bool addCheckForNull;
		
		public bool AddCheckForNull {
			get { return addCheckForNull; }
			set {
				addCheckForNull = value;
				if (value) IsSelected = true;
			}
		}
		
		bool addRangeCheck;
		
		public bool AddRangeCheck {
			get { return addRangeCheck; }
			set {
				addRangeCheck = value;
				if (value) IsSelected = true; 
			}
		}
		
		public string Name {
			get { return field.Name; }
		}
		
		public IReturnType Type {
			get { return field.ReturnType; }
		}
		
		bool IsTypeWithRange(IReturnType type)
		{
			return type.Name == "Int32" ||
				type.Name == "Int16" ||
				type.Name == "Int64" ||
				type.Name == "Single" ||
				type.Name == "Double" ||
				type.Name == "UInt16" ||
				type.Name == "UInt32" ||
				type.Name == "UInt64";
		}
		
		public CtorParamWrapper(IField field)
		{
			if (field == null || field.ReturnType == null)
				throw new ArgumentNullException("field");
			
			this.field = field;
		}
		
		public event PropertyChangedEventHandler PropertyChanged;
		
		protected virtual void OnPropertyChanged(string property)
		{
			if (PropertyChanged != null) {
				PropertyChanged(this, new PropertyChangedEventArgs(property));
			}
		}
	}
}
