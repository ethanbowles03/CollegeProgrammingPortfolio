// Skeleton written by Profs Zachary, Kopta and Martin for CS 3500
// Read the entire skeleton carefully and completely before you
// do anything else!

// Change log:
// Last updated: 9/8, updated for non-nullable types
// Sceleton Filled in by Ethan Bowles
// Sept. 16, 2022

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Text.RegularExpressions;

namespace SpreadsheetUtilities
{
    /// <summary>
    /// Represents formulas written in standard infix notation using standard precedence
    /// rules.  The allowed symbols are non-negative numbers written using double-precision 
    /// floating-point syntax (without unary preceeding '-' or '+'); 
    /// variables that consist of a letter or underscore followed by 
    /// zero or more letters, underscores, or digits; parentheses; and the four operator 
    /// symbols +, -, *, and /.  
    /// 
    /// Spaces are significant only insofar that they delimit tokens.  For example, "xy" is
    /// a single variable, "x y" consists of two variables "x" and y; "x23" is a single variable; 
    /// and "x 23" consists of a variable "x" and a number "23".
    /// 
    /// Associated with every formula are two delegates:  a normalizer and a validator.  The
    /// normalizer is used to convert variables into a canonical form, and the validator is used
    /// to add extra restrictions on the validity of a variable (beyond the standard requirement 
    /// that it consist of a letter or underscore followed by zero or more letters, underscores,
    /// or digits.)  Their use is described in detail in the constructor and method comments.
    /// </summary>
    public class Formula
    {
        private List<string> infixFormula;

        /// <summary>
        /// Creates a Formula from a string that consists of an infix expression written as
        /// described in the class comment.  If the expression is syntactically invalid,
        /// throws a FormulaFormatException with an explanatory Message.
        /// 
        /// The associated normalizer is the identity function, and the associated validator
        /// maps every string to true.  
        /// </summary>
        public Formula(String formula) :
            this(formula, s => s, s => true)
        {
            
        }

        /// <summary>
        /// Creates a Formula from a string that consists of an infix expression written as
        /// described in the class comment.  If the expression is syntactically incorrect,
        /// throws a FormulaFormatException with an explanatory Message.
        /// 
        /// The associated normalizer and validator are the second and third parameters,
        /// respectively.  
        /// 
        /// If the formula contains a variable v such that normalize(v) is not a legal variable, 
        /// throws a FormulaFormatException with an explanatory message. 
        /// 
        /// If the formula contains a variable v such that isValid(normalize(v)) is false,
        /// throws a FormulaFormatException with an explanatory message.
        /// 
        /// Suppose that N is a method that converts all the letters in a string to upper case, and
        /// that V is a method that returns true only if a string consists of one letter followed
        /// by one digit.  Then:
        /// 
        /// new Formula("x2+y3", N, V) should succeed
        /// new Formula("x+y3", N, V) should throw an exception, since V(N("x")) is false
        /// new Formula("2x+y3", N, V) should throw an exception, since "2x+y3" is syntactically incorrect.
        /// </summary>
        public Formula(String formula, Func<string, string> normalize, Func<string, bool> isValid)
        {
            IEnumerable<string> tokens = GetTokens(normalize(formula));
            foreach (string token in tokens)
            {
                if (isVariable(token))
                {
                    if (!isValid(token))
                    {
                        throw new FormulaFormatException("Invalid token in given expression");
                    }
                }
            }

            infixFormula = InfixCheckHelper(tokens.ToList<string>());
            infixFormula = numberNormalizer(infixFormula);
        }

        /// <summary>
        /// Normalized all of the numbers in the expression to be a consisten length
        /// This is done by converting the number to double form and then 
        /// back to string form
        /// This takes advantage of c# converstion methods
        /// </summary>
        /// <param name="infix"></param>
        /// The expression being passed in
        /// <returns></returns>
        private List<string> numberNormalizer(List<string> infix)
        {
            List<string> result = new List<string>();
            for (int i = 0; i < infix.Count; i++)
            {
                if (isNumber(infix[i]))
                {
                    Double.TryParse(infix[i], out double val);
                    result.Add(val.ToString());
                }
                else
                {
                    result.Add(infix[i]);
                }
            }
            return result;
        }
        
        /// <summary>
        /// Main helper method for the constructor
        /// Goes through and makes sure the infix expression passed meets all of 
        /// the requirement of an infix method
        /// Checks for syntactic issues and throws FormulaExceptions
        /// </summary>
        /// <param name="tokens"></param>
        /// The infix expression passed in the form of tokens
        /// <returns></returns>
        /// <exception cref="FormulaFormatException"></exception>
        /// Thrown when the formula doesnt meet the syntacitcal requirements
        private List<string> InfixCheckHelper(List<string> tokens)
        {
            int leftParanCount = 0;
            int rightParanCount = 0;

            //Checks all tokens are valid before validating and normalizing
            foreach (string token in tokens)
            {
                if (!isNumber(token) && !isVariable(token) && !isOperator(token) && !isParan(token))
                {
                    throw new FormulaFormatException("Unkown token contained in expression");
                }
            }
            //Checks if no tokens
            if(tokens.Count() == 0)
            {
                throw new FormulaFormatException("Expression given is empty");
            }
            //Checks the right paranthesis rule and paranthesis match
            foreach (string token in tokens)
            {
                if (isParan(token))
                {
                    if(token.Equals("("))
                        leftParanCount++;
                    else
                        rightParanCount++;
                }

                if (rightParanCount > leftParanCount)
                    throw new FormulaFormatException("Closing paranthesis used before opening");
            }
            if(leftParanCount != rightParanCount)
            {
                throw new FormulaFormatException("Opening anc Closing paranthesis counts dont match");
            }
            //Starting token check
            if (!isNumber(tokens[0]) && !isVariable(tokens[0]) && !tokens[0].Equals("("))
            {
                throw new FormulaFormatException("Expression does not start with a number, variable or open paranthesis");
            }
            //Ending token check
            if (!isNumber(tokens[tokens.Count() - 1]) && !isVariable(tokens[tokens.Count() - 1]) && !tokens[tokens.Count() - 1].Equals(")"))
            {
                throw new FormulaFormatException("Expression does not end with a number, variable or closing paranthesis");
            }
            //Any token following an opening paranthesis or operator must be a var or num or open paran
            for(int i = 0; i < tokens.Count(); i++)
            {
                if ((isOperator(tokens[i]) || tokens[i].Equals("(")) && i + 1 < tokens.Count())
                {
                    if(!isNumber(tokens[i + 1]) && !isVariable(tokens[i+1]) && !tokens[i + 1].Equals("("))
                    {
                        throw new FormulaFormatException("Operator/Paranthesis are not followed by value, var, or opening paranthesis");
                    }
                }
            }
            //Any token that immediately follows a number, a variable, or a closing parenthesis must be either an operator or a closing parenthesis
            for (int i = 0; i < tokens.Count(); i++)
            {
                if ((isNumber(tokens[i]) || isVariable(tokens[i]) || tokens[i].Equals(")")) && i + 1 < tokens.Count())
                {
                    if (!isOperator(tokens[i + 1]) && !tokens[i + 1].Equals(")"))
                    {
                        throw new FormulaFormatException("Number/Variable/Paranthesis are not followed by operator or closing paranthesis");
                    }
                }
            }
            return tokens;
        }

        /// <summary>
        /// Check if token is a variable
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        private bool isVariable(string token)
        {
            if (token[0] != '_' && !Char.IsLetter(token[0]))
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// Checks if token is a number
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        private bool isNumber(string token)
        {
            return Double.TryParse(token, out double value);
        }

        /// <summary>
        /// Checks if token is an operator
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        private bool isOperator(string token)
        {
            String[] operatorList = { "+", "-", "*", "/" };
            if (operatorList.Contains(token))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        //Checks if token is a paranthesis
        private bool isParan(string token)
        {
            String[] operatorList = { "(", ")" };
            if (operatorList.Contains(token))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Evaluates this Formula, using the lookup delegate to determine the values of
        /// variables.  When a variable symbol v needs to be determined, it should be looked up
        /// via lookup(normalize(v)). (Here, normalize is the normalizer that was passed to 
        /// the constructor.)
        /// 
        /// For example, if L("x") is 2, L("X") is 4, and N is a method that converts all the letters 
        /// in a string to upper case:
        /// 
        /// new Formula("x+7", N, s => true).Evaluate(L) is 11
        /// new Formula("x+7").Evaluate(L) is 9
        /// 
        /// Given a variable symbol as its parameter, lookup returns the variable's value 
        /// (if it has one) or throws an ArgumentException (otherwise).
        /// 
        /// If no undefined variables or divisions by zero are encountered when evaluating 
        /// this Formula, the value is returned.  Otherwise, a FormulaError is returned.  
        /// The Reason property of the FormulaError should have a meaningful explanation.
        ///
        /// This method should never throw an exception.
        /// </summary>
        public object Evaluate(Func<string, double> lookup)
        {
            Stack<double> valueStack = new Stack<double>();
            Stack<string> operatorStack = new Stack<string>();

            //Loops through each token in the list, exciuting certain commands based
            //on what the token is
            foreach (string token in infixFormula)
            {
                // Integer Case
                if (Double.TryParse(token, out double val))
                {
                    FormulaError? err = DoubleOnTop(valueStack, operatorStack, val);

                    if(err != null)
                    {
                        return err;
                    }
                }
                // Variable Case
                else if (isVariable(token))
                {
                    try
                    {
                        double variable = lookup(token);
                        FormulaError? err = DoubleOnTop(valueStack, operatorStack, variable);

                        if (err != null)
                        {
                            return err;
                        }
                    }
                    catch(ArgumentException)
                    {
                        return new FormulaError("VarNotFound");
                    }
                }
                // + or - Case
                else if (token.Equals("+") || token.Equals("-"))
                {
                    OpOnTop(valueStack, operatorStack);
                    operatorStack.Push(token);
                }
                // * or / Case
                else if (token.Equals("*") || token.Equals("/"))
                {
                    operatorStack.Push(token);
                }
                // ( case
                else if (token.Equals("("))
                {
                    operatorStack.Push(token);
                }
                // ) case
                else if (token.Equals(")"))
                {
                    OpOnTop(valueStack, operatorStack);
                    operatorStack.Pop();
                    FormulaError? err = AddAndSubOpOnTop(valueStack, operatorStack);
                    if (err != null)
                    {
                        return err;
                    }
                }

            }

            //Final expresion evaluation and return
            if (operatorStack.Count == 0)
            {
                return valueStack.Pop();
            }
            else
            {
                double val1 = valueStack.Pop();
                double val2 = valueStack.Pop();
                string op = operatorStack.Pop();

                if (op.Equals("+"))
                    return val2 + val1;
                else
                    return val2 - val1;
            }
        }

        /// <summary>
        /// Helper method used to multiply or divide two numbers together and opush them to the stack
        /// Checks if the stack has the correct number of values and checks for divide by 0 error
        /// </summary>
        /// <param name="v"></param>
        /// <param name="o"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        private static FormulaError? DoubleOnTop(Stack<double> v, Stack<string> o, double val)
        {
            if (v.Count > 0 && (o.Peek().Equals("*") || o.Peek().Equals("/")))
            {
                double val2 = v.Pop();
                string op = o.Pop();
                if (op.Equals("*"))
                {
                    v.Push(val2 * val);
                }
                else
                {
                    //Checks for dividing by 0
                    if (val == 0)
                    {
                        return new FormulaError("DIVBY0");
                    }
                    else
                    {
                        v.Push(val2 / val);
                    }
                }
            }
            else
            {
                v.Push(val);
            }
            return null;
        }

        /// <summary>
        /// Helper method used to add or subratct two values together and push them to the stack
        /// Checks if stacks have the correct number of values before using
        /// </summary>
        /// <param name="v"></param>
        /// - Value stack
        /// <param name="o"></param>
        /// - Operator Stack
        private static void OpOnTop(Stack<double> v, Stack<string> o)
        {
            if (v.Count > 1 && (o.Peek().Equals("+") || o.Peek().Equals("-")))
            {
                double val1 = v.Pop();
                double val2 = v.Pop();
                string poppedOp = o.Pop();
                if (poppedOp.Equals("+"))
                {
                    v.Push(val2 + val1);
                }
                else
                {
                    v.Push(val2 - val1);
                }
            }
        }

        /// <summary>
        /// Same as method above however it is for multiplication and division instead of add and sub
        /// </summary>
        /// <param name="v"></param>
        /// - Value Stack
        /// <param name="o"></param>
        /// - Operator Stack
        private static FormulaError? AddAndSubOpOnTop(Stack<double> v, Stack<string> o)
        {
            if (o.Count > 0 && v.Count > 1)
            {
                if ((o.Peek().Equals("*") || o.Peek().Equals("/")))
                {
                    double val1 = v.Pop();
                    double val2 = v.Pop();
                    string poppedOp = o.Pop();
                    if (poppedOp.Equals("*"))
                    {
                        v.Push(val2 * val1);
                    }
                    else
                    {
                        //Check divide by 0
                        if (val1 == 0)
                        {
                            return new FormulaError("DIVBY0");
                        }
                        else
                        {
                            v.Push(val2 / val1);
                        }
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Enumerates the normalized versions of all of the variables that occur in this 
        /// formula.  No normalization may appear more than once in the enumeration, even 
        /// if it appears more than once in this Formula.
        /// 
        /// For example, if N is a method that converts all the letters in a string to upper case:
        /// 
        /// new Formula("x+y*z", N, s => true).GetVariables() should enumerate "X", "Y", and "Z"
        /// new Formula("x+X*z", N, s => true).GetVariables() should enumerate "X" and "Z".
        /// new Formula("x+X*z").GetVariables() should enumerate "x", "X", and "z".
        /// </summary>
        public IEnumerable<String> GetVariables()
        {
            List<string> variables = new List<string>();
            foreach (string token in infixFormula)
            {
                if (isVariable(token))
                {
                    if(!variables.Contains(token))
                        variables.Add(token);
                }
            }
            return variables;
        }

        /// <summary>
        /// Returns a string containing no spaces which, if passed to the Formula
        /// constructor, will produce a Formula f such that this.Equals(f).  All of the
        /// variables in the string should be normalized.
        /// 
        /// For example, if N is a method that converts all the letters in a string to upper case:
        /// 
        /// new Formula("x + y", N, s => true).ToString() should return "X+Y"
        /// new Formula("x + Y").ToString() should return "x+Y"
        /// </summary>
        public override string ToString()
        {
            StringBuilder str = new StringBuilder();
            foreach(string token in infixFormula)
            {
                str.Append(token);
            }
            return str.ToString();
        }

        /// <summary>
        /// If obj is null or obj is not a Formula, returns false.  Otherwise, reports
        /// whether or not this Formula and obj are equal.
        /// 
        /// Two Formulae are considered equal if they consist of the same tokens in the
        /// same order.  To determine token equality, all tokens are compared as strings 
        /// except for numeric tokens and variable tokens.
        /// Numeric tokens are considered equal if they are equal after being "normalized" 
        /// by C#'s standard conversion from string to double, then back to string. This 
        /// eliminates any inconsistencies due to limited floating point precision.
        /// Variable tokens are considered equal if their normalized forms are equal, as 
        /// defined by the provided normalizer.
        /// 
        /// For example, if N is a method that converts all the letters in a string to upper case:
        ///  
        /// new Formula("x1+y2", N, s => true).Equals(new Formula("X1  +  Y2")) is true
        /// new Formula("x1+y2").Equals(new Formula("X1+Y2")) is false
        /// new Formula("x1+y2").Equals(new Formula("y2+x1")) is false
        /// new Formula("2.0 + x7").Equals(new Formula("2.000 + x7")) is true
        /// </summary>
        public override bool Equals(object? obj)
        {
            Formula? f = obj as Formula;
            if(!(f is null))
            {
                List<string> formula2 = GetTokens(f.ToString()).ToList();
                if (formula2.Count != infixFormula.Count)
                    return false;
                for(int i = 0; i < infixFormula.Count; i++)
                {
                    if (!formula2[i].Equals(infixFormula[i]))
                    {
                        return false;
                    }
                }
            }
            else
            {
                return false; 
            }
            return true;
        }

        /// <summary>
        /// Reports whether f1 == f2, using the notion of equality from the Equals method.
        /// Note that f1 and f2 cannot be null, because their types are non-nullable
        /// </summary>
        public static bool operator ==(Formula f1, Formula f2)
        {
            if (f1.Equals(f2))
                return true;
            else
                return false;
        }

        /// <summary>
        /// Reports whether f1 != f2, using the notion of equality from the Equals method.
        /// Note that f1 and f2 cannot be null, because their types are non-nullable
        /// </summary>
        public static bool operator !=(Formula f1, Formula f2)
        {
            if (f1.Equals(f2))
                return false;
            else
                return true;
        }

        /// <summary>
        /// Returns a hash code for this Formula.  If f1.Equals(f2), then it must be the
        /// case that f1.GetHashCode() == f2.GetHashCode().  Ideally, the probability that two 
        /// randomly-generated unequal Formulae have the same hash code should be extremely small.
        /// </summary>
        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        /// <summary>
        /// Given an expression, enumerates the tokens that compose it.  Tokens are left paren;
        /// right paren; one of the four operator symbols; a string consisting of a letter or underscore
        /// followed by zero or more letters, digits, or underscores; a double literal; and anything that doesn't
        /// match one of those patterns.  There are no empty tokens, and no token contains white space.
        /// </summary>
        private static IEnumerable<string> GetTokens(String formula)
        {
            // Patterns for individual tokens
            String lpPattern = @"\(";
            String rpPattern = @"\)";
            String opPattern = @"[\+\-*/]";
            String varPattern = @"[a-zA-Z_](?: [a-zA-Z_]|\d)*";
            String doublePattern = @"(?: \d+\.\d* | \d*\.\d+ | \d+ ) (?: [eE][\+-]?\d+)?";
            String spacePattern = @"\s+";

            // Overall pattern
            String pattern = String.Format("({0}) | ({1}) | ({2}) | ({3}) | ({4}) | ({5})",
                                            lpPattern, rpPattern, opPattern, varPattern, doublePattern, spacePattern);

            // Enumerate matching tokens that don't consist solely of white space.
            foreach (String s in Regex.Split(formula, pattern, RegexOptions.IgnorePatternWhitespace))
            {
                if (!Regex.IsMatch(s, @"^\s*$", RegexOptions.Singleline))
                {
                    yield return s;
                }
            }

        }
    }

    /// <summary>
    /// Used to report syntactic errors in the argument to the Formula constructor.
    /// </summary>
    public class FormulaFormatException : Exception
    {
        /// <summary>
        /// Constructs a FormulaFormatException containing the explanatory message.
        /// </summary>
        public FormulaFormatException(String message)
            : base(message)
        {
        }
    }

    /// <summary>
    /// Used as a possible return value of the Formula.Evaluate method.
    /// </summary>
    public struct FormulaError
    {
        /// <summary>
        /// Constructs a FormulaError containing the explanatory reason.
        /// </summary>
        /// <param name="reason"></param>
        public FormulaError(String reason)
            : this()
        {
            Reason = reason;
        }

        /// <summary>
        ///  The reason why this FormulaError was created.
        /// </summary>
        public string Reason { get; private set; }
    }
}