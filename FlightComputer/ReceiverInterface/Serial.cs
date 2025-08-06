using System;
using System.IO.Ports;

namespace ReceiverInterface;

/// <summary>
/// Represents serial interface for getting data from a
/// receiver.
/// </summary>
public class ReceiverInterface : IDisposable
{
    private readonly SerialPort _serialPort;

    private readonly CancellationTokenSource _cts;

    private Task? _receiveTask;

    /// <summary>
    /// Invoked when data is received.
    /// </summary>
    public event EventHandler<string>? ReceivedData;
    
    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="portName"></param>
    public ReceiverInterface(string portName)
    {
        // TODO: add configuration class for these params
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

    /// <summary>
    /// Start receiving data.
    /// </summary>
    public void Start()
    {
        if (_serialPort.IsOpen)
        {
            return;
        }
        
        _serialPort.Open();
        
        _receiveTask = Task.Run(() => ReadLoop(_cts.Token));
    }

    /// <summary>
    /// Main loop for reading data.
    /// </summary>
    /// <param name="token"></param>
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

    /// <summary>
    /// Dispose of this object.
    /// </summary>
    public void Dispose()
    {
        _cts.Cancel();
        _receiveTask?.Wait();
    }
}