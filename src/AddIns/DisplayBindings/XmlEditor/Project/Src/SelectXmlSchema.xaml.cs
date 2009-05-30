using ICSharpCode.Core;
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
using System.Windows.Shapes;

namespace ICSharpCode.XmlEditor
{
    /// <summary>
    /// Interaction logic for SelectXmlSchema.xaml
    /// </summary>
    public partial class SelectXmlSchema : Window
    {
    	string noSelection;
		    	
        public SelectXmlSchema(string[] namespaces)
        {
            InitializeComponent();
            PopulateListBox(namespaces);
            
			noSelection = StringParser.Parse("${res:Dialog.Options.IDEOptions.TextEditor.Behaviour.IndentStyle.None}");
        }
        
		void BtnOKClick(object sender, RoutedEventArgs e)
		{
			this.DialogResult = true;
		}
		
		void BtnCancelClick(object sender, RoutedEventArgs e)
		{
			this.DialogResult = false;
		}
		
		/// <summary>
		/// Gets or sets the selected schema namesace.
		/// </summary>
		public string SelectedNamespaceUri {
			get {
				string namespaceUri = schemaListBox.SelectedItem.ToString();
				if (namespaceUri == noSelection) {
					namespaceUri = string.Empty;
				}
				return namespaceUri;
			}
			set {
				int index = 0;
				if (value.Length > 0) {
					index = schemaListBox.Items.IndexOf(value);
				} 
				
				// Select the option representing "no schema" if 
				// the value does not exist in the list box.
				if (index == -1) {
					index = 0;
				}
				
				schemaListBox.SelectedIndex = index;
			}
		}
		
		void PopulateListBox(string[] namespaces)
		{
			var list = namespaces.OrderBy(item => item);
			
			foreach (string schemaNamespace in namespaces) {
				schemaListBox.Items.Add(schemaNamespace);
			}
				
			schemaListBox.Items.Insert(0, noSelection);
		}
    }
}