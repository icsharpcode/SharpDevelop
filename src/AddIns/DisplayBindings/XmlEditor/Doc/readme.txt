INSTALLATION:
-------------

Using Binaries
--------------

Assuming SharpDevelop is installed in the folder "C:/Program Files/SharpDevelop"

Copy the contents of the "bin" folder to "C:/Program Files/SharpDevelop/AddIns/AddIns/DisplayBindings/XmlEditor".

The binaries should work with SharpDevelop v1.0.3.1768.


Building source
---------------

Copy the contents of the XmlEditor folder to "src/AddIns/DisplayBindings/XmlEditor".

Load the "XmlEditor.cmbx" combine and build the project OR run the "XmlEditor.build" file with NAnt 0.84.


Configuring Schema Auto-completion
----------------------------------

Go to Tools->Options->Text Editor->Xml Schemas.  Use the Add button to add your .xsds to SharpDevelop.



Done:
-----

Properties taken from SharpDevelop.
Handle property changes whilst editor is open.
Validate xml menu only available for the xml editor.
Validation against schema.  Right click and menu option.
User added xsds.
System xsds.
Check that we can still "jump to" NAnt errors in .build files.
Test getting element completion data for the root element of a document.
Code completion window size.  Long namespace URIs do not fit in the window.  Duplicated
   the CodeCompletionWindow class.  The item width calculation should really be in 
   the CodeCompletionListView.
Custom code completion window.  Only autocomplete when the tab or return key
    is pressed.
Validation of the xml against the schema - send output to build window and add
    task list entry. 
Comment folding.
Recognise special file extensions (e.g. build).  Allow user to specify schema
    for a file extension.  This would act as a default, but could be overriden
    by specifying an xmlns in the xml.
