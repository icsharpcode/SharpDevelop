using System;
using System.Collections.Generic;
using System.Text;
using Aga.Controls.Tree;

namespace SampleApp
{
	public class MyNode : Node
	{
		public override string Text
		{
			get
			{
				return base.Text;
			}
			set
			{
				if (string.IsNullOrEmpty(value))
					throw new ArgumentNullException();

				base.Text = value;
			}
		}

		private bool _checked;
		public bool Checked
		{
			get { return _checked; }
			set { _checked = value; }
		}

		public MyNode(string text)
			: base(text)
		{
		}
	}
}
