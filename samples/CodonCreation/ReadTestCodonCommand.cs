using ICSharpCode.Core;
using System;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace CodonCreation
{
	public class ReadTestCodonCommand : AbstractMenuCommand
	{
		public override void Run()
		{
			TestCodon[] testCodons = (TestCodon[])(AddInTree.GetTreeNode("/Samples/CodonCreation").BuildChildItems(this)).ToArray(typeof(TestCodon));
			
			StringBuilder message = new StringBuilder();
			foreach (TestCodon codon in testCodons) {
				message.AppendFormat("{0} ", codon.Text);
			}
			MessageBox.Show(message.ToString(), "TestCodon");
		}
	}
}

