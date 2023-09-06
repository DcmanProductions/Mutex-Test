// LFInteractive LLC. 2021-2024﻿
using System.IO.Pipes;

internal class Program
{
    private static void Main(string[] args)
    {
        string pipeName = "MyNamedPipe";
        _ = new Mutex(true, "MutexTest", out bool createdNew); // Checks if an instance is already created.
        if (!createdNew)
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
        using NamedPipeClientStream clientPipe = new(".", pipeName, PipeDirection.Out);
        try
        {
            clientPipe.Connect();
            using StreamWriter writer = new(clientPipe);
            writer.Write(string.Join(" ", args));
            writer.Flush();
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error sending arguments to existing instance: " + ex.Message);
        }
    }

    private static void StartNamedPipeServer(string pipeName)
    {
        Task.Run(() =>
        {
            while (true)
            {
                Console.WriteLine("Waiting for arguments!");
                using NamedPipeServerStream serverPipe = new(pipeName, PipeDirection.In);
                try
                {
                    serverPipe.WaitForConnection();
                    using (StreamReader reader = new(serverPipe))
                    {
                        string receivedArgs = reader.ReadToEnd();
                        Console.WriteLine($"Received arguments: {receivedArgs}");
                    }
                    serverPipe.Disconnect();
                }
                catch
                {
                }
            }
        });
    }
}