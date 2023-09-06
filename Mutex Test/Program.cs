// LFInteractive LLC. 2021-2024﻿
using System.IO.Pipes;

internal class Program
{
    private static void Main(string[] args)
    {
        string pipeName = "MyNamedPipe"; // This lets the client know which pipe to connect to.
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
        using NamedPipeClientStream clientPipe = new(".", pipeName, PipeDirection.Out); // a client pipe stream is used to send messages.
        try
        {
            clientPipe.Connect(); // attempts to connect to the server.
            using StreamWriter writer = new(clientPipe); // a stream writer is used to write messages to the server.
            writer.Write(string.Join(" ", args)); // join the arguments into a single string and send it to the server.
            writer.Flush(); // flush the stream writer to send the message.
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error sending arguments to existing instance: " + ex.Message); // shit when wrong!?!?!?!
        }
    }

    private static void StartNamedPipeServer(string pipeName)
    {
        Task.Run(() => // Creates a new thread to not lock the main thread.
        {
            while (true) // Creating an infinite loop to keep the server alive.
            {
                Console.WriteLine("Waiting for arguments!");
                using NamedPipeServerStream serverPipe = new(pipeName, PipeDirection.In); // a server pipe stream is used to receive messages.
                try
                {
                    serverPipe.WaitForConnection(); // this method blocks the thread until a client connects.
                    using (StreamReader reader = new(serverPipe))
                    {
                        string receivedArgs = reader.ReadToEnd(); // read the message from the client.
                        Console.WriteLine($"Received arguments: {receivedArgs}");
                    }
                    serverPipe.Disconnect(); // disconnect the client once the message is received.
                }
                catch
                {
                    // This always throws an exception when receiving a message. I don't know why.
                    // Ignore this exception.
                }
            }
        });
    }
}