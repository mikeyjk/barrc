using System;
using System.Threading.Tasks;
using System.Diagnostics;

namespace BarUtil
{
  // describes a bar
  class Bar
  {
    private readonly string m_barPath = "/usr/bin/bar";

    public Process m_process = null;

    public int m_number = 0; // unique bar id
    public string m_fifo = ""; // named pipe input
    public int m_sDir = 0;

    // arguments defined by bar
    public string m_geometery = "";
    public bool m_dockBottom = false;
    public bool m_forceDock = false;
    public bool m_permanent = true;
    public string m_fontOne = "";
    public string m_fontTwo = "";
    public int m_underlineWidth = 0;
    public string m_bgColor = "";
    public string m_fgColor = "";

    public void start()
    {
      string query = "-c ";

      // if there is named pipe input
      if (!string.IsNullOrEmpty(m_fifo)) {

        // let bash handle it
        query += "\"cat " + m_fifo + " | ";
      }

      query += m_barPath + arguments() + "\"";

      m_process = new Process();
      m_process.StartInfo.UseShellExecute = false;
      m_process.StartInfo.RedirectStandardOutput = true;
      m_process.StartInfo.FileName = "bash";
      m_process.StartInfo.Arguments = query;
      Console.WriteLine(m_process.StartInfo.Arguments);
      m_process.Start();
    }

    private string arguments()
    {
      string m_args = "";

      /*
      if (m_geometery = "")
      */

      if (m_dockBottom) { m_args += " -b"; }
      if (m_forceDock) { m_args += " -d"; }
      if (m_permanent) { m_args += " -p"; }

      // if (m_sDir = 0) {}
      if (!string.IsNullOrEmpty(m_fontOne))
      {
        m_args += " -f " + "'" + m_fontOne + "'";

        if (!string.IsNullOrEmpty(m_fontTwo))
        {
          m_args += ", " + "'" + m_fontTwo + "'";
        }
      }

      if (m_underlineWidth > 1)
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

    public void print()
    {
      Console.WriteLine("Bar: #" + m_number);
      Console.WriteLine("\tGeo: " + m_geometery);
      Console.WriteLine("\tDockB: " + m_dockBottom);
      Console.WriteLine("\tForceD: " + m_forceDock);
      Console.WriteLine("\tPerm: " + m_permanent);
      Console.WriteLine("\tFIFO: " + m_fifo);
      Console.WriteLine("\tSDIR: " + m_sDir);
      Console.WriteLine("\tF1: " + m_fontOne);
      Console.WriteLine("\tF2: " + m_fontTwo);
      Console.WriteLine("\tULW: " + m_underlineWidth);
      Console.WriteLine("\tBGC: " + m_bgColor);
      Console.WriteLine("\tFGC: " + m_fgColor);
    }
  }
}
