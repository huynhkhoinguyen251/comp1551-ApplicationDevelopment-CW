
using System.Collections.Generic;  // For List<T> collections
using System.Drawing.Drawing2D;    // For gradient brushes and advanced graphics
using System.Linq;                 // For LINQ operations like FirstOrDefault()

namespace Comp1551_ApplicationDev_CW;
public partial class Form1 : Form
{
    // Private field to hold the main game logic object
    // ENCAPSULATION: Game logic is contained within this object
    private QuizGame _quizGame;
    public Form1()
    {
        InitializeComponent();        
        _quizGame = new QuizGame();  
        AddSampleQuestions();         
        SetupUI();                    
    }

    private void AddSampleQuestions()
    {
        // Sample Multiple Choice Question - demonstrates 4 options with one correct answer
        _quizGame.AddQuestion(new MultipleChoiceQuestion(
            "What is the capital of France?",                                    // Question text
            new List<string> { "London", "Berlin", "Paris", "Madrid" },        // 4 options (A, B, C, D)
            2));                                                                // Correct answer index (0-based: 2 = "Paris")

        // Sample Open-Ended Question - demonstrates multiple acceptable answers
        _quizGame.AddQuestion(new OpenEndedQuestion(
            "What is the largest country in the world by land area?",           // Question text
            new List<string> { "Russia", "Russian Federation" }));             // Multiple acceptable answers

        // Sample True/False Question - demonstrates boolean logic
        _quizGame.AddQuestion(new TrueFalseQuestion(
            "The Amazon River is longer than the Nile River.",                 // Statement to evaluate
            false));                                                            // Correct answer (false - Nile is longer)

        // Another Multiple Choice Question - demonstrates different topic
        _quizGame.AddQuestion(new MultipleChoiceQuestion(
            "Which continent has the most countries?",                          // Question text
            new List<string> { "Asia", "Europe", "Africa", "South America" },  // 4 options
            2));                                                                // Correct answer: Africa (index 2)

        // Another Open-Ended Question - demonstrates alternative names
        _quizGame.AddQuestion(new OpenEndedQuestion(
            "What is the smallest country in the world?",                       // Question text
            new List<string> { "Vatican City", "Vatican", "Holy See" }));      // Multiple ways to say same answer
    }

    private void SetupUI()
    {
        this.Text = "🌍 Geography Quiz Game";
        this.Size = new Size(800, 600);
        this.StartPosition = FormStartPosition.CenterScreen;
        this.BackColor = Color.FromArgb(135, 206, 235); // Sky blue background
        this.Paint += BackgroundPanel_Paint; // Paint directly on form

        // Title label with shadow effect
        Label titleLabel = new Label
        {
            Text = "🌍 GEOGRAPHY QUIZ GAME 🌍",
            Font = new Font("Arial", 28, FontStyle.Bold),
            ForeColor = Color.White,
            BackColor = Color.Transparent,
            AutoSize = true,
            Location = new Point(150, 80)
        };

        Label shadowLabel = new Label
        {
            Text = "🌍 GEOGRAPHY QUIZ GAME 🌍",
            Font = new Font("Arial", 28, FontStyle.Bold),
            ForeColor = Color.FromArgb(100, 0, 0, 0), // Semi-transparent black shadow
            BackColor = Color.Transparent,
            AutoSize = true,
            Location = new Point(152, 82)
        };

        // Subtitle
        Label subtitleLabel = new Label
        {
            Text = "Test your knowledge of our amazing world!",
            Font = new Font("Arial", 14, FontStyle.Italic),
            ForeColor = Color.White,
            BackColor = Color.Transparent,
            AutoSize = true,
            Location = new Point(250, 130)
        };

        // Create Game button with gradient and hover effects
        Button btnCreateGame = CreateStyledButton(
            "🎯 Mode 1: Create Game\n📝 Add / Edit Questions",
            new Point(150, 200),
            new Size(200, 80),
            Color.FromArgb(46, 125, 50), // Dark green
            Color.FromArgb(76, 175, 80)  // Light green
        );
        btnCreateGame.Click += (s, e) => OpenQuestionManager();

        // Play Game button with gradient and hover effects
        Button btnPlayGame = CreateStyledButton(
            "🎮 Mode 2: Play Game\n🏆 Test Your Knowledge",
            new Point(450, 200),
            new Size(200, 80),
            Color.FromArgb(25, 118, 210), // Dark blue
            Color.FromArgb(33, 150, 243)  // Light blue
        );
        btnPlayGame.Click += (s, e) => StartQuiz();

        // Question count with styled background
        Panel countPanel = new Panel
        {
            Size = new Size(300, 60),
            Location = new Point(250, 320),
            BackColor = Color.FromArgb(200, 255, 255, 255), // Semi-transparent white
            BorderStyle = BorderStyle.None
        };
        countPanel.Paint += (s, e) => {
            e.Graphics.FillRoundedRectangle(new SolidBrush(Color.FromArgb(200, 255, 255, 255)),
                new Rectangle(0, 0, countPanel.Width, countPanel.Height), 15);
        };

        Label questionCountLabel = new Label
        {
            Text = $"📊 Questions Available: {_quizGame.TotalQuestions}",
            Font = new Font("Arial", 12, FontStyle.Bold),
            ForeColor = Color.FromArgb(33, 33, 33),
            BackColor = Color.Transparent,
            AutoSize = true,
            Location = new Point(50, 20),
            Name = "questionCount"
        };

        // Exit button
        Button btnExit = CreateStyledButton(
            "❌ Exit",
            new Point(350, 450),
            new Size(100, 40),
            Color.FromArgb(183, 28, 28), // Dark red
            Color.FromArgb(244, 67, 54)  // Light red
        );
        btnExit.Click += (s, e) => Application.Exit();

        // Add controls directly to form
        this.Controls.Add(shadowLabel);
        this.Controls.Add(titleLabel);
        this.Controls.Add(subtitleLabel);
        this.Controls.Add(btnCreateGame);
        this.Controls.Add(btnPlayGame);
        this.Controls.Add(countPanel);
        countPanel.Controls.Add(questionCountLabel);
        this.Controls.Add(btnExit);
    }

    private void OpenQuestionManager()
    {
        // Use the new separated architecture
        QuestionManagementFormNew form = new QuestionManagementFormNew(_quizGame);
        form.ShowDialog();

        // Update question count after returning - search recursively in all controls
        var label = this.Controls.Find("questionCount", true).FirstOrDefault() as Label;
        if (label != null)
            label.Text = $"📊 Questions Available: {_quizGame.TotalQuestions}";
    }

    private void StartQuiz()
    {
        if (_quizGame.TotalQuestions == 0)
        {
            MessageBox.Show("No questions available! Create questions first.");
            return;
        }

        QuizPlayForm playForm = new QuizPlayForm(_quizGame);
        playForm.ShowDialog();
    }

    private Button CreateStyledButton(string text, Point location, Size size, Color darkColor, Color lightColor)
    {
        Button btn = new Button
        {
            Text = text,
            Location = location,
            Size = size,
            Font = new Font("Arial", 11, FontStyle.Bold),
            ForeColor = Color.White,
            BackColor = darkColor,
            FlatStyle = FlatStyle.Flat,
            FlatAppearance = { BorderSize = 0 },
            Cursor = Cursors.Hand,
            TextAlign = ContentAlignment.MiddleCenter
        };

        // Add hover effects
        btn.MouseEnter += (s, e) => {
            btn.BackColor = lightColor;
            btn.Font = new Font("Arial", 11, FontStyle.Bold);
        };

        btn.MouseLeave += (s, e) => {
            btn.BackColor = darkColor;
            btn.Font = new Font("Arial", 11, FontStyle.Bold);
        };

        // Add click animation
        btn.MouseDown += (s, e) => {
            btn.BackColor = Color.FromArgb(darkColor.R - 20, darkColor.G - 20, darkColor.B - 20);
        };

        btn.MouseUp += (s, e) => {
            btn.BackColor = lightColor;
        };

        return btn;
    }

    private void BackgroundPanel_Paint(object sender, PaintEventArgs e)
    {
        // Create a beautiful gradient background
        using (LinearGradientBrush brush = new LinearGradientBrush(
            new Point(0, 0),
            new Point(0, this.Height),
            Color.FromArgb(135, 206, 235), // Sky blue
            Color.FromArgb(70, 130, 180)))  // Steel blue
        {
            e.Graphics.FillRectangle(brush, this.ClientRectangle);
        }

        // Add some decorative elements (clouds)
        DrawClouds(e.Graphics);
    }

    private void DrawClouds(Graphics g)
    {
        using (SolidBrush cloudBrush = new SolidBrush(Color.FromArgb(100, 255, 255, 255)))
        {
            // Cloud 1
            g.FillEllipse(cloudBrush, 50, 30, 60, 40);
            g.FillEllipse(cloudBrush, 80, 20, 80, 50);
            g.FillEllipse(cloudBrush, 120, 30, 60, 40);

            // Cloud 2
            g.FillEllipse(cloudBrush, 600, 50, 70, 45);
            g.FillEllipse(cloudBrush, 630, 40, 90, 55);
            g.FillEllipse(cloudBrush, 680, 50, 70, 45);

            // Cloud 3
            g.FillEllipse(cloudBrush, 300, 400, 50, 30);
            g.FillEllipse(cloudBrush, 320, 395, 70, 40);
            g.FillEllipse(cloudBrush, 360, 400, 50, 30);
        }
    }
}