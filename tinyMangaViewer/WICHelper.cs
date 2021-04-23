﻿using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tinyMangaViewer
{
    public static class WICHelper
    {
        /// <summary>
        /// GUID of the component registration group for WIC decoders
        /// </summary>
        private const string WICDecoderCategory = "{7ED96837-96F0-4812-B211-F13C24117ED3}";

        /// <summary>
        /// Represents information about a WIC decoder
        /// </summary>
        public struct DecoderInfo
        {
            public string FriendlyName;
            public string[] FileExtensions;
        }

        public static Lazy<IEnumerable<DecoderInfo>> Info = new Lazy<IEnumerable<DecoderInfo>>(() =>
            ImageCodecInfo.GetImageDecoders().Select(decoder => new DecoderInfo()
            {
                FriendlyName = decoder.CodecName,
                FileExtensions = decoder.FilenameExtension.Split(';')
            }));

        /// <summary>
        /// Gets a list of additionally registered WIC decoders
        /// </summary>
        /// <returns></returns>
        private static IEnumerable<DecoderInfo> GetAdditionalDecoders()
        {
            var result = new List<DecoderInfo>();

            string baseKeyPath;

            // If we are a 32 bit process running on a 64 bit operating system, 
            // we find our config in Wow6432Node subkey
            if (Environment.Is64BitOperatingSystem && !Environment.Is64BitProcess)
            {
                baseKeyPath = "Wow6432Node\\CLSID";
            }
            else
            {
                baseKeyPath = "CLSID";
            }

            RegistryKey baseKey = Registry.ClassesRoot.OpenSubKey(baseKeyPath, false);
            if (baseKey != null)
            {
                var categoryKey = baseKey.OpenSubKey(WICDecoderCategory + "\\instance", false);
                if (categoryKey != null)
                {
                    // Read the guids of the registered decoders
                    var codecGuids = categoryKey.GetSubKeyNames();

                    foreach (var codecGuid in codecGuids)
                    {
                        // Read the properties of the single registered decoder
                        var codecKey = baseKey.OpenSubKey(codecGuid);
                        if (codecKey != null)
                        {
                            DecoderInfo decoderInfo = new DecoderInfo();
                            decoderInfo.FriendlyName = Convert.ToString(codecKey.GetValue("FriendlyName", ""));
                            decoderInfo.FileExtensions = Convert.ToString(codecKey.GetValue("FileExtensions", "")).Split(',');
                            result.Add(decoderInfo);
                        }
                    }
                }
            }
            return result;
        }
    }
}
