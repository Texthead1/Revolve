using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using static PortalToUnity.Global;

namespace PortalToUnity
{
    public class DigitalPortal : PortalOfPower
    {
        public override bool IsDigital => true;

        public DigitalPortal() : base()
        {
        }

        public override bool PushTransfer(byte[] data, byte length = 0)
        {
            Cache(data);
            PTUManager.LogWarning($"OUTPUT ({(char)data[0]}): {BytesToHexString(data)}", LogPriority.Low);
            return DigitalPortalManager.WritePortalPipe.TransferWrite(data);
        }

        // temp
        public override bool WriteRaw(byte[] data) => false;

        public override void StartReading()
        {
        }

        public override void StopReading()
        {
        }
    }

    public static class DigitalPortalManager
    {
        public static InterProcessConnection ReadPortalPipe { get; private set; }
        public static InterProcessConnection WritePortalPipe { get; private set; }
        public static DigitalPortal DigitalPortal;

        public static async Task Initialize()
        {
            ReadPortalPipe = new InterProcessConnection("DPW_AR");
            WritePortalPipe = new InterProcessConnection("DPR_AW");

            ReadPortalPipe.OnConnected += Connected;
            ReadPortalPipe.OnDisconnected += Disconnected;
            ReadPortalPipe.OnResponse += ReportReceived;
            ReadPortalPipe.Connect();
            WritePortalPipe.Connect();

#if UNITY_EDITOR
            EditorApplication.quitting += Quitting;
#endif
            Application.quitting += Quitting;

            await ReestablishPipes();
        }

        private static async void Connected()
        {
            PortalOfPower portal = new DigitalPortal();
            PortalOfPower.OnRawAdded?.Invoke(portal);
            DigitalPortal = (DigitalPortal)portal;
#if UNITY_EDITOR
            FigureDebugger.Push(portal);
#endif
            await portal.SetUpPortal();
        }

        private static void Disconnected()
        {
            if (DigitalPortal != null)
            {
                bool exists = DigitalPortal.Destroy();
                PortalOfPower.OnRawRemoved?.Invoke(DigitalPortal);
                if (exists)
                    PortalOfPower.OnRemoved?.Invoke(DigitalPortal);
#if UNITY_EDITOR
            FigureDebugger.Pop(DigitalPortal);
#endif
                DigitalPortal = null;
            }
        }

        private static void ReportReceived(byte[] data)
        {
            DigitalPortal?.ReportReceived(data);
        }

        private static void Quitting()
        {
            //if (DigitalPortal != null)
            //    Disconnected();
            ReadPortalPipe.Stop();
            WritePortalPipe.Stop();
        }

        private static async Task ReestablishPipes()
        {
            while (true)
            {
                await Task.Delay(200);

                if (!Application.isPlaying) return;
                if (DigitalPortal != null) continue;

                ReadPortalPipe.OnConnected -= Connected;
                ReadPortalPipe.OnDisconnected -= Disconnected;
                ReadPortalPipe.OnResponse -= ReportReceived;
                ReadPortalPipe.Stop();
                WritePortalPipe.Stop();

                ReadPortalPipe = new InterProcessConnection("DPW_AR");
                WritePortalPipe = new InterProcessConnection("DPR_AW");

                ReadPortalPipe.OnConnected += Connected;
                ReadPortalPipe.OnDisconnected += Disconnected;
                ReadPortalPipe.OnResponse += ReportReceived;
                ReadPortalPipe.Connect();
                WritePortalPipe.Connect();
            }
        }
    }
}