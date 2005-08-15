<?xml version="1.0" encoding="ISO-8859-1"?>
     
<xsl:transform version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	<xsl:template match="ChangeLog">
		<HTML><HEAD></HEAD><BODY>
			<TABLE CLASS="dtTABLE" cellspacing="0">
				<TR>
					<TH align="left" class="copy" width="100">Author</TH>
					<TH align="left" class="copy">Date</TH>
					<TH align="left" class="copy">Change</TH>
				</TR>
			
				<xsl:for-each select="Change">
					<TR>
						<TD align="left" class="copy"><xsl:value-of select="@author"/></TD>
						<TD align="left" class="copy"><xsl:value-of select="@date"/></TD>
						<TD align="left" class="copy"><xsl:value-of select="."/></TD>
					</TR>
				</xsl:for-each>
			</TABLE>

		</BODY></HTML>
	</xsl:template>
</xsl:transform>
