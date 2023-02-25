
Spreadsheet Application
Authors: Conner Fisk, Ethan Bowles

A spreadsheet application which allows the user to create a new spreadsheet, open an existing one, and save a current one in the application. The user can use cells 
and variables (if the variables have a value) of format A1 through Z99 to store specific formulas or values and create desired calculations.

How to use:
	Input your values into desired cell.
	Be sure to either click to another cell or hit enter/return to store your value you just entered.
	For Formulas, be sure to add '=' before your entry.
	If you would like to copy and paste a cell into another:
		select your cell you want to copy, click the "Copy" button, then select the cell you want to paste the contents into and click the "Paste" button.
	To save, enter your desired directory in the specified entry box.
	Be sure to save before closing, resetting, or loading a new spreadsheet.
	    
Unique Functionalities:
Store on click away -> Added the ability to store the contents in the cell when clicked off. This allows the user to, for example, enter a value into cell B2, click 
onto cell C2, and have the B2 value stored without having to hit return/enter. This creates a much more user friendly experience which allows for more fluid workflow.

Copy and Paste buttons -> Added two buttons to the top of the spreadsheet application which allows the user to copy the contents of the current selected cell and then 
paste it into another cell. To do this, select your cell you want to copy, click the "Copy" button, then select the cell you would like to paste the contents into and 
click the "Paste" button.


Major error we encountered:
	A major error we occured on the first day (Monday Oct 17) of working was a git merge conflict which could not be resolved. The process would not allow us 
	to "abort" it and forced us to continue working on one computer as we could not push or pull from the git. We went to get help from TAs the following day 
	and Thursday, but no solution was found. We eventually found a fix, but due to this git glitch, we did not have specific commits for each part of the code 
	we created. The commits that would have been added are stated below.
	
	Entry:
		Added README file to Solution
			10/17/2022

	GITHUB ERROR ENCOUNTERED:
		Merge conflict created an error and could not exit out of the process. Abort would not work.
			10/17/2022

	Code Update:
		Added boxes above the spreadsheet to display the cell name, cell contents, and cell value.
			10/17/2022 

	Code Update:
		Added background spreadsheet and functionality to the cell name, cell contents, and cell value boxes.
			10/17/2022

	Bug Fix:
		Added '=' before a formula entry.
			10/17/2022
	
	Code Update:
		Added functionality to update dependent cells when a cell was changed.
			10/19/2022

	Code Update:
		Added open button functionality which can open a spreadsheet file.
			10/19/2022
			
	Code Update:
		Added a "Info" button that tells the user how to use the spreadsheet.
			10/19/2022

	Code Update:
		Added save button functionality.
			10/20/2022

	Code Update:
		Added functionality which showed error pop-ups when appropriate and informs the user about misuse.
			10/20/2022

	Code Update:
		Added FormulaError's reason to the cell's value if there is a Formula Error.
			10/20/2022

	Code Update:
		Added a help menu which tells the user how to use the spreadsheet.
			10/20/2022
			
	Special Feature Implemented:
		Store on click away. Allows the user to enter contents into a cell, then click to another cell without 
		needing to hit enter/return. The contents in the original cell will be stored.
			10/21/2022
			
	Code Update:
		Implemented functionality for the "New" button.
			10/21/2022
	
	Special Feature Implemented:
		Copy and Paste Button. Allows the user to copy and paste one cell to another.
			10/21/2022
			
	Code Update:
		Implemented functionality for the save. The spreadsheet will no longer display an "override" message more 
		than once for the same file.
			10/21/2022
			
	Code Update:
		Implemented code that allows the user to override the "unsaved changes" message which occurs.
			10/21/2022
