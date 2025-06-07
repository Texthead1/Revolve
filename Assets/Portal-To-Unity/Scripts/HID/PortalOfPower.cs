using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using libusbK;
using PimDeWitte.UnityMainThreadDispatcher;
using UnityEngine;
using static PortalToUnity.Global;

namespace PortalToUnity
{
    public enum PortalLED
    {
        Undefined = -1,
        Left = 0x00,
        Trap = 0x01,
        Right = 0x02
    }

    public enum PortalState
    {
        JustAdded,
        SetUpForInterface,
        CommunicatingWithAntenna,
        Sleeping,
        Ready
    }

    public class PortalOfPower
    {
        public static Action<PortalOfPower> OnAdded;
        public static Action<PortalOfPower> OnRemoved;
        public static Action<PortalOfPower> OnRawAdded;
        public static Action<PortalOfPower> OnRawRemoved;
        public static Action<PortalOfPower> OnPortalIOError;
        public static List<PortalOfPower> Instances = new List<PortalOfPower>();
        public static char[] priorityCommands = new char[] { 'A', 'M', 'Q', 'R', 'W' };

        public Action<PortalOfPower, byte[]> OnInputReport;
        public Action<PortalFigure> OnFigureAdded;
        public Action<PortalFigure, FigureDepartInfo> OnFigureRemoved;
        public Action<PortalFigure> OnFinishedReadingFigure;
        public Action<PortalOfPower, byte[]> OnInterference;
        public Action<PortalOfPower, char, Color32, PortalLED> OnAssumedLEDColorUpdate;
        public UsbK kDevice;
        public KLST_DEVINFO_HANDLE kHandle;
        public PortalFigure[] Figures { get; private set; } = new PortalFigure[FIGURE_INDICIES_COUNT];
        public List<PortalFigure> FiguresInQueue = new List<PortalFigure>();
        public PortalFigure currentlyQueryingFigure;
        public DateTime Timestamp { get; }
        public Color32[] AssumedPortalLEDColor;

        public byte[] ID { get; private set; } = new byte[4];
        public bool Active { get; private set; }
        public ulong SessionID { get; private set; }
        public virtual bool IsDigital => false;

        internal static ulong NextPortalSessionID => _nextSessionID++;
        private static ulong _nextSessionID = 0;

        protected bool isReading;
        internal PortalState State = PortalState.JustAdded;
        private CancellationTokenSource bootCTS;
        private CancellationTokenSource wirelessCTS = new CancellationTokenSource();
        private readonly List<CommandInfo> cachedCommands = new List<CommandInfo>();

        private class CommandInfo
        {
            public char commandChar;
            public DateTime timestamp;
            public bool returned;
            public byte[] extraInfo;

            public CommandInfo(char commandChar, DateTime timestamp, byte[] extraInfo = null)
            {
                this.commandChar = commandChar;
                this.timestamp = timestamp;
                returned = false;
                this.extraInfo = extraInfo;
            }

            public override string ToString() => $"CommandChar: {commandChar}, timestamp: {timestamp}, returned?: {returned}{(extraInfo != null ? $", extraInfo: ({BytesToHexString(extraInfo)})" : null)}";
        }

        protected PortalOfPower()
        {
            for (byte i = 0; i < FIGURE_INDICIES_COUNT; i++)
                Figures[i] = new PortalFigure(this, i);

            Timestamp = DateTime.Now;
            SessionID = NextPortalSessionID;
            AssumedPortalLEDColor = new Color32[] { new Color32(0, 0, 0, 0) };
        }

        public PortalOfPower(KLST_DEVINFO_HANDLE handle)
        {
            kDevice = new UsbK(handle);
            kHandle = handle;

            for (byte i = 0; i < FIGURE_INDICIES_COUNT; i++)
                Figures[i] = new PortalFigure(this, i);

            Timestamp = DateTime.Now;
            SessionID = NextPortalSessionID;
            AssumedPortalLEDColor = new Color32[] { new Color32(0, 0, 0, 0) };
        }

        public static PortalOfPower WithHandle(KLST_DEVINFO_HANDLE handle) => Instances.FirstOrDefault(portal => portal.kHandle.Equals(handle));

        public virtual string GetName() => PortalDatabase.NameFromID(ID);
        public PortalInfo GetPortalInfo() => PortalDatabase.PortalFromID(ID);

        public bool GetPortalInfo(out PortalInfo info)
        {
            info = GetPortalInfo();
            return info == null;
        }

        private void UpdateAssumedLEDColor(char commandChar = 'C', byte r = 0, byte g = 0, byte b = 0, PortalLED led = PortalLED.Undefined)
        {
            void DoCallback(char commandChar, Color32 color, PortalLED led = PortalLED.Undefined) => OnAssumedLEDColorUpdate?.Invoke(this, commandChar, color, led);

            Color32 color = new Color32(0, 0, 0, 0);
            PortalInfo info = GetPortalInfo();

            if (State == PortalState.Sleeping)
            {
                AssumedPortalLEDColor[0] = color;
                DoCallback('\0', color, led);
                return;
            }

            if (info == null)
            {
                color = new Color32(r, g, b, 0);
                AssumedPortalLEDColor[0] = color;
                DoCallback(commandChar, color, led);
                return;
            }

            if (info.LEDType == LEDType.Enhanced)
            {
                PortalLED portalLED = PortalLED.Undefined;
                switch (commandChar)
                {
                    case 'B':
                        AssumedPortalLEDColor[(int)PortalLED.Left] = AssumedPortalLEDColor[(int)PortalLED.Right] = color;
                        break;

                    case 'C':
                        color = new Color32(r, g, b, 0xFF);
                        AssumedPortalLEDColor[(int)PortalLED.Left] = AssumedPortalLEDColor[(int)PortalLED.Right] = color;
                        break;

                    case 'J':
                        if (led == PortalLED.Left || led == PortalLED.Right)
                        {
                            portalLED = led;
                            color = new Color32(r, g, b, 0xFF);
                            AssumedPortalLEDColor[(int)led] = color;
                            break;
                        }
                        return;

                    case 'L':
                        if (led == PortalLED.Left || led == PortalLED.Right)
                        {
                            portalLED = led;
                            color = new Color32(r, g, b, 0xFF);
                            AssumedPortalLEDColor[(int)led] = color;
                            break;
                        }
                        else if (led == PortalLED.Trap)
                        {
                            portalLED = led;
                            color = new Color32(r, r, r, 0xFF);
                            AssumedPortalLEDColor[(int)led] = color;
                            break;
                        }
                        return;
                }
                DoCallback(commandChar, color, portalLED);
                return;
            }

            if (commandChar != 'C') return;
            switch (info.LEDType)
            {
                case LEDType.None:
                    color = new Color32(r, g, b, 0);
                    AssumedPortalLEDColor[0] = color;
                    break;

                case LEDType.FullColor:
                    color = new Color32(r, g, b, 0xFF);
                    AssumedPortalLEDColor[0] = color;
                    break;

                case LEDType.BlueOnly:
                    color = new Color32(0, 0, b, 0xFF);
                    AssumedPortalLEDColor[0] = color;
                    break;
            }
            DoCallback('C', color);
        }

        internal async Task SetUpPortal()
        {
            int timeout = 0;
            bootCTS = new CancellationTokenSource();
            State = PortalState.SetUpForInterface;
            OnInputReport += SetUpPortalSub;

            while (true)
            {
                if (timeout == 10)
                {
                    OnPortalIOError?.Invoke(this);
                    OnInputReport -= SetUpPortalSub;
                    return;
                }
                COMMAND_ResetPortal();

                try
                {
                    await Task.Delay(80 + (timeout * 20), bootCTS.Token);
                }
                catch (OperationCanceledException) { break; }
                timeout++;
            }

            if (State == PortalState.Sleeping)
            {
                OnInputReport -= SetUpPortalSub;
                return;
            }

            timeout = 0;
            bootCTS = new CancellationTokenSource();

            while (true)
            {
                if (timeout == 5)
                {
                    OnPortalIOError?.Invoke(this);
                    OnInputReport -= SetUpPortalSub;
                    return;
                }
                COMMAND_SetAntenna(true);

                try
                {
                    await Task.Delay(80 + (timeout * 20), bootCTS.Token);
                }
                catch (OperationCanceledException) { break; }
                timeout++;
            }

            if (State == PortalState.Sleeping)
            {
                OnInputReport -= SetUpPortalSub;
                return;
            }

            OnInputReport -= SetUpPortalSub;
            Instances.Add(this);
            OnAdded?.Invoke(this);

            void SetUpPortalSub(PortalOfPower _, byte[] data)
            {
                if (data[0] == (byte)'R')
                {
                    if (State == PortalState.SetUpForInterface)
                    {
                        bootCTS.Cancel();
                        State = PortalState.CommunicatingWithAntenna;
                    }
                }
                else if (data[0] == (byte)'A')
                {
                    if (State == PortalState.CommunicatingWithAntenna)
                    {
                        bootCTS.Cancel();
                        State = PortalState.Ready;
                    }
                }
                else if (data[0] == (byte)'Z' && State >= PortalState.CommunicatingWithAntenna)
                {
                    bootCTS.Cancel();
                    State = PortalState.Sleeping;
                }
            }
        }

        private byte[] ConstructCommand(char commandChar, params byte[] commandArgs)
        {
            byte[] command = new byte[REPORT_SIZE];
            command[0] = (byte)commandChar;
            Array.Copy(commandArgs, 0, command, 1, commandArgs.Length);
            return command;
        }

        public void COMMAND_SetAntenna(bool active)
        {
            byte[] command = ConstructCommand
            (
                'A',
                (byte)(active ? 0x01 : 0x00)
            );
            PushTransfer(command, 2);
        }

        public void COMMAND_RestoreBasicColorCycle()
        {
            byte[] command = ConstructCommand('B');
            PushTransfer(command, 1);
            UpdateAssumedLEDColor('B');
        }

        public void COMMAND_SetLEDColor(byte r, byte g, byte b)
        {
            byte[] command = ConstructCommand
            (
                'C',
                r,
                g,
                b
            );
            PushTransfer(command, 4);
            UpdateAssumedLEDColor('C', r, g, b);
        }

        public void COMMAND_SetLEDColor(Color32 color) => COMMAND_SetLEDColor(color.r, color.g, color.b);

        public void COMMAND_SetTraptaniumLEDColor(PortalLED side, byte r, byte g, byte b, short transitionTime)
        {
            if (side == PortalLED.Trap)
                throw new ArgumentException("Traptanium Portal LED controlled via J command must either be left or right.");

            if (transitionTime == 0)
                PTUManager.LogWarning("Transition time set to 0. Use the L command instead if you wish to set a Traptanium LED to a given color with no transition.", LogPriority.Normal);

            byte[] command = ConstructCommand
            (
                'J',
                (byte)side,
                r,
                g,
                b,
                (byte)(transitionTime & 0xFF),
                (byte)(transitionTime >> 0x08)
            );
            PushTransfer(command, 7);
            UpdateAssumedLEDColor('J', r, g, b, side);
        }

        public void COMMAND_SetTraptaniumLEDColor(PortalLED side, Color32 color, short transitionTime) => COMMAND_SetTraptaniumLEDColor(side, color.r, color.g, color.b, transitionTime);

        public void COMMAND_SetTraptaniumLight(PortalLED led, byte r, byte g = 0, byte b = 0, byte unknown = 0)
        {
            byte[] command = ConstructCommand
            (
                'L',
                (byte)led,
                r,
                g,
                b,
                unknown
            );
            PushTransfer(command, 6);
            UpdateAssumedLEDColor('L', r, g, b, led);
        }

        public void COMMAND_SetTraptaniumLight(PortalLED led, Color32 color, byte unknown = 0) => COMMAND_SetTraptaniumLight(led, color.r, color.g, color.b, unknown);

        public void COMMAND_SetTraptaniumSpeaker(bool active)
        {
            byte[] command = ConstructCommand
            (
                'M',
                (byte)(active ? 0x01 : 0x00)
            );
            PushTransfer(command, 2);
        }

        public void COMMAND_FigureQuery(byte index, byte block)
        {
            byte[] command = ConstructCommand
            (
                'Q',
                index,
                block
            );
            PushTransfer(command, 3);
        }

        public void COMMAND_ResetPortal()
        {
            byte[] command = ConstructCommand('R');
            PushTransfer(command, 1);
        }

        public void COMMAND_RequestStatus()
        {
            byte[] command = ConstructCommand('S');
            PushTransfer(command, 1);
        }

        public void COMMAND_SetLightAudioVibrancyTolerance(bool active, byte tolerance, byte unk = 0)
        {
            byte[] command = ConstructCommand
            (
                'V',
                unk,
                (byte)(active ? 0x01 : 0x00),
                tolerance
            );
            PushTransfer(command, 4);
        }

        public void COMMAND_FigureWrite(byte index, byte block, byte[] data)
        {
            if (data.Length != BLOCK_SIZE)
                throw new ArgumentException("Data array must be 16 bytes long.");

            byte[] command = ConstructCommand
            (
                'W',
                index,
                block
            );
            Array.Copy(data, 0, command, 3, 0x10);
            PushTransfer(command, 19);
        }

        // there are references to a 0xAA command in the TFB Portal logs in Skylanders SuperChargers Racing, investigate that
        // it seems relevant to the status command, but doesn't seem to specify the portal
        public void COMMAND_RequestWirelessDongleFirmwareVersion()
        {
            byte[] command = ConstructCommand((char)0xFA);
            PushTransfer(command, 1);
        }

        public async Task<byte[]> QueryFigureAsync(byte index, byte block)
        {
            if (!Figures[index].IsPresent())
                throw new FigureRemovedException("The requested figure does not exist on the Portal of Power.");

            bool query = true;
            bool retryQuery = false;
            byte timeout = 0;
            byte retryCount = 0;
            object tcsLock = new object();

            CancellationTokenSource cts = new CancellationTokenSource();
            TaskCompletionSource<byte[]> tcs = new TaskCompletionSource<byte[]>();

            void SetResult(TaskCompletionSource<byte[]> tcs, object lockObj, Action<TaskCompletionSource<byte[]>> action)
            {
                if (tcs.Task.IsCompleted)
                    return;

                lock (lockObj)
                {
                    if (!tcs.Task.IsCompleted)
                        action(tcs);
                }
                OnInputReport -= HandleReport;
                cts?.Cancel();
            }

            async void HandleReport(PortalOfPower portal, byte[] input)
            {
                try
                {
                    if (input[0] == (byte)'Q')
                    {
                        if ((input[1] & 0x10) != 0 && (input[1] & 0xF) == index && input[2] == block)
                        {
                            byte[] result = new byte[BLOCK_SIZE];
                            Array.Copy(input, 3, result, 0, BLOCK_SIZE);
                            SetResult(tcs, tcsLock, x => x.TrySetResult(result));
                        }
                        else if (retryCount < 2)
                        {
                            if (!query) return;
                            retryCount++;
                            query = false;
                            await Task.Delay(80, cts.Token);
                            COMMAND_RequestStatus();
                        }
                        else
                            SetResult(tcs, tcsLock, x => x.TrySetException(new FigureErrorException("The figure being queried could not be read successfully.")));
                    }

                    if (input[0] != (byte)'S')
                        return;

                    query = true;
                    ulong figurePresences = (ulong)(input[1] | input[2] << 0x08 | input[3] << 0x10 | input[4] << 0x18);
                    FigurePresence figurePresence = (FigurePresence)((figurePresences >> (index * 2)) & 0b11);

                    if (figurePresence == FigurePresence.Present || figurePresence == FigurePresence.JustArrived)
                    {
                        if (!retryQuery)
                        {
                            retryQuery = true;
                            try
                            {
                                while (true)
                                {
                                    // initial delay to circumvent traptanium portal glitch, plus it's plenty fast anyways
                                    await Task.Delay(80, cts.Token);
                                    COMMAND_FigureQuery(index, block);
                                    await Task.Delay(20);
                                }
                            }
                            catch (TaskCanceledException) { }
                        }
                    }
                    else
                        SetResult(tcs, tcsLock, x => x.TrySetException(new FigureRemovedException("The figure being queried was removed.")));
                }
                catch (Exception ex)
                {
                    SetResult(tcs, tcsLock, x => x.TrySetException(ex));
                }
            }

            OnInputReport += HandleReport;

            while (!tcs.Task.IsCompleted && timeout < 4)
            {
                if (!Instances.Contains(this))
                {
                    SetResult(tcs, tcsLock, x => x.TrySetException(new PortalDisconnectedException("The parent Portal of Power was disconnected.")));
                    break;
                }

                COMMAND_FigureQuery(index, block);
                try
                {
                    await Task.Delay(450, cts.Token);
                }
                catch (TaskCanceledException) { break; }
                timeout++;
            }

            if (timeout >= 4)
                SetResult(tcs, tcsLock, x => x.TrySetException(new PortalIOException("No data was responded by the Portal of Power.")));

            try
            {
                return await tcs.Task;
            }
            finally { cts.Dispose(); }
        }

        public async Task<bool> WriteFigureAsync(byte index, byte block, byte[] data)
        {
            if (!Figures[index].IsPresent())
                throw new FigureRemovedException("The requested figure does not exist on the Portal of Power.");

            bool write = true;
            bool retryWrite = false;
            byte timeout = 0;
            byte retryCount = 0;
            object tcsLock = new object();

            TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();
            CancellationTokenSource cts = new CancellationTokenSource();

            void SetResult(TaskCompletionSource<bool> tcs, object lockObj, Action<TaskCompletionSource<bool>> action)
            {
                if (tcs.Task.IsCompleted)
                    return;

                lock (lockObj)
                {
                    if (!tcs.Task.IsCompleted)
                        action(tcs);
                }
                OnInputReport -= HandleReport;
                cts?.Cancel();
            }

            async void HandleReport(PortalOfPower portal, byte[] input)
            {
                try
                {
                    if (input[0] == (byte)'W')
                    {
                        if ((input[1] & 0x10) != 0 && (input[1] & 0xF) == index && input[2] == block)
                            SetResult(tcs, tcsLock, x => x.TrySetResult(true));
                        else if (retryCount < 5)
                        {
                            if (!write) return;
                            retryCount++;
                            write = false;
                            await Task.Delay(80, cts.Token);
                            COMMAND_RequestStatus();
                        }
                        else
                            SetResult(tcs, tcsLock, x => x.TrySetException(new FigureErrorException("The figure being written to could not be read successfully.")));
                    }

                    if (input[0] != (byte)'S')
                        return;

                    write = true;
                    ulong figurePresences = (ulong)(input[1] | input[2] << 0x08 | input[3] << 0x10 | input[4] << 0x18);
                    FigurePresence figurePresence = (FigurePresence)((figurePresences >> (index * 2)) & 0b11);

                    if (figurePresence == FigurePresence.Present || figurePresence == FigurePresence.JustArrived)
                    {
                        if (!retryWrite)
                        {
                            retryWrite = true;
                            try
                            {
                                while (true)
                                {
                                    // initial delay to circumvent traptanium portal glitch, plus it's plenty fast anyways
                                    await Task.Delay(80, cts.Token);
                                    COMMAND_FigureWrite(index, block, data);
                                    await Task.Delay(20);
                                }
                            }
                            catch (TaskCanceledException) { }
                        }
                    }
                    else
                        SetResult(tcs, tcsLock, x => x.TrySetException(new FigureRemovedException("The figure being written to was removed.")));
                }
                catch (Exception ex)
                {
                    SetResult(tcs, tcsLock, x => x.TrySetException(ex));
                }
            }

            OnInputReport += HandleReport;

            while (!tcs.Task.IsCompleted && timeout < 4)
            {
                if (!Instances.Contains(this))
                {
                    SetResult(tcs, tcsLock, x => x.TrySetException(new PortalDisconnectedException("The parent Portal of Power was disconnected.")));
                    break;
                }

                COMMAND_FigureWrite(index, block, data);
                try
                {
                    await Task.Delay(450, cts.Token);
                }
                catch (TaskCanceledException) { break; }
                timeout++;
            }

            if (timeout >= 4)
                SetResult(tcs, tcsLock, x => x.TrySetException(new PortalIOException("No data was responded by the Portal of Power.")));

            try
            {
                return await tcs.Task;
            }
            finally { cts.Dispose(); }
        }

        public virtual void PlayAudio(AudioClip audioClip)
        {
            PortalInfo portalInfo = GetPortalInfo();
            if (portalInfo == null || !portalInfo.AdditionalHardwarePieces.HasFlag(HardwarePieces.Speaker))
            {
                PTUManager.LogWarning($"{GetName()} cannot play audio as it does not have a speaker or one cannot be verified due to a missing corresponding PortalInfo at \"Assets/Resources/Portal-To-Unity/Portals/\".", LogPriority.High);
                return;
            }

            const int chunkSize = 16;
            int currentPosition = 0;
            int sampleCount = audioClip.samples;
            float[] samples = new float[chunkSize];
            byte[] data = new byte[chunkSize * 2];

            PTUManager.Log($"Started playing {audioClip.name} on {GetName()}.", LogPriority.Normal);
            while (currentPosition < sampleCount)
            {
                audioClip.GetData(samples, currentPosition);

                for (int num = 0; num < samples.Length; num++)
                {
                    if (currentPosition + num > sampleCount)
                        break;

                    ushort sample16 = (ushort)(Mathf.Clamp(samples[num] * 0.5f, -1f, 1f) * 32767f);
                    Array.Copy(BitConverter.GetBytes(sample16), 0, data, num * 2, 2);
                }

                if (!WriteRaw(data))
                {
                    PTUManager.LogWarning("Audio transmission terminated. Portal write error, most likely as a result of it being disconnected.", LogPriority.Normal);
                    return;
                }
                currentPosition += chunkSize;
            }
            PTUManager.Log($"Finished playing {audioClip.name} on {GetName()}.", LogPriority.Normal);
        }

        // todo: rewrite
        public async Task PlayTraptaniumAudio(AudioClip audioClip, float audioMult = 1)
        {
            PortalInfo portalInfo = GetPortalInfo();
            if (portalInfo == null || !portalInfo.AdditionalHardwarePieces.HasFlag(HardwarePieces.Speaker))
            {
                PTUManager.LogWarning($"{GetName()} cannot play audio as it does not have a speaker or one cannot be verified due to a missing PortalInfo at \"Assets/Resources/Portal-To-Unity/Portals/\".", LogPriority.High);
                return;
            }

            const int CHUNK_SIZE = 16;
            const int CHUNK_COUNT = 8;
            const int TRANSFER_SIZE = CHUNK_SIZE * CHUNK_COUNT;
            int position = 0;
            int sampleCount = audioClip.samples;
            float[] samples = new float[TRANSFER_SIZE];
            byte[] sampleData = new byte[TRANSFER_SIZE * 2];

            PTUManager.Log($"Started playing {audioClip.name} on {GetName()}.", LogPriority.Normal);
            await UnityMainThreadDispatcher.Instance().EnqueueAsync(() => audioClip.GetData(samples, position));

            new Thread(async () =>
            {
                while (position < sampleCount)
                {
                    float[] nextSamples = new float[TRANSFER_SIZE];
                    
                    await UnityMainThreadDispatcher.Instance().EnqueueAsync(() =>
                    {
                        // Check that we're not exceeding past the amount of samples present in the file when grabbing future ones
                        if (sampleCount - (position + TRANSFER_SIZE) >= 0)
                            audioClip.GetData(nextSamples, position + TRANSFER_SIZE);
                    });

                    for (int i = 0; i < CHUNK_COUNT; i++)
                    {
                        int offset = i * CHUNK_SIZE;

                        for (int j = 0; j < CHUNK_SIZE; j++)
                        {
                            int sampleIndex = offset + j;
                            if (position + sampleIndex >= sampleCount)
                                break;

                            ushort sample = (ushort)(Mathf.Clamp(samples[sampleIndex] * audioMult, -1f, 1f) * 32767f);
                            Array.Copy(BitConverter.GetBytes(sample), 0, sampleData, sampleIndex * 2, 2);
                        }
                        byte[] transferData = new byte[CHUNK_SIZE * 2];
                        Array.Copy(sampleData, offset * 2, transferData, 0, CHUNK_SIZE * 2);

                        if (!WriteRaw(transferData))
                            throw new IOException("Could not transfer AudioClip data successfully");
                    }
                    position += TRANSFER_SIZE;
                    samples = nextSamples;
                }
            }).Start();
            PTUManager.Log($"Finished playing {audioClip.name} on {GetName()}.", LogPriority.Normal);
        }

        // HID READ/WRITE RELATED FUNCTIONS

        public virtual bool PushTransfer(byte[] data, byte length = 0)
        {
            Cache(data);
            PTUManager.LogWarning($"OUTPUT ({(char)data[0]}): {BytesToHexString(data)}", LogPriority.Low);
            return kDevice.ControlTransfer(SetupPacket(length), data, length, out _, IntPtr.Zero);
        }

        public virtual bool WriteRaw(byte[] data) => kDevice.WritePipe(0x02, data, (uint)data.Length, out _, IntPtr.Zero);

        private WINUSB_SETUP_PACKET SetupPacket(byte length) => new WINUSB_SETUP_PACKET
        {
            RequestType = 0x21,
            Request = 0x09,
            Value = 0x0200,
            Index = 0x0000,
            Length = (ushort)(0x0008 + length)
        };

        private void EvaluateCommandCache()
        {
            DateTime update = DateTime.Now;

            if (cachedCommands.Count > 0)
            {
                PTUManager.Log("Cached commands before filter:", LogPriority.Debug);
                foreach (CommandInfo command in cachedCommands)
                    PTUManager.Log(command.ToString(), LogPriority.Debug);

                cachedCommands.RemoveAll(x => (update - x.timestamp).TotalMilliseconds >= (x.returned ? 400 : 7500));

                PTUManager.Log("Cached commands after filter:", LogPriority.Debug);

                if (cachedCommands.Count > 0)
                {
                    foreach (CommandInfo command in cachedCommands)
                        PTUManager.Log(command.ToString(), LogPriority.Debug);
                    return;
                }
                PTUManager.Log("(EMPTY)", LogPriority.Debug);
            }
        }

        protected void Cache(byte[] input)
        {
            char commandChar = (char)input[0];
            if (!priorityCommands.Contains(commandChar)) return;

            EvaluateCommandCache();

            // for these commands we also want to store and check against the figure index and requested block
            if (commandChar == 'Q' || commandChar == 'W')
            {
                byte[] sequenceInfo = new byte[2] { (byte)(input[1] & 0xF), input[2] };
                List<CommandInfo> matches = cachedCommands.Where(x => x.commandChar == commandChar).ToList();
                CommandInfo command = new CommandInfo(commandChar, DateTime.Now, sequenceInfo);

                if (matches.Count > 0)
                {
                    foreach (CommandInfo match in matches)
                    {
                        if (match.extraInfo.SequenceEqual(sequenceInfo))
                        {
                            UpdateCommand(command, match);
                            return;
                        }
                    }
                }
                cachedCommands.Add(command);
            }
            else
            {
                CommandInfo command = new CommandInfo(commandChar, DateTime.Now);
                if (cachedCommands.Any(x => x.commandChar == commandChar))
                {
                    UpdateCommand(command, cachedCommands.FirstOrDefault(x => x.commandChar == commandChar));
                    return;
                }
                cachedCommands.Add(command);
            }
        }

        private void UpdateCommand(CommandInfo commandIn, CommandInfo cachedCommand)
        {
            cachedCommand.timestamp = commandIn.timestamp;
            cachedCommand.returned = false;
        }

        private bool CheckCommandCache(byte[] input)
        {
            char commandChar = (char)input[0];
            if (!priorityCommands.Contains(commandChar))
                return false;

            EvaluateCommandCache();

            if (commandChar == 'Q' || commandChar == 'W')
            {
                byte[] sequenceInfo = new byte[2] { (byte)(input[1] & 0xF), input[2] };
                List<CommandInfo> matches = cachedCommands.Where(x => x.commandChar == commandChar).ToList();
                CommandInfo command = new CommandInfo(commandChar, DateTime.Now, sequenceInfo);

                if (matches.Count > 0)
                {
                    foreach (CommandInfo match in matches)
                    {
                        if (match.extraInfo.SequenceEqual(sequenceInfo))
                        {
                            PTUManager.Log($"Found {commandChar} command in queue with matching info ({BytesToHexString(sequenceInfo)})", LogPriority.Debug);
                            match.returned = true;
                            return false;
                        }
                    }
                }
                PTUManager.Log($"Did not find {commandChar} command in queue with matching info ({BytesToHexString(sequenceInfo)})", LogPriority.Debug);
                OnInterference.Invoke(this, input);
                return true;
            }
            else
            {
                CommandInfo command = new CommandInfo(commandChar, DateTime.Now);
                if (cachedCommands.Any(x => x.commandChar == commandChar))
                {
                    PTUManager.Log($"Found {commandChar} command in queue", LogPriority.Debug);
                    cachedCommands.FirstOrDefault(x => x.commandChar == commandChar).returned = true;
                    return false;
                }
                PTUManager.Log($"Did not find {commandChar} command in queue", LogPriority.Debug);
                OnInterference.Invoke(this, input);
                return true;
            }
        }

        public virtual async void StartReading()
        {
            isReading = true;
            byte[] buffer = new byte[REPORT_SIZE];

            while (isReading)
            {
                /*
                 * hopefully fixes an issue where the c# libusbK wrapper can return multiple stale pipe reads when the device is unplugged but before the removal is detected
                 * the small delay should negate the race condition and the minimal delay shouldn't be impactful
                 */

                await Task.Delay(3);

                if (await Task.Run(() => kDevice.ReadPipe(0x81, buffer, (uint)buffer.Length, out _, IntPtr.Zero)))
                    ReportReceived(buffer);

                Array.Clear(buffer, 0, buffer.Length);
            }
        }

        public virtual void StopReading() => isReading = false;

        public virtual bool Destroy()
        {
            bootCTS?.Cancel();
            bootCTS?.Dispose();
            wirelessCTS?.Cancel();
            wirelessCTS?.Dispose();

            if (Instances.Contains(this))
            {
                Instances.Remove(this);
                for (int i = 0; i < FIGURE_INDICIES_COUNT; i++)
                {
                    OnFigureRemoved?.Invoke(Figures[i], FigureDepartInfo.ParentPortalDisconnected);
                    Figures[i].Dispose();
                }
                return true;
            }
            return false;
        }

        public async void ReportReceived(byte[] data)
        {
            char commandChar = (char)data[0];

            if (commandChar != 'S')
                PTUManager.Log($"INPUT ({commandChar}): {BytesToHexString(data)}", LogPriority.Low);

            if (CheckCommandCache(data)) return;

            if (data[0] == (byte)'Z')
            {
                if (State != PortalState.Sleeping)
                {
                    State = PortalState.Sleeping;
                    for (int i = 0; i < FIGURE_INDICIES_COUNT; i++)
                    {
                        OnFigureRemoved?.Invoke(Figures[i], FigureDepartInfo.ParentPortalDisconnected);
                        Figures[i].Reset();
                    }
                    Instances.Remove(this);
                    UpdateAssumedLEDColor();
                    OnRemoved?.Invoke(this);
                }
                WakeQueue();
            }
            else if (data[0] != 0xFA && !Instances.Contains(this) && State == PortalState.Sleeping)
                await SetUpPortal();

            switch (commandChar)
            {
                case 'A':
                    Active = data[1] != 0x00;
                    break;

                case 'R':
                    if (State >= PortalState.Sleeping)
                    {
                        for (int i = 0; i < FIGURE_INDICIES_COUNT; i++)
                        {
                            PortalFigure figure = Figures[i];
                            OnFigureRemoved?.Invoke(figure, FigureDepartInfo.PortalIndiciesReset);
                            figure.Reset();
                        }
                        currentlyQueryingFigure = null;
                        FiguresInQueue.Clear();
                    }

                    // accomodate for weird wireless dongle "bug" (or whatever returning this id is meant to signify)
                    if (data[1] == 0x90 && data[2] == 0x00) break;

                    byte[] bytes = new byte[4];
                    Array.Copy(data, 1, bytes, 0, 4);
                    ID = TrimTrailingZeros(bytes);

                    if (GetPortalInfo().LEDType == LEDType.Enhanced)
                        AssumedPortalLEDColor = new Color32[3];

                    break;

                case 'S':
                    if (State < PortalState.Sleeping) break;
                    ulong figurePresences = (ulong)(data[1] | data[2] << 0x08 | data[3] << 0x10 | data[4] << 0x18);

                    for (int i = 0; i < FIGURE_INDICIES_COUNT; i++)
                    {
                        FigurePresence presence = (FigurePresence)((figurePresences >> (i * 2)) & 0b11);
                        PortalFigure figure = Figures[i];

                        switch (presence)
                        {
                            case FigurePresence.NotPresent:
                            case FigurePresence.JustDeparted:
                                if (figure.IsPresent())
                                {
                                    figure.Presence = presence;

                                    // add a proper method to free resources
                                    figure.Reset();
#if UNITY_EDITOR
                                    FigureDebugger.Pop(figure);
#endif
                                    OnFigureRemoved?.Invoke(figure, FigureDepartInfo.RemovedFromPortal);
                                }
                                break;

                            case FigurePresence.Present:
                            case FigurePresence.JustArrived:
                                if (!figure.IsPresent())
                                {
                                    figure.Presence = presence;
                                    OnFigureAdded?.Invoke(figure);
                                }
                                break;
                        }
                    }
                    break;
            }
            OnInputReport?.Invoke(this, data);
        }

        private async void WakeQueue()
        {
            wirelessCTS?.Cancel();
            wirelessCTS = new CancellationTokenSource();

            try
            {
                await Task.Delay(150, wirelessCTS.Token);
                if (State == PortalState.Sleeping)
                    await SetUpPortal();
            }
            catch (TaskCanceledException) {}
        }
    }
}