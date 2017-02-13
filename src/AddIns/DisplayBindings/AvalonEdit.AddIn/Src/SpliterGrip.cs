/*
 * Created by SharpDevelop.
 * User: Abdelkarim
 * Date: 9/22/2014
 * Time: 8:07 AM
 * 
 */
using System;
using System.Collections;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Threading;
namespace ICSharpCode.AvalonEdit.AddIn
{
	public class SpliterGrip : Thumb
	{
		#region "Constructors"
		/// <summary>
		/// initializes static members of the <see cref="SpliterGrip"/> class.
		/// </summary>
		static SpliterGrip()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(SpliterGrip), new FrameworkPropertyMetadata(typeof(SpliterGrip)));
		}

		#endregion
		#region "Properties"
		#region Orientation
		/// <summary>
		/// Orientation Read-Only Dependency Property
		/// </summary>
		private static readonly DependencyPropertyKey OrientationPropertyKey = DependencyProperty.RegisterReadOnly("Orientation", typeof(Orientation), typeof(SpliterGrip), new FrameworkPropertyMetadata(Orientation.Horizontal));

		public static readonly DependencyProperty OrientationProperty = OrientationPropertyKey.DependencyProperty;

		/// <summary>
		/// Gets the Orientation property. This dependency property 
		/// indicates ....
		/// </summary>
		public Orientation Orientation {
			get {
				return (Orientation)GetValue(OrientationProperty);
			}
			internal set {
				SetValue(OrientationPropertyKey, value);
			}
		}
		#endregion
		#endregion
	}
}


