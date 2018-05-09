using System;
using System.Diagnostics;
using System.Globalization;
using Medella.TdsClient.Contants;

namespace Medella.TdsClient.TDS.Reader.StringHelpers
{
    public sealed class SqlCollations
    {
        // First 20 bits of info field represent the lcid, bits 21-25 are compare options
        private const uint IgnoreCase = 1 << 20; // bit 21 - IgnoreCase
        private const uint IgnoreNonSpace = 1 << 21; // bit 22 - IgnoreNonSpace / IgnoreAccent
        private const uint IgnoreWidth = 1 << 22; // bit 23 - IgnoreWidth
        private const uint IgnoreKanaType = 1 << 23; // bit 24 - IgnoreKanaType
        private const uint BinarySort = 1 << 24; // bit 25 - BinarySort

        internal const uint MaskLcid = 0xfffff;
        private const int LcidVersionBitOffset = 28;
        private const uint MaskLcidVersion = unchecked((uint) (0xf << LcidVersionBitOffset));
        private const uint MaskCompareOpt = IgnoreCase | IgnoreNonSpace | IgnoreWidth | IgnoreKanaType | BinarySort;

        public uint Info;
        public byte SortId;

        internal int LcId
        {
            // First 20 bits of info field represent the lcid
            get => unchecked((int) (Info & MaskLcid));
            set
            {
                var lcid = value & (int) MaskLcid;
                Debug.Assert(lcid == value, "invalid set_LCID value");

                // Some new Katmai LCIDs do not have collation with version = 0
                // since user has no way to specify collation version, we set the first (minimal) supported version for these collations
                var versionBits = FirstSupportedCollationVersion(lcid) << LcidVersionBitOffset;
                Debug.Assert((versionBits & MaskLcidVersion) == versionBits, "invalid version returned by FirstSupportedCollationVersion");

                // combine the current compare options with the new locale ID and its first supported version
                Info = (Info & MaskCompareOpt) | unchecked((uint) lcid) | unchecked((uint) versionBits);
            }
        }

        private static int FirstSupportedCollationVersion(int lcid)
        {
            // NOTE: switch-case works ~3 times faster in this case than search with Dictionary
            switch (lcid)
            {
                case 1044: return 2; // Norwegian_100_BIN
                case 1047: return 2; // Romansh_100_BIN
                case 1056: return 2; // Urdu_100_BIN
                case 1065: return 2; // Persian_100_BIN
                case 1068: return 2; // Azeri_Latin_100_BIN
                case 1070: return 2; // Upper_Sorbian_100_BIN
                case 1071: return 1; // Macedonian_FYROM_90_BIN
                case 1081: return 1; // Indic_General_90_BIN
                case 1082: return 2; // Maltese_100_BIN
                case 1083: return 2; // Sami_Norway_100_BIN
                case 1087: return 1; // Kazakh_90_BIN
                case 1090: return 2; // Turkmen_100_BIN
                case 1091: return 1; // Uzbek_Latin_90_BIN
                case 1092: return 1; // Tatar_90_BIN
                case 1093: return 2; // Bengali_100_BIN
                case 1101: return 2; // Assamese_100_BIN
                case 1105: return 2; // Tibetan_100_BIN
                case 1106: return 2; // Welsh_100_BIN
                case 1107: return 2; // Khmer_100_BIN
                case 1108: return 2; // Lao_100_BIN
                case 1114: return 1; // Syriac_90_BIN
                case 1121: return 2; // Nepali_100_BIN
                case 1122: return 2; // Frisian_100_BIN
                case 1123: return 2; // Pashto_100_BIN
                case 1125: return 1; // Divehi_90_BIN
                case 1133: return 2; // Bashkir_100_BIN
                case 1146: return 2; // Mapudungan_100_BIN
                case 1148: return 2; // Mohawk_100_BIN
                case 1150: return 2; // Breton_100_BIN
                case 1152: return 2; // Uighur_100_BIN
                case 1153: return 2; // Maori_100_BIN
                case 1155: return 2; // Corsican_100_BIN
                case 1157: return 2; // Yakut_100_BIN
                case 1164: return 2; // Dari_100_BIN
                case 2074: return 2; // Serbian_Latin_100_BIN
                case 2092: return 2; // Azeri_Cyrillic_100_BIN
                case 2107: return 2; // Sami_Sweden_Finland_100_BIN
                case 2143: return 2; // Tamazight_100_BIN
                case 3076: return 1; // Chinese_Hong_Kong_Stroke_90_BIN
                case 3098: return 2; // Serbian_Cyrillic_100_BIN
                case 5124: return 2; // Chinese_Traditional_Pinyin_100_BIN
                case 5146: return 2; // Bosnian_Latin_100_BIN
                case 8218: return 2; // Bosnian_Cyrillic_100_BIN

                default: return 0; // other LCIDs have collation with version 0
            }
        }

        internal static bool AreSame(SqlCollations a, SqlCollations b)
        {
            if (a == null || b == null)
                return a == b;
            return a.Info == b.Info && a.SortId == b.SortId;
        }
    }

    public static class SqlCollationExtentions
    {
        public static int GetCodePage(this SqlCollations collation)
        {
            if (0 != collation.SortId)
                return TdsEnums.CODE_PAGE_FROM_SORT_ID[collation.SortId];

            var success = false;
            var cultureId = collation.LcId;
            var codePage = 0;
            try
            {
                codePage = CultureInfo.GetCultureInfo(cultureId).TextInfo.ANSICodePage;

                // SqlHot 50001398: CodePage can be zero, but we should defer such errors until
                //  we actually MUST use the code page (i.e. don't error if no ANSI data is sent).
                success = true;
            }
            catch (ArgumentException)
            {
            }

            // If we failed, it is quite possible this is because certain culture id's
            // were removed in Win2k and beyond, however Sql Server still supports them.
            // In this case we will mask off the sort id (the leading 1). If that fails, 
            // or we have a culture id other than the cases below, we throw an error and 
            // throw away the rest of the results. 

            //  Sometimes GetCultureInfo will return CodePage 0 instead of throwing.
            //  This should be treated as an error and functionality switches into the following logic.
            if (success && codePage != 0)
                return codePage;
            switch (cultureId)
            {
                case 0x10404: // zh-TW
                case 0x10804: // zh-CN
                case 0x10c04: // zh-HK
                case 0x11004: // zh-SG
                case 0x11404: // zh-MO
                case 0x10411: // ja-JP
                case 0x10412: // ko-KR
                    // If one of the following special cases, mask out sortId and
                    // retry.
                    cultureId = cultureId & 0x03fff;

                    try
                    {
                        return new CultureInfo(cultureId).TextInfo.ANSICodePage;
                    }
                    catch (ArgumentException)
                    {
                    }

                    break;
                case 0x827: // Mapping Non-supported Lithuanian code page to supported Lithuanian.
                    try
                    {
                        return new CultureInfo(0x427).TextInfo.ANSICodePage;
                    }
                    catch (ArgumentException)
                    {
                    }

                    break;
            }

            if (!success) throw new Exception("Unsupported collation");

            return codePage;
        }
    }
}