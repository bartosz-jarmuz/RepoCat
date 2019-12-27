using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace RepoCat.Transmission.Client
{

    /// <summary>
    /// Represents an abstract base for a command line parameter set
    /// </summary>
    public abstract class ParameterSet
    {


        /// <summary>
        /// Creates a new instance of the class
        /// </summary>
        protected ParameterSet()
        {

        }

        /// <summary>
        /// Creates a new instance and loads the parameters provided in the constructor
        /// </summary>
        /// <param name="args"></param>
        protected ParameterSet(string[] args)
        {
            this.OriginalParameterInputString = string.Join(" ", args);
            this.LoadParameters(args);
        }

        /// <summary>
        /// Creates a new instance and loads the parameters provided in the argument string of constructor
        /// </summary>
        /// <param name="argumentString"></param>
        protected ParameterSet(string argumentString) //do not chain constructors or you will break the preservation of the original input string
        {
            this.OriginalParameterInputString = argumentString;
            this.LoadParameters(Parser.Split(argumentString));
        }

        /// <summary>
        /// Gets the input string as it was provided to the parameter class
        /// </summary>
        public string OriginalParameterInputString { get; private set; } = "";

        /// <summary>
        /// The collection of parameters recognized from the input string (not including default values that the parameter set class might contain)
        /// </summary>
        public IReadOnlyCollection<KeyValuePair<string, string>> OriginalParameterCollection { get; private set; }

        /// <summary>
        /// Loads parameter values to the public instance properties of this class
        /// </summary>
        /// <param name="argumentString"></param>
        public void LoadParameters(string argumentString)
        {
            this.OriginalParameterInputString = argumentString;
            this.LoadParameters(Parser.Split(argumentString));
        }

        /// <summary>
        /// Loads parameter values to the public instance properties of this class
        /// </summary>
        /// <param name="args"></param>
        public void LoadParameters(string[] args)
        {
            this.OriginalParameterInputString = string.Join(" ", args);
            this.OriginalParameterCollection = Parser.GetParameters(args);

            var properties = this.GetType().GetProperties();

            foreach (KeyValuePair<string, string> keyValuePair in this.OriginalParameterCollection)
            {
                var matchingProperty = properties.FirstOrDefault(x => x.Name.Equals(keyValuePair.Key, StringComparison.OrdinalIgnoreCase));
                if (matchingProperty != null)
                {
                    SetPropertyValueFromString(this, matchingProperty, keyValuePair.Value);
                }
            }
        }

        /// <summary>
        /// Converts the object to a set of parameters in form of a string (like for ProcessStartInfo.Arguments)
        /// </summary>
        public string SaveAsParameters(bool skipNullAndEmpty = true, bool skipDefaultsForValueTypes = true)
        {
            StringBuilder sb = new StringBuilder();

            this.TraverseParameters((k, v) => sb.Append($"--{k}=\"{v}\" "), skipNullAndEmpty, skipDefaultsForValueTypes);

            return sb.ToString();
        }

        /// <summary>
        /// Gets all the parameter properties for this class. <br/>
        /// Parameter properties are the instance properties with public getter and setter - i.e. all the properties which can be assigned by passing them as argument string
        /// </summary>
        public IReadOnlyCollection<KeyValuePair<string, string>> GetParameterCollection(bool skipNullAndEmpty = false, bool skipDefaultsForValueTypes = false)
        {
            var list = new List<KeyValuePair<string, string>>();
            this.TraverseParameters((k, v) => list.Add(new KeyValuePair<string, string>(k, v?.ToString())), skipNullAndEmpty, skipDefaultsForValueTypes);

            return list.AsReadOnly();
        }

        private void TraverseParameters(Action<string, string> outputParameters, bool skipNullAndEmpty, bool skipDefaultsForValueTypes)
        {
            var properties = this.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.SetProperty);

            foreach (PropertyInfo propertyInfo in properties)
            {
                object value = propertyInfo.GetValue(this);

                if (!propertyInfo.CanWrite || !propertyInfo.CanRead)
                {
                    continue;
                }

                // Get and set methods have to be public
                if (propertyInfo.GetGetMethod(false) == null || propertyInfo.GetSetMethod(false) == null)
                {
                    continue;
                }

                if (skipNullAndEmpty && (value == null || string.IsNullOrEmpty(value.ToString())) && (!propertyInfo.PropertyType.IsValueType || Nullable.GetUnderlyingType(propertyInfo.PropertyType) != null))
                {
                    continue;
                }

                if (skipDefaultsForValueTypes && propertyInfo.PropertyType.IsValueType)
                {
                    var defaultValue = Activator.CreateInstance(propertyInfo.PropertyType);

                    if (value != null && value.Equals(defaultValue))
                    {
                        continue;
                    }

                }

                if (value is DateTime dt)
                {
                    outputParameters(propertyInfo.Name, dt.ToString("O"));
                }
                else
                {
                    outputParameters(propertyInfo.Name, value?.ToString());
                }


            }
        }

        private static void SetPropertyValueFromString<T>(T objectToAssign, PropertyInfo property, string value)
        {
            var theType = Nullable.GetUnderlyingType(property.PropertyType);
            if (theType == null)
            {
                theType = property.PropertyType;
            }
            property.SetValue(objectToAssign, TypeDescriptor.GetConverter(theType).ConvertFromInvariantString(value));
        }
        /// <summary>
        /// Parses a string with parameters in a '-key value' format
        /// </summary>

        public static class Parser
        {
            private static readonly Regex Splitter = new Regex(@"^-{1,2}|^/|=|:", RegexOptions.IgnoreCase | RegexOptions.Compiled);

            private static readonly Regex Remover = new Regex(@"^['""]?(.*?)['""]?$", RegexOptions.IgnoreCase | RegexOptions.Compiled);

            /// <summary>
            /// Splits the string to get argument tokens. <br/>
            /// The expected behaviour is the same as a Console Application would have when parsing a ProcessStartInfo.Arguments string into array of strings.
            /// </summary>
            /// <param name="input">The input.</param>
            /// <returns>System.String[].</returns>
            public static string[] Split(string input)
            {
                if (String.IsNullOrEmpty(input))
                {
                    return Array.Empty<string>();
                }
                bool inQuotes = false;
                return Split(input, c =>
                {
                    if (c == '\"')
                        inQuotes = !inQuotes;

                    return !inQuotes && c == ' ';
                }).Select<string, string>(arg => TrimMatchingQuotes(arg.Trim(), '\"'))
                    .Where(arg => !String.IsNullOrEmpty(arg)).ToArray();
            }

            /// <summary>
            /// Gets a collection of key value pairs - parameter name and value. <br/>
            /// In case of positional parameters (i.e. parameters without name), key is an empty string.<br/>
            /// In case of 'switch' parameters (i.e. parameters without value, like '--debug'), the value is set to 'True'<para/>
            /// The collection might contain multiple identical keys - especially in case of positional parameters, where key is always empty string
            /// </summary>
            /// <param name="argumentString">The argument string.</param>
            public static IReadOnlyCollection<KeyValuePair<string, string>> GetParameters(string argumentString)
            {
                return GetParameters(Split(argumentString));
            }


            /// <summary>
            /// Gets a collection of key value pairs - parameter name and value. <br/>
            /// In case of positional parameters (i.e. parameters without name), key is an empty string.<br/>
            /// In case of 'switch' parameters (i.e. parameters without value, like '--debug'), the value is set to 'True'<para/>
            /// The collection might contain multiple identical keys - especially in case of positional parameters, where key is always empty string
            /// </summary>
            /// <param name="args">The arguments.</param>
            public static IReadOnlyCollection<KeyValuePair<string, string>> GetParameters(string[] args)
            {
                if (args == null)
                {
                    return new Dictionary<string, string>();
                }
                //based upon
                //https://www.codeproject.com/Articles/3111/C-NET-Command-Line-Arguments-Parser

                List<KeyValuePair<string, string>> parameters = new List<KeyValuePair<string, string>>();
                // Valid parameters forms:
                // {-,/,--}param{ ,=,:}((",')value(",'))
                // Examples: 
                // -param1 value1 --param2 /param3:"Test-:-work" 
                //   /param4=happy -param5 '--=nice=--'
                string parameter = null;
                foreach (string txt in args)
                {
                    // Look for new parameters (-,/ or --) and a possible enclosed value (=,:)
                    string[] parts = Splitter.Split(txt, 3);
                    switch (parts.Length)
                    {
                        // Found a value (for the last parameter found (space separator))
                        case 1:
                            if (parameter != null)
                            {
                                parts[0] = Remover.Replace(parts[0], "$1");
                                parameters.Add(new KeyValuePair<string, string>(parameter, parts[0]));
                                parameter = null;
                            }
                            else
                            {
                                parameters.Add(new KeyValuePair<string, string>("", parts[0])); //unnamed parameter (a simple value)
                            }
                            // else Error: no parameter waiting for a value (skipped)
                            break;

                        // Found just a parameter
                        case 2:
                            // The last parameter is still waiting. With no value, set it to true.
                            if (parameter != null)
                            {
                                parameters.Add(new KeyValuePair<string, string>(parameter, "True"));
                            }
                            parameter = parts[1];
                            break;

                        // Parameter with enclosed value
                        case 3:
                            // The last parameter is still waiting. With no value, set it to true.
                            if (parameter != null)
                            {
                                parameters.Add(new KeyValuePair<string, string>(parameter, "True"));
                            }

                            parameter = parts[1];

                            // Remove possible enclosing characters (",')
                            parts[2] = Remover.Replace(parts[2], "$1");
                            parameters.Add(new KeyValuePair<string, string>(parameter, parts[2]));

                            parameter = null;
                            break;
                    }
                }
                // In case a parameter is still waiting
                if (parameter != null)
                {
                    parameters.Add(new KeyValuePair<string, string>(parameter, "True"));
                }

                return parameters.AsReadOnly();
            }


            private static IEnumerable<string> Split(string str, Func<char, bool> controller)
            {
                int nextPiece = 0;

                for (int c = 0; c < str.Length; c++)
                {
                    if (controller(str[c]))
                    {
                        yield return str.Substring(nextPiece, c - nextPiece);
                        nextPiece = c + 1;
                    }
                }

                yield return str.Substring(nextPiece);
            }

            private static string TrimMatchingQuotes(string input, char quote)
            {
                if ((input.Length >= 2) &&
                    (input[0] == quote) && (input[input.Length - 1] == quote))
                    return input.Substring(1, input.Length - 2);

                return input;
            }
        }
    }


}