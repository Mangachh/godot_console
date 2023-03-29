using Godot;
using System;
using System.Collections.Generic;

namespace Godot
{
    /// <summary>
    /// A simple console <see cref="Control"/>. Used for diferents projects, now it's for debug
    /// prourposes
    /// </summary>
    public partial class ConsoleUI : Control
    {
        /// <summary>
        /// Chache for populating the <see cref="unusedLabels"/>
        /// </summary>
        [Export]
        private byte TEXTHOLDERCACHE;

        [Export]
        private TestResource test;

        /// <summary>
        /// Name of the <see cref="textHolder"/>. It's a horizontal container that organizes the labels
        /// </summary>
        [Export]
        private string TEXTHOLDERNAME = "TextHolder";

        /// <summary>
        /// Font size of the labels
        /// </summary>
        [Export]
        private int fontSize;

        /// <summary>
        /// Font of the label, is an overrride
        /// </summary>
        [Export]
        private FontFile FONT;

        /// <summary>
        /// Background needed for the rect
        /// </summary>
        private Control background;

        /// <summary>
        /// Parent of the labels
        /// </summary>
        private Control textHolder;

        /// <summary>
        /// Unused labels. We use a simple pool (TODO: create a generic)
        /// </summary>
        private List<Label> unusedLabels = new List<Label>();

        /// <summary>
        /// Used labels.
        /// </summary>
        private List<Label> usedLabels = new List<Label>();
        
        /// <summary>
        /// Default color of the messages written in the console.
        /// Used on <see cref="ConsoleUI.WriteMessage(in string)"
        /// </summary>
        [Export]
        private Color COLORDEFAULT;

        /// <summary>
        /// The color of the Error messages written in the console.
        /// Used on <see cref="ConsoleUI.WriteError(in string)"
        /// </summary>
        [Export]
        private Color COLORWARNING;

        /// <summary>
        /// The color of the Error messages written in the console.
        /// Used on <see cref="ConsoleUI.WriteWarning(in string)"
        /// </summary>
        [Export]
        private Color COLORERROR;

        /// <summary>
        /// Max height of the container
        /// </summary>
        private int maxHeight;


        /// <summary>
        /// Initialices the visual console, setting all the children.
        /// OJU! call this after inserting the children. It doesn't work
        /// with EnterTree
        /// </summary>
        public void Init()
        {
            GD.Print("ConsoleUI entered the tree");

            this.textHolder = base.FindChild(TEXTHOLDERNAME) as Control;

            if (textHolder != null)
            {
                GD.Print("Text Holder Found");
                this.maxHeight = (int)this.textHolder.Size.Y;
            }
            else
            {
                GD.PrintErr("Text Holder not found on Console");
            }

            this.LabelListsInit();
        }

        public override void _Ready()
        {
            GD.Print("Hellooooooooo");
            this.Init();
            // TODO: change this to open console as default???
            MyConsole.IsOn = true;
            MyConsole.Console = this;
            MyConsole.Write("Damm, this is MY console");
            MyConsole.WriteError(this.test.a.ToString());
        }

        /// <summary>
        /// Initializes the <see cref="unusedLabels"/>, <see cref="usedLabels"/> and <see cref="toWrite"/>. Also, populates
        /// the <see cref="unusedLabels"/> with a <see cref="TEXTHOLDERCACHE"/> number of labels
        /// </summary>
        private void LabelListsInit()
        {
            unusedLabels = new List<Label>(TEXTHOLDERCACHE);
            usedLabels = new List<Label>();

            for (byte i = 0; i < TEXTHOLDERCACHE; i++)
            {
                unusedLabels.Add(this.LabelInit());
            }
        }

        /// <summary>
        /// Initialices a new label with the basic settings and not visible
        /// </summary>
        /// <returns>A new label</returns>
        private Label LabelInit()
        {
            Label l = new Label();
            l.Hide();
            l.AddThemeFontOverride("font", FONT);
            l.AddThemeFontSizeOverride("font_size", this.fontSize);
            l.AutowrapMode = TextServer.AutowrapMode.WordSmart;
            return l;
        }        

        # region Write methods 

        public void WriteMessage(in string message)
        {
            this.WriteOnConsole(message, COLORDEFAULT);
        }

        public void WriteWarning(in string message)
        {
            this.WriteOnConsole(message, COLORWARNING);
        }

        public void WriteError(in string message)
        {
            this.WriteOnConsole(message, COLORERROR);
        }


        /// <summary>
        /// Writes a message with a font color on the console
        /// </summary>
        /// <param name="message">Message to write</param>
        /// <param name="color">Font color</param>
        public void WriteOnConsole(in string message, in Color color)
        {
            Label label;

            //check for used and unused
            if (unusedLabels.Count > 0)
            {
                label = unusedLabels[0];
                unusedLabels.RemoveAt(0);
            }
            else
            {
                label = this.LabelInit();
            }


            usedLabels.Add(label);
            //textHolder.AddChild(label);
            this.SetLabel(ref label, message, color);

            this.textHolder.AddChild(label);
            base.CallDeferred(nameof(this.PutLabelOnPosition), label);
            //this.PutLabelOnPosition(label);
        }

        # endregion


        private void SetLabel(ref Label label, in string message, in Color color)
        {
            label.Name = "Labelnum_";            
            label.Text = message;
            label.Size = new Vector2(this.textHolder.Size.X, label.Size.Y);
            label.Show();
            label.SelfModulate = color;
        }

        /// <summary>
        /// Puts a label on a position inside <see cref="textHolder"/> using the <see cref="backgroung.Size.y"/>.
        /// the label is inside the box and handles the elimination of the most ancient label, if needed (yes, ancient, as Chutlhu)
        /// </summary>
        /// <param name="lbl">The label to put</param>
        private void PutLabelOnPosition(Label lbl)
        {
            // as the vertical aligment works automatic, we span
            // the label and check if the top left is outside the box; if so, 
            // delete the first label and check again
            bool inside = false;
            int bottom = (int)(lbl.Size.Y + lbl.GlobalPosition.Y);

            while (inside == false)
            {
                if (bottom > this.maxHeight + this.textHolder.GlobalPosition.Y)
                {
                    GD.Print("Delete label");
                    Label temp = this.textHolder.GetChild<Label>(0);
                    this.textHolder.RemoveChild(temp);
                    bottom -= (int)temp.Size.Y;
                }else{
                    inside = true;
                }
            }
        }


        /// <summary>
        /// Remove a label from <see cref="usedLabels"/> and move to
        /// <see cref="unusedLabels"/>
        /// </summary>
        /// <param name="label"></param>
        private void RemoveLabel(in Label label)
        {
            unusedLabels.Add(label);
            label.Hide();
            usedLabels.Remove(label);
            label.Text = "";
        }

        /// <summary>
        /// Clears the text of the console
        /// </summary>
        public void ClearConsoleText()
        {
            Label label;
            for (int i = 0; i < textHolder.GetChildCount(); i++)
            {
                label = textHolder.GetChild<Label>(i);
                textHolder.RemoveChild(label);
                this.RemoveLabel(label);
            }
        }       
    }
}
