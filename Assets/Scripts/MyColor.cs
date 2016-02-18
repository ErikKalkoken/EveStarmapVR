using UnityEngine;

// Helper class adding additional named colors based on standard HTML color names for C# scripting in Unity
// Colors are defined according to information from W3Schools.com Please see their page for a representation of the colors (http://www.w3schools.com/tags/ref_colornames.asp)
// Written by Erik Kalkoken for Unity 3D in 2014
// Licence: No rights reserved - public doman
// Script example: Color itemColor = MyColor.aquamarine;

/// <summary>
/// Helper class with additional named colors for Unity based on standard HTML color names
/// </summary>
public class MyColor
{
	public static Color aliceBlue
	{
		get
		{
			return new Color32 (240,248,255,255);
		}
	}
	
	
	public static Color antiqueWhite
	{
		get
		{
			return new Color32 (250,235,215,255);
		}
	}
	
	
	public static Color aqua
	{
		get
		{
			return new Color32 (0,255,255,255);
		}
	}
	
	
	public static Color aquamarine
	{
		get
		{
			return new Color32 (127,255,212,255);
		}
	}
	
	
	public static Color azure
	{
		get
		{
			return new Color32 (240,255,255,255);
		}
	}
	
	
	public static Color beige
	{
		get
		{
			return new Color32 (245,245,220,255);
		}
	}
	
	
	public static Color bisque
	{
		get
		{
			return new Color32 (255,228,196,255);
		}
	}
	
	
	public static Color black
	{
		get
		{
			return new Color32 (0,0,0,255);
		}
	}
	
	
	public static Color blanchedAlmond
	{
		get
		{
			return new Color32 (255,235,205,255);
		}
	}
	
	
	public static Color blue
	{
		get
		{
			return new Color32 (0,0,255,255);
		}
	}
	
	
	public static Color blueViolet
	{
		get
		{
			return new Color32 (138,43,226,255);
		}
	}
	
	
	public static Color brown
	{
		get
		{
			return new Color32 (165,42,42,255);
		}
	}
	
	
	public static Color burlyWood
	{
		get
		{
			return new Color32 (222,184,135,255);
		}
	}
	
	
	public static Color cadetBlue
	{
		get
		{
			return new Color32 (95,158,160,255);
		}
	}
	
	
	public static Color chartreuse
	{
		get
		{
			return new Color32 (127,255,0,255);
		}
	}
	
	
	public static Color chocolate
	{
		get
		{
			return new Color32 (210,105,30,255);
		}
	}
	
	
	public static Color coral
	{
		get
		{
			return new Color32 (255,127,80,255);
		}
	}
	
	
	public static Color cornflowerBlue
	{
		get
		{
			return new Color32 (100,149,237,255);
		}
	}
	
	
	public static Color cornsilk
	{
		get
		{
			return new Color32 (255,248,220,255);
		}
	}
	
	
	public static Color crimson
	{
		get
		{
			return new Color32 (220,20,60,255);
		}
	}
	
	
	public static Color cyan
	{
		get
		{
			return new Color32 (0,255,255,255);
		}
	}
	
	
	public static Color darkBlue
	{
		get
		{
			return new Color32 (0,0,139,255);
		}
	}
	
	
	public static Color darkCyan
	{
		get
		{
			return new Color32 (0,139,139,255);
		}
	}
	
	
	public static Color darkGoldenRod
	{
		get
		{
			return new Color32 (184,134,11,255);
		}
	}
	
	
	public static Color darkGray
	{
		get
		{
			return new Color32 (169,169,169,255);
		}
	}
	
	
	public static Color darkGreen
	{
		get
		{
			return new Color32 (0,100,0,255);
		}
	}
	
	
	public static Color darkKhaki
	{
		get
		{
			return new Color32 (189,183,107,255);
		}
	}
	
	
	public static Color darkMagenta
	{
		get
		{
			return new Color32 (139,0,139,255);
		}
	}
	
	
	public static Color darkOliveGreen
	{
		get
		{
			return new Color32 (85,107,47,255);
		}
	}
	
	
	public static Color darkOrange
	{
		get
		{
			return new Color32 (255,140,0,255);
		}
	}
	
	
	public static Color darkOrchid
	{
		get
		{
			return new Color32 (153,50,204,255);
		}
	}
	
	
	public static Color darkRed
	{
		get
		{
			return new Color32 (139,0,0,255);
		}
	}
	
	
	public static Color darkSalmon
	{
		get
		{
			return new Color32 (233,150,122,255);
		}
	}
	
	
	public static Color darkSeaGreen
	{
		get
		{
			return new Color32 (143,188,143,255);
		}
	}
	
	
	public static Color darkSlateBlue
	{
		get
		{
			return new Color32 (72,61,139,255);
		}
	}
	
	
	public static Color darkSlateGray
	{
		get
		{
			return new Color32 (47,79,79,255);
		}
	}
	
	
	public static Color darkTurquoise
	{
		get
		{
			return new Color32 (0,206,209,255);
		}
	}
	
	
	public static Color darkViolet
	{
		get
		{
			return new Color32 (148,0,211,255);
		}
	}
	
	
	public static Color deepPink
	{
		get
		{
			return new Color32 (255,20,147,255);
		}
	}
	
	
	public static Color deepSkyBlue
	{
		get
		{
			return new Color32 (0,191,255,255);
		}
	}
	
	
	public static Color dimGray
	{
		get
		{
			return new Color32 (105,105,105,255);
		}
	}
	
	
	public static Color dodgerBlue
	{
		get
		{
			return new Color32 (30,144,255,255);
		}
	}
	
	
	public static Color fireBrick
	{
		get
		{
			return new Color32 (178,34,34,255);
		}
	}
	
	
	public static Color floralWhite
	{
		get
		{
			return new Color32 (255,250,240,255);
		}
	}
	
	
	public static Color forestGreen
	{
		get
		{
			return new Color32 (34,139,34,255);
		}
	}
	
	
	public static Color fuchsia
	{
		get
		{
			return new Color32 (255,0,255,255);
		}
	}
	
	
	public static Color gainsboro
	{
		get
		{
			return new Color32 (220,220,220,255);
		}
	}
	
	
	public static Color ghostWhite
	{
		get
		{
			return new Color32 (248,248,255,255);
		}
	}
	
	
	public static Color gold
	{
		get
		{
			return new Color32 (255,215,0,255);
		}
	}
	
	
	public static Color goldenRod
	{
		get
		{
			return new Color32 (218,165,32,255);
		}
	}
	
	
	public static Color gray
	{
		get
		{
			return new Color32 (128,128,128,255);
		}
	}
	
	
	public static Color green
	{
		get
		{
			return new Color32 (0,128,0,255);
		}
	}
	
	
	public static Color greenYellow
	{
		get
		{
			return new Color32 (173,255,47,255);
		}
	}
	
	
	public static Color honeyDew
	{
		get
		{
			return new Color32 (240,255,240,255);
		}
	}
	
	
	public static Color hotPink
	{
		get
		{
			return new Color32 (255,105,180,255);
		}
	}
	
	
	public static Color indianRed
	{
		get
		{
			return new Color32 (205,92,92,255);
		}
	}
	
	
	public static Color indigo
	{
		get
		{
			return new Color32 (75,0,130,255);
		}
	}
	
	
	public static Color ivory
	{
		get
		{
			return new Color32 (255,255,240,255);
		}
	}
	
	
	public static Color khaki
	{
		get
		{
			return new Color32 (240,230,140,255);
		}
	}
	
	
	public static Color lavender
	{
		get
		{
			return new Color32 (230,230,250,255);
		}
	}
	
	
	public static Color lavenderBlush
	{
		get
		{
			return new Color32 (255,240,245,255);
		}
	}
	
	
	public static Color lawnGreen
	{
		get
		{
			return new Color32 (124,252,0,255);
		}
	}
	
	
	public static Color lemonChiffon
	{
		get
		{
			return new Color32 (255,250,205,255);
		}
	}
	
	
	public static Color lightBlue
	{
		get
		{
			return new Color32 (173,216,230,255);
		}
	}
	
	
	public static Color lightCoral
	{
		get
		{
			return new Color32 (240,128,128,255);
		}
	}
	
	
	public static Color lightCyan
	{
		get
		{
			return new Color32 (224,255,255,255);
		}
	}
	
	
	public static Color lightGoldenRodYellow
	{
		get
		{
			return new Color32 (250,250,210,255);
		}
	}
	
	
	public static Color lightGray
	{
		get
		{
			return new Color32 (211,211,211,255);
		}
	}
	
	
	public static Color lightGreen
	{
		get
		{
			return new Color32 (144,238,144,255);
		}
	}
	
	
	public static Color lightPink
	{
		get
		{
			return new Color32 (255,182,193,255);
		}
	}
	
	
	public static Color lightSalmon
	{
		get
		{
			return new Color32 (255,160,122,255);
		}
	}
	
	
	public static Color lightSeaGreen
	{
		get
		{
			return new Color32 (32,178,170,255);
		}
	}
	
	
	public static Color lightSkyBlue
	{
		get
		{
			return new Color32 (135,206,250,255);
		}
	}
	
	
	public static Color lightSlateGray
	{
		get
		{
			return new Color32 (119,136,153,255);
		}
	}
	
	
	public static Color lightSteelBlue
	{
		get
		{
			return new Color32 (176,196,222,255);
		}
	}
	
	
	public static Color lightYellow
	{
		get
		{
			return new Color32 (255,255,224,255);
		}
	}
	
	
	public static Color lime
	{
		get
		{
			return new Color32 (0,255,0,255);
		}
	}
	
	
	public static Color limeGreen
	{
		get
		{
			return new Color32 (50,205,50,255);
		}
	}
	
	
	public static Color linen
	{
		get
		{
			return new Color32 (250,240,230,255);
		}
	}
	
	
	public static Color magenta
	{
		get
		{
			return new Color32 (255,0,255,255);
		}
	}
	
	
	public static Color maroon
	{
		get
		{
			return new Color32 (128,0,0,255);
		}
	}
	
	
	public static Color mediumAquaMarine
	{
		get
		{
			return new Color32 (102,205,170,255);
		}
	}
	
	
	public static Color mediumBlue
	{
		get
		{
			return new Color32 (0,0,205,255);
		}
	}
	
	
	public static Color mediumOrchid
	{
		get
		{
			return new Color32 (186,85,211,255);
		}
	}
	
	
	public static Color mediumPurple
	{
		get
		{
			return new Color32 (147,112,219,255);
		}
	}
	
	
	public static Color mediumSeaGreen
	{
		get
		{
			return new Color32 (60,179,113,255);
		}
	}
	
	
	public static Color mediumSlateBlue
	{
		get
		{
			return new Color32 (123,104,238,255);
		}
	}
	
	
	public static Color mediumSpringGreen
	{
		get
		{
			return new Color32 (0,250,154,255);
		}
	}
	
	
	public static Color mediumTurquoise
	{
		get
		{
			return new Color32 (72,209,204,255);
		}
	}
	
	
	public static Color mediumVioletRed
	{
		get
		{
			return new Color32 (199,21,133,255);
		}
	}
	
	
	public static Color midnightBlue
	{
		get
		{
			return new Color32 (25,25,112,255);
		}
	}
	
	
	public static Color mintCream
	{
		get
		{
			return new Color32 (245,255,250,255);
		}
	}
	
	
	public static Color mistyRose
	{
		get
		{
			return new Color32 (255,228,225,255);
		}
	}
	
	
	public static Color moccasin
	{
		get
		{
			return new Color32 (255,228,181,255);
		}
	}
	
	
	public static Color navajoWhite
	{
		get
		{
			return new Color32 (255,222,173,255);
		}
	}
	
	
	public static Color navy
	{
		get
		{
			return new Color32 (0,0,128,255);
		}
	}
	
	
	public static Color oldLace
	{
		get
		{
			return new Color32 (253,245,230,255);
		}
	}
	
	
	public static Color olive
	{
		get
		{
			return new Color32 (128,128,0,255);
		}
	}
	
	
	public static Color oliveDrab
	{
		get
		{
			return new Color32 (107,142,35,255);
		}
	}
	
	
	public static Color orange
	{
		get
		{
			return new Color32 (255,165,0,255);
		}
	}
	
	
	public static Color orangeRed
	{
		get
		{
			return new Color32 (255,69,0,255);
		}
	}
	
	
	public static Color orchid
	{
		get
		{
			return new Color32 (218,112,214,255);
		}
	}
	
	
	public static Color paleGoldenRod
	{
		get
		{
			return new Color32 (238,232,170,255);
		}
	}
	
	
	public static Color paleGreen
	{
		get
		{
			return new Color32 (152,251,152,255);
		}
	}
	
	
	public static Color paleTurquoise
	{
		get
		{
			return new Color32 (175,238,238,255);
		}
	}
	
	
	public static Color paleVioletRed
	{
		get
		{
			return new Color32 (219,112,147,255);
		}
	}
	
	
	public static Color papayaWhip
	{
		get
		{
			return new Color32 (255,239,213,255);
		}
	}
	
	
	public static Color peachPuff
	{
		get
		{
			return new Color32 (255,218,185,255);
		}
	}
	
	
	public static Color peru
	{
		get
		{
			return new Color32 (205,133,63,255);
		}
	}
	
	
	public static Color pink
	{
		get
		{
			return new Color32 (255,192,203,255);
		}
	}
	
	
	public static Color plum
	{
		get
		{
			return new Color32 (221,160,221,255);
		}
	}
	
	
	public static Color powderBlue
	{
		get
		{
			return new Color32 (176,224,230,255);
		}
	}
	
	
	public static Color purple
	{
		get
		{
			return new Color32 (128,0,128,255);
		}
	}
	
	
	public static Color red
	{
		get
		{
			return new Color32 (255,0,0,255);
		}
	}
	
	
	public static Color rosyBrown
	{
		get
		{
			return new Color32 (188,143,143,255);
		}
	}
	
	
	public static Color royalBlue
	{
		get
		{
			return new Color32 (65,105,225,255);
		}
	}
	
	
	public static Color saddleBrown
	{
		get
		{
			return new Color32 (139,69,19,255);
		}
	}
	
	
	public static Color salmon
	{
		get
		{
			return new Color32 (250,128,114,255);
		}
	}
	
	
	public static Color sandyBrown
	{
		get
		{
			return new Color32 (244,164,96,255);
		}
	}
	
	
	public static Color seaGreen
	{
		get
		{
			return new Color32 (46,139,87,255);
		}
	}
	
	
	public static Color seaShell
	{
		get
		{
			return new Color32 (255,245,238,255);
		}
	}
	
	
	public static Color sienna
	{
		get
		{
			return new Color32 (160,82,45,255);
		}
	}
	
	
	public static Color silver
	{
		get
		{
			return new Color32 (192,192,192,255);
		}
	}
	
	
	public static Color skyBlue
	{
		get
		{
			return new Color32 (135,206,235,255);
		}
	}
	
	
	public static Color slateBlue
	{
		get
		{
			return new Color32 (106,90,205,255);
		}
	}
	
	
	public static Color slateGray
	{
		get
		{
			return new Color32 (112,128,144,255);
		}
	}
	
	
	public static Color snow
	{
		get
		{
			return new Color32 (255,250,250,255);
		}
	}
	
	
	public static Color springGreen
	{
		get
		{
			return new Color32 (0,255,127,255);
		}
	}
	
	
	public static Color steelBlue
	{
		get
		{
			return new Color32 (70,130,180,255);
		}
	}
	
	
	public static Color tan
	{
		get
		{
			return new Color32 (210,180,140,255);
		}
	}
	
	
	public static Color teal
	{
		get
		{
			return new Color32 (0,128,128,255);
		}
	}
	
	
	public static Color thistle
	{
		get
		{
			return new Color32 (216,191,216,255);
		}
	}
	
	
	public static Color tomato
	{
		get
		{
			return new Color32 (255,99,71,255);
		}
	}
	
	
	public static Color turquoise
	{
		get
		{
			return new Color32 (64,224,208,255);
		}
	}
	
	
	public static Color violet
	{
		get
		{
			return new Color32 (238,130,238,255);
		}
	}
	
	
	public static Color wheat
	{
		get
		{
			return new Color32 (245,222,179,255);
		}
	}
	
	
	public static Color white
	{
		get
		{
			return new Color32 (255,255,255,255);
		}
	}
	
	
	public static Color whiteSmoke
	{
		get
		{
			return new Color32 (245,245,245,255);
		}
	}
	
	
	public static Color yellow
	{
		get
		{
			return new Color32 (255,255,0,255);
		}
	}
	
	
	public static Color yellowGreen
	{
		get
		{
			return new Color32 (154,205,50,255);
		}
	}
	
	


}

