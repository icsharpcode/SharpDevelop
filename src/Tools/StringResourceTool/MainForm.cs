/*
 * Created by SharpDevelop.
 * User: Daniel Grunwald
 * Date: 08.10.2005
 * Time: 19:47
 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Resources;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace StringResourceTool
{
	public class MainForm : System.Windows.Forms.Form
	{
		public MainForm()
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			try {
				using (StreamReader r = new StreamReader("password.txt")) {
					userNameTextBox.Text = r.ReadLine();
					passwordTextBox.Text = r.ReadLine();
				}
				savePasswordCheckBox.Checked = true;
			} catch {}
		}
		
		[STAThread]
		public static void Main(string[] args)
		{
			if (args.Length == 3) {
				try {
					string userName, password;
					using (StreamReader r = new StreamReader("password.txt")) {
						userName = r.ReadLine();
						password = r.ReadLine();
					}
					TranslationServer server = new TranslationServer(new TextBox());
					if (!server.Login(userName, password)) {
						MessageBox.Show("Login failed");
						return;
					}
					server.AddResourceString(args[0], args[1], args[2]);
					MessageBox.Show("Resource string added to database on server");
					return;
				} catch (Exception ex) {
					MessageBox.Show(ex.ToString());
				}
			}
			Application.EnableVisualStyles();
			Application.Run(new MainForm());
		}
		
		#region Windows Forms Designer generated code
		/// <summary>
		/// This method is required for Windows Forms designer support.
		/// Do not change the method contents inside the source code editor. The Forms designer might
		/// not be able to load this method if it was changed manually.
		/// </summary>
		private void InitializeComponent()
		{
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(621, 399);
			groupBox1 = new System.Windows.Forms.GroupBox();
			// 
			// groupBox1
			// 
			deleteStringsButton = new System.Windows.Forms.Button();
			// 
			// deleteStringsButton
			// 
			deleteStringsButton.Enabled = false;
			deleteStringsButton.Location = new System.Drawing.Point(411, 20);
			deleteStringsButton.Name = "deleteStringsButton";
			deleteStringsButton.Size = new System.Drawing.Size(144, 23);
			deleteStringsButton.TabIndex = 7;
			deleteStringsButton.Text = "Delete resource strings";
			deleteStringsButton.Click += new System.EventHandler(this.DeleteStringsButtonClick);
			button4 = new System.Windows.Forms.Button();
			// 
			// button4
			// 
			button4.Enabled = false;
			button4.Location = new System.Drawing.Point(292, 20);
			button4.Name = "button4";
			button4.Size = new System.Drawing.Size(113, 23);
			button4.TabIndex = 6;
			button4.Text = "Download database";
			button4.Click += new System.EventHandler(this.DownloadButtonClick);
			savePasswordCheckBox = new System.Windows.Forms.CheckBox();
			// 
			// savePasswordCheckBox
			// 
			savePasswordCheckBox.Location = new System.Drawing.Point(182, 44);
			savePasswordCheckBox.Name = "savePasswordCheckBox";
			savePasswordCheckBox.Size = new System.Drawing.Size(104, 24);
			savePasswordCheckBox.TabIndex = 5;
			savePasswordCheckBox.Text = "Save password";
			button3 = new System.Windows.Forms.Button();
			// 
			// button3
			// 
			button3.Location = new System.Drawing.Point(182, 20);
			button3.Name = "button3";
			button3.Size = new System.Drawing.Size(75, 23);
			button3.TabIndex = 4;
			button3.Text = "Login";
			button3.Click += new System.EventHandler(this.Button3Click);
			passwordTextBox = new System.Windows.Forms.TextBox();
			// 
			// passwordTextBox
			// 
			passwordTextBox.Location = new System.Drawing.Point(76, 42);
			passwordTextBox.Name = "passwordTextBox";
			passwordTextBox.PasswordChar = '●';
			passwordTextBox.Size = new System.Drawing.Size(100, 21);
			passwordTextBox.TabIndex = 3;
			passwordTextBox.UseSystemPasswordChar = true;
			userNameTextBox = new System.Windows.Forms.TextBox();
			// 
			// userNameTextBox
			// 
			userNameTextBox.Location = new System.Drawing.Point(76, 19);
			userNameTextBox.Name = "userNameTextBox";
			userNameTextBox.Size = new System.Drawing.Size(100, 21);
			userNameTextBox.TabIndex = 1;
			label2 = new System.Windows.Forms.Label();
			// 
			// label2
			// 
			label2.Location = new System.Drawing.Point(6, 40);
			label2.Name = "label2";
			label2.Size = new System.Drawing.Size(64, 23);
			label2.TabIndex = 2;
			label2.Text = "Password:";
			label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			label1 = new System.Windows.Forms.Label();
			// 
			// label1
			// 
			label1.Location = new System.Drawing.Point(6, 17);
			label1.Name = "label1";
			label1.Size = new System.Drawing.Size(64, 23);
			label1.TabIndex = 0;
			label1.Text = "Username:";
			label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			groupBox1.Controls.Add(deleteStringsButton);
			groupBox1.Controls.Add(button4);
			groupBox1.Controls.Add(savePasswordCheckBox);
			groupBox1.Controls.Add(button3);
			groupBox1.Controls.Add(passwordTextBox);
			groupBox1.Controls.Add(userNameTextBox);
			groupBox1.Controls.Add(label2);
			groupBox1.Controls.Add(label1);
			groupBox1.Location = new System.Drawing.Point(12, 12);
			groupBox1.Name = "groupBox1";
			groupBox1.Size = new System.Drawing.Size(597, 74);
			groupBox1.TabIndex = 0;
			groupBox1.TabStop = false;
			groupBox1.Text = "Translation server";
			groupBox1.SuspendLayout();
			groupBox1.ResumeLayout(false);
			groupBox1.PerformLayout();
			button2 = new System.Windows.Forms.Button();
			// 
			// button2
			// 
			button2.Location = new System.Drawing.Point(142, 92);
			button2.Name = "button2";
			button2.Size = new System.Drawing.Size(124, 23);
			button2.TabIndex = 2;
			button2.Text = "Find missing strings";
			button2.Click += new System.EventHandler(this.Button2Click);
			button1 = new System.Windows.Forms.Button();
			// 
			// button1
			// 
			button1.Location = new System.Drawing.Point(12, 91);
			button1.Name = "button1";
			button1.Size = new System.Drawing.Size(124, 23);
			button1.TabIndex = 1;
			button1.Text = "Find unused strings";
			button1.Click += new System.EventHandler(this.Button1Click);
			outputTextBox = new System.Windows.Forms.TextBox();
			// 
			// outputTextBox
			// 
			outputTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
			                                                              | System.Windows.Forms.AnchorStyles.Left)
			                                                             | System.Windows.Forms.AnchorStyles.Right)));
			outputTextBox.Location = new System.Drawing.Point(12, 120);
			outputTextBox.Multiline = true;
			outputTextBox.Name = "outputTextBox";
			outputTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			outputTextBox.Size = new System.Drawing.Size(597, 267);
			outputTextBox.TabIndex = 3;
			this.Controls.Add(groupBox1);
			this.Controls.Add(button2);
			this.Controls.Add(button1);
			this.Controls.Add(outputTextBox);
			this.Name = "MainForm";
			this.Text = "StringResourceTool";
			this.SuspendLayout();
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		private System.Windows.Forms.Button deleteStringsButton;
		private System.Windows.Forms.Button button4;
		private System.Windows.Forms.CheckBox savePasswordCheckBox;
		private System.Windows.Forms.Button button3;
		private System.Windows.Forms.TextBox passwordTextBox;
		private System.Windows.Forms.TextBox userNameTextBox;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Button button2;
		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.TextBox outputTextBox;
		#endregion
		
		void Button1Click(object sender, EventArgs e)
		{
			button1.Enabled = false;
			Display(FindMissing(FindResourceStrings(), FindUsedStrings()));
			button1.Enabled = true;
		}
		
		void Button2Click(object sender, EventArgs e)
		{
			button2.Enabled = false;
			Display(FindMissing(FindUsedStrings(), FindResourceStrings()));
			button2.Enabled = true;
		}
		
		void Display(List<string> list)
		{
			StringBuilder b = new StringBuilder();
			foreach (string entry in list) {
				b.AppendLine(entry);
			}
			outputTextBox.Text = b.ToString();
		}
		
		/// <summary>Gets entries in t1 that are missing from t2.</summary>
		List<string> FindMissing(HashSet<string> t1, HashSet<string> t2)
		{
			return t1.Except(t2).OrderBy(s=>s).ToList();
		}
		
		HashSet<string> FindUsedStrings()
		{
			HashSet<string> t = new HashSet<string>();
			FindUsedStrings(t, @"..\..\..\..\..");
			return t;
		}
		void FindUsedStrings(HashSet<string> t, string path)
		{
			foreach (string subPath in Directory.GetDirectories(path)) {
				if (!(subPath.EndsWith(".svn") || subPath.EndsWith("\\obj"))) {
					FindUsedStrings(t, subPath);
				}
			}
			foreach (string fileName in Directory.EnumerateFiles(path)) {
				switch (Path.GetExtension(fileName).ToLowerInvariant()) {
					case ".cs":
					case ".boo":
						FindUsedStrings(fileName, t, resourceService);
						break;
					case ".xaml":
						FindUsedStrings(fileName, t, xamlLocalize, xamlLocalizeElementSyntax);
						break;
					case ".resx":
					case ".resources":
					case ".dll":
					case ".exe":
					case ".pdb":
						break;
					default:
						FindUsedStrings(fileName, t);
						break;
				}
			}
		}
		
		const string resourceNameRegex = @"[\.\w\d]+";
		
		readonly static Regex pattern         = new Regex(@"\$\{res:(" + resourceNameRegex + @")\}", RegexOptions.Compiled);
		readonly static Regex resourceService = new Regex(@"ResourceService.GetString\(\""(" + resourceNameRegex + @")\""\)", RegexOptions.Compiled);
		readonly static Regex xamlLocalize = new Regex(@"\{\w+:Localize\s+(" + resourceNameRegex + @")\}", RegexOptions.Compiled);
		readonly static Regex xamlLocalizeElementSyntax = new Regex(@"\<\w+:LocalizeExtension\s+Key\s*=\s*[""'](" + resourceNameRegex + @")[""']", RegexOptions.Compiled);
		
		void FindUsedStrings(string fileName, HashSet<string> t, params Regex[] extraPatterns)
		{
			StreamReader sr = File.OpenText(fileName);
			string content = sr.ReadToEnd();
			sr.Close();
			foreach (Match m in pattern.Matches(content)) {
				//Debug.WriteLine(fileName);
				t.Add(m.Groups[1].Captures[0].Value);
			}
			foreach (var extraPattern in extraPatterns) {
				foreach (Match m in extraPattern.Matches(content)) {
					//Debug.WriteLine(fileName);
					t.Add(m.Groups[1].Captures[0].Value);
				}
			}
		}
		const string srcDir = @"..\..\..\..\";
		HashSet<string> FindResourceStrings()
		{
			var rs = new ResXResourceReader(srcDir + @"..\data\resources\StringResources.resx");
			HashSet<string> t = new HashSet<string>();
			foreach (DictionaryEntry e in rs) {
				t.Add(e.Key.ToString());
			}
			rs.Close();
			return t;
		}
		
		TranslationServer server;
		
		void Button3Click(object sender, EventArgs e)
		{
			server = new TranslationServer(outputTextBox);
			if (savePasswordCheckBox.Checked) {
				using (StreamWriter w = new StreamWriter("password.txt")) {
					w.WriteLine(userNameTextBox.Text);
					w.WriteLine(passwordTextBox.Text);
				}
			} else {
				File.Delete("password.txt");
			}
			if (server.Login(userNameTextBox.Text, passwordTextBox.Text)) {
				button4.Enabled = true;
				deleteStringsButton.Enabled = true;
			}
		}
		
		void DownloadButtonClick(object sender, EventArgs e)
		{
			EventHandler onDownloadFinished = delegate {
				outputTextBox.Text += "\r\nLoading database...";
				Application.DoEvents();
				
				ResourceDatabase db = ResourceDatabase.Load("LocalizeDb_DL_Corsavy.mdb");
				outputTextBox.Text += "\r\nCreating resource files...";
				Application.DoEvents();
				BuildResourceFiles.Build(db, Path.Combine(srcDir, "../data/resources"),
				                         text => { outputTextBox.Text += "\r\n" + text; Application.DoEvents();});
				
				outputTextBox.Text += "\r\nBuilding SharpDevelop...";
				RunBatch(Path.Combine(srcDir, ".."), "debugbuild.bat", null);
			};
			server.DownloadDatabase("LocalizeDb_DL_Corsavy.mdb", onDownloadFinished);
			//onDownloadFinished(null, null);
		}
		
		void RunBatch(string dir, string batchFile, MethodInvoker exitCallback)
		{
			BeginInvoke(new MethodInvoker(delegate {
			                              	outputTextBox.Text += "\r\nRun " + dir + batchFile + "...";
			                              }));
			ProcessStartInfo psi = new ProcessStartInfo("cmd", "/c " + batchFile);
			psi.WorkingDirectory = dir;
			Process p = Process.Start(psi);
			if (exitCallback != null) {
				p.EnableRaisingEvents = true;
				p.Exited += delegate {
					p.Dispose();
					exitCallback();
				};
			}
		}
		
		void DeleteStringsButtonClick(object sender, EventArgs e)
		{
			List<string> list = new List<string>();
			string preview = "";
			foreach (string line in outputTextBox.Lines) {
				if (line.Length > 0) {
					list.Add(line);
					if (preview.Length == 0) {
						preview = line;
					} else if (preview.Length < 100) {
						preview += ", " + line;
					}
				}
			}
			if (MessageBox.Show("Do you really want to delete the " + list.Count + " resource strings (" + preview + ")"
			                    , "Delete resources", MessageBoxButtons.YesNo) == DialogResult.Yes) {
				server.DeleteResourceStrings(list.ToArray());
			}
		}
	}
}
