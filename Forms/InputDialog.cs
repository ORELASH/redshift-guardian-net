using System;
using System.Drawing;
using System.Windows.Forms;

namespace RedshiftGuardianNET.Forms
{
    /// <summary>
    /// Simple input dialog for getting text input from user
    /// </summary>
    public class InputDialog : Form
    {
        private Label labelPrompt;
        private TextBox textInput;
        private Button buttonOk;
        private Button buttonCancel;

        public string InputText
        {
            get { return textInput.Text; }
            set { textInput.Text = value; }
        }

        public InputDialog(string prompt, string title, string defaultValue)
        {
            InitializeComponent();
            this.Text = title;
            labelPrompt.Text = prompt;
            textInput.Text = defaultValue;
        }

        private void InitializeComponent()
        {
            this.Size = new Size(400, 150);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            labelPrompt = new Label();
            labelPrompt.Location = new Point(12, 15);
            labelPrompt.Size = new Size(360, 20);
            this.Controls.Add(labelPrompt);

            textInput = new TextBox();
            textInput.Location = new Point(12, 40);
            textInput.Size = new Size(360, 20);
            this.Controls.Add(textInput);

            buttonOk = new Button();
            buttonOk.Text = "OK";
            buttonOk.Location = new Point(200, 75);
            buttonOk.Size = new Size(80, 25);
            buttonOk.DialogResult = DialogResult.OK;
            this.Controls.Add(buttonOk);

            buttonCancel = new Button();
            buttonCancel.Text = "Cancel";
            buttonCancel.Location = new Point(290, 75);
            buttonCancel.Size = new Size(80, 25);
            buttonCancel.DialogResult = DialogResult.Cancel;
            this.Controls.Add(buttonCancel);

            this.AcceptButton = buttonOk;
            this.CancelButton = buttonCancel;
        }

        public static string Show(string prompt, string title, string defaultValue)
        {
            using (InputDialog dialog = new InputDialog(prompt, title, defaultValue))
            {
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    return dialog.InputText;
                }
                return null;
            }
        }
    }
}
