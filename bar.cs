using System;
using System.Diagnostics; // process functionality

namespace BarUtil
{
  class Bar
  {
    // barrc related data
    private readonly string m_barPath = "/usr/bin/bar";
    public Process m_process = null;
    public int m_number = 0; // unique bar id
    public string m_fifo = ""; // named pipe input

    // arguments defined by bar
    public int m_renderMonitor = 0; // render to monitor #
    public string m_geometery = ""; // bar geometery
    public bool m_top = true; // true = top, false = bottom
    public bool m_forceDock = false; // force dock for some wm's
    public bool m_permanent = true; // stay open
    public string m_fontOne = ""; // primary font
    public string m_fontTwo = ""; // secondary font
    public int m_underlineWidth = 0;
    public string m_bgColor = "";
    public string m_fgColor = "";

    /*
     * Execute/run a bar instance with the appropriate
     * command line arguments.
     *
     * Bash is used to handle the piped data.
     */
    public void start()
    {
      string query = "-c \""; // let bash know we are providing arguments

      // if there is a named pipe, use cat
      // to pass this to bar
      if (!string.IsNullOrEmpty(m_fifo)) {
        query += "cat " + m_fifo + " | ";
      }

      // call bar with the appropriate arguments
      query += m_barPath + arguments() + "\"";

      m_process = new Process();
      m_process.StartInfo.UseShellExecute = false;
      m_process.StartInfo.RedirectStandardOutput = true;
      m_process.StartInfo.FileName = "bash";
      m_process.StartInfo.Arguments = query;
      Console.WriteLine(m_process.StartInfo.Arguments);

      m_process.Start();
    }

    /*
     * Populate the arguments to pass to
     * the bar binary.
     */
    private string arguments()
    {
      string m_args = "";

      if (m_top) { m_args += " -b"; }
      if (m_forceDock) { m_args += " -d"; }
      if (m_permanent) { m_args += " -p"; }

      if (!string.IsNullOrEmpty(m_geometery))
      {
        m_args += " -g " + m_geometery;
      }

      if (!string.IsNullOrEmpty(m_fontOne))
      {
        m_args += " -f " + "'" + m_fontOne + "'";

        // we only handle a secondary font
        // if a primary font is provided
        if (!string.IsNullOrEmpty(m_fontTwo))
        {
          m_args += ", " + "'" + m_fontTwo + "'";
        }
      }

      if (m_underlineWidth > 1) // TODO: verify default
      {
        m_args += " -u " + m_underlineWidth;
      }

      if (!string.IsNullOrEmpty(m_bgColor))
      {
        m_args += " -B '" + m_bgColor + "'";
      }

      if (!string.IsNullOrEmpty(m_fgColor))
      {
        m_args += " -F '" + m_fgColor + "'";
      }

      return (m_args);
    }

    /*
     * Verbose / debug output.
     */
    public void print()
    {
      Console.WriteLine("Bar: #" + m_number);
      Console.WriteLine("\tGeo: " + m_geometery);
      Console.WriteLine("\tDockB: " + m_top);
      Console.WriteLine("\tForceD: " + m_forceDock);
      Console.WriteLine("\tPerm: " + m_permanent);
      Console.WriteLine("\tFIFO: " + m_fifo);
      Console.WriteLine("\tSDIR: " + m_renderMonitor);
      Console.WriteLine("\tF1: " + m_fontOne);
      Console.WriteLine("\tF2: " + m_fontTwo);
      Console.WriteLine("\tULW: " + m_underlineWidth);
      Console.WriteLine("\tBGC: " + m_bgColor);
      Console.WriteLine("\tFGC: " + m_fgColor);
    }
  }
}
