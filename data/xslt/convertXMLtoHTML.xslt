<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet xmlns:game="http://www.silentstrike.com/game" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="1.0">
    <xsl:output method="html" indent="yes" />

    <xsl:template match="/">
        <html lang="en">
        <head>
            <meta charset="UTF-8" />
            <meta name="viewport" content="width=device-width, initial-scale=1.0" />
            <title>Silent Strike</title>
            <link rel="stylesheet" href="../css/style.css" />
        </head>
        <body>
            <div id="maze-container">
                <p class='title'>
                    <strong>Maze</strong>
                </p>
                 <!-- We stock the location of the player here to make the code more readable later -->
                <xsl:variable name="playerX" select="game:Game/game:player/game:X" />
                <xsl:variable name="playerY" select="game:Game/game:player/game:Y" /> 
                <xsl:variable name="playerOrientation" select="game:Game/game:player/game:Orientation" /> 

                <!-- Loop through the rows of the grid -->
                <xsl:for-each select="game:Game/game:maze/game:Grid/game:Row">
                    <div class="row">
                        <xsl:for-each select="game:Node">
                            <div class="node">
                                <!-- Add classes based on walls -->
                                <xsl:attribute name="class">
                                    <xsl:text>node</xsl:text>
                                    <xsl:if test="game:TOP='true'"> wall-top</xsl:if>
                                    <xsl:if test="game:RIGHT='true'"> wall-right</xsl:if>
                                    <xsl:if test="game:BOTTOM='true'"> wall-bottom</xsl:if>
                                    <xsl:if test="game:LEFT='true'"> wall-left</xsl:if>
                                </xsl:attribute>

                                <!-- Check if it's the player's position -->
                                <xsl:if test="game:X=$playerX and game:Y=$playerY">
                                    <p class="player">
                                        <xsl:call-template name="PrintPerson">
                                            <xsl:with-param name="orientation" select="$playerOrientation" />
                                        </xsl:call-template>
                                    </p>
                                </xsl:if>

                                <!-- Check if there is an enemy at this position -->
                               <xsl:call-template name="PrintEnemies">
                                    <xsl:with-param name="nodeX" select="game:X" />
                                    <xsl:with-param name="nodeY" select="game:Y" />
                                </xsl:call-template>
                            </div>
                        </xsl:for-each>
                    </div>
                </xsl:for-each>
            </div>
        </body>
        </html>
    </xsl:template>

    <!-- Go through the array of enemies and if one is found in the parameter of the nodes passed as argument return it -->
    <xsl:template name="PrintEnemies">
        <xsl:param name="nodeX" />
        <xsl:param name="nodeY" />

        <xsl:for-each select="../../../../game:enemies/game:Enemy">
            <xsl:if test="game:X=$nodeX and game:Y=$nodeY and game:Alive='true'">
                <p class="red">
                    <xsl:call-template name="PrintPerson">
                        <xsl:with-param name="orientation" select="game:Orientation" />
                    </xsl:call-template>
                </p>
            </xsl:if>
        </xsl:for-each>
    </xsl:template>

    <!-- Function to convert the enemy to one of the symbols (<, >, v, ʌ) based on the orientation  -->
    <xsl:template name="PrintPerson">
        <xsl:param name="orientation" />
        <xsl:choose>
            <xsl:when test="$orientation='EAST'">&gt;</xsl:when>
            <xsl:when test="$orientation='SOUTH'">v</xsl:when>
            <xsl:when test="$orientation='WEST'">&lt;</xsl:when>
            <xsl:when test="$orientation='NORTH'">ʌ</xsl:when>
        </xsl:choose>
    </xsl:template>

</xsl:stylesheet>
