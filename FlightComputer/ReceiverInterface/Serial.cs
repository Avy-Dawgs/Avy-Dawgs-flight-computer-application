using System;
using System.IO.Ports;

namespace ReceiverInterface;

public class ReceiverInterface : IDisposable
{
    private readonly SerialPort _serialPort;

    private readonly CancellationTokenSource _cts;

    private Task? _receiveTask;

    public event EventHandler<string>? ReceivedData;
    
    public ReceiverInterface(string portName)
    {
        _serialPort = new SerialPort()
        {
            PortName = portName,
            BaudRate = 115200, 
            Parity = Parity.None, 
            DataBits = 8, 
            StopBits = StopBits.One, 
            Handshake = Handshake.None, 
        };

        _cts = new();
    }

    public void Start()
    {
        if (_serialPort.IsOpen)
        {
            return;
        }
        
        _serialPort.Open();
        
        _receiveTask = Task.Run(() => ReadLoop(_cts.Token));
    }

    private void ReadLoop(CancellationToken token)
    {
        while (true)
        {
            if (token.IsCancellationRequested)
            {
                break;
            }

            string readString = _serialPort.ReadExisting();
            if (readString.Length > 0)
            {
                ReceivedData?.Invoke(this, readString);
            }
        }
    }

    public void Dispose()
    {
        _cts.Cancel();
        _receiveTask?.Wait();
    }
}