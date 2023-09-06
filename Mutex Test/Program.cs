// LFInteractive LLC. 2021-2024﻿
using System.IO.Pipes;

internal class Program
{
    private static void Main(string[] args)
    {
        string pipeName = "MyNamedPipe";

        if (args.Length > 0)
        {
            // Send arguments to the existing instance.
            SendArgumentsToExistingInstance(pipeName, args);
        }
        else
        {
            // Create the named pipe server and keep listening for incoming arguments.
            StartNamedPipeServer(pipeName);

            // Your application logic for the first instance goes here.
            Console.WriteLine("This is the first instance. Press any key to exit.");
            Console.ReadKey();
        }
    }

    private static void SendArgumentsToExistingInstance(string pipeName, string[] args)
    {
        using (NamedPipeClientStream clientPipe = new NamedPipeClientStream(".", pipeName, PipeDirection.Out))
        {
            try
            {
                clientPipe.Connect();
                using (StreamWriter writer = new StreamWriter(clientPipe))
                {
                    writer.Write(string.Join(" ", args));
                    writer.Flush();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error sending arguments to existing instance: " + ex.Message);
            }
        }
    }

    private static void StartNamedPipeServer(string pipeName)
    {
        while (true)
        {
            using (NamedPipeServerStream serverPipe = new NamedPipeServerStream(pipeName, PipeDirection.In))
            {
                try
                {
                    serverPipe.WaitForConnection();
                    using (StreamReader reader = new StreamReader(serverPipe))
                    {
                        string receivedArgs = reader.ReadToEnd();
                        Console.WriteLine("Received arguments: " + receivedArgs);
                    }
                    serverPipe.Disconnect();
                }
                catch
                {
                }
            }
        }
    }
}