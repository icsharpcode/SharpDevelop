<!-- 
	Converts PartCover code coverage output report to an NCover report.
-->
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="1.0">
	<xsl:output version="1.0" encoding="utf-8" omit-xml-declaration="no"/>

	<xsl:template match="/">
		<xsl:element name="coverage">
			<xsl:attribute name="name">
				<xsl:value-of select=""
			</xsl:attribute>
		</xsl:element>
	</xsl:template>
</xsl:stylesheet>