using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace SampleApp
{
    public partial class ColumnHandling : UserControl
    {
        public ColumnHandling()
        {
            InitializeComponent();
        }

		private void treeViewAdv1_NodeMouseDoubleClick(object sender, Aga.Controls.Tree.TreeNodeAdvMouseEventArgs e)
		{
			Console.WriteLine("DblClick {0}", e.Node);
		}
    }
}
