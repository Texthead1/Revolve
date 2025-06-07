using System;
using System.IO.Pipes;
using UnityEngine;
using static PortalToUnity.Global;

namespace PortalToUnity
{
    public class InterProcessConnection : IDisposable
    {
        public PipeStream Pipe;
        public byte[] Data = new byte[REPORT_SIZE];

        public Action OnConnected;
        public Action OnDisconnected;
        public Action<byte[]> OnResponse;

        private bool connected;
        private readonly string pipeName;

        public InterProcessConnection(string pipeName)
        {
            this.pipeName = pipeName;
        }

        public void Connect()
        {
            if (!Application.isPlaying) return;

            NamedPipeClientStream pipe = new NamedPipeClientStream(".", pipeName, PipeDirection.InOut, PipeOptions.Asynchronous | PipeOptions.WriteThrough);

            try
            {
                pipe.Connect(10);
            }
            catch (Exception) { return; }

            pipe.ReadMode = PipeTransmissionMode.Message;

            Data = new byte[REPORT_SIZE];
            Pipe = pipe;
            connected = true;
            OnConnected?.Invoke();
            StartReading();
        }

        public void Stop()
        {
            if (!connected) return;

            connected = false;
            Pipe?.Close();
        }

        private void StartReading()
        {
            if (Pipe.IsConnected)
            {
                try
                {
                    Pipe.BeginRead(Data, 0, REPORT_SIZE, ManageReport, Pipe);
                }
                catch (Exception) { goto fail; }
                return;
            }

            fail:
            connected = false;
            Pipe.Close();
            OnDisconnected?.Invoke();
        }

        private void ManageReport(IAsyncResult result)
        {
            if (result.AsyncState == null) return;

            if (Pipe.EndRead(result) != 0)
                OnResponse?.Invoke(Data);

            StartReading();
        }

        public bool TransferWrite(byte[] data)
        {
            if (!connected)
                return false;

            try
            {
                Pipe.BeginWrite(data, 0, data.Length, WriteComplete, Pipe);
            }
            catch (Exception)
            {
                Pipe.Close();
                return false;
            }
            return true;
        }

        private void WriteComplete(IAsyncResult result) => ((PipeStream)result.AsyncState)?.EndWrite(result);

        public void Dispose()
        {
            Pipe?.Dispose();
        }
    }
}