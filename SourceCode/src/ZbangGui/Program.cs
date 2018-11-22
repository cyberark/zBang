using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Compression;
using System.IO;
using ZbangGui.Properties;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using Microsoft.Win32;
using System.Windows.Forms;

namespace ZbangGui
{
    public class Program
    {
        [DllImport( "kernel32.dll" )]
        static extern IntPtr GetConsoleWindow();

        [DllImport( "user32.dll" )]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        const int SW_HIDE = 0;
        const int SW_SHOW = 5;

        static int rundebug = 1;


        public static void Main(string[] args)
        {
            var handle = GetConsoleWindow();
            // Hide
            //Console.ReadKey();

            ShowWindow(handle, SW_HIDE);

            GetDotNetVersion.Get45PlusFromRegistry();



            if( args.Length >= 1 )
            {
                if( args[0] == "compress" )
                {
                    // start by copying all relevant data to version directory
                    Process.Start( "xcopy.exe", "\"c:\\Users\\nimrod\\Documents\\Visual Studio 2015\\Projects\\graphviz4net_b19bb0cdc8c6\\src\\Graphviz4Net.WPF.Example\\bin\\release\\*.dll\" \"../version/System32/bin/release/*.*\" /Y" );
                    Process.Start( "xcopy.exe", "\"c:\\Users\\nimrod\\Documents\\Visual Studio 2015\\Projects\\graphviz4net_b19bb0cdc8c6\\src\\Graphviz4Net.WPF.Example\\bin\\release\\*.exe\" \"../version/System32/bin/release/*.*\" /Y");

                    for( int i = 0; i < 5; i++ )
                    {
                        Thread.Sleep( 1000 );
                        Console.Write( "." );
                    }

                    if( File.Exists( @"./version.zip" ) )
                    {
                        File.Delete( @"./version.zip" );
                    }

                    try
                    {
                        ZipFile.CreateFromDirectory("../version", "version.zip");
                    }
                    catch( Exception e)
                    {
                        MessageBox.Show(e.Message);
                        return;
                    }
                    Console.WriteLine( "version.zip created and was embedded..." );
                    //Console.ReadKey();
                    return;
                }
                else if( args[0] == "help" || args[0] == "?" )
                {
                    Console.WriteLine( "\nusage: ZbangGui.exe compress\tTo prepare a new compression version.zip\nZbangGui.exe to extract and execute the tool.\nZbangGui.exe debug\t to debug it\n\n" );
                    return;
                }
                else if( args[0] == "debug" )
                {
                    rundebug = 1;
                    if( rundebug == 1)
                    {
                        ShowWindow(handle, SW_SHOW);
                    }
                    ExtractZip();
                }
            }
            else
            {
                if (rundebug == 1)
                    ShowWindow(handle, SW_SHOW);

                ExtractZip();
                //ZipFile.ExtractToDirectory( "version.zip", "c:\\temp\\" );
            }
        } // endfunc main


        static string dirName;


        /// <summary>
        /// Extracts the contents of a zip file to the 
        /// Temp Folder
        /// </summary>
        private static void ExtractZip()
        {
            try
            {
                if( rundebug == 1 )
                {
                    Console.WriteLine( "[1] Starting ZBang...." );
                }
                Random random = new Random();
                int rnd = random.Next();
                string cur = Directory.GetCurrentDirectory();
                dirName = cur + "\\__tempgui" + rnd.ToString();
                if( rundebug == 1 )
                {
                    Console.WriteLine( "[2] Zbang dir is " + dirName );
                }
                Directory.CreateDirectory( dirName);
                if( rundebug == 1 )
                {
                    Console.WriteLine( "[3] Extracting files..." );
                }

                if( File.Exists( dirName + @"/version.zip" ) )
                {
                    File.Delete( dirName + @"/version.zip" );
                }
                if( rundebug == 1 )
                {
                    Console.WriteLine( "[4] Building file structure..." );
                }
                File.WriteAllBytes( dirName  + @"/version.zip", Resources.version );
                if( rundebug == 1 )
                {
                    Console.WriteLine( "[5] Uncompressing..." );
                }
                //extract the contents of the file we created
                ZipFile.ExtractToDirectory( dirName  + @"/version.zip", dirName );
                Directory.SetCurrentDirectory( dirName + "/System32/bin/release" );
                if( rundebug == 1 )
                {
                    Console.WriteLine( "[6] Launching!" );
                }
                LaunchCommandLineApp();
                Directory.SetCurrentDirectory( "../../../../" );
                Directory.Delete( dirName, true );                    // try to delete everything
            }
            catch( Exception e )
            {
                //handle the error
                Console.WriteLine( e.Message );
                //Console.ReadKey();
            }
        }



        /// <summary>
        /// Launch the legacy application with some options set.
        /// </summary>
        static void LaunchCommandLineApp()
        {
            // Use ProcessStartInfo class
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.CreateNoWindow = false;
            startInfo.UseShellExecute = false;
            startInfo.FileName = "Graphviz4Net.WPF.Example.exe";
            startInfo.WindowStyle = ProcessWindowStyle.Maximized;
            startInfo.Arguments = "";

            string cur = Directory.GetCurrentDirectory();
            //Console.WriteLine("[*] curdir=" + cur);
            try
            {
                // Start the process with the info we specified.
                // Call WaitForExit and then the using statement will close.
                using( Process exeProcess = Process.Start( startInfo ) )
                {
                    exeProcess.WaitForExit();
                    Thread.Sleep( 2000 );
                    
                }
            }
            catch( Exception e)
            {
                // Log error.
                Console.WriteLine( "Log Error: {0}", e.Message );
            }
        } // endfunc
    } // endclass


    public static class GetDotNetVersion
    {
        public static bool Get45PlusFromRegistry()
        {
            const string subkey = @"SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Full\";

            using( RegistryKey ndpKey = RegistryKey.OpenBaseKey( RegistryHive.LocalMachine, RegistryView.Registry32 ).OpenSubKey( subkey ) )
            {
                if( ndpKey != null && ndpKey.GetValue( "Release" ) != null )
                {
                    Console.WriteLine( ".NET Framework Version: " + CheckFor45PlusVersion( (int)ndpKey.GetValue( "Release" ) ) );
                    return true;
                }
                else
                {
                    Console.WriteLine( ".NET Framework Version 4.5 or later is not detected." );
                    return false;
                }
            }
        } // endfunc

        // Checking the version using >= will enable forward compatibility.
        private static string CheckFor45PlusVersion(int releaseKey)
        {
            if( releaseKey >= 460798 )
                return "4.7 or later";
            if( releaseKey >= 394802 )
                return "4.6.2";
            if( releaseKey >= 394254 )
            {
                return "4.6.1";
            }
            if( releaseKey >= 393295 )
            {
                return "4.6";
            }
            if( (releaseKey >= 379893) )
            {
                return "4.5.2";
            }
            if( (releaseKey >= 378675) )
            {
                return "4.5.1";
            }
            if( (releaseKey >= 378389) )
            {
                return "4.5";
            }
            // This code should never execute. A non-null release key should mean
            // that 4.5 or later is installed.
            return "No 4.5 or later version detected";
        }
    }


} // endnamespace
