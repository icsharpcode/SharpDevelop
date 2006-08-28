
Updates the SharpDevelop Setup information
------------------------------------------

Product Revision (Subversion revision number)
Product Code Guid (Generating a new Guid for each build)
Package Code Guid (Each new msi gets a new Guid)

The build server and the buildSetup.bat executes the UpdateSetupInfo tool before 
building SharpDevelop.Setup.sln. The SharpDevelop.Setup project does not use the tool.

Operation
---------

1) The SharpDevelop.Setup.wixproj.user is is generated each time the tool is run
based on the SharpDevelop.Setup.wixproj.user.template file. The product revision,
product code guid and package code guid are inserted into the newly generated
file. The last used revision is written to a REVISION file which is put in
the Setup\bin folder. This not in the repository and is only used to stop
the tool from regenerating the product guid if the revision number has not
changed.

2) The build server and buildSetup.bat will run the UpdateSetupInfo tool.
This is not done by the SharpDevelop.Setup project itself intentionally so 
nothing changes when building the project from inside SharpDevelop. The
modified SharpDevelop.Setup.wixproj.user need not be checked into the
repository on each build on the build server.

3) The tool does not update the Guids if the revision number has not changed.
It detects the previously used revision number by reading the REVISION
file in the Setup\bin folder. It is important that Guids are not
regenerated when the revision number has not changed otherwise the
user can install two versions of SharpDevelop 2.1 side by side using
two different installers both for the same revision but each containing
different product guids.

Creating Releases
-----------------

When creating a release either the setup msi from the build server should be
used or that generated after running buildSetup.bat. This will update the
guids and revision number if the current revision has changed.


