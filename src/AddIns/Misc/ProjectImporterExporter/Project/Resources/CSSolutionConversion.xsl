<?xml version="1.0"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:convtool="urn:convtool">
	<xsl:param name="ProjectTitle"/>
	<xsl:template match="/VisualStudioProject/CSHARP">
		<Project name = "{$ProjectTitle}"
		         standardNamespace="{Build/Settings/@RootNamespace}"
		         description = ""
		         newfilesearch = "None"
		         enableviewstate = "True"
		         version = "1.1"
		         projecttype = "C#">
			
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
						<xsl:when test="@Link">
							<xsl:choose>
								<xsl:when test="@DependentUpon">
									<File name = "{convtool:ImportDependentResource(@Link,@DependentUpon,/VisualStudioProject/CSHARP/Build/Settings/@RootNamespace)}"
									      buildaction="EmbedAsResource"
									      subtype = "Code"/>
								</xsl:when>
								<xsl:otherwise>
									<File name = "{convtool:ImportResource(@Link,/VisualStudioProject/CSHARP/Build/Settings/@RootNamespace)}"
									      buildaction="EmbedAsResource"
									      subtype = "Code"/>
								</xsl:otherwise>
							</xsl:choose>
						</xsl:when>
						<xsl:otherwise>
							<xsl:choose>
								<xsl:when test="@DependentUpon">
									<File name = "{convtool:ImportDependentResource(@RelPath,@DependentUpon,/VisualStudioProject/CSHARP/Build/Settings/@RootNamespace)}"
									      buildaction="EmbedAsResource"
									      subtype = "Code"/>
								</xsl:when>
								<xsl:otherwise>
									<File name = "{convtool:ImportResource(@RelPath,/VisualStudioProject/CSHARP/Build/Settings/@RootNamespace)}"
									      buildaction="EmbedAsResource"
									      subtype = "Code"/>
								</xsl:otherwise>
							</xsl:choose>
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
			<xsl:apply-templates select="Build/Settings"/>
			
			<!-- Transform References -->
			<xsl:apply-templates select="Build/References"/>
		</Project>
	</xsl:template>
	
	<!-- Transform settings (easy) -->
	<xsl:template match="Settings">
		<Configurations active="Debug">
			<xsl:for-each select="Config">
				<Configuration runwithwarnings="{convtool:Negate(@TreatWarningsAsErrors)}" name="{@Name}">
					<CodeGeneration runtime="MsNet"
					                compiler="Csc"
					                warninglevel="{@WarningLevel}"
					                includedebuginformation="{convtool:EnsureBool(@DebugSymbols)}"
					                optimize="{convtool:EnsureBool(@Optimize)}"
					                unsafecodeallowed="{convtool:EnsureBool(@AllowUnsafeBlocks)}"
					                generateoverflowchecks="{convtool:EnsureBool(@CheckForOverflowUnderflow)}"
					                mainclass="{../@StartupObject}"
					                target="{../@OutputType}"
					                definesymbols="{@DefineConstants}"
					                generatexmldocumentation="False"
					                win32Icon="{convtool:VerifyFileLocation(../@ApplicationIcon)}" />
					<Execution commandlineparameters=""
					           consolepause="True" />
					<Output directory="{convtool:VerifyFileLocation(@OutputPath)}"
					        assembly="{../@AssemblyName}" />
				</Configuration>
			</xsl:for-each>
		</Configurations>
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
