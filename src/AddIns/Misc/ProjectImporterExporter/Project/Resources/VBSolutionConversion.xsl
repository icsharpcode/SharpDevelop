<?xml version="1.0"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:convtool="urn:convtool">
	<xsl:param name="ProjectTitle"/>
	<xsl:template match="/VisualStudioProject/VisualBasic">
		<Project name = "{$ProjectTitle}"
		         standardNamespace="{Build/Settings/@RootNamespace}"
		         description = ""
		         newfilesearch = "None"
		         enableviewstate = "True"
		         version = "1.1"
		         projecttype = "VBNET">
			
			<!-- Transform Contents -->
			<Contents>
				
				<xsl:for-each select="Files/Include/File[@BuildAction ='Compile']">
					<xsl:choose>
						<xsl:when test="@Link">
							<File name = "{convtool:VerifyFileLocation(@Link)}"
							      buildaction="Compile"
							      subtype = "Code"/>
						</xsl:when>
						<xsl:otherwise>
							<File name = "{convtool:VerifyFileLocation(@RelPath)}"
							      buildaction="Compile"
							      subtype = "Code"/>
						</xsl:otherwise>
					</xsl:choose>
				</xsl:for-each>
				
				<!-- convert 'resources' -->
				<xsl:for-each select="Files/Include/File[@BuildAction ='EmbeddedResource']">
					<xsl:choose>
						<!-- VB.NET in VS has a different resource management than C# -->
						<xsl:when test="@Link">
							<File name = "{convtool:ImportDependentResource(@Link,@DependentUpon,/VisualStudioProject/VisualBasic/Build/Settings/@RootNamespace)}"
							      buildaction="EmbedAsResource"
							      subtype = "Code"/>
						</xsl:when>
						<xsl:otherwise>
							<File name = "{convtool:ImportDependentResource(@RelPath,@DependentUpon,/VisualStudioProject/VisualBasic/Build/Settings/@RootNamespace)}"
							      buildaction="EmbedAsResource"
							      subtype = "Code"/>
						</xsl:otherwise>
					</xsl:choose>
				</xsl:for-each>
				
				<!-- Non-buildable files part of the project -->
				<xsl:for-each select="Files/Include/File[(@BuildAction ='None') or (@BuildAction ='Content')]">
					<xsl:choose>
						<xsl:when test="@Link">
							<File name = "{convtool:VerifyFileLocation(@Link)}"
							      buildaction="Nothing"
							      subtype = "Code"/>
						</xsl:when>
						<xsl:otherwise>
							<File name = "{convtool:VerifyFileLocation(@RelPath)}"
							      buildaction="Nothing"
							      subtype = "Code"/>
						</xsl:otherwise>
					</xsl:choose>
				</xsl:for-each>
			</Contents>
			
			<DeploymentInformation target="" script="" strategy="File" />
			
			<!-- Transform Settings -->
			
			<xsl:for-each select="Build/Settings">
				<Configurations active="Debug">
					<xsl:for-each select="Config">
						
						<Configuration runwithwarnings="{convtool:Negate(@TreatWarningsAsErrors)}" name="{@Name}">
							
							<xsl:element name = "CodeGeneration" >
								<xsl:attribute name = "includedebuginformation"><xsl:value-of select = "convtool:EnsureBool(@DebugSymbols)"/></xsl:attribute>
								<xsl:attribute name = "optimize"><xsl:value-of select = "convtool:EnsureBool(@Optimize)"/></xsl:attribute>
								<xsl:attribute name = "generateoverflowchecks"><xsl:value-of select = "convtool:EnsureBool(@CheckForOverflowUnderflow)"/></xsl:attribute>
								<xsl:attribute name = "rootnamespace"><xsl:value-of select = "../@RootNamespace"/></xsl:attribute>
								<xsl:attribute name = "mainclass"><xsl:value-of select = "../@StartupObject"/></xsl:attribute>
								<xsl:attribute name = "target"><xsl:value-of select = "../@OutputType"/></xsl:attribute>
								<xsl:attribute name = "definesymbols"><xsl:value-of select = "@DefineConstants"/></xsl:attribute>
								<xsl:attribute name = "generatexmldocumentation">False</xsl:attribute>
								<xsl:attribute name = "win32Icon"><xsl:value-of select = "convtool:VerifyFileLocation(../@ApplicationIcon)"/></xsl:attribute>
								<xsl:attribute name = "imports"><xsl:for-each select="/VisualStudioProject/VisualBasic/Build/Imports/Import"><xsl:value-of select = "@Namespace"/>,</xsl:for-each></xsl:attribute>
							</xsl:element>
							
							<VBDOC outputfile=""
							       enablevbdoc="False"
							       filestoparse=""
							       commentprefix="" />
							<Execution consolepause="True"
							           commandlineparameters="" />
							<Output directory="{@OutputPath}"
							        assembly="{../@AssemblyName}" />
						</Configuration>
					</xsl:for-each>
				</Configurations>
			</xsl:for-each>
			
			<!-- Transform References -->
			<xsl:apply-templates select="Build/References"/>
		</Project>
	</xsl:template>
	
	<!-- Transform references (a bit like frungy) -->
	<xsl:template match="References">
		<References>
			<xsl:for-each select="Reference[@AssemblyName]">
				<xsl:if test="convtool:ShouldGenerateReference('True', @AssemblyName, @HintPath)">
					<Reference type  = "{convtool:GenerateReferenceType(@AssemblyName, @HintPath)}"
					           refto = "{convtool:GenerateReference(@AssemblyName, @HintPath)}"/>
				</xsl:if>
			</xsl:for-each>
			<xsl:for-each select="Reference[@Project]">
				<Reference type  = "Project"
				           refto = "{@Name}"/>
			</xsl:for-each>
		</References>
	</xsl:template>
</xsl:stylesheet>
