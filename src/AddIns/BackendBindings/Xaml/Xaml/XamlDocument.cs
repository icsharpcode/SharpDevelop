using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Diagnostics;
using System.Windows.Markup;

namespace ICSharpCode.Xaml
{
	public class XamlDocument : IUriContext
	{
		internal XamlDocument(XamlProject project)
		{
			Project = project;
			Parser = new XamlParser(this);
			Trackers.Add(new InstanceTracker());
			Trackers.Add(new XmlTracker());
		}

		public XamlParser Parser { get; private set; }
		public XamlProject Project { get; private set; }
		public List<XamlDocument> Dependencies = new List<XamlDocument>();
		public List<XamlDocumentError> Errors = new List<XamlDocumentError>();
		public List<Tracker> Trackers = new List<Tracker>();
		public XDocument XmlDocument;

		public event EventHandler RootChanged;
		public event DocumentChangedEventHandler DocumentChanged;

		bool parsing;

		ObjectNode root;

		public ObjectNode Root
		{
			get
			{
				return root;
			}
			set
			{
				var e = new DocumentChangedEventArgs() { OldNode = root, NewNode = value };
				root = value;
				RaiseDocumentChanged(e);

				if (RootChanged != null) {
					RootChanged(this, EventArgs.Empty);
				}
			}
		}

		public ObjectNode ParseObject(string text)
		{
			throw new NotImplementedException();
		}

		public ObjectNode CreateObject(object instance)
		{
			return new ObjectNode(this, instance);
		}

		public void Parse(string text)
		{			
			ObjectNode result = null;
			Errors.Clear();
			parsing = true;

			try {
				XmlDocument = XDocument.Parse(text);
				result = Parser.CreateObjectNodeFromXmlElement(XmlDocument.Root, false);
				foreach (var node in result.DescendantsAndSelf()) {
					node.Document = this;
				}
			}
			catch (Exception x) {
			}

			parsing = false;
			Root = result;
		}

		public bool CanSave
		{
			get { return XmlDocument != null; }
		}

		public string Save()
		{
			//return XamlFormatter.Format(XmlDocument.ToString());

			var settings = new XmlWriterSettings() {
				Indent = true,
				IndentChars = "    ",
				NewLineOnAttributes = true,
				OmitXmlDeclaration = true
			};
			var sb = new StringBuilder();
			using (var writer = XmlWriter.Create(sb, settings)) {
				XmlDocument.WriteTo(writer);
			}

			return sb.ToString();
		}

		internal void RaiseDocumentChanged(DocumentChangedEventArgs e)
		{
			if (!parsing) {
				foreach (var t in Trackers) {
					t.Process(e);
				}
				if (DocumentChanged != null) {
					DocumentChanged(this, e);
				}
			}
		}

		#region IUriContext Members

		Uri baseUri;

		public Uri BaseUri
		{
			get
			{
				return baseUri;
			}
			set
			{
				if (baseUri != null) {
					Project.Documents.Remove(baseUri);
				}
				baseUri = value;
				if (baseUri != null) {
					Project.Documents[baseUri] = this;
				}
			}
		}

		#endregion
	}

	public delegate void DocumentChangedEventHandler(object sender, DocumentChangedEventArgs e);

	public class DocumentChangedEventArgs : EventArgs
	{
		public XamlNode OldNode;
		public XamlNode OldParent;
		public XamlNode NewNode;
	}
}
