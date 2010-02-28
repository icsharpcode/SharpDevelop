// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Siegfried Pammer" email="siegfriedpammer@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

using ICSharpCode.AvalonEdit.Snippets;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Dom.Refactoring;
using ICSharpCode.SharpDevelop.Editor;

namespace SharpRefactoring.Gui
{
	/// <summary>
	/// Interaction logic for OverrideToStringMethodDialog.xaml
	/// </summary>
	public partial class OverrideToStringMethodDialog : AbstractInlineRefactorDialog
	{
		List<Wrapper<IField>> fields;
		
		public OverrideToStringMethodDialog(InsertionContext context, ITextEditor editor, ITextAnchor anchor, IList<IField> fields)
			: base(context, editor, anchor)
		{
			InitializeComponent();
			
			this.fields = fields.Select(f => new Wrapper<IField>() { Entity = f }).ToList();
			this.listBox.ItemsSource = this.fields.Select(i => i.Create(null));
		}
		
		protected override string GenerateCode(CodeGenerator generator, IClass currentClass)
		{
			var fields = this.fields
				.Where(f => f.IsChecked)
				.Select(f2 => f2.Entity.Name)
				.ToArray();
			
			if (fields.Any()) {
				StringBuilder formatString = new StringBuilder("[" + currentClass.Name + " ");
				
				for (int i = 0; i < fields.Length; i++) {
					if (i != 0)
						formatString.Append(", ");
					formatString.AppendFormat("{0}={{{1}}}", generator.GetPropertyName(fields[i]), i);
				}
				
				formatString.Append("]");
				
				return "return string.Format(\"" + formatString.ToString() + "\", " + string.Join(", ", fields) + ");";
			}
			
			return "return string.Format(\"[" + currentClass.Name + "]\");";
		}
	}
}
