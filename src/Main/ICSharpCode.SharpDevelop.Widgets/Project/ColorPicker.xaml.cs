// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel;

namespace ICSharpCode.SharpDevelop.Widgets
{
	public partial class ColorPicker
	{
		public ColorPicker()
		{
			InitializeComponent();
		}

		public static readonly DependencyProperty ColorProperty =
			DependencyProperty.Register("Color", typeof(Color), typeof(ColorPicker),
			                            new FrameworkPropertyMetadata(new Color(), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

		public Color Color {
			get { return (Color)GetValue(ColorProperty); }
			set { SetValue(ColorProperty, value); }
		}

		public static readonly DependencyProperty HProperty =
			DependencyProperty.Register("H", typeof(int), typeof(ColorPicker));

		public int H {
			get { return (int)GetValue(HProperty); }
			set { SetValue(HProperty, value); }
		}

		public static readonly DependencyProperty SProperty =
			DependencyProperty.Register("S", typeof(int), typeof(ColorPicker));

		public int S {
			get { return (int)GetValue(SProperty); }
			set { SetValue(SProperty, value); }
		}

		public static readonly DependencyProperty VProperty =
			DependencyProperty.Register("V", typeof(int), typeof(ColorPicker));

		public int V {
			get { return (int)GetValue(VProperty); }
			set { SetValue(VProperty, value); }
		}

		public static readonly DependencyProperty RProperty =
			DependencyProperty.Register("R", typeof(byte), typeof(ColorPicker));

		public byte R {
			get { return (byte)GetValue(RProperty); }
			set { SetValue(RProperty, value); }
		}

		public static readonly DependencyProperty GProperty =
			DependencyProperty.Register("G", typeof(byte), typeof(ColorPicker));

		public byte G {
			get { return (byte)GetValue(GProperty); }
			set { SetValue(GProperty, value); }
		}

		public static readonly DependencyProperty BProperty =
			DependencyProperty.Register("B", typeof(byte), typeof(ColorPicker));

		public byte B {
			get { return (byte)GetValue(BProperty); }
			set { SetValue(BProperty, value); }
		}

		public static readonly DependencyProperty AProperty =
			DependencyProperty.Register("A", typeof(byte), typeof(ColorPicker));

		public byte A {
			get { return (byte)GetValue(AProperty); }
			set { SetValue(AProperty, value); }
		}

		public static readonly DependencyProperty HexProperty =
			DependencyProperty.Register("Hex", typeof(string), typeof(ColorPicker));

		public string Hex {
			get { return (string)GetValue(HexProperty); }
			set { SetValue(HexProperty, value); }
		}

		public static readonly DependencyProperty HueColorProperty =
			DependencyProperty.Register("HueColor", typeof(Color), typeof(ColorPicker));

		public Color HueColor {
			get { return (Color)GetValue(HueColorProperty); }
			set { SetValue(HueColorProperty, value); }
		}

		bool updating;

		protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
		{
			base.OnPropertyChanged(e);

			if (updating) return;
			updating = true;
			try {
				if (e.Property == ColorProperty) {
					UpdateSource(ColorSource.Hsv);
					UpdateRest(ColorSource.Hsv);
				} else if (e.Property == HProperty || e.Property == SProperty || e.Property == VProperty) {
					var c = ColorHelper.ColorFromHsv(H, S / 100.0, V / 100.0);
					c.A = A;
					Color = c;
					UpdateRest(ColorSource.Hsv);
				} else if (e.Property == RProperty || e.Property == GProperty || e.Property == BProperty || e.Property == AProperty) {
					Color = Color.FromArgb(A, R, G, B);
					UpdateRest(ColorSource.Rgba);
				} else if (e.Property == HexProperty) {
					Color = ColorHelper.ColorFromString(Hex);
					UpdateRest(ColorSource.Hex);
				}
			} finally {
				updating = false;
			}
		}

		void UpdateRest(ColorSource source)
		{
			HueColor = ColorHelper.ColorFromHsv(H, 1, 1);
			UpdateSource((ColorSource)(((int)source + 1) % 3));
			UpdateSource((ColorSource)(((int)source + 2) % 3));
		}

		void UpdateSource(ColorSource source)
		{
			if (source == ColorSource.Hsv) {
				double h, s, v;
				ColorHelper.HsvFromColor(Color, out h, out s, out v);

				H = (int)h;
				S = (int)(s * 100);
				V = (int)(v * 100);
			}
			else if (source == ColorSource.Rgba) {
				R = Color.R;
				G = Color.G;
				B = Color.B;
				A = Color.A;
			}
			else {
				Hex = ColorHelper.StringFromColor(Color);
			}
		}

		enum ColorSource
		{
			Hsv, Rgba, Hex
		}
	}

	class HexTextBox : TextBox
	{
		protected override void OnKeyDown(KeyEventArgs e)
		{
			if (e.Key == Key.Enter) {
				var b = BindingOperations.GetBindingExpressionBase(this, TextProperty);
				if (b != null) {
					b.UpdateTarget();
				}
				SelectAll();
			}
		}
	}
}
