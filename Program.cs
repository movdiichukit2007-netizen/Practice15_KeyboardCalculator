using System;
using System.Windows.Forms;

namespace Practice15_KeyboardCalculator;

internal static class Program
{
    [STAThread]
    private static void Main()
    {
        ApplicationConfiguration.Initialize();
        Application.Run(new MainForm());
    }
}
