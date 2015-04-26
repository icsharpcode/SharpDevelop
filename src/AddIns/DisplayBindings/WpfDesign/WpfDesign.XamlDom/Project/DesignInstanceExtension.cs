using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Markup;

namespace ICSharpCode.WpfDesign.XamlDom
{
	/// <summary>
	/// A class which implements the DesignInstanceExtension normally defined in the Blend Namespace.
	/// </summary>
	public class DesignInstanceExtension : MarkupExtension
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="DesignInstanceExtension"/> class.
		/// </summary>
		/// <param name="type">The type to create.</param>
		public DesignInstanceExtension(Type type)
		{
			this.Type = type;
		}

		/// <summary>
		/// Gets or sets the type to create.
		/// Use <see cref="IsDesignTimeCreatable"/> to specify whether an instance of your type or a designer-generated substitute type is created.
		/// </summary>
		public Type Type { get; set; }

		/// <summary>
		/// Gets or sets if the design instance is created from your type (true) or of a designer-generated substitute type (false). 
		/// </summary>
		public bool IsDesignTimeCreatable { get; set; }

		/// <summary>
		/// Gets or sets if the design instance is a list of the specified type. 
		/// </summary>
		public bool CreateList { get; set; }

		/// <inheritdoc/>
		public override object ProvideValue(IServiceProvider serviceProvider)
		{
			return null;
		}
	}
}
