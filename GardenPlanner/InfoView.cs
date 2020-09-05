using System.Collections.Generic;
using Gtk;
namespace GardenPlanner
{
    public class InfoView : TextView
    {
        public readonly TextTag headline = new TextTag("headline")
        {
            Weight = Pango.Weight.Bold,
            Underline = Pango.Underline.Single
        };
        public readonly TextTag italic = new TextTag("italic")
        {
            Style = Pango.Style.Italic
        };
        public readonly TextTag bold = new TextTag("bold")
        {
            Weight = Pango.Weight.Bold
        };
        public readonly TextTag weak = new TextTag("weak")
        {
            Foreground = "red"
        };

        class InfoTag
        {
            public TextTag tag;
            public int start, end;
        }

        private List<InfoTag> InfoTags = new List<InfoTag>();

        public InfoView() : base()
        {
            Buffer.TagTable.Add(headline);
            Buffer.TagTable.Add(bold);
            Buffer.TagTable.Add(italic);
            Buffer.TagTable.Add(weak);

            WrapMode = WrapMode.Word;
            Editable = false;
        }

        public int AddEntry(string entry, bool newline = true)
        {
            string s = entry + (newline ? "\n" : "");
            Buffer.Text += s;
            return s.Length;
        }

        //TextIter iter;
        public void AddEntry(string entry, TextTag tag, bool newline = true)
        {
            //iter = InfoView.Buffer.EndIter;
            //InfoView.Buffer.InsertWithTags(ref iter, entry + (newline ? "\n" : ""), tag);

            int len = AddEntry(entry, newline);
            TextIter end = Buffer.EndIter;
            TextIter start = Buffer.EndIter;

            start.BackwardChars(len);
            InfoTags.Add(new InfoTag { start = start.Offset, end = end.Offset, tag = tag });
            //InfoView.Buffer.ApplyTag(tag, start, end);
        }

        public void ApplyTags()
        {
            foreach (InfoTag infoTag in InfoTags)
            {
                TextIter startIter = Buffer.GetIterAtOffset(infoTag.start);
                TextIter endIter = Buffer.GetIterAtOffset(infoTag.end);

                Buffer.ApplyTag(infoTag.tag, startIter, endIter);
            }
        }
    }
}
