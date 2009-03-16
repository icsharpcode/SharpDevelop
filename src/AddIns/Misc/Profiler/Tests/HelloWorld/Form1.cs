using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Runtime.CompilerServices;

namespace HelloWorld
{
	public partial class Form1 : Form
	{
		public Form1()
		{
			InitializeComponent();
		}

		private void button1_Click(object sender, EventArgs e)
		{
			MessageBox.Show("Hello world!");
		}
		
		void Button2Click(object sender, EventArgs e)
		{
			Thread t = new Thread(new ThreadStart(MyThread));
			t.Name = "Testthread";
			t.Start();
		}
		
		void MyThread()
		{
			MethodA();
			Thread.Sleep(750);
			MethodB();
		}
		
		[MethodImpl(MethodImplOptions.NoInlining)]
		void MethodA() {}
		
		[MethodImpl(MethodImplOptions.NoInlining)]
		void MethodB() {}
	}
}