<?xml version="1.0"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:Conversion="urn:Conversion">
	<xsl:output method = "xml" indent = "yes" />
	
	<xsl:template match = "/VisualStudioProject/*" >
		<Project DefaultTargets="Build" xmlns="http://schemas.microsoft.com/developer/msbuild/2003">
		<!-- <xsl:element name = "Project" namespace="http://schemas.microsoft.com/developer/msbuild/2003" >-->
			<!-- <xsl:attribute name = "DefaultTargets">Build</xsl:attribute> -->
			
			<!-- Global project options -->
			<xsl:element name = "PropertyGroup" >
				<xsl:element name = "Configuration" >
					<xsl:attribute name = "Condition"> '$(Configuration)' == '' </xsl:attribute>
					<xsl:value-of select = "Build/Settings/Config[1]/@Name" />
				</xsl:element>
				<xsl:element name = "Platform" ><xsl:attribute name = "Condition"> '$(Platform)' == '' </xsl:attribute>AnyCPU</xsl:element>
				<!--<xsl:element name = "ProductVersion">8.0.40607</xsl:element>-->
				<xsl:element name = "SchemaVersion">2.0</xsl:element>
				<xsl:element name = "ProjectGuid"><xsl:value-of select = "@ProjectGuid" /></xsl:element>
				<xsl:element name = "RootNamespace"><xsl:value-of select = "Build/Settings/@RootNamespace" /></xsl:element>
				<xsl:element name = "AssemblyName"><xsl:value-of select = "Build/Settings/@AssemblyName" /></xsl:element>
				<xsl:element name = "OutputType"><xsl:value-of select = "Build/Settings/@OutputType" /></xsl:element>
				<xsl:element name = "ApplicationIcon"><xsl:value-of select = "Build/Settings/@ApplicationIcon" /></xsl:element>
				<xsl:element name = "RunPostBuildEvent">OnSuccessfulBuild</xsl:element>
				<xsl:element name = "PreBuildEvent"><xsl:value-of select = "Build/Settings/@PreBuildEvent" /></xsl:element>
				<xsl:element name = "PostBuildEvent"><xsl:value-of select = "Build/Settings/@PostBuildEvent" /></xsl:element>
				<xsl:element name = "StartupObject"><xsl:value-of select = "Build/Settings/@StartupObject" /></xsl:element>
				<xsl:element name = "NoConfig">false</xsl:element>
			</xsl:element>
			
			<!-- Configurations -->
			<xsl:for-each select="Build/Settings/Config">
				<xsl:element name = "PropertyGroup" >
					<xsl:attribute name = "Condition"> '$(Configuration)|$(Platform)' == '<xsl:value-of select = "@Name" />|AnyCPU' </xsl:attribute>
					
					<xsl:element name = "NoStdLib"><xsl:value-of select = "@NoStdLib" /></xsl:element>
					<xsl:element name = "WarningLevel"><xsl:value-of select = "@WarningLevel" /></xsl:element>
					<xsl:element name = "NoWarn"><xsl:value-of select = "@NoWarn" /></xsl:element>
					<xsl:element name = "DebugSymbols"><xsl:value-of select = "@DebugSymbols" /></xsl:element>
					<xsl:element name = "Optimize"><xsl:value-of select = "@Optimize" /></xsl:element>
					<xsl:element name = "AllowUnsafeBlocks"><xsl:value-of select = "@AllowUnsafeBlocks" /></xsl:element>
					<xsl:element name = "CheckForOverflowUnderflow"><xsl:value-of select = "@CheckForOverflowUnderflow" /></xsl:element>
					<xsl:element name = "DefineConstants"><xsl:value-of select = "@DefineConstants" /></xsl:element>
					<xsl:element name = "OutputPath"><xsl:value-of select = "@OutputPath" /></xsl:element>
					<xsl:element name = "TreatWarningsAsErrors"><xsl:value-of select = "@TreatWarningsAsErrors" /></xsl:element>
				</xsl:element>
			</xsl:for-each>
			
			<xsl:element name = "ItemGroup">
				<xsl:for-each select="Build/References/Reference[@AssemblyName]">
					<xsl:element name = "Reference" >
						<xsl:attribute name = "Include"><xsl:value-of select = "@AssemblyName" /></xsl:attribute>
						<xsl:if test="Conversion:IsNotGacReference(@HintPath)">
							<xsl:element name = "HintPath" ><xsl:value-of select = "@HintPath" /></xsl:element>
							<xsl:element name = "Private" ><xsl:value-of select = "@Private" /></xsl:element>
						</xsl:if>
					</xsl:element>
				</xsl:for-each>
			</xsl:element>
			
			<xsl:element name = "ItemGroup">
				<xsl:for-each select="Files/Include/File">
					<xsl:element name = "{@BuildAction}">
						<xsl:choose>
							<xsl:when test="@Link">
								<xsl:attribute name = "Include"><xsl:value-of select = "@Link" /></xsl:attribute>
								<xsl:element name = "Link"><xsl:value-of select = "@RelPath" /></xsl:element>
							</xsl:when>
							<xsl:otherwise>
								<xsl:attribute name = "Include"><xsl:value-of select = "@RelPath" /></xsl:attribute>
							</xsl:otherwise>
						</xsl:choose>
						<xsl:if test="@DependentUpon">
							<xsl:element name = "DependentUpon"><xsl:value-of select = "@DependentUpon" /></xsl:element>
						</xsl:if>
					</xsl:element>
				</xsl:for-each>
			</xsl:element>
			
			<xsl:element name = "ItemGroup">
				<xsl:for-each select="Build/Imports/Import">
					<xsl:element name = "Import" >
						<xsl:attribute name = "Include"><xsl:value-of select = "@Namespace" /></xsl:attribute>
					</xsl:element>
				</xsl:for-each>
			</xsl:element>
			
			<xsl:element name = "ItemGroup">
				<xsl:for-each select="Build/References/Reference[@Project]">
					<xsl:element name = "ProjectReference" >
						<xsl:attribute name = "Include"><xsl:value-of select = "Conversion:GetRelativeProjectPathByGuid(@Name, @Project)" /></xsl:attribute>
						
						<xsl:element name = "Project"><xsl:value-of select = "@Project" /></xsl:element>
						<xsl:element name = "Name"><xsl:value-of select = "@Name" /></xsl:element>
						<xsl:element name = "Private" ><xsl:value-of select = "@Private" /></xsl:element>
					</xsl:element>
				</xsl:for-each>
			</xsl:element>
			
			<xsl:element name = "Import" >
				<xsl:attribute name = "Project">$(MSBuildBinPath)\Microsoft.<xsl:value-of select = "Conversion:GetLanguageName()" />.Targets</xsl:attribute>
			</xsl:element>
		
		
		</Project>
<!--		</xsl:element>-->
	</xsl:template>
</xsl:stylesheet>
