using System.Collections.Generic;
using Gtk;

namespace GardenPlanner
{
    public abstract class InfoAffectableWindow : Window
    {
        VPaned VPaned = new VPaned();

        //VBox InfoBox = new VBox();
        TextView InfoView = new TextView();

        HButtonBox ActionButtonBox = new HButtonBox();
        Button CloseButton = new Button(new Label("Close"));
        Button EditButton = new Button(new Label("Edit"));

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

        public InfoAffectableWindow(string title, bool isEdited = false) : base(WindowType.Toplevel)
        {
            Modal = true;
            Title = title;

            VPaned.Add1(InfoView);
            VPaned.Add2(ActionButtonBox);

            this.Add(VPaned);

            ActionButtonBox.Add(CloseButton);
            ActionButtonBox.Add(EditButton);

            CloseButton.Clicked += (object sender, System.EventArgs e) => this.Destroy();
            EditButton.Clicked += (object sender, System.EventArgs e) =>
            {
                if (isEdited)
                    this.Destroy();
                else
                    Edit();
            };

            InfoView.Buffer.TagTable.Add(headline);
            InfoView.Buffer.TagTable.Add(bold);
            InfoView.Buffer.TagTable.Add(italic);
            InfoView.Buffer.TagTable.Add(weak);

            InfoView.WrapMode = WrapMode.Word;
            InfoView.Editable = false;

        }
              
        protected int AddEntry(string entry, bool newline=true)
        {
            string s = entry + (newline ? "\n" : "");
            InfoView.Buffer.Text += s;
            return s.Length;
        }

        //TextIter iter;
        protected void AddEntry(string entry, TextTag tag, bool newline=true)
        {
            //iter = InfoView.Buffer.EndIter;
            //InfoView.Buffer.InsertWithTags(ref iter, entry + (newline ? "\n" : ""), tag);

            int len = AddEntry(entry, newline);
            TextIter end = InfoView.Buffer.EndIter;
            TextIter start = InfoView.Buffer.EndIter;

            start.BackwardChars(len);
            InfoTags.Add(new InfoTag { start = start.Offset, end = end.Offset, tag = tag });
            //InfoView.Buffer.ApplyTag(tag, start, end);
        }

        protected void ApplyTags()
        {
            foreach (InfoTag infoTag in InfoTags)
            {
                TextIter startIter = InfoView.Buffer.GetIterAtOffset(infoTag.start);
                TextIter endIter = InfoView.Buffer.GetIterAtOffset(infoTag.end);

                InfoView.Buffer.ApplyTag(infoTag.tag, startIter, endIter);
            }
        }

        protected abstract void Edit();
    }
}
