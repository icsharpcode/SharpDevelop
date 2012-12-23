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
using System.Net;
using System.Resources;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
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
			
			Dictionary<string, string> languages = new Dictionary<string, string>() {
				{ "cz", "Czech" },
				{ "nl", "Dutch" },
				{ "fr", "French" },
				{ "de", "German" },
				{ "it", "Italian" },
				{ "pt", "Portuguese" },
				{ "es", "Spanish" },
				{ "se", "Swedish" },
				{ "goisern", "Goiserisch" },
				{ "ru", "Russian" },
				{ "br", "Brazilian Portuguese" },
				{ "pl", "Polish" },
				{ "jp", "Japanese" },
				{ "th", "Thai" },
				{ "kr", "Korean" },
				{ "dk", "Danish" },
				{ "hu", "Hungarian" },
				{ "ro", "Romanian" },
				{ "cn-gb", "Chinese Simplified" },
				{ "cn-big", "Chinese Traditional" },
				{ "ca", "Catalan" },
				{ "bg", "Bulgarian" },
				{ "urdu", "Urdu" },
				{ "be", "Belarusian" },
				{ "el", "Greek" },
				{ "tr", "Turkish" },
				{ "sk", "Slovak" },
				{ "lt", "Lithuanian" },
				{ "he", "Hebrew" },
				{ "sl", "Slovenian" },
				{ "es-mx", "Spanish (Mexico)" },
				{ "af", "Afrikaans" },
				{ "vi", "Vietnamese" },
				{ "ar", "Arabic" },
				{ "no", "Norwegian" },
				{ "fa", "Persian" },
				{ "sr", "Serbian" },
				{ "fi", "Finnish" },
				{ "hr", "Croatian" },
				{ "id", "Indonesian" }
			};
			
			// Clear the combobox
			comboBox1.DataSource = null;
			comboBox1.Items.Clear();

			// Bind the combobox
			comboBox1.DataSource = new BindingSource(languages, null);
			comboBox1.DisplayMember = "Value";
			comboBox1.ValueMember = "Key";
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
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.comboBox1 = new System.Windows.Forms.ComboBox();
			this.label3 = new System.Windows.Forms.Label();
			this.deleteStringsButton = new System.Windows.Forms.Button();
			this.button4 = new System.Windows.Forms.Button();
			this.savePasswordCheckBox = new System.Windows.Forms.CheckBox();
			this.button3 = new System.Windows.Forms.Button();
			this.passwordTextBox = new System.Windows.Forms.TextBox();
			this.userNameTextBox = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.button2 = new System.Windows.Forms.Button();
			this.button1 = new System.Windows.Forms.Button();
			this.outputTextBox = new System.Windows.Forms.TextBox();
			this.button5 = new System.Windows.Forms.Button();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.comboBox1);
			this.groupBox1.Controls.Add(this.label3);
			this.groupBox1.Controls.Add(this.deleteStringsButton);
			this.groupBox1.Controls.Add(this.button4);
			this.groupBox1.Controls.Add(this.savePasswordCheckBox);
			this.groupBox1.Controls.Add(this.button3);
			this.groupBox1.Controls.Add(this.passwordTextBox);
			this.groupBox1.Controls.Add(this.userNameTextBox);
			this.groupBox1.Controls.Add(this.label2);
			this.groupBox1.Controls.Add(this.label1);
			this.groupBox1.Location = new System.Drawing.Point(12, 12);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(597, 100);
			this.groupBox1.TabIndex = 0;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Translation server";
			// 
			// comboBox1
			// 
			this.comboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBox1.FormattingEnabled = true;
			this.comboBox1.Items.AddRange(new object[] {
									"cz\">Czech</option>",
									"<option value=\"nl\">Dutch</option>",
									"<option value=\"fr\">French</option>",
									"<option selected=\"\" value=\"de\">German</option>",
									"<option value=\"it\">Italian</option>",
									"<option value=\"pt\">Portuguese</option>",
									"<option value=\"es\">Spanish</option>",
									"<option value=\"se\">Swedish</option>",
									"<option value=\"goisern\">Goiserisch</option>",
									"<option value=\"ru\">Russian</option>",
									"<option value=\"br\">Brazilian Portuguese</option>",
									"<option value=\"pl\">Polish</option>",
									"<option value=\"jp\">Japanese</option>",
									"<option value=\"th\">Thai</option>",
									"<option value=\"kr\">Korean</option>",
									"<option value=\"dk\">Danish</option>",
									"<option value=\"hu\">Hungarian</option>",
									"<option value=\"ro\">Romanian</option>",
									"<option value=\"cn-gb\">Chinese Simplified</option>",
									"<option value=\"cn-big\">Chinese Traditional</option>",
									"<option value=\"ca\">Catalan</option>",
									"<option value=\"bg\">Bulgarian</option>",
									"<option value=\"urdu\">Urdu</option>",
									"<option value=\"be\">Belarusian</option>",
									"<option value=\"el\">Greek</option>",
									"<option value=\"tr\">Turkish</option>",
									"<option value=\"sk\">Slovak</option>",
									"<option value=\"lt\">Lithuanian</option>",
									"<option value=\"he\">Hebrew</option>",
									"<option value=\"sl\">Slovenian</option>",
									"<option value=\"es-mx\">Spanish (Mexico)</option>",
									"<option value=\"af\">Afrikaans</option>",
									"<option value=\"vi\">Vietnamese</option>",
									"<option value=\"ar\">Arabic</option>",
									"<option value=\"no\">Norwegian</option>",
									"<option value=\"fa\">Persian</option>",
									"<option value=\"sr\">Serbian</option>",
									"<option value=\"fi\">Finnish</option>",
									"<option value=\"hr\">Croatian</option>",
									"<option value=\"id\">Indonesian </option>"});
			this.comboBox1.Location = new System.Drawing.Point(76, 65);
			this.comboBox1.Name = "comboBox1";
			this.comboBox1.Size = new System.Drawing.Size(121, 21);
			this.comboBox1.TabIndex = 9;
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(12, 68);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(58, 13);
			this.label3.TabIndex = 8;
			this.label3.Text = "Language:";
			// 
			// deleteStringsButton
			// 
			this.deleteStringsButton.Enabled = false;
			this.deleteStringsButton.Location = new System.Drawing.Point(411, 20);
			this.deleteStringsButton.Name = "deleteStringsButton";
			this.deleteStringsButton.Size = new System.Drawing.Size(144, 23);
			this.deleteStringsButton.TabIndex = 7;
			this.deleteStringsButton.Text = "Delete resource strings";
			this.deleteStringsButton.Click += new System.EventHandler(this.DeleteStringsButtonClick);
			// 
			// button4
			// 
			this.button4.Enabled = false;
			this.button4.Location = new System.Drawing.Point(292, 20);
			this.button4.Name = "button4";
			this.button4.Size = new System.Drawing.Size(113, 23);
			this.button4.TabIndex = 6;
			this.button4.Text = "Download database";
			this.button4.Click += new System.EventHandler(this.DownloadButtonClick);
			// 
			// savePasswordCheckBox
			// 
			this.savePasswordCheckBox.Location = new System.Drawing.Point(182, 44);
			this.savePasswordCheckBox.Name = "savePasswordCheckBox";
			this.savePasswordCheckBox.Size = new System.Drawing.Size(104, 24);
			this.savePasswordCheckBox.TabIndex = 5;
			this.savePasswordCheckBox.Text = "Save password";
			// 
			// button3
			// 
			this.button3.Location = new System.Drawing.Point(182, 20);
			this.button3.Name = "button3";
			this.button3.Size = new System.Drawing.Size(75, 23);
			this.button3.TabIndex = 4;
			this.button3.Text = "Login";
			this.button3.Click += new System.EventHandler(this.Button3Click);
			// 
			// passwordTextBox
			// 
			this.passwordTextBox.Location = new System.Drawing.Point(76, 42);
			this.passwordTextBox.Name = "passwordTextBox";
			this.passwordTextBox.PasswordChar = '●';
			this.passwordTextBox.Size = new System.Drawing.Size(100, 20);
			this.passwordTextBox.TabIndex = 3;
			this.passwordTextBox.UseSystemPasswordChar = true;
			// 
			// userNameTextBox
			// 
			this.userNameTextBox.Location = new System.Drawing.Point(76, 19);
			this.userNameTextBox.Name = "userNameTextBox";
			this.userNameTextBox.Size = new System.Drawing.Size(100, 20);
			this.userNameTextBox.TabIndex = 1;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(6, 40);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(64, 23);
			this.label2.TabIndex = 2;
			this.label2.Text = "Password:";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(6, 17);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(64, 23);
			this.label1.TabIndex = 0;
			this.label1.Text = "Username:";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// button2
			// 
			this.button2.Location = new System.Drawing.Point(141, 118);
			this.button2.Name = "button2";
			this.button2.Size = new System.Drawing.Size(124, 23);
			this.button2.TabIndex = 2;
			this.button2.Text = "Find missing strings";
			this.button2.Click += new System.EventHandler(this.Button2Click);
			// 
			// button1
			// 
			this.button1.Location = new System.Drawing.Point(11, 118);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(124, 23);
			this.button1.TabIndex = 1;
			this.button1.Text = "Find unused strings";
			this.button1.Click += new System.EventHandler(this.Button1Click);
			// 
			// outputTextBox
			// 
			this.outputTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
									| System.Windows.Forms.AnchorStyles.Left) 
									| System.Windows.Forms.AnchorStyles.Right)));
			this.outputTextBox.Location = new System.Drawing.Point(12, 147);
			this.outputTextBox.Multiline = true;
			this.outputTextBox.Name = "outputTextBox";
			this.outputTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			this.outputTextBox.Size = new System.Drawing.Size(597, 309);
			this.outputTextBox.TabIndex = 3;
			// 
			// button5
			// 
			this.button5.Location = new System.Drawing.Point(271, 118);
			this.button5.Name = "button5";
			this.button5.Size = new System.Drawing.Size(280, 23);
			this.button5.TabIndex = 4;
			this.button5.Text = "Upload resources (check language! dangerous!)";
			this.button5.UseVisualStyleBackColor = true;
			this.button5.Click += new System.EventHandler(this.Button5Click);
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(621, 468);
			this.Controls.Add(this.button5);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.button2);
			this.Controls.Add(this.button1);
			this.Controls.Add(this.outputTextBox);
			this.Name = "MainForm";
			this.Text = "StringResourceTool";
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.ComboBox comboBox1;
		private System.Windows.Forms.Button button5;
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
		
		void Button5Click(object sender, EventArgs e)
		{
			server.SetLanguage(comboBox1.SelectedValue.ToString());
			using (OpenFileDialog dialog = new OpenFileDialog()) {
				dialog.Filter = "String resources (.resources)|*.resources";
				if (dialog.ShowDialog() != DialogResult.OK) return;
				ImportResourcesFile(dialog.FileName);
			}
		}

		void ImportResourcesFile(string fileName)
		{
			using (ResourceReader r = new ResourceReader(fileName)) {
				IDictionaryEnumerator enumerator = r.GetEnumerator();
				while (enumerator.MoveNext()) {
					try {
						server.UpdateTranslation(enumerator.Key.ToString(), enumerator.Value.ToString());
					} catch (WebException ex) {
						outputTextBox.AppendText(Environment.NewLine + "could not update: " + enumerator.Key + ": " + ex.Message);
					}
				}
			}
		}
	}
}
