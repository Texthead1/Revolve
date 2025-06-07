using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using libusbK;
using UnityEditor;
using UnityEngine;

namespace PortalToUnity
{
    public static class Global
    {
        public const string PORTAL_TO_UNITY_TARGET_NAMESPACE = "PortalToUnity";

        public const int PORTAL_VENDOR_ID = 0x1430;
        public const int PORTAL_PRODUCT_ID = 0x0150;
        public const int PORTAL_PRODUCT_ID_XBOX360 = 0x1F17;
        public const int PORTAL_PRODUCT_ID_XBOXONE = 0x09AB;
        public const int REPORT_SIZE = 0x20;
        public const int FIGURE_INDICIES_COUNT = 0x10;
        public const int MAX_SIMULTANEOUS_TAGS_GEN1 = 0x04;
        public const int MAX_SIMULTANEOUS_TAGS_GEN2 = 0x08;

        public const int SALT_LENGTH = 0x35;
        public const int REGION_COUNT = 2;
        public const int BLOCK_SIZE = 0x10;
        public const int BLOCK_COUNT = 0x40;
        public const int TAG_HEADER_SIZE = BLOCK_SIZE * 2;
        public const int KEY_SIZE = TAG_HEADER_SIZE + 1 + SALT_LENGTH;
        public const int SECTOR_PERMISSION_0 = 0x690F0F0F;
        public const int SECTOR_PERMISSION_FULL = 0x69080F7F;
        public const int DATA_REGION0_OFFSET = 0x08;
        public const int DATA_REGION1_OFFSET = 0x24;
        public const int RFID_TARGET_ATQA = 0x0F01;
        public const byte RFID_TARGET_SAK = 0x81;

        // Just to guide the range in the original cyos data byte boundaries. There is no strict byte management needed however
        public const int CYOS_DATA_LENGTH = 0x45;

        // The audio configuration for the Traptanium Speaker, requires 8000hz mono audio
        public const int AUDIO_TARGET_SAMPLE_RATE = 8000;
        public const int AUDIO_TARGET_CHANNELS = 1;

        public static readonly byte[] XBOX360_DATA_HEADER = new byte[2] { 0x0B, 0x14 };
        public static readonly byte[] XBOX360_AUDIO_HEADER = new byte[2] { 0x0B, 0x17 };

        public static bool IsPortalOfPower(this KLST_DEVINFO_HANDLE info) => info.Common.Vid == PORTAL_VENDOR_ID && (info.Common.Pid == PORTAL_PRODUCT_ID || info.Common.Pid == PORTAL_PRODUCT_ID_XBOX360);
        public static string BytesToHexString(byte[] data) => string.Join(" ", data.Select(x => x.ToString("X2")));

        public static byte[] TrimTrailingZeros(byte[] data)
        {
            int lastIndex = Array.FindLastIndex(data, x => x != 0);

            if (lastIndex == -1)
                return new byte[0];

            byte[] trimmedArray = new byte[lastIndex + 1];
            Array.Copy(data, trimmedArray, lastIndex + 1);
            return trimmedArray;
        }

        public static byte[] AddZeroPadding(byte[] array, int amount)
        {
            if (amount == 0)
                return array;

            int length = array.Length;

            byte[] newArray = new byte[length + amount];
            Array.Copy(array, 0, newArray, amount, length);
            return newArray;
        }

        public static int BlockSizeOf<T>() where T : struct => Marshal.SizeOf<T>() / BLOCK_SIZE;

        public static bool IsAccessControlBlock(byte block) => (block % 4) == 3;

        // 420707233300200 is the base-29 10 digit max ((29^10) - 1)
        public static bool ValidTradingCardID(ulong value) => value <= 420707233300200 && value != 0;

#if UNITY_EDITOR
        public static string GetFriendlyToyCode(Skylander skylander)
        {
            static string Construct(string name, int value) => $"{name} ({value})";

            string assemblyDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            Assembly loadedAssembly = Assembly.LoadFrom(Path.Combine(assemblyDirectory, "Assembly-CSharp.dll"));

            var enumTypes = loadedAssembly.GetTypes().Where(t => t.IsEnum && t.GetCustomAttribute<MarkAsToyCodeAttribute>() != null);
            enumTypes = enumTypes.OrderBy(t => t.Namespace == PORTAL_TO_UNITY_TARGET_NAMESPACE ? 0 : 1).ToList();

            var findTarget = enumTypes.Where(x => x.Name == skylander.ToyCodeEnumType).FirstOrDefault();
            if (findTarget != null && findTarget != default)
            {
                if (Enum.GetValues(findTarget).Cast<int>().Contains(skylander.CharacterID))
                    return Construct(Enum.GetName(findTarget, (int)skylander.CharacterID), skylander.CharacterID);
            }

            foreach (Type type in enumTypes)
            {
                if (type.Name == skylander.ToyCodeEnumType) continue;

                if (Enum.GetValues(type).Cast<int>().Contains(skylander.CharacterID))
                    return Construct(Enum.GetName(type, (int)skylander.CharacterID), skylander.CharacterID);
            }
            return Construct("Unknown", skylander.CharacterID);
        }

        public static string GetFriendlyToyCode(ushort characterID, string toyCodeEnumType = "ToyCode")
        {
            static string Construct(string name, int value) => $"{name} ({value})";

            string assemblyDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            Assembly loadedAssembly = Assembly.LoadFrom(Path.Combine(assemblyDirectory, "Assembly-CSharp.dll"));

            var enumTypes = loadedAssembly.GetTypes().Where(t => t.IsEnum && t.GetCustomAttribute<MarkAsToyCodeAttribute>() != null);
            enumTypes = enumTypes.OrderBy(t => t.Namespace == PORTAL_TO_UNITY_TARGET_NAMESPACE ? 0 : 1).ToList();

            var findTarget = enumTypes.Where(x => x.Name == toyCodeEnumType).FirstOrDefault();
            if (findTarget != null && findTarget != default)
            {
                if (Enum.GetValues(findTarget).Cast<int>().Contains(characterID))
                    return Construct(Enum.GetName(findTarget, (int)characterID), characterID);
            }

            foreach (Type type in enumTypes)
            {
                if (type.Name == toyCodeEnumType) continue;

                if (Enum.GetValues(type).Cast<int>().Contains(characterID))
                    return Construct(Enum.GetName(type, (int)characterID), characterID);
            }
            return Construct("Unknown", characterID);
        }
#endif
        public static string SpyroTagToHexString<T>(T spyroTag) where T : struct
        {
            int size = Marshal.SizeOf(spyroTag);
            byte[] bytes = new byte[size];

            IntPtr ptr = Marshal.AllocHGlobal(size);

            try
            {
                Marshal.StructureToPtr(spyroTag, ptr, true);
                Marshal.Copy(ptr, bytes, 0, size);
            }
            finally
            {
                Marshal.FreeHGlobal(ptr);
            }

            StringBuilder hexString = new StringBuilder(size * 3);

            for (int i = 0; i < size; i++)
            {
                if (i > 0 && i % BLOCK_SIZE == 0)
                    hexString.AppendLine();

                hexString.Append($"{bytes[i]:X2} ");
            }
            return hexString.ToString();
        }

#if UNITY_EDITOR
        public static string GetSelectedPathOrFallback()
        {
            string path = AssetDatabase.GetAssetPath(Selection.activeObject);

            if (string.IsNullOrEmpty(path))
                return "Assets/Portal-To-Unity/";

            if (!AssetDatabase.IsValidFolder(path))
                path = Path.GetDirectoryName(path);

            return path;
        }
#endif

        public static unsafe (ushort, VariantID) GetCharacterAndVariantIDs(PortalFigure figure) => (figure.TagHeader->toyType, new VariantID(figure.TagHeader->subType));

        // Basic audio resampling implementation. Is inferior to Unity's Import Settings and other converters, suggest pre-converting your audio, or improving this method in future
        public static AudioClip ResampleAudioClipForTraptanium(AudioClip clip)
        {
            if (clip.frequency == AUDIO_TARGET_SAMPLE_RATE && clip.channels == AUDIO_TARGET_CHANNELS)
                return clip;

            float[] audioData = new float[clip.samples * clip.channels];
            clip.GetData(audioData, 0);

            float[] convertedData = ResampleAudio(audioData, clip.frequency, clip.channels);
            convertedData = ConvertChannels(convertedData, clip.channels);

            int newSampleCount = Mathf.FloorToInt((float)convertedData.Length / AUDIO_TARGET_CHANNELS);
            AudioClip newClip = AudioClip.Create($"{clip.name} (Resampled)", newSampleCount, AUDIO_TARGET_CHANNELS, AUDIO_TARGET_SAMPLE_RATE, false);
            newClip.SetData(convertedData, 0);

            return newClip;

            static float[] ConvertChannels(float[] inputData, int inputChannels)
            {
                if (inputChannels == AUDIO_TARGET_CHANNELS)
                    return inputData;

                int sampleCount = inputData.Length / inputChannels;
                float[] outputData = new float[sampleCount * AUDIO_TARGET_CHANNELS];

                if (inputChannels == 2)
                {
                    for (int i = 0; i < sampleCount; i++)
                        outputData[i] = (inputData[i * 2] + inputData[i * 2 + 1]) / 2;
                }
                else
                    PTUManager.LogError("Unsupported channel conversion. AudioClip must be Mono or Stereo for conversion.", LogPriority.High);

                return outputData;
            }

            static float[] ResampleAudio(float[] inputData, int inputSampleRate, int channels)
            {
                if (inputSampleRate == AUDIO_TARGET_SAMPLE_RATE)
                    return inputData;

                int inputSampleCount = inputData.Length / channels;
                int targetSampleCount = Mathf.FloorToInt(inputSampleCount * (AUDIO_TARGET_SAMPLE_RATE / (float)inputSampleRate));
                float[] outputData = new float[targetSampleCount * channels];

                for (int i = 0; i < targetSampleCount; i++)
                {
                    float inputIndex = i * (inputSampleRate / (float)AUDIO_TARGET_SAMPLE_RATE);
                    int index = Mathf.FloorToInt(inputIndex) * channels;

                    for (int channel = 0; channel < channels; channel++)
                    {
                        if (index < inputData.Length - channels)
                            outputData[i * channels + channel] = inputData[index + channel];
                    }
                }
                return outputData;
            }
        }

        public static string FlagsToString<T>(T flags) where T : Enum
        {
            if (Convert.ToInt32(flags) == 0)
                return "None";

            return string.Join(", ", Enum.GetValues(typeof(T)).Cast<T>().Where(flag => flags.HasFlag(flag) && Convert.ToInt32(flags) != 0));
        }

#if UNITY_EDITOR
        // reference: https://github.com/Unity-Technologies/UnityCsReference/blob/d6f29af28d9f82f07d2a29dc8484458adf861486/Editor/Mono/ProjectBrowser/ProjectWindowUtil.cs#L1046
        public static bool AskForDeletion(this ScriptableObject obj)
        {
            string title = "Delete selected asset?";

            string infoText = $"   {AssetDatabase.GetAssetPath(obj)}\n\nYou cannot undo the delete assets action.";
            int index = infoText.IndexOf("Assets/Resources/Portal-To-Unity/");

            infoText = infoText.Substring(0, index) + "Assets/Resources/Portal-To-Unity/" + infoText.Substring(index + "Assets/Resources/Portal-To-Unity/".Length);
            return EditorUtility.DisplayDialog(title, infoText, "Delete", "Cancel");
        }
#endif
        public static bool RangeInclusive(this int var, int lower, int upper) => var >= lower && var <= upper;

        public static string ToCamelCase(string input)
        {
            var words = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (words.Length == 0) return input;

            return string.Concat(words.Select(word => char.ToUpper(word[0]) + word.Substring(1)));
        }

        public static string RemoveDiacritics(string input)
        {
            string normalizedString = input.Normalize(NormalizationForm.FormD);
            return new string(normalizedString
                .Where(c => CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                .ToArray());
        }
        
        public static string RemoveSymbols(string input) => Regex.Replace(input, "[^a-zA-Z0-9]", "");
    }
}