namespace ButtonDemo
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.Button0.Click += new EventHandler(Button_Click!);
            this.Button1.Click += new EventHandler(Button_Click!);
            this.Button2.Click += new EventHandler(Button_Click!);
            this.Button3.Click += new EventHandler(Button_Click!);
            this.Button4.Click += new EventHandler(Button_Click!);
            this.Button5.Click += new EventHandler(Button_Click!);
            this.Button6.Click += new EventHandler(Button_Click!);
            this.Button7.Click += new EventHandler(Button_Click!);
            this.Button8.Click += new EventHandler(Button_Click!);
            this.Button9.Click += new EventHandler(Button_Click!);
            this.ButtonSpace.Click += new EventHandler(Button_Click!);
            this.ButtonClear.Click += new EventHandler(ButtonClear_Click!);
            this.ButtonBack.Click += new EventHandler(ButtonBack_Click!);
        }

        private void Button_Click(object sender, EventArgs e)
        {
            var button = (ImageButton)sender;
            var text = button.Text;
            this.textBox1.Text += button.Text;
        }

        private void ButtonClear_Click(object sender, EventArgs e)
        {
            this.textBox1.Text = string.Empty;
        }

        private void ButtonBack_Click(object sender, EventArgs e)
        {
            if (this.textBox1.Text.Length > 0)
            {
                this.textBox1.Text = this.textBox1.Text.Substring(0, this.textBox1.Text.Length - 1);
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            ButtonClear.Enabled = this.textBox1.Text.Length > 0;
            textBox1.Focus();
            textBox1.SelectionStart = textBox1.Text.Length;
        }

        private void Form1_KeyPress(object sender, KeyPressEventArgs e)
        {
            var button = e.KeyChar switch
            {
                '1' => Button1,
                '2' => Button2,
                '3' => Button3,
                '4' => Button4,
                '5' => Button5,
                '6' => Button6,
                '7' => Button7,
                '8' => Button8,
                '9' => Button9,
                '0' => Button0,
                'c' => ButtonClear,
                ' ' => ButtonSpace,
                '\b' => ButtonBack,
                _ => null
            };
            if (button != null && button.Enabled)
            {
                button.PerformClick(200);
            }

        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            var image = radioButton3D.Checked ? Images.button_3D_60x60 : Images.button60x60;
            var imageLong = radioButton3D.Checked ? Images.button_3D_60x60 : Images.button200x60;
            this.Button0.Image = image;
            this.Button1.Image = image;
            this.Button2.Image = image;
            this.Button3.Image = image;
            this.Button4.Image = image;
            this.Button5.Image = image;
            this.Button6.Image = image;
            this.Button7.Image = image;
            this.Button8.Image = image;
            this.Button9.Image = image;
            this.ButtonClear.Image = image;
            this.ButtonBack.Image = image;
            this.ButtonSpace.Image = imageLong;

        }
    }
}
