using static System.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;

namespace Assessment
{

    class indexIDSequence
    {
        /// <summary>
        /// level4Index reads the contents from file name and saves the sequence ID and its position to output
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="output"></param>
        public void level4Index(string fileName, string output)
        {
            string line;
            int pos = 0;
            StreamReader file = new StreamReader(fileName);
            StreamWriter fileOutput = new StreamWriter(output);

            //loop through file until there are no lines left
            while ((line = file.ReadLine()) != null)
            {
                //loop through ever char in the line
                for (int i = 0; i < line.Length; i++)
                {
                pos++;
                    //if ti finds a ">" that means it is a sequence ID, so write it
                    if (line[i] == '>')
                    {
                        string linePart = line.Substring(i + 1, 11);
                        fileOutput.WriteLine($"{linePart} {pos}");
                    }
                }
                pos++;
            }
            WriteLine("Indexing Complete");
            file.Close();
            fileOutput.Close();
        }

        /// <summary>
        /// level4Search uses the index file to search fileName for the sequences ID's in query file and writes the
        /// result to output file
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="fileIndex"></param>
        /// <param name="queryFile"></param>
        /// <param name="outputFile"></param>
        public void level4Search(string fileName, string fileIndex, string queryFile, string outputFile)
        {
            string inputLine;
            string indexLine;
            bool foundSeq = false;
            string text;

            //make sure the query file exists
            try
            {
                int count = 0;
                //open the input and output files
                StreamReader indexFile = new StreamReader(fileIndex);
                StreamReader InputFile = new StreamReader(queryFile);
                StreamWriter OutputFile = new StreamWriter(outputFile);

                //loop through the input line by line until an end is reached
                while ((inputLine = InputFile.ReadLine()) != null)
                {
                    foundSeq = false;
                    while ((indexLine = indexFile.ReadLine()) != null)
                    {
                        string indexPart = indexLine.Substring(0, 11);
                        //WriteLine(indexPart);
                        int indexNumber = int.Parse(indexLine.Substring(indexPart.Length + 1 , indexLine.Length-(indexPart.Length + 1)));

                        //if the ID in input Line matches that in indexPart
                        if (inputLine == indexPart)
                        {
                            //open the fasta file
                            using (StreamReader fs = new StreamReader(fileName))
                            {
                                foundSeq = true;
                                fs.BaseStream.Seek(indexNumber-1, SeekOrigin.Begin); //Minus 1 off the index number so that the ">" is included
                                //write the metadata and sequence to OutputFile
                                text = fs.ReadLine();
                                OutputFile.WriteLine(text);
                                text = fs.ReadLine();
                                OutputFile.WriteLine(text);
                                OutputFile.WriteLine(); // Add a space between sequences
                                count++;
                            }
                        }
                    }
                    indexFile.BaseStream.Seek(0, SeekOrigin.Begin);
                    //if no matching sequence was dound
                    if (!foundSeq)
                    {
                        WriteLine("Error, Sequence {0} not found.", inputLine);
                    }
                }
                //close the output and input files
                InputFile.Close();
                OutputFile.Close();
                indexFile.Close();
                WriteLine($"Search Complete, with {count} result/s");
            }
            catch
            {
                WriteLine("Please enter a valid query or index file name");
            }
        }
    }

    class Program
    {
        static List<int> pos = new List<int>(); // an array to keep ine positions in
        static List<int> size = new List<int>(); // an array to keep ine size in
        static int counter = 0; // line counter in text file
        static string line; // line of text
        static int position = 0; // file position of first line

        //error handle
        static bool inputValid = false;

        static void Main(string[] args)
        {

            indexIDSequence level4 = new indexIDSequence();



            //level1
            string searchType = "";
            string fileName = "";
            int startSearch = 0;
            int endSearch = 0;

            //level2
            string searchWord = "";

            //level3
            string inputFile = "";
            string outputFile = "";

            //level 4 
            string indexFile = "";

            //level 5
            string sequence = "";

            //level 6
            string metaData = "";

            //level 7
            string regexString = "";

            //make sure the user has entered arguements
            if (args.Length > 0)
            {
                //start a switch statment for the level type
                switch (args[0])
                {
                    case "-level1":
                        //make sure the correct number of arguements was provided
                        if (args.Length == 4)
                        {
                            searchType = args[0];
                            fileName = args[1];
                            //make sure the numbers parsed are actually numbers
                            try
                            {
                                startSearch = int.Parse(args[2]) - 1;
                                endSearch = startSearch + (int.Parse(args[3]) * 2); //add endsearch on to start search and times it by two since this number indicates the number of sequences
                                readFile(fileName);
                                //readfile will indicate if the file is valid, check this bool
                                if(inputValid)
                                {
                                    level1Search(fileName, startSearch, endSearch);
                                }
                            }
                            catch
                            {
                                WriteLine("Please Enter Numbers for the Beggining and End of the Search.");
                            }
                        }
                        else
                        {
                            WriteLine("Please Enter a Valid Number of arguments, 4 (-level1 [file name] [start position] [num lines to read])");
                        }
                        break;

                    case "-level2":
                        //make sure the correct number of arguements was provided
                        if (args.Length == 3)
                        {
                            searchType = args[0];
                            fileName = args[1];
                            searchWord = args[2];
                            readFile(fileName);
                            //readfile will indicate if the file is valid, check this bool
                            if (inputValid)
                            {
                                level2Search(fileName, searchWord);
                            }
                        }
                        else
                        {
                            WriteLine("Please Enter a Valid Number of arguments, 3 (-level2 [file name] [experiment number])");
                        }
                        break;

                    case "-level3":
                        //make sure the correct number of arguements was provided
                        if (args.Length == 4)
                        {
                            searchType = args[0];
                            fileName = args[1];
                            inputFile = args[2];
                            outputFile = args[3];
                            readFile(fileName);
                            //readfile will indicate if the file is valid, check this bool
                            if (inputValid)
                            {
                                level3Search(fileName, inputFile, outputFile);
                            }
                        }
                        else
                        {
                            WriteLine("Please Enter a Valid Number of arguments, 4 (-level3 [file Name] [query file] [output file])");
                        }
                        break;

                    case "-IndexSequence16S":
                        //make sure the correct number of arguements was provided
                        if (args.Length == 3)
                        {
                            searchType = args[0];
                            fileName = args[1];
                            outputFile = args[2];
                            readFile(fileName);
                            //readfile will indicate if the file is valid, check this bool
                            if (inputValid)
                            {
                                level4.level4Index(fileName, outputFile);
                            }
                        }
                        else
                        {
                            WriteLine("Please Enter a Valid Number of arguments, 3 (-IndexSequence16S [file Name] [output file])");
                        }
                        break;

                    case "-level4":
                        //make sure the correct number of arguements was provided
                        if (args.Length == 5)
                        {
                            searchType = args[0];
                            fileName = args[1];
                            indexFile = args[2];
                            inputFile = args[3];
                            outputFile = args[4];
                            readFile(fileName);
                            //readfile will indicate if the file is valid, check this bool
                            if (inputValid)
                            {
                                level4.level4Search(fileName,indexFile, inputFile, outputFile);
                            }
                        }
                        else
                        {
                            WriteLine("Please Enter a Valid Number of arguments, 5 (-level4 [file Name] [index file] [query file] [output file])");
                        }
                        break;

                    case "-level5":
                        //make sure the correct number of arguements was provided
                        if (args.Length == 3)
                        {
                            searchType = args[0];
                            fileName = args[1];
                            sequence = args[2];
                            readFile(fileName);
                            //readfile will indicate if the file is valid, check this bool
                            if (inputValid)
                            {
                                level5Search(fileName, sequence);
                            }
                        }
                        else
                        {
                            WriteLine("Please Enter a Valid Number of arguments, 3 (-level5 [file Name] [Sequence])");
                        }
                        break;

                    case "-level6":
                        //make sure the correct number of arguements was provided
                        if (args.Length == 3)
                        {
                            searchType = args[0];
                            fileName = args[1];
                            metaData = args[2];
                            readFile(fileName);
                            //readfile will indicate if the file is valid, check this bool
                            if (inputValid)
                            {
                                level6Search(fileName, metaData);
                            }
                        }
                        else
                        {
                            WriteLine("Please Enter a Valid Number of arguments, 3 (-level6 [file Name] [Meta Data])");
                        }
                        break;

                    case "-level7":
                        //make sure the correct number of arguements was provided
                        if (args.Length == 3)
                        {
                            searchType = args[0];
                            fileName = args[1];
                            regexString = args[2];
                            readFile(fileName);
                            //readfile will indicate if the file is valid, check this bool
                            if (inputValid)
                            {
                                level7Search(fileName, regexString);
                            }
                        }
                        else
                        {
                            WriteLine("Please Enter a Valid Number of arguments, 3 (-level7 [file Name] [regexString])");
                        }
                        break;

                    //if no level type was provided
                    default:
                        WriteLine("Please Enter a Valid Search Level.");
                        break;
                    
                } 
            }
            else
            {
                WriteLine("Please include all parameters");
            }

            // Suspend the screen.  
            ReadLine();
        }

       /// <summary>
       /// readFile saves the position and length of every line in the file to it's respective array.
       /// Additionaly it makes sure the file exists and is type fasta
       /// </summary>
       /// <param name="fileName"></param>
        static void readFile(string fileName)
        {
            //make sure the file exists
            try
            {
                //make sure the file is fasta
                if (fileName.Contains(".fasta"))
                {
                    // Read the file and and store the line position in pos and line size in size.
                    StreamReader file = new StreamReader(fileName);
                    while ((line = file.ReadLine()) != null)
                    {
                        pos.Insert(counter, position); // store line position
                        size.Insert(counter, line.Length + 1); // store line size
                        counter++;
                        position = position + line.Length + 1; // add 1 for '\n' character in file
                    }
                    file.Close();
                    inputValid = true;
                }
                else
                {
                    WriteLine("File has to be type fasta");
                    inputValid = false;
                }
            }
            catch
            {
                WriteLine("Cound not find file with name {0}", fileName);
                inputValid = false;
            }
        }

        /// <summary>
        /// level1Search searches through fileName from position startSearch to endSearch.
        /// and provides an error message is these values are wrong.
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="startSearch"></param>
        /// <param name="endSearch"></param>
        static void level1Search(string fileName, int startSearch, int endSearch)
        {
            int count = 0;
            //make sure all parsed variables are within an acceptable range
            if (startSearch >= 0 && startSearch <= counter && startSearch != endSearch && endSearch <= counter)
            {
                //open the fasta file
                    FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read);
                    using (fs)
                    { 
                        //loop through the file from the start position provided to the end position provided
                        for (int n = startSearch; n < endSearch; n++)
                        {              
                            byte[] bytes = new byte[size[n]]; //create a byte variable of size n being that position in the size array
                            fs.Seek(pos[n], SeekOrigin.Begin);// seek to line n (current line)
                            fs.Read(bytes, 0, size[n]); // get the data off disk 
                            string line = Encoding.Default.GetString(bytes); //save the data as a string
                            WriteLine(line); // display the line
                            count++;
                        }
                    }
            }
            else
            {
                WriteLine("Please enter valid search numbers.");
                WriteLine("For example the search should not start at 0 or lower.");
                WriteLine("The search should not start at or pass {0}.", counter);
                WriteLine("And lastly the start and end of the search should not be equal.");
                WriteLine("The values provided were Start: {0}, End: {1}", startSearch+1, endSearch+1);
            }
            WriteLine($"Search Complete, with {count/2} result/s");
        }

        /// <summary>
        /// level2Search searches fileName for a sequenceID provided as searchWord
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="searchWord"></param>
        static void level2Search(string fileName, string searchWord)
        {
            int count = 0;
            //open the fasta file
            using (FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read))
            {
                bool foundSeq = false;

                //loop through the whole file incrementing by 2 since we only need to look at the metadata (more effecient)
                for (int n = 0; n < counter; n+=2)
                {
                    byte[] bytes = new byte[size[n]]; //create a byte variable of size n being that position in the size array
                    fs.Seek(pos[n], SeekOrigin.Begin);// seek to line n (current line)
                    fs.Read(bytes, 0, size[n]); // get the data off disk 
                    string line = Encoding.Default.GetString(bytes); //save the data as a string

                    //loop through every letter in the line
                    for (int letter = 0; letter < line.Length; letter++)
                    {
                        if (line[letter] == '>')
                        {
                            //if the letter is > then grabe the next 11 characters which is the sequence ID
                            string linePart = line.Substring(letter+1, 11);

                            //if the sequence ID provied and the sequence ID found are equal display the meta data and sequence
                            if (searchWord == linePart)
                            {
                                WriteLine(line);
                                byte[] bytes2 = new byte[size[n + 1]];
                                fs.Seek(pos[n + 1], SeekOrigin.Begin);
                                fs.Read(bytes2, 0, size[n + 1]);
                                WriteLine(Encoding.Default.GetString(bytes2));
                                foundSeq = true;
                                count++;
                                break;
                            }
                        }
                    }
                }

                if (!foundSeq)
                {
                    WriteLine("Error, Sequence {0} not found.", searchWord);
                }
            }
            WriteLine($"Search Complete, with {count} result/s");
        }

        /// <summary>
        /// level3Search searches fileName for a sequenceID provided in inputFile
        /// and saves the result to outputFile
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="inputFile"></param>
        /// <param name="outputFile"></param>
        static void level3Search(string fileName, string inputFile, string outputFile)
        {
            string inputLine;

            //make sure the query file exists
            try
            {
                //open the input and output files
                StreamReader InputFile = new StreamReader(inputFile);
                StreamWriter OutputFile = new StreamWriter(outputFile);
                int count = 0;
                //loop through the input line by line until an end is reached
                while ((inputLine = InputFile.ReadLine()) != null)
                {
                    //open the fasta file
                    using (FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read))
                    {
                        bool foundSeq = false;

                        //loop through the whole file incrementing by 2 since we only need to look at the metadata (more effecient)
                        for (int n = 0; n < counter; n+=2)
                        {
                            byte[] bytes = new byte[size[n]]; //create a byte variable of size n being that position in the size array
                            fs.Seek(pos[n], SeekOrigin.Begin);// seek to line n (current line)
                            fs.Read(bytes, 0, size[n]); // get the data off disk 
                            string line = Encoding.Default.GetString(bytes); //save the data as a string

                            //loop through every letter in the line
                            for (int letter = 0; letter < line.Length; letter++)
                            {
                                if (line[letter] == '>')
                                {
                                    //if the letter is > then grabe the next 11 characters which is the sequence ID
                                    string linePart = line.Substring(letter + 1, 11);

                                    //if the sequence ID provied and the sequence ID found are equal write the meta data and sequence to the output file
                                    if (inputLine == linePart)
                                    {
                                        OutputFile.WriteLine(line);
                                        byte[] bytes2 = new byte[size[n + 1]];
                                        fs.Seek(pos[n + 1], SeekOrigin.Begin);
                                        fs.Read(bytes2, 0, size[n + 1]);

                                        line = Encoding.Default.GetString(bytes2);
                                        OutputFile.WriteLine(line);
                                        foundSeq = true;
                                        count++;
                                        break;
                                    }
                                }
                            }
                        }

                        //if no matching sequence was dound
                        if (!foundSeq)
                        {
                            WriteLine("Error, Sequence {0} not found.", inputLine);
                        }
                    }
                }
                //close the output and input files
                InputFile.Close();
                OutputFile.Close();
                WriteLine($"Search Complete, with {count} result/s");
            }
            catch
            {
                WriteLine("Please enter a valid query file name");
            }
        }

        /// <summary>
        /// level5Search searches fileName for a sequenceID that has a sequence which contains sequence
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="sequence"></param>
        static void level5Search(string fileName, string sequence)
        {
            int count = 0;
            using (FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read))
            {
                // the "using" construct ensures that the FileStream is properly closed/disposed 
                bool foundSeq = false;
                string line;

                for (int n = 0; n < counter; n++)
                {             
                    byte[] bytes = new byte[size[n]];
                    fs.Seek(pos[n], SeekOrigin.Begin);// seek to line n (note: there is no actual disk access yet)
                    fs.Read(bytes, 0, size[n]); // get the data off disk - now there is disk access
                                                //System.Console.WriteLine("Line[{n}] : position {n}, size {n}", n, pos[n], size[n]);
                    //save what was pulled from disk to line
                    line = Encoding.Default.GetString(bytes);
                    //if line contains sequence
                    if (line.Contains(sequence))
                    {
                        //grab the previous line (That contains the ID)
                        byte[] bytes2 = new byte[size[n - 1]];
                        fs.Seek(pos[n - 1], SeekOrigin.Begin);
                        fs.Read(bytes2, 0, size[n - 1]);
                        line = Encoding.Default.GetString(bytes2);
                        //loop through the line letter by letter until a ">" is reached then print what is after that(sequenceID)
                        for (int letter = 0; letter < line.Length; letter++)
                        {
                            if (line[letter] == '>')
                            {
                                string linePart = line.Substring(letter + 1, 11);
                                WriteLine(linePart);
                                foundSeq = true;
                                count++;
                            }
                        }
                    }
                }

                if (!foundSeq)
                {
                    WriteLine("Error, Sequence {0} not found.", sequence);
                }
            }
            WriteLine($"Search Complete, with {count} result/s");
        }

        /// <summary>
        /// level6Search searches the file for any sequenceID that has metadata matching what is provided
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="metaData"></param>
        static void level6Search(string fileName, string metaData)
        {
            int count = 0;
            using (FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read))
            {
                // the "using" construct ensures that the FileStream is properly closed/disposed 
                bool foundSeq = false;
                string line;

                //loop through for every second line (Metadata line)
                for (int n = 0; n < counter; n+=2)
                {
                    //grab a line from the file              
                    byte[] bytes = new byte[size[n]];
                    fs.Seek(pos[n], SeekOrigin.Begin);// seek to line n (note: there is no actual disk access yet)
                    fs.Read(bytes, 0, size[n]); // get the data off disk - now there is disk access
                                                //System.Console.WriteLine("Line[{n}] : position {n}, size {n}", n, pos[n], size[n]);
                    line = Encoding.Default.GetString(bytes);

                    //get the length of the meta data expected
                    int metaLength = metaData.Length;

                    //loop through every char in the line
                    //(minus metadata length otherwise we are trying to reach out of bounds)
                    for (int letter = 0; letter < line.Length-metaLength; letter++)
                    {
                        //grab a string of letter metaLength long at position letter
                        string linePart = line.Substring(letter, metaLength);
                        //if the string grabbed matched the metaData provided
                        if (linePart.ToLower() == metaData.ToLower())
                        {
                            //loop backwards through line until a ">" is found
                            //this is the sequenceID for metaData
                            for (int letter2 = letter; letter2 >=0; letter2--)
                            {
                                if (line[letter2] == '>')
                                {
                                    //print the sequenceID then break back into the loop forwards through Line
                                    //to see if there are any other matches
                                    string linePart2 = line.Substring(letter2 + 1, 11);
                                    WriteLine(linePart2);
                                    foundSeq = true;
                                    count++;
                                    break;
                                }
                            }
                        }
                    }
                }

                if (!foundSeq)
                {
                    WriteLine("Error, Meta Data {0} not found.", metaData);
                }
            }
            WriteLine($"Search Complete, with {count} result/s");
        }

        /// <summary>
        /// level7Search searches fileName for any sequence matching regexString and sends back the match
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="regexString"></param>
        static void level7Search(string fileName, string regexString)
        {
            int count = 0;
            using (FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read))
            {
                // the "using" construct ensures that the FileStream is properly closed/disposed 
                bool foundSeq = false;
                string line;

                //replaces all the * with regex and saves it to a new string
                string regex = regexString.Replace("*", @"(.*?)");

                //loop through the file adding by 2 so as to only search sequences 
                for (int n = 1; n < counter; n += 2)
                {
                    //saves the line to line             
                    byte[] bytes = new byte[size[n]];
                    fs.Seek(pos[n], SeekOrigin.Begin);// seek to line n (note: there is no actual disk access yet)
                    fs.Read(bytes, 0, size[n]); // get the data off disk - now there is disk access
                                                //System.Console.WriteLine("Line[{n}] : position {n}, size {n}", n, pos[n], size[n]);
                    line = Encoding.Default.GetString(bytes);

                    //searches line for anything matching regex
                    Match match = Regex.Match(line, regex);

                    //if the search was a success print match to screen
                    if (match.Success)
                    {
                        count++;
                        WriteLine(match);
                        foundSeq = true;
                    }
                          
                }

                if (!foundSeq)
                {
                    WriteLine("Error, Sequence Matching {0} not found.", regexString);
                }
            }
            WriteLine($"Search Complete, with {count} result/s");
        }
    }
}
