// Copyright (c) 2020 Jahangmar
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program. If not, see <https://www.gnu.org/licenses/>.
//

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
            int len = AddEntry(entry, newline);
            TextIter end = Buffer.EndIter;
            TextIter start = Buffer.EndIter;

            start.BackwardChars(len);
            InfoTags.Add(new InfoTag { start = start.Offset, end = end.Offset, tag = tag });
        }

        public void ApplyTags()
        {
            foreach (InfoTag infoTag in InfoTags)
            {
                TextIter startIter = Buffer.GetIterAtOffset(infoTag.start);
                TextIter endIter = Buffer.GetIterAtOffset(infoTag.end);

                Buffer.ApplyTag(infoTag.tag, startIter, endIter);
            }
            InfoTags.Clear();
        }
    }
}
