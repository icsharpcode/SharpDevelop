<?xml version="1.0"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:convtool="urn:convtool">
	<xsl:param name="ProjectGUID"/>
	<xsl:param name="LanguageName"/>
	<xsl:param name="FrameworkPath"/>
	<xsl:template match="/Project">
		<VisualStudioProject>
			<xsl:element name = "{$LanguageName}">
				
				<xsl:attribute name = "ProjectType">Local</xsl:attribute>
				<xsl:attribute name = "ProductVersion">7.10.3077</xsl:attribute>
				<xsl:attribute name = "SchemaVersion">2.0</xsl:attribute>
				<xsl:attribute name = "ProjectGuid"><xsl:value-of select = "$ProjectGUID"/></xsl:attribute>
				
				<Build>
					<Settings
						ApplicationIcon = "{Configuration/CodeGeneration/@win32icon}"
						AssemblyKeyContainerName = ""
						AssemblyName = "{Configuration/Output/@assembly}"
						AssemblyOriginatorKeyFile = ""
						DefaultClientScript = "JScript"
						DefaultHTMLPageLayout = "Grid"
						DefaultTargetSchema = "IE50"
						DelaySign = "false"
						OutputType = "{Configuration/CodeGeneration/@target}"
						PreBuildEvent = ""
						PostBuildEvent = ""
						RootNamespace = "{@standardNamespace}"
						RunPostBuildEvent = "OnBuildSuccess"
						StartupObject = "{Configuration/CodeGeneration/@mainclass}"
					>
						<xsl:for-each select="Configurations/Configuration">
							<Config
								Name = "{convtool:AddConfig(@name)}"
								AllowUnsafeBlocks = "{CodeGeneration/@unsafecodeallowed}"
								BaseAddress = "285212672"
								CheckForOverflowUnderflow = "{CodeGeneration/@generateoverflowchecks}"
								ConfigurationOverrideFile = ""
								DefineConstants = "{CodeGeneration/@definesymbols}"
								DocumentationFile = ""
								DebugSymbols = "{CodeGeneration/@includedebuginformation}"
								FileAlignment = "4096"
								IncrementalBuild = "false"
								NoStdLib = "{CodeGeneration/@nostdlib}"
								NoWarn = "{CodeGeneration/@nowarn}"
								Optimize = "{CodeGeneration/@optimize}"
								OutputPath = "{Output/@directory}"
								RegisterForComInterop = "false"
								RemoveIntegerChecks = "false"
								TreatWarningsAsErrors = "{convtool:Negate(@runwithwarnings)}"
								WarningLevel = "{CodeGeneration/@warninglevel}"
							/>
						</xsl:for-each>
					</Settings>
					<References>
						<Reference
							Name = "System"
							AssemblyName = "System"
							HintPath = "{$FrameworkPath}System.dll"
						/>
						<Reference
							Name = "System.Data"
							AssemblyName = "System.Data"
							HintPath = "{$FrameworkPath}System.Data.dll"
						/>
						<Reference
							Name = "System.XML"
							AssemblyName = "System.Xml"
							HintPath = "{$FrameworkPath}System.XML.dll"
						/>
						<Reference
							Name = "System.Drawing"
							AssemblyName = "System.Drawing"
							HintPath = "{$FrameworkPath}System.Drawing.dll"
						/>
						<Reference
							Name = "System.Windows.Forms"
							AssemblyName = "System.Windows.Forms"
							HintPath = "{$FrameworkPath}System.Windows.Forms.dll"
						/>
						<xsl:for-each select="References/Reference[@type='Assembly']">
							<Reference
								Name = "{convtool:FileNameWithoutExtension(@refto)}"
								AssemblyName = "{convtool:FileNameWithoutExtension(@refto)}"
								HintPath = "{convtool:VerifyFileLocation(@refto)}"
							/>
						</xsl:for-each>
						<xsl:for-each select="References/Reference[@type='Project']">
							<Reference
								Name = "{@refto}"
								Project = "{convtool:GetProjectGUID(@refto)}"
								Package = "{convtool:GetPackageGUID(@refto)}"
							/>
						</xsl:for-each>
					</References>
				</Build>
				<Files>
					<Include>
						<xsl:for-each select="Contents/File[@buildaction!='Exclude' and @subtype!='Directory']">
							<File
								RelPath = "{convtool:VerifyFileLocation(@name)}"
								BuildAction = "{convtool:ConvertBuildAction(@buildaction)}"
							/>
						</xsl:for-each>
					</Include>
				</Files>
			</xsl:element>
		</VisualStudioProject>
	</xsl:template>
</xsl:stylesheet>
