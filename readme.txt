---------------------------------------------------------------------------------------------------
! Name: Eve Galaxy Viewer
! Version:	0.6.1
! System:	Windows PC with Oculus Rift DK2
! Author:	Erik Kalkoken
----------------------------------------------------------------------------------------------------
! History:
! 2016-02-18 v0.6.1 Performance improvements
! 2016-02-17 v0.6.0 Migrated to Unity 5 and SDK 0.8.0
----------------------------------------------------------------------------------------------------

1. Summary
----------------------------------------------

Eve Starmap VR is an application for the Oculus Rift showing the galaxy of the popular science fiction MMORG Eve Online in virtual reality. The user is able to see over 7.500 solar systems and jump connections in 3D space and fly around freely to explore the galaxy. Additonal information about the solar systems like security status, regions or factions can be activated through an on-screen menu.

Features:
- Shows all star systems with their jump connections of the Eve Online galaxy in space (excluding WH space)
- Solar systems are shown with selectable color schemes: security status, factions, regions, star color
- Names of nearest solar systems are shown in space
- Region and faction names are displayed in space
- User can switch between options in menu (e.g. color scheme for solar systems)
- User can move freely using a xbox controller or the keyboard
- Works on both Oculus Rifs and normal screen
- Atmospheric background music is played

2. About this demo
----------------------------------------------
I am an active Eve Online player so one of my main motivations was to see the Eve galaxy in VR. As far as I know this is the first demo to provide this. Since this is my first project with Unity it was also a great opportunity to learn about the tool and scripting in C#. 

I am planning to further improve this demo in the future and would very much welcome your feedback.

3. Controls
----------------------------------------------
This demo can be operated with keyboard or game controller. The game controller is recommended for best user experience.

Keyboard
------------------
ESC			Quit the demo
R			Recenter the camera
W		 	Forward
S			Backward
A			Left
D			Right
C			Up
Y			Down
Q			Yaw left
E			Yaw right
Shift			Accelerate
TAB			Open/close the option menu
Up/Down			Switch between items
Left/Right		Switch between options
ENTER			Choose current option

Game controller (XBox type)
------------------
BACK			Recenter the camera
Left-Stick Up	 	Forward
Left-Stick Down		Backward
Left-Stick Left		Left
Left-Stick Right	Right
Right-Stick Up		Up
Right-Stick Down	Down
Right-Stick Left	Yaw left
Right-Stick Right	Yaw right
Left-Shoulder Button	Accelerate
START			Open/close the option menu
D-PAD Up/Down		Switch between items
D-PAD Left/Right	Switch between options
A-Button		Choose current option
B-Button		Close the option menu

4. Performance
----------------------------------------------
The demo has been build with performance in mind and is running with smooth 75Hz most of the time on the reference system. 

Please be aware though that judder will most likely occur in situation with very high amount of objects, e.g. when displaying the complete universe with all 7.500+ solar systems and 14.000+ jump connections on screen.

While generating the galaxy on startup the screen will freeze. This is normal behavior - just wait a few seconds until the galaxy is ready. This takes about 5-6 seconds on the reference system.

5. Reference system
----------------------------------------------
For reference this demo has been developed and tested with the following system configuration:
- Windows 7 64-Bit
- Intel i7-4770K, 16 GB RAM
- Oculus Rift DK2
- NVIDIA GTX 970
- Oculus PC SDK 0.8.0 Beta
- Unity 5.3.2 Personal Edition

6. Sources
----------------------------------------------
Data - Eve Online Static Data Export and based on Phoebe_1.0.
Music - "CYAN 010 | Smooth Genestar - The Source" by CYAN MUSIC. It can be found here: https://soundcloud.com/cyan-music/smooth-genestar-the-source

7. Legal
----------------------------------------------
All data used in this demo belongs to CCP and is used in accordance to the "Developer License Agreement" as defined on https://developers.eveonline.com/resource/license-agreement.
For the data the following applies: © 2014 CCP hf. All rights reserved. "EVE", "EVE Online", "CCP", and all related logos and images are trademarks or registered trademarks of CCP hf. 

The music is licenced under CC BY NC SA. Please see link for licence details: http://creativecommons.org/licenses/by-nc-nd/3.0/

This application is licende under the MIT licence - with the exception of material owned by CCP and CYAN MUSIC as mentioned above. See attache licence file for details.


8. Contact
----------------------------------------------
For any questions regarding this demo please feel free to contact the author.

Please see the contact details below:

In-game:	Erik Kalkoken
Email:		erik.kalkoken@gmail.com
Facebook:	Erik Kalkoken
Website:	http://kalkoken.altervista.org/ (including link to download exe)
Blog:		http://kalkoken.altervista.org/blog/


Erik Kalkoken
February 2016
