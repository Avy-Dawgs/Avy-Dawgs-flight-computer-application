/*
 * Simple test of receiving data over a serial port.
 */

using ReceiverInterface;

Console.WriteLine("Starting.");

ReceiverInterface.ReceiverInterface receiver = new(args[0]);

receiver.ReceivedData += Handler;
receiver.Start();

while (true)
{
    
}

void Handler(object? sender, string receivedData)
{
    Console.Write(receivedData);
}