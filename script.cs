using System;
using System.Diagnostics; // process

namespace BarUtil
{
    enum Position
    {
	Left,
	Center,
	Right
    };

    class Script
    {
	public Process m_process = null;
	
	// full path if not in $PATH
	public string m_name = "";
	public string m_path = "";
	public string m_arguments = "";
	public bool m_poll = false;
	public string m_frequency = "";
	public string m_fifo = "";
	public bool m_swapColor = false;
	public Position m_position = Position.Center;
	public string m_bColor = "#000000";
	public string m_fColor = "#FFFFFF";
	public string m_uColor = "#FFFFFF";
	public bool m_clickable = false;
	public string m_onClick = "";	
	public bool m_underLine = false;
	public bool m_overLine = false;

	public void print()
	{
	    Console.WriteLine("Script Name: " + m_name);
	    Console.WriteLine("\tPath: " + m_path);
	    Console.WriteLine("\tArguments: " + m_arguments);
	    Console.WriteLine("\tPoll: " + m_poll); 
	    Console.WriteLine("\tm_frequency: " + m_frequency);
	    Console.WriteLine("\tm_fifo: " + m_fifo);
	    Console.WriteLine("\tm_swapColor: " + m_swapColor);
	    Console.WriteLine("\tm_position: " + m_position);
	    Console.WriteLine("\tm_bColor: " + m_bColor);
	    Console.WriteLine("\tm_fColor: " + m_fColor);
	    Console.WriteLine("\tm_uColor: " + m_uColor);
	    Console.WriteLine("\tm_clickable: " + m_clickable);
	    Console.WriteLine("\tm_onClick: " + m_onClick);
	    Console.WriteLine("\tm_underLine: " + m_underLine);
	    Console.WriteLine("\tm_overLine: " + m_overLine);
	}
    }
}
