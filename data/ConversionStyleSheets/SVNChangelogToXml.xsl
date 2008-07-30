<xsl:transform version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	<xsl:template match="log">
		<ChangeLog project="SharpDevelop">
			<xsl:for-each select="./logentry">
				<Change>
					<xsl:attribute name="author">
						<xsl:variable name="orig_author" select="./author" />
						<xsl:variable name="author">
							<xsl:choose>
								<xsl:when test="$orig_author = 'mikekrueger'">
	   								<xsl:text>Mike Krüger</xsl:text>
	  							</xsl:when>
	  							<xsl:when test="$orig_author = 'roma'">
	   								<xsl:text>Roman Taranchenko</xsl:text>
	  							</xsl:when>
	  							<xsl:when test="$orig_author = 'georgb'">
	   								<xsl:text>Georg Brandl</xsl:text>
	  							</xsl:when>
								<xsl:when test="$orig_author = 'andreapaatz'">
	   								<xsl:text>Andrea Paatz</xsl:text>
	  							</xsl:when>
								<xsl:when test="$orig_author = 'danielgrunwald'">
	   								<xsl:text>Daniel Grunwald</xsl:text>
	  							</xsl:when>
	  							<xsl:when test="$orig_author = 'denise'">
	   								<xsl:text>Denis Erchoff</xsl:text>
	  							</xsl:when>	  	
	  							<xsl:when test="$orig_author = 'markuspalme'">
	   								<xsl:text>Markus Palme</xsl:text>
	  							</xsl:when>
	  							<xsl:when test="$orig_author = 'ivoko'">
	   								<xsl:text>Ivo Kovacka</xsl:text>
	  							</xsl:when>
	  							<xsl:when test="$orig_author = 'jfreilly'">
	   								<xsl:text>John Reilly</xsl:text>
	  							</xsl:when>
	  							<xsl:when test="$orig_author = 'christophw'">
	   								<xsl:text>Christoph Wille</xsl:text>
	  							</xsl:when>
	  							<xsl:when test="$orig_author = 'alexandre'">
									<xsl:text>Alexandre Semenov</xsl:text>
	  							</xsl:when>
	  							<xsl:when test="$orig_author = 'nikola'">
									<xsl:text>Nikola Kavaldjiev</xsl:text>
	  							</xsl:when>
	  							<xsl:when test="$orig_author = 'mattward'">
									<xsl:text>Matt Ward</xsl:text>
	  							</xsl:when>
	  							<xsl:when test="$orig_author = 'mathiassimmack'">
									<xsl:text>Mathias Simmack</xsl:text>
	  							</xsl:when>
	  							<xsl:when test="$orig_author = 'davidsrbecky'">
									<xsl:text>David Srbecký</xsl:text>
	  							</xsl:when>
	  							<xsl:when test="$orig_author = 'davidalpert'">
									<xsl:text>David Alpert</xsl:text>
	  							</xsl:when>
	  							<xsl:when test="$orig_author = 'peterforstmeier'">
									<xsl:text>Peter Forstmeier</xsl:text>
	  							</xsl:when>
	  							<xsl:when test="$orig_author = 'scottferrett'">
									<xsl:text>Scott Ferrett</xsl:text>
	  							</xsl:when>
	  							<xsl:when test="$orig_author = 'dickonfield'">
									<xsl:text>Dickon Field</xsl:text>
	  							</xsl:when>
	  							<xsl:when test="$orig_author = 'itaibarhaim'">
									<xsl:text>Itai Bar-Haim</xsl:text>
	  							</xsl:when>
	  							<xsl:when test="$orig_author = 'christianhornung'">
									<xsl:text>Christian Hornung</xsl:text>
	  							</xsl:when>
	  							<xsl:when test="$orig_author = 'justindearing'">
									<xsl:text>Justin Dearing</xsl:text>
	  							</xsl:when>
	  							<xsl:when test="$orig_author = 'russellwilkins'">
									<xsl:text>Russell Wilkins</xsl:text>
	  							</xsl:when>
								<xsl:when test="$orig_author = 'robertpickering'">
									<xsl:text>Robert Pickering</xsl:text>
	  							</xsl:when>
								<xsl:when test="$orig_author = 'siegfriedpammer'">
									<xsl:text>Siegfried Pammer</xsl:text>
	  							</xsl:when>
								<xsl:when test="$orig_author = 'ivanshumilin'">
									<xsl:text>Ivan Shumilin</xsl:text>
	  							</xsl:when>
	  							<xsl:otherwise>
	  								<xsl:value-of select="./author" />
	  							</xsl:otherwise>
							</xsl:choose>
						</xsl:variable>
						<xsl:value-of select="$author" />
					</xsl:attribute>
					<xsl:attribute name="date">
						<xsl:value-of select="./date" />
					</xsl:attribute>
					<xsl:value-of select="./msg" />
				 </Change>
			</xsl:for-each>
		</ChangeLog>
	</xsl:template>
</xsl:transform>
