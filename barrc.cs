using System;
using System.IO; // read RC files
using System.Diagnostics; // launch executables
using System.Collections.Generic; // List<>

namespace BarUtil
{
  /**
   *  'barrc'
   *     1) Read 'barrc' configuration file.
   *	   2) Read 'scriptrc' configuration file.
   *     3) Execute bar based on config as seperate process.
   *     4) Execute scripts based on config as seperate processes.
   *     5) Respond to cli queries as seperate process.
   *
   *	-c 'config_path/'
   *	-h print help
   *	-v debug output
   *	-l list currently running scripts
   *	TODO: edit running command arguments?
   *	TODO: case insensitivity
   */
  class BarRc
  {
    // default rc file path
    private string m_barrcPath =
      Environment.GetEnvironmentVariable("HOME") + "/.config/bar/.barrc";
    private string m_scriptrcPath =
      Environment.GetEnvironmentVariable("HOME") + "/.config/bar/.scriptrc";

    private int m_maxBars = 4; // TODO: determine max bars from rc
    private bool m_debug = false; // -v output

    private List<Bar> m_bars = null;
    private List<Script> m_scripts = null;

    // TODO: EOF instead, determine bar num from this
    // process each line of the bar config
    public void parseBarLine(int barNum, string line)
    {
      for(int i = 0; i < m_bars.Count; ++i)
      {
        if(m_bars[i].m_number == barNum)
        {
          if(line.Contains("Geometery="))
          {
            string[] toke = line.Split('=', '#');

            if(!string.IsNullOrEmpty(toke[1]))
            {
              m_bars[i].m_geometery = toke[1].Trim();
            }

            // else // missing or invalid data
            // TODO: raise an erorr? only on debug?
          }

          if(line.Contains("Position="))
          {
            string[] toke = line.Split('=', '#');

            if(!string.IsNullOrEmpty(toke[1]))
            {
              if(toke[1].Contains("Top"))
              {
                m_bars[i].m_top = true;
              }
              else if(toke[1].Contains("Bottom"))
              {
                m_bars[i].m_top = false;
              }
            }
          }

          if(line.Contains("ForceDock="))
          {
            string[] toke = line.Split('=', '#');

            if(!string.IsNullOrEmpty(toke[1]))
            {
              m_bars[i].m_forceDock = Convert.ToBoolean(toke[1]);
            }
          }

          if(line.Contains("Permanent="))
          {
            string[] toke = line.Split('=', '#');

            if(!string.IsNullOrEmpty(toke[1]))
            {
              m_bars[i].m_permanent = Convert.ToBoolean(toke[1]);
            }
          }

          if(line.Contains("FIFO="))
          {
            string[] toke = line.Split('=', '#');

            if(!string.IsNullOrEmpty(toke[1]))
            {
              m_bars[i].m_fifo = toke[1].Trim();
            }
          }

          if(line.Contains("Monitor="))
          {
            string[] toke = line.Split('=', '#');

            if(!string.IsNullOrEmpty(toke[1]))
            {
              int temp = 1;

              if(Int32.TryParse(toke[1].Trim(), out temp))
              {
                m_bars[i].m_renderMonitor = temp;
              }
              else
              {
                m_bars[i].m_renderMonitor = 1;
              }
            }
          }

          if(line.Contains("FontOne="))
          {
            string[] toke = line.Split('=', '#');

            if(!string.IsNullOrEmpty(toke[1]))
            {
              m_bars[i].m_fontOne = toke[1].Trim();
            }
          }

          if(line.Contains("FontTwo="))
          {
            string[] toke = line.Split('=', '#');

            if(!string.IsNullOrEmpty(toke[1]))
            {
              m_bars[i].m_fontTwo = toke[1].Trim();
            }
          }

          if(line.Contains("ULWidth="))
          {
            string[] toke = line.Split('=', '#');

            if(!string.IsNullOrEmpty(toke[1]))
            {
              // TODO: make this less of a POS lel
              try {
                m_bars[i].m_underlineWidth = Convert.ToInt16(toke[1]);
              }
              catch (Exception e) {
                // silent
                Console.WriteLine("Exception with ulwidth: " + e.Message);
              }
            }
          }

          if(line.Contains("BColor="))
          {
            string[] toke = line.Split('=', '#');

            if(toke[1] != null && !string.IsNullOrEmpty(toke[1]))
            {
              m_bars[i].m_bgColor = toke[1].Trim();
            }
          }

          if(line.Contains("FColor="))
          {
            string[] toke = line.Split('=', '#');

            if(!string.IsNullOrEmpty(toke[1]))
            {
              m_bars[i].m_fgColor = toke[1].Trim();
            }
          }
        }
      }
    }

    // read and load bar config into memory
    private void parseBarConfig()
    {
      // amount of referenced bars
      int bars = 0;

      // a vector of integers denoting bars
      List<int> barNums = new List<int>();
      m_bars = new List<Bar>();

      if(m_debug)
      {
        Console.WriteLine("Attempting to open file: " + m_barrcPath);
      }

      try // try and open file
      {
        // open file stream for reading
        StreamReader reader = new StreamReader(m_barrcPath);

        if(m_debug)
        {
          Console.WriteLine(m_barrcPath + " opened.");
        }

        string line = reader.ReadLine();

        // TODO: do we need this?
        // while ! EOF read each line
        while(line != null)
        {
          // for the maximum amount of bars
          for(int barNum = 1; barNum < m_maxBars + 1; ++barNum)
          {
            // if the line contains a bar reference
            // bar reference is in the format 'n*'
            if(line.Contains(barNum.ToString()))
            {
              // assume it is a new bar reference
              bool isNew = true;

              // for the length of the previous bar references
              for(int prevNum = 0; prevNum < barNums.Count; ++prevNum)
              {
                // if this reference is all ready noted
                if(barNums[prevNum] == barNum)
                {
                  isNew = false; // it isn't new
                }
              }

              if(isNew) // if a new bar is referenced
              {
                if(m_debug) // verbose output
                {
                  Console.WriteLine("New bar reference detected.");
                  Console.WriteLine("Running bar reference total: " + bars + ".");
                }

                bars++; // increment barcounter
                barNums.Add(barNum); // store bar number reference

                Bar temp = new Bar(); // create temp bar
                temp.m_number = barNum; // store the number
                m_bars.Add(temp); // add the bar to the vector
              }

              // process each line
              parseBarLine(barNum, line);
            }
          }

          line = reader.ReadLine();
        }
      }
      catch(Exception err)
      {
        // this one
        Console.WriteLine("Exception: " + err.Message);
      }
    }

    // process a line of the config
    public void parseScriptLine(int scriptNum, string line)
    {
      // this is probably unecessary
      if(m_scripts == null)
      {
        Console.WriteLine("Structure to store script data is null.");
        throw new NullReferenceException();
      }

      // for the length of m_scripts
      // keeping in mind m_scripts should have just had
      // a script object added for flow to get here
      for(int i = 0; i < m_scripts.Count; ++i)
      {
        // find the bar being referred to
        if(scriptNum == (i + 1))
        {
          // check for accepted config data
          if(line.Contains("Name:"))
          {
            // split string
            string[] toke = line.Split(':');

            // it is expected to be the 2nd token
            // make sure it isn't null before accessing
            if(toke[1] != null && !string.IsNullOrEmpty(toke[1]))
            {
              m_scripts[i].m_name = toke[1].Trim();
            }
            // else // missing or invalid data
            // not providing a value shouldn't necessarily
            // raise an error
          }

          if(line.Contains("Path:"))
          {
            string[] toke = line.Split(':');

            if(toke[1] != null && !string.IsNullOrEmpty(toke[1]))
            {
              m_scripts[i].m_path = toke[1].Trim();
            }
          }

          if(line.Contains("Arguments:"))
          {
            string[] toke = line.Split(':');

            if(toke[1] != null && !string.IsNullOrEmpty(toke[1]))
            {
              m_scripts[i].m_arguments = toke[1].Trim();
            }
          }

          if(line.Contains("Poll:"))
          {
            string[] toke = line.Split(':');

            if(toke[1] != null && !string.IsNullOrEmpty(toke[1]))
            {
              m_scripts[i].m_poll = Convert.ToBoolean(toke[1]);
            }
          }

          if(line.Contains("Frequency:"))
          {
            string[] toke = line.Split(':');

            if(toke[1] != null && !string.IsNullOrEmpty(toke[1]))
            {
              m_scripts[i].m_frequency = toke[1].Trim();
            }
          }
        }
      }
    }

    // read and load script config into memory
    private void parseScriptConfig()
    {
      int scripts = 0;

      m_scripts = new List<Script>();

      if(m_debug) // verbose output
      {
        Console.WriteLine("Attempting to open file: " + m_scriptrcPath);
      }

      try // try and open file
      {
        // open file stream for reading
        StreamReader reader = new StreamReader(m_scriptrcPath);

        if(m_debug)
        {
          Console.WriteLine(m_scriptrcPath + " opened.");
        }

        string line = null;

        // while ! EOF read each line
        while((line = reader.ReadLine()) != null)
        {
          // if the line contains a script reference
          if(line.Contains("Name:"))
          {
            // insert new element
            scripts++;
            Script temp = new Script();
            m_scripts.Add(temp);
          }

          // process each line
          parseScriptLine(scripts, line);
        }
      }
      catch(Exception err)
      {
        Console.WriteLine("Exception: " + err.Message);
      }
    }

    private void handleArgs(string[] args)
    {
      // iterate through arguments
      for(int i = 0; i < args.Length; ++i)
      {
        if(args[i].Equals("-v")) // verbose
        {
          m_debug = true;
        }
        else if(args[i].Equals("-h")) // help message
        {
        }
        else if(args[i].Contains("-c"))
        {
          m_barrcPath = args[i + 1];
        }
        else
        {
        }
      }
    }

    private void handleInput()
    {
      string input = null;

      while((input = Console.ReadLine()) != "exit")
      {
        Console.WriteLine(input);
      }
    }

    public static void Main(string[] args)
    {
      // create class instance (C# weirdness)
      BarRc barrc = new BarRc();

      // handle command line arguments
      barrc.handleArgs(args);

      // if the config file cannot be found
      if(!File.Exists(barrc.m_barrcPath))
      {
        Console.WriteLine("Cannot locate bar config: " + barrc.m_barrcPath);
        throw new FileNotFoundException();
      }
      else // rc located successfully
      {
        // #1 read/store scriptsrc
        barrc.parseScriptConfig();

        if(barrc.m_debug) // verbose output
        {
          Console.WriteLine(barrc.m_scripts.Count + " scripts detected.");

          for(int i = 0; i < barrc.m_scripts.Count; ++i) // for each bar
          {
            barrc.m_scripts[i].print(); // print config
          }
        }

        // #2 load/store bar config
        barrc.parseBarConfig();

        if(barrc.m_debug) // verbose output
        {
          Console.WriteLine(barrc.m_bars.Count + " bars detected.");

          for(int i = 0; i < barrc.m_bars.Count; ++i) // for each bar
          {
            barrc.m_bars[i].print(); // print config
          }
        }

        // #3 run scripts
        for(int i = 0; i < barrc.m_scripts.Count; ++i)
        {
          // Start the process.
        }

        // #4 thread: run bar(s)
        for(int i = 0; i < barrc.m_bars.Count; ++i)
        {
          barrc.m_bars[i].start();
        }

        // #5 thread: respond to stdin

        // #6 wait on all threads
        bool done = false;
        while (!done)
        {
          int barsDone = 0;

          for (int i = 0; i < barrc.m_bars.Count; ++i)
          {
            if (barrc.m_bars[i].m_process.HasExited)
            {
              barsDone++;
            }
          }

          if (barsDone == barrc.m_bars.Count - 1) {
            done = true;
          }
        }

        Console.WriteLine("Finished.");
      }
    }
  }
}
