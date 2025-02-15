﻿//Simple TMX file writer class for producing the error log of failed sentences. 

using System;
using System.IO;
using System.Text;

namespace Mts.Common.Tmx
{
    public class TmxWriter : IDisposable
    {
        private const bool WriteToCSV = true;
        private CsvWriter csvwriter;
        private const string creationtool = "Microsoft Document Translator";
        private const string creationtoolversion = "2015.10.25";


        private StreamWriter TmxStream;
        public enum TUError {good, lengthratio, sentencecountmismatch, tagging};

        /// <summary>
        /// Creates and initializes a TMX file for writing
        /// </summary>
        /// <param name="TmxFilename">TMX file name</param>
        public TmxWriter(string filename, string sourcelanguage, string targetlanguage)
        {
            this.TmxStream = new StreamWriter(filename, false, Encoding.UTF8);
            TmxWriteHeader(sourcelanguage);
            if (WriteToCSV) {
                this.csvwriter = new CsvWriter(Path.GetFileNameWithoutExtension(filename) + ".csv", sourcelanguage, targetlanguage);
            }
            return;
        }


        public void TmxWriteSegment(string sourcesegment, string targetsegment, string sourcelang, string targetlang, TUError tustatus)
        {
            this.TmxStream.WriteLine("<tu>");
            this.TmxStream.WriteLine("<prop type=\"error\">{0}</prop>", statusmessage(tustatus));
            this.TmxStream.WriteLine("<tuv xml:lang=\"{0}\">", sourcelang);
            this.TmxStream.WriteLine("<seg>{0}</seg>\n</tuv>", sourcesegment);
            this.TmxStream.WriteLine("<tuv xml:lang=\"{0}\">", targetlang);
            this.TmxStream.WriteLine("<seg>{0}</seg>\n</tuv>", targetsegment);
            this.TmxStream.WriteLine("</tu>");
            if (WriteToCSV)
            {
                csvwriter.WriteSegment(sourcesegment, targetsegment, tustatus);
            }
        }

        private string statusmessage(TUError tustatus)
        {
            switch (tustatus)
            {
                case TUError.good:
                    return ("Good");
                case TUError.lengthratio:
                    return ("Length ratio exceeded");
                case TUError.sentencecountmismatch:
                    return ("Sentence count mismatch");
                case TUError.tagging:
                    return ("Sentence contains tags");
                default:
                    return("");
            }
        }

        private void TmxWriteHeader(string sourcelanguage)
        {
            this.TmxStream.WriteLine("<tmx version=\"1.4\">");
            this.TmxStream.WriteLine("<header\ncreationtool=\"{0}\"\ncreationtoolversion=\"{1}\"\ncreationdate=\"{2}\"\nsrclang=\"{3}\">", creationtool, creationtoolversion, DateTime.Now.ToString("yyyyMMddThhmmssZ"), sourcelanguage);
            this.TmxStream.WriteLine("</header>");
            this.TmxStream.WriteLine("<body>");
        }

        public void Dispose()
        {
            this.TmxStream.WriteLine("</body>");
            this.TmxStream.WriteLine("</tmx>");
            this.TmxStream.Flush();
            this.TmxStream.Close();
            this.TmxStream.Dispose();
            if (WriteToCSV) { this.csvwriter.Dispose(); }
        }
    }
}
