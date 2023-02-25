//Written by Ethan Bowles
//Date: September 23, 2022
//Version 2: September 30,2022
//Spreadsheet class

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection.Metadata;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.VisualBasic;
using Newtonsoft.Json;
using SpreadsheetUtilities;
using static System.Net.Mime.MediaTypeNames;

namespace SS
{
    /// <summary>
    /// Class used to represent a spreadsheet object
    /// Has the ability to add, replace and remove things from cells
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class Spreadsheet : AbstractSpreadsheet
    {

        //Private Fields
        //One to store all cells that are not empty and another to store the
        //dependancy graph
        [JsonProperty(PropertyName = "cells")]
        private Dictionary<string, Cell> NonEmptycells;
        private DependencyGraph dg;

        /// <summary>
        /// 0 Parameter constructor that sets the validator to true
        /// the normailizer to stay the same
        /// and the version to default
        /// Makes a new cells list and dependancy graph
        /// </summary>
        public Spreadsheet() :
            this(s => true, s => s, "default")
        {
            NonEmptycells = new Dictionary<string, Cell>();
            dg = new DependencyGraph();
        }

        /// <summary>
        /// 3 Parameter constructor that sets the validator to the user given
        /// the normailizer to the normailzer given by the user
        /// and the version to the string given
        /// Makes a new cells list and dependancy graph
        /// </summary>
        public Spreadsheet(Func<string, bool> isValid, Func<string, string> normalize, string version) :
            base(isValid, normalize, version)
        {
            NonEmptycells = new Dictionary<string, Cell>();
            dg = new DependencyGraph();
        }

        /// <summary>
        /// 4 Parameter constructor that sets the validator to the user given
        /// the normailizer to the normailzer given by the user
        /// and the version to the string given
        /// Makes a new cells list and dependancy graph
        /// 
        /// Furthermore a file path is given which is used to construct a spreadsheet
        /// If the file contains json that results in a spreadsheet it will construct one
        /// Errors are thrown if
        /// If the version of the saved spreadsheet does not match the version parameter provided to the constructor
        /// If any of the names contained in the saved spreadsheet are invalid
        /// If any invalid formulas or circular dependencies are encountered
        /// If there are any problems opening, reading, or closing the file(such as the path not existing)
        /// </summary>
        public Spreadsheet(string filePath, Func<string, bool> isValid, Func<string, string> normalize, string version) :
           base(isValid, normalize, version)
        {
            NonEmptycells = new Dictionary<string, Cell>();
            dg = new DependencyGraph();

            try
            {
                //Constructs the spreadsheet
                string message = File.ReadAllText(filePath);
                Spreadsheet? list = JsonConvert.DeserializeObject<Spreadsheet>(message);

                if (list == null)
                    throw new NullReferenceException();

                if (list.Version != Version)
                    throw new SpreadsheetReadWriteException("Version does not match");

                foreach (string name in list.NonEmptycells.Keys)
                {
                    SetContentsOfCell(name, list.NonEmptycells[name].StringForm);
                }
            }
            catch (Exception)
            {
                //Thrown if any errors
                throw new SpreadsheetReadWriteException("Unable to create spreadsheet from file");
            }
        }

        /// <summary>
        /// Refrence that is set to true if the spreadsheet had been changed in any way
        /// </summary>
        public override bool Changed { get; protected set; }

        /// <summary>
        /// Implemented get cell contents method
        /// Gets the contents, not the value, of the cell specified by the name
        /// If the name doesnt exist then an InvalidNameException is thrown
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        /// <exception cref="InvalidNameException"></exception>
        public override object GetCellContents(string name)
        {
            name = Normalize(name);
            //Invalid name check
            if (!CheckCellName(name) || !IsValid(name))
                throw new InvalidNameException();

            //If the cell is empty return blank string
            if (NonEmptycells.ContainsKey(name))
            {
                return NonEmptycells[name].Contents;
            }
            else
            {
                return "";
            }
        }

        /// <summary>
        /// Implemented get cell contents method
        /// Gets the value, not the contents, of the cell specified by the name
        /// If the name doesnt exist then an InvalidNameException is thrown
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        /// <exception cref="InvalidNameException"></exception>
        public override object GetCellValue(string name)
        {
            name = Normalize(name);

            //Invalid name check
            if (!CheckCellName(name) || !IsValid(name))
                throw new InvalidNameException();

            //If the cell is empty return blank string
            if (NonEmptycells.ContainsKey(name))
            {
                return NonEmptycells[name].Value;
            }
            else
            {
                return "";
            }
        }

        /// <summary>
        /// Implementation of the get names of all nonempty cells method
        /// This will return all the non empty cells in the sheet
        /// </summary>
        /// <returns></returns>
        public override IEnumerable<string> GetNamesOfAllNonemptyCells()
        {
            //Lists through the nonempty cells and adds to list
            List<string> names = new List<string>();
            foreach (string name in NonEmptycells.Keys)
            {
                names.Add(name);
            }
            return names;
        }

        /// <summary>
        /// Method that is usable by the user that calls one of the three submethods below
        /// Used to simplift the code so the user only needs to call one type of method
        /// </summary>
        /// <param name="name"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        /// <exception cref="InvalidNameException"></exception>
        public override IList<string> SetContentsOfCell(string name, string content)
        {
            IList<string> cells;
            name = Normalize(name);
            //Invalid name check
            if (!CheckCellName(name) || !IsValid(name))
                throw new InvalidNameException();

            //Checks if formula
            if (content.Equals(""))
            {
                cells = SetCellContents(name, content);
            }
            else if (content[0] == '=')
            {
                cells = SetCellContents(name, new Formula(content.Substring(1), Normalize, CheckVar));
            }
            else
            {
                //Checks if double or string
                if (Double.TryParse(content, out double val))
                {
                    cells = SetCellContents(name, val);
                }
                else
                {
                    cells = SetCellContents(name, content);
                }
            }

            //Recalculates all required cells
            foreach (string cell in cells)
            {
                NonEmptycells[cell].Recalculate(lookup);
            }

            Changed = true;
            return cells;
        }

        /// <summary>
        /// Implementation of the set cells method for numbers
        /// Sets the contents of a cell, and updates all the correct dependencies in the graph
        /// Throws an excepetion if the name is not valid
        /// </summary>
        /// <param name="name"></param>
        /// <param name="number"></param>
        /// <returns></returns>
        /// <exception cref="InvalidNameException"></exception>
        protected override IList<string> SetCellContents(string name, double number)
        {
            //Updates dependacies and adds
            if (NonEmptycells.ContainsKey(name))
            {
                dg.ReplaceDependents(name, new List<string>());
                NonEmptycells[name] = new Cell(name, number);
            }
            else
            {
                NonEmptycells.Add(name, new Cell(name, number));
            }

            return GetCellsToRecalculate(name).ToList();
        }

        /// <summary>
        /// Implementation of the set cells method for strings
        /// Sets the contents of a cell, and updates all the correct dependencies in the graph
        /// Throws an excepetion if the name is not valid
        /// </summary>
        /// <param name="name"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        /// <exception cref="InvalidNameException"></exception>
        protected override IList<string> SetCellContents(string name, string text)
        {
            //Updates dependacies and adds
            if (NonEmptycells.ContainsKey(name))
            {
                dg.ReplaceDependents(name, new List<string>());
                NonEmptycells[name] = new Cell(name, text);
            }
            else
            {
                NonEmptycells.Add(name, new Cell(name, text));
            }

            return GetCellsToRecalculate(name).ToList();
        }

        /// <summary>
        /// Implementation of the set cells method for formulas
        /// Throws an excepetion if the name is not valid
        /// Sets the contents of the cell to the value, and upates all dependancies
        /// If there is circular exception the method throws and restores the prior graph and sheet
        /// </summary>
        /// <param name="name"></param>
        /// <param name="formula"></param>
        /// <returns></returns>
        /// <exception cref="InvalidNameException"></exception>
        /// <exception cref="CircularException"></exception>
        protected override IList<string> SetCellContents(string name, Formula formula)
        {
            //Gets a list of the variables in the formula
            List<string> vars = formula.GetVariables().ToList();

            //Checks to see if circular exeption is thrown
            //If it is it removes the dependacies added to the graph and throws
            try
            {
                foreach (string var in vars)
                    dg.AddDependency(name, var);

                GetCellsToRecalculate(name).ToList();

                if (NonEmptycells.ContainsKey(name))
                {
                    dg.ReplaceDependents(name, vars);
                    NonEmptycells[name] = new Cell(name, formula, lookup);
                }
                else
                {
                    NonEmptycells.Add(name, new Cell(name, formula, lookup));
                }

                foreach (string var in vars)
                    dg.AddDependency(name, var);

                return GetCellsToRecalculate(name).ToList();
            }
            catch (CircularException)
            {
                foreach (string var in vars)
                    dg.RemoveDependency(name, var);

                throw new CircularException();
            }
        }

        /// <summary>
        /// Helper method used in the abstract GetCellsToRecalculate
        /// Returns the cells that are directly dependent on the cell name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        protected override IEnumerable<string> GetDirectDependents(string name)
        {
            name = Normalize(name);
            List<string> dependees = new List<string>();
            foreach (string dep in dg.GetDependees(name))
            {
                if (!dependees.Contains(dep))
                {
                    dependees.Add(dep);
                }
            }
            return dependees;
        }

        /// <summary>
        /// Helper method to check the cell name for valid syntax
        /// </summary>
        /// <param name="Name"></param>
        /// <returns></returns>
        private bool CheckCellName(string Name)
        {
            string str1 = new string(Name.Where(c => char.IsDigit(c)).ToArray());
            string str2 = new string(Name.Where(c => char.IsLetter(c)).ToArray());

            if (!Name.Equals(str2 + str1))
                return false;

            Regex re = new Regex(@"([a-zA-Z]+)(\d+)");
            Match result = re.Match(Name);

            str1 = result.Groups[1].Value;
            str2 = result.Groups[2].Value;

            if (str1.Length == 0 || str2.Length == 0)
                return false;

            return true;
        }

        /// <summary>
        /// Implementation of the save method from the abstract class
        /// Serialized the object and saves it to the file if the file name is valid
        /// </summary>
        /// <param name="filename"></param>
        /// <exception cref="SpreadsheetReadWriteException"></exception>
        public override void Save(string filename)
        {
            try
            {
                string message = JsonConvert.SerializeObject(this, Newtonsoft.Json.Formatting.Indented);
                File.WriteAllText(filename, message);
                Changed = false;
            }
            catch (Exception)
            {
                throw new SpreadsheetReadWriteException("Problem with file in the save method");
            }
        }

        /// <summary>
        /// Helper method used to look up the value of variables for the formula class
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        private double lookup(string token)
        {
            if (NonEmptycells.ContainsKey(token))
            {
                if (NonEmptycells[token].Value is Double)
                {
                    Double.TryParse(NonEmptycells[token].Value.ToString(), out double val);
                    return val;
                }
                else
                {
                    throw new ArgumentException();
                }
            }
            else
            {
                throw new ArgumentException();
            }
        }

        /// <summary>
        /// Helper method passed into the formula class to see if the 
        /// variables are valid for the spreadsheet and isvalid delegate
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        private bool CheckVar(string s)
        {
            if (CheckCellName(s) && IsValid(s))
                return true;
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Cell class
        /// This class is used to represent a cell object
        /// This is used in the spreadsheet
        /// It holds a name, contents and value associated to the cell
        /// </summary>
        private class Cell
        {
            /// <summary>
            /// Fields for name, contents and value
            /// ADDED value for stringform
            /// </summary>
            [JsonIgnore]
            public string Name { get; private set; }
            [JsonIgnore]
            public object Contents { get; private set; }
            [JsonIgnore]
            public object Value { get; private set; }
            [JsonProperty(PropertyName = "stringForm")]
            public string StringForm { get; private set; }

            public Cell()
            {
                Name = "";
                Contents = "";
                StringForm = "";
                Value = "";
            }

            /// <summary>
            /// Constructor for the cell class the sets up a string content and value
            /// </summary>
            /// <param name="name"></param>
            /// <param name="contents"></param>
            public Cell(string name, string contents)
            {
                Name = name;
                Contents = contents;
                StringForm = contents;
                Value = contents;
            }

            /// <summary>
            /// Constructor for the cell class the sets up a double content and value
            /// </summary>
            /// <param name="name"></param>
            /// <param name="contents"></param>
            public Cell(string name, double contents)
            {
                Name = name;
                Contents = contents;
                StringForm = contents.ToString();
                Value = contents;
            }

            /// <summary>
            /// Constructor for the cell class the sets up a formula content and double value
            /// </summary>
            /// <param name="name"></param>
            /// <param name="contents"></param>
            public Cell(string name, Formula contents, Func<string, double> lookup)
            {
                Name = name;
                Contents = contents;
                StringForm = "=" + contents.ToString();
                Value = contents.Evaluate(lookup);
            }

            /// <summary>
            /// Method that recalucted the given cells value
            /// </summary>
            /// <param name="lookup"></param>
            public void Recalculate(Func<string, double> lookup)
            {
                if (this.Contents.GetType() == typeof(Formula))
                {
                    Formula formula = (Formula)this.Contents;
                    Value = formula.Evaluate(lookup);
                }
            }
        }
    }
}
