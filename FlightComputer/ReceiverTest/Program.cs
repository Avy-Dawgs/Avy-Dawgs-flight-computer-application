using ReceiverInterface;
// See https://aka.ms/new-console-template for more information

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