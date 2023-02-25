// Written by Ethan Bowles & Conner Fisk
// Date: Oct 19, 2022
// Version 1.1:  Oct 21, 2022
// GUI class for the Spreadsheet

using SpreadsheetUtilities;
using SS;
using System.Text.RegularExpressions;

namespace SpreadsheetGUI;

/// <summary>
/// This class holds all methods used for create a GUI for a spreadsheet.
/// </summary>
public partial class MainPage : ContentPage
{
    //Fields
    private Spreadsheet ss; //Spreadsheet object
    private Dictionary<int, string> letterMap = new Dictionary<int, string>(); //Dictionary used to get a letter from the row number.
    private Dictionary<string, int> numberMap = new Dictionary<string, int>(); //Dictionary used to get the row number from a letter.
    private int lastRow, lastCol; //Used to store last values
    private string CopiedContent; //Used to store the contnet of the copied cell
    private bool PathChanged; //Keeps track on whether or not the FilePath has changed

    /// <summary>
    /// Constructor for the spreadsheet GUI.
    /// </summary>
	public MainPage()
    {
        InitializeComponent();

        // Initialize the default values needed to create the GUI of the spreadsheet with the specified characteristics.
        spreadsheetGrid.SelectionChanged += displaySelection;
        spreadsheetGrid.SetSelection(0, 0);
        spreadsheetGrid.BackgroundColor = Colors.AliceBlue;

        // Initialize lastCol, lastRow, and CopiedContent to 'default' values.
        lastCol= 0;
        lastRow= 0;
        CopiedContent = null;

        //Created the new spreadsheet object with the correct cell requirements
        Regex match = new Regex("^[a-zA-Z][1-9][0-9]{0,2}$");
        ss = new Spreadsheet(x => match.IsMatch(x), x => x.ToUpper(), "ps6");

        //Constructs two new list making it easier to get cell names and columns
        for (int i = 0; i < 26; i++)
        {
            letterMap.Add(i, ((char)(i + 65)).ToString());
        }
        for (int i = 0; i < 26; i++)
        {
            numberMap.Add(((char)(i + 65)).ToString(), i);
        }
    }

    /// <summary>
    /// This method holds the needed code in order to display a selected cell on the grid.
    /// </summary>
    /// <param name="grid"></param>
    private void displaySelection(SpreadsheetGrid grid)
    {
        //Gets the column and row number of the selected cell.
        spreadsheetGrid.GetSelection(out int col, out int row);

        //Gets the value at the given cell at col, row
        spreadsheetGrid.GetValue(col, row, out string value);

        //Calls the helper which stores the value when clicked to a different cell.
        CompleteValueHelper(lastRow, lastCol);
        //Sets the current selected cell.
        spreadsheetGrid.SetSelection(col, row);

        //Gets the cell name from the retrieved col and row values.
        string cellName = letterMap[col] + (row + 1).ToString();

        //Sets the name of the selected cell to cellName
        Name.Text = cellName;

        //If the cell is a Formula, put an '=' in front of it before setting the Contents.
        if (ss.GetCellContents(cellName) is Formula)
        {
            Contents.Text = "=" + ss.GetCellContents(cellName).ToString();
        }
        else //If it is not a Formula, simply, set the Contents.
        {
            Contents.Text = ss.GetCellContents(cellName).ToString();
        }
        //Checks if the value of the cell is a FormulaError
        if (ss.GetCellValue(cellName).GetType() == typeof(FormulaError))
        {
            //Stores the FormulaError and sets the cell's value text to the FormulaError's reason.
            FormulaError fe = (FormulaError)ss.GetCellValue(cellName);
            Value.Text = fe.Reason;
            //Set the value text to Red to show the error more clearly to the user.
            Value.TextColor = Colors.Red;
        }
        else //If it is not a FormulaError, simply set the Value.Text to the cell's value as a string with the color black.
        {
            Value.Text = ss.GetCellValue(cellName).ToString();
            Value.TextColor = Colors.Black;
        }
    }

    /// <summary>
    /// Method that gets called when the contents have changed in a cell.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OnContentsChanged(Object sender, EventArgs e)
    {
        //Gets the current column and row of the selected cell.
        spreadsheetGrid.GetSelection(out int col, out int row);
        //Sets lastRow and last Col to row and col.
        lastRow = row;
        lastCol = col;
        //Updates the value of the selected cell at col, row.
        spreadsheetGrid.SetValue(col, row, Contents.Text);
    }

    /// <summary>
    /// Method that gets called when the contents have completed (stored) in a cell.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OnContentsCompleted(Object sender, EventArgs e)
    {

        //Gets the current column and row of the selected cell.
        spreadsheetGrid.GetSelection(out int col, out int row);
        //Updates the value of the selected cell at col, row.
        spreadsheetGrid.SetSelection(col, row);

        //Calls the helper method used to store a value when the user clicks to another cell.
        CompleteValueHelper(row, col);
    }

    /// <summary>
    /// Helper method that is used to store the value in a given cell when the user clicks to a different cell.
    /// </summary>
    /// <param name="row"></param>
    /// <param name="col"></param>
    private void CompleteValueHelper(int row, int col)
    {
        //Get the cell name from the given row and col.
        string cellName = letterMap[col] + (row + 1).ToString();

        //If the user just clicks on the cell without entering anything, do nothing.
        if(Contents.Text == "")
            return;

        //Sets the contents of the cell in the spreadsheet
        List<string> list = new();
        try
        {
            list = ss.SetContentsOfCell(cellName, Contents.Text).ToList();
        }
        catch (FormulaFormatException) //Displays an error pop up when a FormulaFormatException is caught.
        {
            DisplayAlert("Error", "There was a FormulaFormatException", "OK");
        }
        catch (CircularException) //Displays an error pop up when a CircularException is caught.
        {
            DisplayAlert("Error", "There was a Circular Execption in you Spreadsheet", "OK");
        }

        //Updates all the cells that need to be updated due to dependency.
        foreach (string cell in list)
        {
            //Gets the cells column and row .
            int colNum = numberMap[cell[0].ToString()];
            int.TryParse(cell.Substring(1), out int rowNum);
            rowNum--;

            //Set the selection to the column and row retrieved above.
            spreadsheetGrid.SetSelection(colNum, rowNum);

            //Checks if the value is a FormulaError
            if (ss.GetCellValue(cell).GetType() == typeof(FormulaError))
            {
                //Stores the FormulaError and sets the cell's value text to the FormulaError's reason.
                FormulaError fe = (FormulaError)ss.GetCellValue(cell);
                Value.Text = fe.Reason;
                //Set the value text to Red to show the error more clearly to the user.
                Value.TextColor = Colors.Red;
                //Set the value of the cell to the FormulaError's reason.
                spreadsheetGrid.SetValue(colNum, rowNum, fe.Reason);
            }
            else //If it is not a FormulaError, simply set the Value.Text to the cell's value as a string with the color black, then set the value of the cell.
            {
                Value.Text = ss.GetCellValue(cell).ToString();
                Value.TextColor = Colors.Black;
                spreadsheetGrid.SetValue(colNum, rowNum, ss.GetCellValue(cell).ToString());
            }
        }
        //Set the selection of the current cell.
        spreadsheetGrid.SetSelection(col, row);
    }

    /// <summary>
    /// Method that gets called when the "Open" button is clicked in the GUI.
    /// This can load a .sprd file into the GUI.
    /// </summary>
    private async void OpenClicked(Object sender, EventArgs e)
    {
        //If the spreadsheet has unsaved changes, display a pop up message warning the user.
        if (ss.Changed)
        {
            bool answer = await DisplayAlert("Unsaved Changes", "There are unsaved changed in the current file. Are you sure you want to open a new file?",
                "Open", "Cancel");
            if (!answer) //If the user hits cancel, do nothing.
            {
                return;
            }
        }
        try
        {
            //Retrieve the fileResult.
            FileResult fileResult = await FilePicker.Default.PickAsync();

            if (fileResult != null)
            {
                System.Diagnostics.Debug.WriteLine("Successfully chose file: " + fileResult.FileName);

                //Generate a new empty spreadsheet with the needed normalizer and validator.
                Regex match = new Regex("^[a-zA-Z][1-9][0-9]{0,2}$");
                ss = new Spreadsheet(fileResult.FullPath, x => match.IsMatch(x), x => x.ToUpper(), "ps6");
                spreadsheetGrid.Clear();
                Contents.Text = "";

                //Get all of the cells from the selected file.
                List<string> list = ss.GetNamesOfAllNonemptyCells().ToList();

                //For every cell from the selected file, add it to the spreadsheet.
                foreach (string cell in list)
                {
                    int colNum = numberMap[cell[0].ToString()];
                    int.TryParse(cell.Substring(1), out int rowNum);
                    rowNum--;
                    spreadsheetGrid.SetSelection(colNum, rowNum);
                    spreadsheetGrid.SetValue(colNum, rowNum, ss.GetCellValue(cell).ToString());
                    Value.Text = ss.GetCellValue(cell).ToString();
                    Value.TextColor = Colors.Black;
                }
            }
            else //If there was no fileResult, write a message.
            {
                Console.WriteLine("No file selected.");
            }
        }
        catch (Exception ex) //If an error occurred, tell the user.
        {
            Console.WriteLine("Error opening file:");
            Console.WriteLine(ex);
        }
    }

    /// <summary>
    /// Method that gets called when the "New" button is clicked in the GUI.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void NewClicked(Object sender, EventArgs e)
    {
        //If the spreadsheet has been edited since the last save, warn the user that there are unsaved changes.
        if (ss.Changed)
        {
            bool answer = await DisplayAlert("Unsaved Changes", "There are unsaved changed in the current file. Are you sure you want to create a new file?",
                "Open", "Cancel");
            if (!answer) //If the user hits cancel, return nothing.
            {
                return;
            }
            //If the user overrides the message, create a new spreadsheet.
            Regex match = new Regex("^[a-zA-Z][1-9][0-9]{0,2}$");
            ss = new Spreadsheet(x => match.IsMatch(x), x => x.ToUpper(), "ps6");
            spreadsheetGrid.Clear();
            Contents.Text = "";
        }
        else //Creates a new spreadsheet if the spreadsheet has not been edited since last save.
        {
            Regex match = new Regex("^[a-zA-Z][1-9][0-9]{0,2}$");
            ss = new Spreadsheet(x => match.IsMatch(x), x => x.ToUpper(), "ps6");
            spreadsheetGrid.Clear();
            Contents.Text = "";
        }
    }

    /// <summary>
    /// Method that gets called when the "Save" button is clicked in the GUI.
    /// This saves the current spreadsheet which the user is working on.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void SaveClicked(Object sender, EventArgs e)
    {
        try
        {
            //If the file path already exists, warn the user that they will override the existing file if desired.
            if (File.Exists(FilePath.Text))
            {
                if (PathChanged) //Makes sure the path is different than the previous saved path in order to stop the alert if it has already been saved to at least once
                {
                    bool answer = await DisplayAlert("File Exists", "There is a file with this name that already exists, would you like to override it?",
           "Override", "Cancel");
                    if (!answer) //If the user decies not to override the existing file, simply return.
                    {
                        return;
                    }
                }

            }
            //Save the file to the given path and set PathChanged to false.
            ss.Save(FilePath.Text);
            PathChanged = false;
        }
        catch (SpreadsheetReadWriteException) // If a SpreadsheetReadWriteException is caught, display an error pop up explaining that the given path is not valid.
        {
            await DisplayAlert("Incorrect Path Provided", "There was a problem with the file path provided." +
                "Either the path is incorrect or the wrong format of file is being used", "OK");
        }
    }


    /// <summary>
    /// Method that gets called when the "Info" button is clicked in the GUI.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void InfoClicked(Object sender, EventArgs e)
    {
        //Displays info to the user on how to properly use the spreadsheet.
        DisplayAlert("Spreadsheet user info.", "- Input your values into desired cell." +
            "\n- MAKE SURE either click to another cell or hit enter/return to store your value you just entered." +
            "\n- For Formulas, be sure to add '=' before your entry." +
            "\n- If you would like to copy and paste a cell into another, select your cell you want to copy," +
            " click the 'Copy' button, then select  the cell you want to paste the contents into and click" +
            " the 'Paste' button." +
            "\n- To save, enter your desired directory in the specified entry box." +
            "\n- Be sure to save before closing, resetting, or loading a new spreadsheet.", "Done");
    }

    /// <summary>
    /// This method is used to make sure the PathChanged bool is updated if the user puts in a new file path.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void FilePathChanged(Object sender, EventArgs e)
    {
        //Update PathChanged to true
        PathChanged = true;
    }

    /// <summary>
    /// This method is used when the "Copy" button is clicked.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void CopyClicked(Object sender, EventArgs e)
    {
        //Store the current contents into CopiedContent.
        CopiedContent = Contents.Text;
    }

    /// <summary>
    /// This method is used when the "Paste" button is clicked.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void PasteClicked(Object sender, EventArgs e)
    {
        if(CopiedContent != null) //Makes sure there is content that is copied.
        {
            //Set the contents in CopiedContent to the desired cell.
            spreadsheetGrid.GetSelection(out int col, out int row);
            Contents.Text = CopiedContent;

            //Call the helper method used to store a cell's value when the user clicks to another cell.
            CompleteValueHelper(row, col);
        }
    }
}
