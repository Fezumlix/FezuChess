using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace FezuChess
{
    public class PromotionForm : Form
    {
        public PieceType PromotionType { get; private set; }
        
        public PromotionForm()
        {
            string path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            var queen = Image.FromFile(Path.Combine(path.Substring(0, path.Length - 9), @"pieces\q_b.png"));
            var rook = Image.FromFile(Path.Combine(path.Substring(0, path.Length - 9), @"pieces\r_b.png"));
            var bishop = Image.FromFile(Path.Combine(path.Substring(0, path.Length - 9), @"pieces\b_b.png"));
            var knight = Image.FromFile(Path.Combine(path.Substring(0, path.Length - 9), @"pieces\n_b.png"));
            
            var queenButton = new Button
            {
                Name = "queen",
                Image = queen,
                Size = new(60, 60),
                Location = new(20, 20)
            };
            var rookButton = new Button
            {
                Name = "rook",
                Image = rook,
                Size = new(60, 60),
                Location = new(100, 20)
            };
            var bishopButton = new Button
            {
                Name = "bishop",
                Image = bishop,
                Size = new(60, 60),
                Location = new(20, 100)
            };
            var knightButton = new Button
            {
                Name = "knight",
                Image = knight,
                Size = new(60, 60),
                Location = new(100, 100)
            };
            Controls.Add(queenButton);
            Controls.Add(rookButton);
            Controls.Add(bishopButton);
            Controls.Add(knightButton);
            queenButton.Click += ClickHandler;
            rookButton.Click += ClickHandler;
            bishopButton.Click += ClickHandler;
            knightButton.Click += ClickHandler;
            
            Size = new(180, 180);
            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.None;
        }
        
        void ClickHandler(object sender, System.EventArgs e)
        {
            var button = (Button) sender;
            PromotionType = button.Name switch
            {
                "queen" => PieceType.Queen,
                "rook" => PieceType.Rook,
                "bishop" => PieceType.Bishop,
                "knight" => PieceType.Knight,
                _ => PieceType.Queen
            };
            Close();
        }


        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // PromotionForm
            // 
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "PromotionForm";
            this.ResumeLayout(false);
        }
    }
}