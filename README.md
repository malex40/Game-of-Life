---
title: Life
author: Matt Grant - <n10799982>
date: 17/09/2020
---

# LIFE

## Build Instructions
---------------------
Open the solutions file titled **Life.sln** in Visual Studios. Naviagate to the **Build** tab of the menu bar. In here you will find a button for **Build Solution**.  

To make a *Release* build, Select **Configuration Manager** from within the **Build** tab and change the projects configuration from *Debug* to *Release*


## Usage 
--------
### **Opening a Command Prompt**
To run this program from the command line, it is easiest to navigate to the folder containing the build with Windows File Explorer.  

The default path for this programs build should be found in:  
"Path\To\CAB201_2020S2_ProjectPartA_n10799982\Life\Life\bin\Release\netcoreapp3.1

Once inside the folder containging the build, there should be a program file named **life.dll**. Select the location bar along the top and type **cmd**, this will open a Command Prompt in the location of the folder.

### **Executing the program**
Default syntax to run the simulation from Command Prompt is as follows:
> \>dotnet life.dll \[\<arguments>\]  

When this command is execute the simulation will alert the user with the status of the argument processing and show the current setting that the simulation will run with. If there is any issues processing arguments the console will alert the user of this and will reset only the incorrect arguments to defaults. The simulation will then wait for a key press of **only** the space-bar before beginning.

The \[\<arguments>\] are **optional** and can be used to change the simulation settings from the defaults. Multiple arguments can be used. Each argument will be described below.

#### **Dimensions**
The size of the game board can be set with the use of the  --dimensions argument. This argument must be followed by two integers, one for rows and one for columns, both between 4 and 48 inclusive. The default for this argument is 16 rows and 16 columns.  
>Example: --dimensions \<rows> \<columns>

#### **Periodic**
The --periodic argument will determine whether the game board has a hard border or if it wraps around. This argument takes no parameters. The default for this argument is False. (ie. the game board has a hard border.).
>Example: --periodic

#### **Random Factor**
The --random argument determines the probability that any cell will be alive for the first generation. This probability is for each individual cell alone, not all cells as a whole. (ie. 50% that a certain cell will be alive, **NOT** 50% of all cells alive.) This argument must be followed by one floating point number between 0 and 1 inclusive. The default for this argument is 0.5 (50%).
>Example: --random \<probability>

#### **Input File**
The --seed argument allows the user to specify what cells will be alive in the first generation. This argument must be followed by a valid file path to a file with a *.seed* file extension. The default for this argument is no input file used.  
>Example: --seed \<fileName>


#### **Generations**
The --generations argument determines how many generations the simulation will run. This argument must be followed by an integer greater than 0. The default for this argument is 50 generations.
>Exampled: --generations \<number>

#### **Refresh Rate**
The --max-update argument determines the refresh rate of the simulation in updates per second. This argument must be followed by a floating point number between 1 and 30 inclusive. The default for this argument is 5 updates per second

>Example: --max-update \<updateRate>

#### **Step Mode**
The --step argument enables the user to wait for the space-bar to be pressed to advance to the next generation. This argument takes no parameters. The default for this argument is False (ie the simulation will not run in step mode) 
>Example: --step

## Notes 
--------

- It is recommended that the arguments be handed to the simulation in the order they are detailed in above, however the simulation will still function correctly despite the order of arguments.
- The --seed argument will take precedence over the --random argument.
- The --seed argument will only initialise cells within the size of --dimensions. If there are cells specified in the file that are outside the scope of the board they will be excluded.
- The --step argument will still be restricted by the --max-update argument. This stops the user from skipping too many generations with a single press of the space-bar key.

