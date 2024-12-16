<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="1.0"
                xmlns:menu="http://www.silentstrike.com/menu">
    <xsl:output method="html" indent="yes" />

    <xsl:template match="/menu:saved_games">
        <html>
        <head>
            <title>Saved Games</title>
            <link rel="stylesheet" href="css/menu_style.css" />
        </head>
        <body>
            <h1>Saved Games</h1>
            <ol>
                <xsl:for-each select="menu:file">
                    <li>
                        <!-- Set the href to the name of the file -->
                        <a href="{menu:path}">
                            <strong><xsl:value-of select="menu:name" /></strong>
                            <xsl:text> Last modified on: </xsl:text>
                            <xsl:value-of select="menu:last_modified" />
                        </a>
                    </li>
                </xsl:for-each>
            </ol>
        </body>
        </html>
    </xsl:template>

</xsl:stylesheet>
